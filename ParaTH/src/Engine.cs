using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ParaTH;

public struct Position { public float X; public float Y; }
public struct Velocity { public float DX; public float DY; }
public struct Spin { public float Angle; public float Speed; }

public sealed class Engine : Game
{
    private AssetManager assetManager = null!;
    private StgBatch stgBatch = null!;
    private Matrix projection;

    private World world = null!;
    private SpriteAsset bullet = null!;
    private readonly List<Entity> tracked = new();
    private readonly Random rng = new(12345);

    private int frameCount;
    private bool isPaused;
    private bool stepRequested;
    private KeyboardState currKb;
    private KeyboardState prevKb;

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

        bullet = assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "heart_pink");

        world = new World(
            baseChunkByteSize: 16384,
            baseChunkEntityCount: 128,
            initialArchetypeCapacity: 8,
            initialEntityCapacity: 512);

        for (int i = 0; i < 15; i++) SpawnStatic();
        for (int i = 0; i < 45; i++) SpawnMoving();
    }

    private void SpawnStatic()
    {
        var e = world.CreateEntity(new Position
        {
            X = 100f + (float)(rng.NextDouble() * 1080),
            Y = 100f + (float)(rng.NextDouble() * 520)
        });
        tracked.Add(e);
    }

    private void SpawnMoving()
    {
        var e = world.CreateEntity(new Position
        {
            X = 100f + (float)(rng.NextDouble() * 1080),
            Y = 100f + (float)(rng.NextDouble() * 520)
        });
        world.AddComponent(e, new Velocity
        {
            DX = (float)(rng.NextDouble() * 3 - 1.5),
            DY = (float)(rng.NextDouble() * 3 - 1.5)
        });
        if (rng.Next(3) == 0)
            world.AddComponent(e, new Spin { Speed = (float)(rng.NextDouble() * 0.15 - 0.075) });
        tracked.Add(e);
    }

    private void DestroyRandom()
    {
        if (tracked.Count == 0) return;
        int idx = rng.Next(tracked.Count);
        if (world.IsAlive(tracked[idx]))
            world.DestroyEntity(tracked[idx]);
        tracked.RemoveAt(idx);
    }

    private bool IsKeyPressed(Keys k) => prevKb.IsKeyUp(k) && currKb.IsKeyDown(k);

    protected override void Update(GameTime gameTime)
    {
        currKb = Keyboard.GetState();

        if (IsKeyPressed(Keys.P)) isPaused = !isPaused;
        if (IsKeyPressed(Keys.K)) { if (!isPaused) isPaused = true; stepRequested = true; }

        if (IsKeyPressed(Keys.D1)) SpawnStatic();
        if (IsKeyPressed(Keys.D2)) SpawnMoving();
        if (IsKeyPressed(Keys.D3)) for (int i = 0; i < 30; i++) SpawnMoving();
        if (IsKeyPressed(Keys.D4)) DestroyRandom();
        if (IsKeyPressed(Keys.D5)) for (int i = 0; i < 30; i++) DestroyRandom();

        // toggle spin on a random entity
        if (IsKeyPressed(Keys.D6) && tracked.Count > 0)
        {
            var e = tracked[rng.Next(tracked.Count)];
            if (world.IsAlive(e))
            {
                if (world.HasComponent<Spin>(e))
                    world.RemoveComponent<Spin>(e);
                else
                    world.AddComponent(e, new Spin { Speed = 0.1f });
            }
        }

        // add velocity to a random static entity
        if (IsKeyPressed(Keys.D7) && tracked.Count > 0)
        {
            var e = tracked[rng.Next(tracked.Count)];
            if (world.IsAlive(e) && !world.HasComponent<Velocity>(e))
                world.AddComponent(e, new Velocity { DX = 2f, DY = -1.5f });
        }

        // remove velocity from a random moving entity
        if (IsKeyPressed(Keys.D8) && tracked.Count > 0)
        {
            var e = tracked[rng.Next(tracked.Count)];
            if (world.IsAlive(e) && world.HasComponent<Velocity>(e))
                world.RemoveComponent<Velocity>(e);
        }

        prevKb = currKb;

        if (!isPaused || stepRequested)
        {
            stepRequested = false;
            frameCount++;

            // movement system
            var velDesc = new QueryDescriptor();
            velDesc.WithAll<Velocity>();
            world.Query(velDesc, e =>
            {
                ref var p = ref world.GetComponent<Position>(e);
                ref var v = ref world.GetComponent<Velocity>(e);
                p.X += v.DX;
                p.Y += v.DY;
                if (p.X < 0 || p.X > 1280) v.DX = -v.DX;
                if (p.Y < 0 || p.Y > 720) v.DY = -v.DY;
            });

            // spin system
            var spinDesc = new QueryDescriptor();
            spinDesc.WithAll<Spin>();
            world.Query(spinDesc, e =>
            {
                ref var s = ref world.GetComponent<Spin>(e);
                s.Angle += s.Speed;
            });
        }

        tracked.RemoveAll(e => !world.IsAlive(e));

        Window.Title = isPaused
            ? $"F{frameCount} | alive:{tracked.Count} [PAUSED — K:step P:resume]"
            : $"F{frameCount} | alive:{tracked.Count}";

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(15, 10, 30));
        stgBatch.Begin(SamplerState.PointClamp, RasterizerState.CullCounterClockwise, null, projection);

        var posDesc = new QueryDescriptor();
        posDesc.WithAll<Position>();
        world.Query(posDesc, e =>
        {
            ref var p = ref world.GetComponent<Position>(e);

            float rot = 0f;
            if (world.HasComponent<Spin>(e))
                rot = world.GetComponent<Spin>(e).Angle;

            bool moving = world.HasComponent<Velocity>(e);
            var color = moving ? Color.Cyan : Color.Yellow;
            var blend = moving ? StgBlendState.Additive : StgBlendState.Alpha;

            stgBatch.Draw(
                bullet.Texture,
                new Vector2(p.X, p.Y),
                bullet.SourceRect,
                color, rot, bullet.Anchor,
                Vector2.One * 1.5f,
                SpriteEffects.None, 0, blend);
        });

        var font = assetManager.Load<FontAsset>("fonts/mspgothic.ttf", "touhou_font").GetFont(18);

        var counted = world.CountEntities(QueryDescriptor.MatchAll);

        stgBatch.DrawString(font,
            $"entities: {tracked.Count}   frame: {frameCount}   counted entities: {counted}",
            new Vector2(8, 4), Color.White, 200, StgBlendState.Alpha);
        stgBatch.DrawString(font,
            "1:静止弾  2:移動弾  3:+30  4:削除  5:-30  6:spin切替  7:加速  8:停止  P:pause  K:step",
            new Vector2(8, 26), Color.Gray, 200, StgBlendState.Alpha);

        stgBatch.End();
        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        world?.Dispose();
        base.UnloadContent();
    }
}
