using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ParaTH;

public struct Position { public float X; public float Y; }
public struct Velocity { public float DX; public float DY; }
public struct Spin { public float Angle; public float Speed; }
public struct Lifetime { public int Ticks; }
public struct Gravity { public float G; }
public struct Orbit { public float CenterX; public float CenterY; public float Radius; public float Angle; public float Speed; }
public struct Mutator { public int Threshold; }
public struct Renderable { public byte SpriteType; public byte ColorIndex; public float Scale; }

public sealed class Engine : Game
{
    private AssetManager assetManager = null!;
    private StgBatch stgBatch = null!;
    private Matrix projection;

    private World world = null!;

    private SpriteAsset[] hearts = new SpriteAsset[8];
    private SpriteAsset[] arrows = new SpriteAsset[8];
    private readonly string[] colorNames = { "black", "red", "pink", "blue", "cyan", "green", "yellow", "gray" };

    private readonly Random rng = new(12345);

    private int frameCount;
    private bool isPaused;
    private bool stepRequested;
    private KeyboardState currKb;
    private KeyboardState prevKb;

    private double fpsTimer;
    private int fpsCounter;
    private int currentFps;

    private readonly List<Entity> entitiesToDestroy = new(10000);
    private readonly List<Entity> entitiesToMutate = new(10000);

    // ── 计时器 ──
    private readonly Stopwatch sw = new();
    private readonly StringBuilder profileSb = new(512);

    // 各阶段耗时（微秒），保留最近一帧的值
    private double tSpawn;
    private double tLifetime;
    private double tMutateQuery;
    private double tGravity;
    private double tMovement;
    private double tOrbit;
    private double tSpin;
    private double tMutateApply;
    private double tDestroy;
    private double tUpdateTotal;
    private double tDraw;

    // Stopwatch 频率转微秒的乘数，只算一次
    private static readonly double TicksToUs = 1_000_000.0 / Stopwatch.Frequency;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private long Stamp() { sw.Restart(); return 0; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double Elapsed()
    {
        sw.Stop();
        return sw.ElapsedTicks * TicksToUs;
    }

    public Engine()
    {
        var gdm = new GraphicsDeviceManager(this);
        gdm.PreferredBackBufferWidth = 1280;
        gdm.PreferredBackBufferHeight = 720;
        gdm.SynchronizeWithVerticalRetrace = false;
        gdm.GraphicsProfile = GraphicsProfile.HiDef;
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromTicks(166667);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        assetManager = new AssetManager("Asset");
        assetManager.RegisterLoader(new TextureAssetLoader(GraphicsDevice));
        assetManager.RegisterLoader(new SpriteAssetLoader(assetManager));
        assetManager.RegisterLoader(new FontAssetLoader());

        stgBatch = new StgBatch(GraphicsDevice);
        projection = Matrix.CreateOrthographicOffCenter(0, 1280, 720, 0, 0, 1);

        for (int i = 0; i < 8; i++)
        {
            hearts[i] = assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", $"heart_{colorNames[i]}");
            arrows[i] = assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", $"arrow_{colorNames[i]}");
        }

        world = new World(
            baseChunkByteSize: 16384,
            baseChunkEntityCount: 256,
            initialArchetypeCapacity: 32,
            initialEntityCapacity: 500000);
    }

    private void SpawnWave(int count)
    {
        using var allVel = ScopedPooledArray<Velocity>.Rent(count);
        using var allLife = ScopedPooledArray<Lifetime>.Rent(count);
        using var allRend = ScopedPooledArray<Renderable>.Rent(count);
        using var allSpin = ScopedPooledArray<float>.Rent(count);
        using var cats = ScopedPooledArray<byte>.Rent(count);

        int n0 = 0, n1 = 0, n2 = 0, n3 = 0;

        for (int i = 0; i < count; i++)
        {
            float angle = (float)(rng.NextDouble() * Math.PI * 2);
            float speed = (float)(rng.NextDouble() * 3f + 1f);
            allVel[i] = new Velocity
            {
                DX = MathF.Cos(angle) * speed,
                DY = MathF.Sin(angle) * speed
            };

            allLife[i] = new Lifetime { Ticks = rng.Next(300, 600) };
            allRend[i] = new Renderable
            {
                SpriteType = (byte)rng.Next(2),
                ColorIndex = (byte)rng.Next(8),
                Scale = 1.0f
            };

            bool hasSpin = rng.Next(2) == 0;
            bool hasGrav = rng.Next(5) == 0;

            if (hasSpin)
                allSpin[i] = (float)(rng.NextDouble() * 0.2 - 0.1);

            byte cat = (byte)((hasSpin ? 1 : 0) | (hasGrav ? 2 : 0));
            cats[i] = cat;

            switch (cat)
            {
                case 0: n0++; break;
                case 1: n1++; break;
                case 2: n2++; break;
                case 3: n3++; break;
            }
        }

        if (n0 > 0)
        {
            using var ents = ScopedPooledArray<Entity>.Rent(n0);
            using var pos = ScopedPooledArray<Position>.Rent(n0);
            using var vel = ScopedPooledArray<Velocity>.Rent(n0);
            using var life = ScopedPooledArray<Lifetime>.Rent(n0);
            using var rend = ScopedPooledArray<Renderable>.Rent(n0);

            for (int i = 0, j = 0; i < count; i++)
            {
                if (cats[i] != 0) continue;
                pos[j] = new Position { X = 640, Y = 360 };
                vel[j] = allVel[i];
                life[j] = allLife[i];
                rend[j] = allRend[i];
                j++;
            }

            world.CreateEntityBulk(ents.AsSpan(), pos.AsSpan(), vel.AsSpan(),
                                   life.AsSpan(), rend.AsSpan());
        }

        if (n1 > 0)
        {
            using var ents = ScopedPooledArray<Entity>.Rent(n1);
            using var pos = ScopedPooledArray<Position>.Rent(n1);
            using var vel = ScopedPooledArray<Velocity>.Rent(n1);
            using var life = ScopedPooledArray<Lifetime>.Rent(n1);
            using var rend = ScopedPooledArray<Renderable>.Rent(n1);
            using var spin = ScopedPooledArray<Spin>.Rent(n1);

            for (int i = 0, j = 0; i < count; i++)
            {
                if (cats[i] != 1) continue;
                pos[j] = new Position { X = 640, Y = 360 };
                vel[j] = allVel[i];
                life[j] = allLife[i];
                rend[j] = allRend[i];
                spin[j] = new Spin { Speed = allSpin[i] };
                j++;
            }

            world.CreateEntityBulk(ents.AsSpan(), pos.AsSpan(), vel.AsSpan(),
                                   life.AsSpan(), rend.AsSpan(), spin.AsSpan());
        }

        if (n2 > 0)
        {
            using var ents = ScopedPooledArray<Entity>.Rent(n2);
            using var pos = ScopedPooledArray<Position>.Rent(n2);
            using var vel = ScopedPooledArray<Velocity>.Rent(n2);
            using var life = ScopedPooledArray<Lifetime>.Rent(n2);
            using var rend = ScopedPooledArray<Renderable>.Rent(n2);
            using var grav = ScopedPooledArray<Gravity>.Rent(n2);

            for (int i = 0, j = 0; i < count; i++)
            {
                if (cats[i] != 2) continue;
                pos[j] = new Position { X = 640, Y = 360 };
                vel[j] = allVel[i];
                life[j] = allLife[i];
                rend[j] = allRend[i];
                grav[j] = new Gravity { G = 0.05f };
                j++;
            }

            world.CreateEntityBulk(ents.AsSpan(), pos.AsSpan(), vel.AsSpan(),
                                   life.AsSpan(), rend.AsSpan(), grav.AsSpan());
        }

        if (n3 > 0)
        {
            using var ents = ScopedPooledArray<Entity>.Rent(n3);
            using var pos = ScopedPooledArray<Position>.Rent(n3);
            using var vel = ScopedPooledArray<Velocity>.Rent(n3);
            using var life = ScopedPooledArray<Lifetime>.Rent(n3);
            using var rend = ScopedPooledArray<Renderable>.Rent(n3);
            using var spin = ScopedPooledArray<Spin>.Rent(n3);
            using var grav = ScopedPooledArray<Gravity>.Rent(n3);

            for (int i = 0, j = 0; i < count; i++)
            {
                if (cats[i] != 3) continue;
                pos[j] = new Position { X = 640, Y = 360 };
                vel[j] = allVel[i];
                life[j] = allLife[i];
                rend[j] = allRend[i];
                spin[j] = new Spin { Speed = allSpin[i] };
                grav[j] = new Gravity { G = 0.05f };
                j++;
            }

            world.CreateEntityBulk(ents.AsSpan(), pos.AsSpan(), vel.AsSpan(),
                                   life.AsSpan(), rend.AsSpan(), spin.AsSpan(), grav.AsSpan());
        }
    }

    private void SpawnMutators(int count)
    {
        using var ents = ScopedPooledArray<Entity>.Rent(count);
        using var pos = ScopedPooledArray<Position>.Rent(count);
        using var vel = ScopedPooledArray<Velocity>.Rent(count);
        using var life = ScopedPooledArray<Lifetime>.Rent(count);
        using var mut = ScopedPooledArray<Mutator>.Rent(count);
        using var rend = ScopedPooledArray<Renderable>.Rent(count);

        for (int i = 0; i < count; i++)
        {
            pos[i] = new Position { X = (float)(rng.NextDouble() * 1280), Y = 720 };
            vel[i] = new Velocity
            {
                DX = (float)(rng.NextDouble() * 4 - 2),
                DY = -(float)(rng.NextDouble() * 5 + 2)
            };
            life[i] = new Lifetime { Ticks = 400 };
            mut[i] = new Mutator { Threshold = rng.Next(150, 250) };
            rend[i] = new Renderable { SpriteType = 1, ColorIndex = 1, Scale = 1.5f };
        }

        world.CreateEntityBulk(ents.AsSpan(), pos.AsSpan(), vel.AsSpan(),
                               life.AsSpan(), mut.AsSpan(), rend.AsSpan());
    }

    private void ClearRandomHalf()
    {
        var query = world.GetOrCreateQuery(in QueryDescriptor.MatchAll);
        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                var entities = chunk.Entities;
                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    if (rng.Next(2) == 0) entitiesToDestroy.Add(entities.UnsafeAt(i));
                }
            }
        }
    }

    private bool IsKeyPressed(Keys k) => prevKb.IsKeyUp(k) && currKb.IsKeyDown(k);

    protected override void Update(GameTime gameTime)
    {
        long updateStart = Stopwatch.GetTimestamp();

        currKb = Keyboard.GetState();

        if (IsKeyPressed(Keys.P)) isPaused = !isPaused;
        if (IsKeyPressed(Keys.K)) { if (!isPaused) isPaused = true; stepRequested = true; }

        // ── Spawn (计时) ──
        double spawnUs = 0;
        if (IsKeyPressed(Keys.D1)) { Stamp(); SpawnWave(10000); spawnUs += Elapsed(); }
        if (IsKeyPressed(Keys.D2)) { Stamp(); SpawnWave(50000); spawnUs += Elapsed(); }
        if (IsKeyPressed(Keys.D3)) { Stamp(); SpawnMutators(10000); spawnUs += Elapsed(); }
        if (spawnUs > 0) tSpawn = spawnUs;

        if (IsKeyPressed(Keys.D4)) ClearRandomHalf();
        if (IsKeyPressed(Keys.D5))
        {
            var query = world.GetOrCreateQuery(in QueryDescriptor.MatchAll);
            foreach (var archetype in query.GetMatchingArchetypesSpan())
            {
                if (!archetype.Has<Spin>())
                {
                    foreach (ref var chunk in archetype.Chunks.AsSpan())
                    {
                        var entities = chunk.Entities;
                        for (int i = 0; i < chunk.EntityCount; i++)
                        {
                            if (rng.Next(10) == 0)
                                world.AddComponent(entities.UnsafeAt(i), new Spin { Speed = 0.05f });
                        }
                    }
                }
            }
        }

        prevKb = currKb;

        fpsTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (fpsTimer >= 1.0)
        {
            currentFps = fpsCounter;
            fpsCounter = 0;
            fpsTimer--;
        }

        if (!isPaused || stepRequested)
        {
            stepRequested = false;
            frameCount++;

            // ── [系统 1] 生命周期 ──
            Stamp();
            {
                var lifeDesc = new QueryDescriptor().WithAll<Lifetime>();
                var lifeQuery = world.GetOrCreateQuery(in lifeDesc);
                foreach (var arch in lifeQuery.GetMatchingArchetypesSpan())
                {
                    foreach (ref var chunk in arch.Chunks.AsSpan())
                    {
                        chunk.GetFilledComponentSpan<Lifetime>(out var lSpan);
                        var entities = chunk.Entities;
                        for (int i = 0; i < chunk.EntityCount; i++)
                        {
                            ref var l = ref lSpan[i];
                            l.Ticks--;
                            if (l.Ticks <= 0) entitiesToDestroy.Add(entities.UnsafeAt(i));
                        }
                    }
                }
            }
            tLifetime = Elapsed();

            // ── [系统 2] 突变查询 ──
            Stamp();
            {
                var mutateDesc = new QueryDescriptor().WithAll<Lifetime, Mutator>();
                var mutateQuery = world.GetOrCreateQuery(in mutateDesc);
                foreach (var arch in mutateQuery.GetMatchingArchetypesSpan())
                {
                    foreach (ref var chunk in arch.Chunks.AsSpan())
                    {
                        chunk.GetComponentSpan<Lifetime, Mutator>(out var lSpan, out var mSpan);
                        var entities = chunk.Entities;
                        for (int i = 0; i < chunk.EntityCount; i++)
                        {
                            if (lSpan[i].Ticks == mSpan[i].Threshold)
                                entitiesToMutate.Add(entities.UnsafeAt(i));
                        }
                    }
                }
            }
            tMutateQuery = Elapsed();

            // ── [系統 3] 重力 ──
            Stamp();
            {
                var gravDesc = new QueryDescriptor().WithAll<Velocity, Gravity>();
                var gravQuery = world.GetOrCreateQuery(in gravDesc);
                foreach (var arch in gravQuery.GetMatchingArchetypesSpan())
                {
                    foreach (ref var chunk in arch.Chunks.AsSpan())
                    {
                        chunk.GetComponentSpan<Velocity, Gravity>(out var vSpan, out var gSpan);
                        for (int i = 0; i < chunk.EntityCount; i++)
                            vSpan[i].DY += gSpan[i].G;
                    }
                }
            }
            tGravity = Elapsed();

            // ── [系統 4] 运动 ──
            Stamp();
            {
                var velDesc = new QueryDescriptor().WithAll<Position, Velocity>();
                var velQuery = world.GetOrCreateQuery(in velDesc);
                foreach (var arch in velQuery.GetMatchingArchetypesSpan())
                {
                    foreach (ref var chunk in arch.Chunks.AsSpan())
                    {
                        chunk.GetComponentSpan<Position, Velocity>(out var pSpan, out var vSpan);
                        for (int i = 0; i < chunk.EntityCount; i++)
                        {
                            ref var p = ref pSpan[i];
                            ref var v = ref vSpan[i];
                            p.X += v.DX;
                            p.Y += v.DY;
                            if (p.X < 0 || p.X > 1280) v.DX = -v.DX;
                            if (p.Y < 0 || p.Y > 720) v.DY = -v.DY;
                        }
                    }
                }
            }
            tMovement = Elapsed();

            // ── [系统 5] 轨道 ──
            Stamp();
            {
                var orbitDesc = new QueryDescriptor().WithAll<Position, Orbit>();
                var orbitQuery = world.GetOrCreateQuery(in orbitDesc);
                foreach (var arch in orbitQuery.GetMatchingArchetypesSpan())
                {
                    foreach (ref var chunk in arch.Chunks.AsSpan())
                    {
                        chunk.GetComponentSpan<Position, Orbit>(out var pSpan, out var oSpan);
                        for (int i = 0; i < chunk.EntityCount; i++)
                        {
                            ref var p = ref pSpan[i];
                            ref var o = ref oSpan[i];
                            o.Angle += o.Speed;
                            p.X = o.CenterX + MathF.Cos(o.Angle) * o.Radius;
                            p.Y = o.CenterY + MathF.Sin(o.Angle) * o.Radius;
                            o.Radius += 0.5f;
                        }
                    }
                }
            }
            tOrbit = Elapsed();

            // ── [系统 6] 自旋 ──
            Stamp();
            {
                var spinDesc = new QueryDescriptor().WithAll<Spin>();
                var spinQuery = world.GetOrCreateQuery(in spinDesc);
                foreach (var arch in spinQuery.GetMatchingArchetypesSpan())
                {
                    foreach (ref var chunk in arch.Chunks.AsSpan())
                    {
                        chunk.GetFilledComponentSpan<Spin>(out var sSpan);
                        for (int i = 0; i < chunk.EntityCount; i++)
                            sSpan[i].Angle += sSpan[i].Speed;
                    }
                }
            }
            tSpin = Elapsed();

            // ── 突变执行 ──
            Stamp();
            foreach (var e in entitiesToMutate)
            {
                if (!world.IsAlive(e)) continue;

                ref var p = ref world.GetComponent<Position>(e);

                if (world.HasComponent<Velocity>(e)) world.RemoveComponent<Velocity>(e);
                if (world.HasComponent<Gravity>(e)) world.RemoveComponent<Gravity>(e);

                world.AddComponent(e, new Orbit
                {
                    CenterX = p.X,
                    CenterY = p.Y,
                    Radius = 10f,
                    Angle = (float)rng.NextDouble() * 10f,
                    Speed = 0.1f
                });

                if (world.HasComponent<Renderable>(e))
                {
                    ref var r = ref world.GetComponent<Renderable>(e);
                    r.SpriteType = 0;
                    r.ColorIndex = 3;
                }
            }
            entitiesToMutate.Clear();
            tMutateApply = Elapsed();

            // ── 销毁 ──
            Stamp();
            foreach (var e in entitiesToDestroy)
                if (world.IsAlive(e)) world.DestroyEntity(e);
            entitiesToDestroy.Clear();
            tDestroy = Elapsed();
        }

        tUpdateTotal = (Stopwatch.GetTimestamp() - updateStart) * TicksToUs;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        long drawStart = Stopwatch.GetTimestamp();

        fpsCounter++;

        GraphicsDevice.Clear(new Color(15, 10, 30));
        stgBatch.Begin(SamplerState.PointClamp, RasterizerState.CullCounterClockwise, null, projection);

        var renderDesc = new QueryDescriptor().WithAll<Position, Renderable>();
        var renderQuery = world.GetOrCreateQuery(in renderDesc);

        foreach (var archetype in renderQuery.GetMatchingArchetypesSpan())
        {
            bool hasSpin = archetype.Has<Spin>();
            bool hasVelocity = archetype.Has<Velocity>();
            bool hasOrbit = archetype.Has<Orbit>();

            var blend = hasOrbit ? StgBlendState.Additive : StgBlendState.Alpha;

            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<Position, Renderable>(out var pSpan, out var rSpan);

                Span<Spin> sSpan = default;
                Span<Velocity> vSpan = default;
                if (hasSpin) chunk.GetFilledComponentSpan<Spin>(out sSpan);
                if (hasVelocity) chunk.GetFilledComponentSpan<Velocity>(out vSpan);

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    ref var p = ref pSpan[i];
                    ref var r = ref rSpan[i];

                    float rot = 0f;
                    if (hasSpin)
                        rot = sSpan[i].Angle;
                    else if (hasVelocity)
                        rot = MathF.Atan2(vSpan[i].DY, vSpan[i].DX) + MathF.PI / 2f;

                    SpriteAsset sprite = r.SpriteType == 0
                        ? hearts[r.ColorIndex]
                        : arrows[r.ColorIndex];

                    stgBatch.Draw(
                        sprite.Texture,
                        new Vector2(p.X, p.Y),
                        sprite.SourceRect,
                        Color.White, rot, sprite.Anchor,
                        Vector2.One * r.Scale,
                        SpriteEffects.None, 0, blend);
                }
            }
        }

        // ── HUD ──
        var font = assetManager.Load<FontAsset>("fonts/mspgothic.ttf", "touhou_font").GetFont(18);

        var countedEntities = world.CountEntities(QueryDescriptor.MatchAll);
        var countedArchetypes = world.CountArchetypes(QueryDescriptor.MatchAll);
        var counterChunks = world.CountChunks(QueryDescriptor.MatchAll);

        Color fpsColor = currentFps < 58 ? Color.Red : Color.LimeGreen;

        stgBatch.DrawString(font,
            $"Entities: {countedEntities} | Archetypes: {countedArchetypes} | Chunks: {counterChunks} | Frame: {frameCount}",
            new Vector2(8, 4), Color.Blue, 200, StgBlendState.Alpha);
        stgBatch.DrawString(font, $"[ FPS: {currentFps} ]",
            new Vector2(1150, 4), fpsColor, 200, StgBlendState.Alpha);
        stgBatch.DrawString(font,
            "1:+10K弾幕  2:+50K弾幕  3:+10K突変弾  4:随機刪半  5:随機加自旋  P:暫停  K:単歩",
            new Vector2(8, 26), Color.Yellow, 200, StgBlendState.Alpha);

        // ── Profile 面板 ──
        tDraw = (Stopwatch.GetTimestamp() - drawStart) * TicksToUs;

        profileSb.Clear();
        profileSb.AppendLine($"── Profile (us) ──");
        profileSb.AppendLine($"Spawn:     {tSpawn,9:F1}");
        profileSb.AppendLine($"Lifetime:  {tLifetime,9:F1}");
        profileSb.AppendLine($"MutQuery:  {tMutateQuery,9:F1}");
        profileSb.AppendLine($"Gravity:   {tGravity,9:F1}");
        profileSb.AppendLine($"Movement:  {tMovement,9:F1}");
        profileSb.AppendLine($"Orbit:     {tOrbit,9:F1}");
        profileSb.AppendLine($"Spin:      {tSpin,9:F1}");
        profileSb.AppendLine($"MutApply:  {tMutateApply,9:F1}");
        profileSb.AppendLine($"Destroy:   {tDestroy,9:F1}");
        profileSb.AppendLine($"Update:    {tUpdateTotal,9:F1}");
        profileSb.Append($"Draw:      {tDraw,9:F1}");

        stgBatch.DrawString(font, profileSb.ToString(),
            new Vector2(1060, 50), Color.White, 200, StgBlendState.Alpha);

        stgBatch.End();
        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        world?.Dispose();
        base.UnloadContent();
    }
}
