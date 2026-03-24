using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ParaTH;

public sealed class TestScript(BulletManager bulletManager)
{
    int counter;

    public void Update()
    {
        if (counter % 1 == 0)
        {
            float way = 250;
            float angleOffset = counter / 10f;
            for (int i = 0; i < way; i++)
            {
                var angle = angleOffset + (MathHelper.TwoPi / way * i);
                var delta = new Vector2(
                    100f * MathF.Cos(angle),
                    100f * MathF.Sin(angle));

                // position test
                bulletManager.SpawnBullet()
                    .SetPosition(new Vector2(200, 200))
                    .SetSprite("heart_pink", Color.White, 100, StgBlendState.Additive)
                    .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                    .LerpAddPosition(delta, 120, EaseType.OutQuad)
                    .LerpAddPosition(-delta, 120, EaseType.InQuad)
                    .SetVelocity(delta / 30f)
                    .AutoSyncTransformRotation()
                    .Build();

                // velocity test
                bulletManager.SpawnBullet()
                    .SetPosition(new Vector2(640, 360))
                    .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
                    .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                    .SetVelocity(2f, angle).AutoSyncTransformRotation()
                    .Delay(60)
                    .SetVelocity(Vector2.UnitY * 2).Delay(10)
                    .AddVelocity(Vector2.UnitY * 2).Delay(10)
                    .LerpToVelocity(Vector2.UnitY * 2, 30, EaseType.InQuad).Delay(10)
                    .LerpAddVelocity(Vector2.UnitY * 2, 30, EaseType.InQuad).Delay(10)
                    .SetVelocityMagnitude(4f).Delay(10)
                    .AddVelocityMagnitude(4f).Delay(10)
                    .LerpToVelocityMagnitude(4f, 30, EaseType.InQuad).Delay(10)
                    .LerpAddVelocityMagnitude(4f, 30, EaseType.InQuad).Delay(10)
                    .SetVelocityAngle(0).Delay(10)
                    .AddVelocityAngle(MathHelper.PiOver2).Delay(10)
                    .LerpToVelocityAngle(0, 30, EaseType.InQuad).Delay(10)
                    .LerpAddVelocityAngle(MathHelper.PiOver2, 30, EaseType.InQuad).Delay(10)
                    .Build();

                // acceleration test
                bulletManager.SpawnBullet()
                    .SetPosition(new Vector2(640, 360))
                    .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
                    .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                    .SetVelocity(2f, angle).AutoSyncTransformRotation()
                    .Delay(60)
                    .SetAcceleration(Vector2.UnitY * 0.05f)
                    .Build();

                // curve Test
                bulletManager.SpawnBullet()
                    .SetPosition(new Vector2(640, 360))
                    .SetSprite("heart_pink", Color.White, 100, StgBlendState.Alpha)
                    .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                    .SetVelocity(2f, angle).AutoSyncTransformRotation()
                    .SetAngularVelocity(MathHelper.Pi / 60).Delay(30)
                    .SetAngularVelocity(-MathHelper.Pi / 60).Delay(30)
                    .SetAngularVelocity(MathHelper.Pi / 60).Delay(30)
                    .SetAngularVelocity(-MathHelper.Pi / 60).Delay(30)
                    .SetAngularVelocity(MathHelper.Pi / 60).Delay(30)
                    .SetAngularVelocity(-MathHelper.Pi / 60).Delay(30)
                    .SetAngularVelocity(0)
                    .Build();

                // spawnAnimation test
                bulletManager.SpawnBullet()
                    .SetPosition(new Vector2(640, 360))
                    .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                    .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
                    .SetVelocity(2f, angle).AutoSyncTransformRotation()
                    .Build();
            }
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

    private BulletSystem bulletSystem = null!;
    private RenderSystem renderSystem = null!;

    private TestScript script = null!;

    private InputManager Input { get; } = InputManager.Instance;
    bool isPaused = false;
    bool shouldAdvance = false;

    private double fpsTimer;
    private int fpsCounter;
    private int currentFps;

    private FontAsset debugFontAsset = null!;

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

        assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "heart_pink");
        assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "arrow_pink");

        debugFontAsset = assetManager.Load<FontAsset>("fonts/mspgothic.ttf", "touhou_font");

        stgBatch = new StgBatch(GraphicsDevice);
        projection = Matrix.CreateOrthographicOffCenter(0, 1280, 720, 0, 0, 1);

        world = new World(
            baseChunkByteSize: 16384,
            baseChunkEntityCount: 100,
            initialArchetypeCapacity: 2,
            initialEntityCapacity: 50000);

        bulletManager = new BulletManager(world, assetManager);

        bulletSystem = new BulletSystem(world);
        renderSystem = new RenderSystem(world, stgBatch);

        script = new(bulletManager);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Input.IsKeyPressed(Keys.P))
            isPaused = !isPaused;

        if (Input.IsKeyPressed(Keys.K))
            shouldAdvance = true;

        fpsTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (fpsTimer >= 1.0)
        {
            currentFps = fpsCounter;
            fpsCounter = 0;
            fpsTimer -= 1.0;
        }

        if (!isPaused || shouldAdvance)
        {
            if (currentFps > 55)
                script.Update();
            bulletSystem.Update();
        }

        shouldAdvance = false;
        Input.Update();

        base.Update(gameTime);
    }

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

        stgBatch.DrawString(font,
            $"Entities: {entityCount}  |  Archetypes: {archetypeCount}  |  Chunks: {chunkCount}",
            new Vector2(8, 4), Color.Gray, 200, StgBlendState.Alpha);

        stgBatch.DrawString(font,
            $"FPS: {currentFps}",
            new Vector2(1190, 4), fpsColor, 200, StgBlendState.Alpha);

        stgBatch.End();
        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        world?.Dispose();
        base.UnloadContent();
    }
}
