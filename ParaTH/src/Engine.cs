using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed class TestScript(BulletManager bulletManager)
{
    int counter;

    public void Update()
    {
        float angleOffset = counter / 10f;

        if (counter % 4 == 0)
        {
            float way = 5;
            for (int i = 0; i < way; i++)
            {
                var angle = angleOffset + (MathHelper.TwoPi / way * i);
                var delta = new Vector2(
                    100f * MathF.Cos(angle),
                    100f * MathF.Sin(angle));

                //// position test
                //bulletManager.SpawnBullet()
                //    .SetPosition(new Vector2(200, 200))
                //    .SetSprite("heart_pink", Color.White, 100, StgBlendState.Additive)
                //    .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                //    .LerpAddPosition(delta, 120, EaseType.OutQuad)
                //    .LerpAddPosition(-delta, 120, EaseType.InQuad)
                //    .SetVelocity(delta / 30f)
                //    .AutoSyncRenderStateRotation()
                //    .Build();

                //// velocity test
                //bulletManager.SpawnBullet()
                //    .SetPosition(new Vector2(320, 240))
                //    .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
                //    .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                //    .SetVelocity(2f, angle).AutoSyncRenderStateRotation()
                //    .Delay(60)
                //    .SetVelocity(Vector2.UnitY * 2).Delay(10)
                //    .AddVelocity(Vector2.UnitY * 2).Delay(10)
                //    .LerpToVelocity(Vector2.UnitY * 2, 30, EaseType.InQuad).Delay(10)
                //    .LerpAddVelocity(Vector2.UnitY * 2, 30, EaseType.InQuad).Delay(10)
                //    .SetVelocityMagnitude(4f).Delay(10)
                //    .AddVelocityMagnitude(4f).Delay(10)
                //    .LerpToVelocityMagnitude(4f, 30, EaseType.InQuad).Delay(10)
                //    .LerpAddVelocityMagnitude(4f, 30, EaseType.InQuad).Delay(10)
                //    .SetVelocityAngle(0).Delay(10)
                //    .AddVelocityAngle(MathHelper.PiOver2).Delay(10)
                //    .LerpToVelocityAngle(0, 30, EaseType.InQuad).Delay(10)
                //    .LerpAddVelocityAngle(MathHelper.PiOver2, 30, EaseType.InQuad).Delay(10)
                //    .Build();

                //// acceleration test
                //bulletManager.SpawnBullet()
                //    .SetPosition(new Vector2(320, 240))
                //    .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
                //    .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                //    .SetVelocity(2f, angle).AutoSyncRenderStateRotation()
                //    .Delay(60)
                //    .SetAcceleration(Vector2.UnitY * 0.05f)
                //    .Build();

                //// curve Test
                //bulletManager.SpawnBullet()
                //    .SetPosition(new Vector2(320, 240))
                //    .SetSprite("heart_pink", Color.White, 100, StgBlendState.Alpha)
                //    .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                //    .SetVelocity(2f, angle).AutoSyncRenderStateRotation()
                //    .SetAngularVelocity(MathHelper.Pi / 60).Delay(30)
                //    .SetAngularVelocity(-MathHelper.Pi / 60).Delay(30)
                //    .SetAngularVelocity(MathHelper.Pi / 60).Delay(30)
                //    .SetAngularVelocity(-MathHelper.Pi / 60).Delay(30)
                //    .SetAngularVelocity(MathHelper.Pi / 60).Delay(30)
                //    .SetAngularVelocity(-MathHelper.Pi / 60).Delay(30)
                //    .SetAngularVelocity(0)
                //    .Build();

                //// spawnAnimation test
                //bulletManager.SpawnBullet()
                //    .SetPosition(new Vector2(320, 240))
                //    .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                //    .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
                //    .SetAnimation("fireball_red", Color.White, 100, StgBlendState.Alpha)
                //    .SetVelocity(2f, angle).LerpAddVelocityMagnitude(12f, 120, EaseType.Linear).AutoSyncRenderStateRotation()
                //    .Build();
            }
        }

        if (counter == 10)
        {
            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(400, 400))
                .SetSprite("bigball_red", Color.White, 90, StgBlendState.Additive)
                .SetCircleCollider(16f).SetCollisionGroup(0b0000_0001).SetTargetGroup(0b0000_0010)
                .Build();
        }

        if (counter % 1 == 0)
        {
            //    // spawn control test
            //    bulletManager.SpawnBullet()
            //        .SetPosition(new Vector2(320, 240))
            //        .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
            //        .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
            //        .SetAnimation("fireball_red", Color.White, 100, StgBlendState.Alpha)
            //        .SetVelocity(2f, 0)
            //        .SetSpawningCircle(8, 4, 0.5f, 0, 0.01f, 100)
            //        .SetSpawningSpreadByDelta(9, MathHelper.Pi / 8, 3)
            //        .SetSpawningSpreadByTotal(9, MathHelper.Pi, 3, 0.1f)
            //        .Build();

            // spawnAnimation test
            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(320, 240))
                .SetSpawnAnimation("mist_red", 2, 0, 0, 11, EaseType.Linear)
                .SetSprite("heart_red", Color.White, 100, StgBlendState.Alpha)
                .SetVelocity(2f, angleOffset).SetSpawningCircle(500)
                .LerpAddVelocityMagnitude(4f, 6, EaseType.Linear)//.SyncRenderStateRotation()
                .SetCircleCollider(4f).SetCollisionGroup(0b0000_0010)
                .Build();
        }

        counter++;
    }
}

public sealed class Engine : Game
{
    private AssetManager assetManager = null!;
    private StgBatch stgBatch = null!;
    private Matrix projection;
    private World world = null!;

    private BulletManager bulletManager = null!;

    private MovementSystem movementSystem = null!;
    private AnimationSystem animationSystem = null!;
    private RenderSystem renderSystem = null!;
    private CollisionSystem collisionSystem = null!;
    private LifetimeSystem lifetimeSystem = null!;

    private Rectangle gameBounds = new(0, 0, 640, 480); // new(640 / 4, 480 / 4, 640 / 2, 480 / 2);

    private TestScript script = null!;

    private InputManager Input { get; } = InputManager.Instance;
    bool isPaused = false;
    bool shouldAdvance = false;

    private double fpsTimer;
    private int fpsCounter;
    private int currentFps;

    private bool laserStop = false;
    private int laserMovementCounter;
    private SpriteAsset testLaserSprite = null!;
    private UnsafePooledQueue<Vector2> laserNodeQueue = new();
    private int maxNodes = 64;

    private FontAsset debugFontAsset = null!;

    public Engine()
    {
        var gdm = new GraphicsDeviceManager(this);
        gdm.PreferredBackBufferWidth = 640;
        gdm.PreferredBackBufferHeight = 480;
        gdm.SynchronizeWithVerticalRetrace = false;
        gdm.GraphicsProfile = GraphicsProfile.HiDef;
        IsMouseVisible = false;
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
        assetManager.RegisterLoader(new AnimationAssetLoader(assetManager));

        assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "heart_pink");
        assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "arrow_pink");

        assetManager.Load<AnimationAsset>("bullet/bullet_animations.txt", "fireball_red");

        debugFontAsset = assetManager.Load<FontAsset>("fonts/mspgothic.ttf", "touhou_font");

        stgBatch = new StgBatch(GraphicsDevice);
        projection = Matrix.CreateOrthographicOffCenter(0, 640, 480, 0, 0, 1);

        world = new World(
            baseChunkByteSize: 16384,
            baseChunkEntityCount: 100,
            initialArchetypeCapacity: 2,
            initialEntityCapacity: 50000);

        bulletManager = new BulletManager(world, assetManager);

        movementSystem = new MovementSystem(world);
        animationSystem = new AnimationSystem(world);
        renderSystem = new RenderSystem(world, stgBatch, gameBounds);
        collisionSystem = new CollisionSystem(world);
        lifetimeSystem = new LifetimeSystem(world, gameBounds);

        script = new(bulletManager);

        testLaserSprite = assetManager.Load<SpriteAsset>("bullet/laser_sprites.txt", "curvelaser_lime");
        //testLaserSprite = assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "mediumball_blue");
    }

    protected override void Update(GameTime gameTime)
    {
        if (Input.IsKeyPressed(Keys.P))
            isPaused = !isPaused;

        if (Input.IsKeyPressed(Keys.K))
            shouldAdvance = true;

        if (Input.IsKeyPressed(Keys.L))
            laserStop = !laserStop;

        fpsTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (fpsTimer >= 1.0)
        {
            currentFps = fpsCounter;
            fpsCounter = 0;
            fpsTimer -= 1.0;
        }

        // curvy laser test
        if (!isPaused || shouldAdvance)
        {
            var ms = Mouse.GetState();
            Vector2 currentEmitterPos = new Vector2(ms.X, ms.Y);

            //if (!laserStop)
            //    laserMovementCounter++;
            //float angle = laserMovementCounter / 40f;
            //float radius = 100f + 50 * MathF.Sin(laserMovementCounter / 5f);
            //Vector2 center = new(320, 240);
            //Vector2 currentEmitterPos = center + new Vector2(
            //    radius * MathF.Cos(angle),
            //    radius * MathF.Sin(angle));

            laserNodeQueue.Enqueue(currentEmitterPos);

            while (laserNodeQueue.Count > maxNodes)
                laserNodeQueue.Dequeue();
        }

        if (!isPaused || shouldAdvance)
        {
            if (currentFps > 58)
            {
                //script.Update();
            }
            animationSystem.Update();
            movementSystem.Update();
            lifetimeSystem.Update();
            collisionSystem.Update();
        }

        shouldAdvance = false;
        Input.Update();

        base.Update(gameTime);
    }

    [SkipLocalsInit]
    protected override void Draw(GameTime gameTime)
    {
        fpsCounter++;

        GraphicsDevice.Clear(new Color(15, 10, 30));
        stgBatch.Begin(SamplerState.PointClamp, RasterizerState.CullCounterClockwise,
                       null, projection);

        renderSystem.Update();

        var font = debugFontAsset.GetFont(18);

        var entityCount = world.CountEntities(QueryDescriptor.MatchAll);
        var archetypeCount = world.CountArchetypes(QueryDescriptor.MatchAll);
        var chunkCount = world.CountChunks(QueryDescriptor.MatchAll);

        Color fpsColor = currentFps < 58 ? Color.Red : Color.LimeGreen;

        laserNodeQueue.AsSpans(out var first, out var second);

        Span<Vector2> renderNodeSpan = stackalloc Vector2[laserNodeQueue.Count];

        if (second == Span<Vector2>.Empty)
        {
            renderNodeSpan = first;
        }
        else
        {
            first.CopyTo(renderNodeSpan);
            second.CopyTo(renderNodeSpan.Slice(first.Length));
        }

        stgBatch.DrawCurvyLaser(
            testLaserSprite.Texture,
            testLaserSprite.SourceRect,
            MathHelper.Pi,
            renderNodeSpan,
            16,
            Color.White,
            150,
            StgBlendState.Alpha
        );

        stgBatch.DrawString(font,
            $"Entities: {entityCount}  |  Archetypes: {archetypeCount}  |  Chunks: {chunkCount}",
            new Vector2(8, 4), Color.Gray, 200, StgBlendState.Alpha);

        stgBatch.DrawString(font,
            $"FPS: {currentFps}",
            new Vector2(572, 4), fpsColor, 200, StgBlendState.Alpha);

        stgBatch.End();
        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        world?.Dispose();
        base.UnloadContent();
    }
}
