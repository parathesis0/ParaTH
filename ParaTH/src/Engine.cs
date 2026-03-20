using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ParaTH;

public sealed class TestScript(BulletManager bulletManager)
{
    int counter;

    public void Update(AssetManager assetManager)
    {
        var heart = assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "heart_pink");
        var arrow = assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "arrow_pink");

        if (counter % 5 == 0)
        {
            float way = 4;
            float angleOffset = counter / 10f;
            for (int i = 0; i < way; i++)
            {
                float angle = angleOffset + (MathHelper.TwoPi / way * i);

                // curve Test

                //bulletManager.SpawnBullet(new Vector2(640, 360))
                //    .SetSprite(heart, Color.White, 100, StgBlendState.Alpha)
                //    .SetMovement(2f, Vector2.Zero, angle).EnableSyncTransformRotation()
                //        .SetAngularVelocity(MathHelper.Pi / 60).DelayAngularVelocity(30)
                //        .SetAngularVelocity(-MathHelper.Pi / 60).DelayAngularVelocity(30)
                //        .AngularVelocityLoopFrom(0, 1)
                //    .Build();

                bulletManager.SpawnBullet(new Vector2(640, 360))
                        .SetSprite(heart, Color.White, 100, StgBlendState.Alpha)
                        .SetMovement(2f, Vector2.Zero, angle).EnableSyncTransformRotation()
                            .DelayVelocity(60)
                            //.LerpVelocityRelative(2f, MathHelper.Pi, 10, Easing.Linear)
                            .LerpVelocityTo(Vector2.UnitY, 10, Easing.Linear)
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

    private MovementSystem movementSystem = null!;
    private RenderSystem renderSystem = null!;

    private TestScript script = null!;

    private InputManager inputManager = InputManager.Instance;
    bool isPaused = false;
    bool shouldAdvance = false;

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

        world = new World(
            baseChunkByteSize: 16384,
            baseChunkEntityCount: 100,
            initialArchetypeCapacity: 2,
            initialEntityCapacity: 50000);

        bulletManager = new BulletManager(world);

        movementSystem = new MovementSystem(world);
        renderSystem = new RenderSystem(world, stgBatch);

        script = new(bulletManager);
    }

    protected override void Update(GameTime gameTime)
    {
        if (inputManager.IsKeyPressed(Keys.P))
            isPaused = !isPaused;

        if (inputManager.IsKeyPressed(Keys.K))
            shouldAdvance = true;

        if (!isPaused || shouldAdvance)
        {
            script.Update(assetManager);
            movementSystem.Update();
        }

        shouldAdvance = false;
        inputManager.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(15, 10, 30));
        stgBatch.Begin(SamplerState.PointClamp, RasterizerState.CullCounterClockwise,
                       null, projection);

        renderSystem.Update();

        stgBatch.End();
        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        world?.Dispose();
        base.UnloadContent();
    }
}
