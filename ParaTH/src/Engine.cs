using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ParaTH;

public sealed class TestScript(BulletManager bulletManager, World world, Engine engine, AssetManager asset)
{
    Entity[] reimu = null!;
    Entity[] youmu = null!;

    int counter;

    public void Update()
    {
        float angleOffset = counter / 10f;

        if (counter % 1 == 0)
        {
            const float Way = 5;
            for (int i = 0; i < Way; i++)
            {
                var angle = angleOffset + (MathHelper.TwoPi / Way * i);
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
                    .SyncRendererRotation()
                    .Build();

                // velocity test
                bulletManager.SpawnBullet()
                    .SetPosition(new Vector2(320, 240))
                    .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
                    .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                    .SetVelocity(2f, angle)
                    .SyncRendererRotation()
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
                    .SetPosition(new Vector2(320, 240))
                    .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
                    .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                    .SetVelocity(2f, angle)
                    .SyncRendererRotation()
                    .Delay(60)
                    .SetAcceleration(Vector2.UnitY * 0.05f)
                    .Build();

                // curve Test
                bulletManager.SpawnBullet()
                    .SetPosition(new Vector2(320, 240))
                    .SetSprite("heart_pink", Color.White, 100, StgBlendState.Alpha)
                    .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                    .SetVelocity(2f, angle)
                    .SyncRendererRotation()
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
                    .SetPosition(new Vector2(320, 240))
                    .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                    .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
                    .SetAnimation("fireball_red", Color.White, 100, StgBlendState.Alpha)
                    .SetVelocity(2f, angle).LerpAddVelocityMagnitude(12f, 120, EaseType.Linear)
                    .SyncRendererRotation()
                    .Build();
            }
        }

        if (counter == 10)
        {
            // target test
            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(400, 400))
                .SetSprite("bigball_red", Color.White, 90, StgBlendState.Additive)
                .SetCircleCollider(16f).SetCollisionGroup(0b0000_0001).SetTargetGroup(0b0000_0010)
                .Build();

            // player sprite test
            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(200, 300))
                .SetAnimation("reimu_idle", Color.White, 90, StgBlendState.Alpha, 0f)
                .Build();
            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(250, 300))
                .SetAnimation("reimu_left", Color.White, 90, StgBlendState.Alpha, 0f)
                .Build();
            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(300, 300))
                .SetAnimation("reimu_right", Color.White, 90, StgBlendState.Alpha, 0f)
                .Build();
            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(350, 300))
                .SetAnimation("reimu_transition_left", Color.White, 90, StgBlendState.Alpha, 0f)
                .Build();
            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(400, 300))
                .SetAnimation("reimu_transition_right", Color.White, 90, StgBlendState.Alpha, 0f)
                .Build();

            // walk animatior test
            reimu = new Entity[1];

            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(200, 200))
                .SetSprite("bigball_red", Color.White, 90, StgBlendState.Additive, 0)
                .LerpAddPosition(new Vector2(160, 0), 80, EaseType.Linear)
                .LerpAddPosition(new Vector2(0, 160), 80, EaseType.Linear)
                .LerpAddPosition(new Vector2(-160, 0), 80, EaseType.Linear)
                .LerpAddPosition(new Vector2(0, -160), 80, EaseType.Linear)
                .LerpAddPosition(new Vector2(54, 0), 12, EaseType.Linear)
                .LerpAddPosition(new Vector2(-108, 0), 24, EaseType.Linear)
                .LerpAddPosition(new Vector2(54, 0), 12, EaseType.Linear)
                .Build(reimu);

            world.AddComponent<WalkAnimator>(reimu[0], new WalkAnimator(
                asset.Get<AnimationAsset>("reimu_idle"),
                asset.Get<AnimationAsset>("reimu_left"),
                asset.Get<AnimationAsset>("reimu_right"),
                asset.Get<AnimationAsset>("reimu_transition_left"),
                asset.Get<AnimationAsset>("reimu_transition_right")));

            // mixed animation test
            youmu = new Entity[1];

            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(100, 100))
                .SetSprite("bigball_red", Color.White, 90, StgBlendState.Additive, 0)
                .LerpAddPosition(new Vector2(160, 40), 80, EaseType.SmoothStep)
                .LerpAddPosition(new Vector2(-200, 40), 80, EaseType.SmoothStep)
                .Delay(100)
                .LerpAddPosition(new Vector2(40, -80), 80, EaseType.SmoothStep)
                .Build(youmu);

            world.AddComponent<SpriteAnimator>(youmu[0], new SpriteAnimator(
                asset.Get<AnimationAsset>("youmu_slash_right"), false));

            world.AddComponent<WalkAnimator>(youmu[0], new WalkAnimator(
                asset.Get<AnimationAsset>("youmu_idle"),
                asset.Get<AnimationAsset>("youmu_left"),
                asset.Get<AnimationAsset>("youmu_right"),
                asset.Get<AnimationAsset>("youmu_transition_left"),
                asset.Get<AnimationAsset>("youmu_transition_right")));
        }

        // really ugly test code. will be better after we have coroutine
        if (counter == 200)
        {
            var youmu = this.youmu[0];

            ref var walkAnim = ref world.GetComponent<WalkAnimator>(youmu);
            walkAnim.IsActive = false;
            ref var animation = ref world.GetComponent<SpriteAnimator>(youmu);
            animation.IsActive = true;
        }

        if (counter == 240)
        {
            var youmu = this.youmu[0];

            ref var walkAnim = ref world.GetComponent<WalkAnimator>(youmu);
            walkAnim.IsActive = false;
            ref var animation = ref world.GetComponent<SpriteAnimator>(youmu);
            animation.Animation = asset.Get<AnimationAsset>("youmu_slash_down");
            animation.Counter = 0;
            animation.FrameIndex = 0;
            animation.IsReverse = false;
            animation.IsActive = true;
        }

        if (counter == 270)
        {
            var youmu = this.youmu[0];

            ref var walkAnim = ref world.GetComponent<WalkAnimator>(youmu);
            walkAnim.IsActive = true;
            ref var animation = ref world.GetComponent<SpriteAnimator>(youmu);
            animation.IsActive = false;
        }

        if (counter % 1 == 0)
        {
            // spawn control test
            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(320, 240))
                .SetSpawnAnimation("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
                .SetAnimation("fireball_red", Color.White, 100, StgBlendState.Alpha)
                .SetVelocity(2f, 0)
                .SetSpawningCircle(8, 4, 0.5f, 0, 0.01f, 100)
                .SetSpawningSpreadByDelta(9, MathHelper.Pi / 8, 3)
                .SetSpawningSpreadByTotal(9, MathHelper.Pi, 3, 0.1f)
                .Build();

            // spawnAnimation test
            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(320, 240))
                .SetSpawnAnimation("mist_red", 2, 0, 0, 11, EaseType.Linear)
                .SetSprite("heart_red", Color.White, 100, StgBlendState.Alpha)
                .SetVelocity(2f, angleOffset).SetSpawningCircle(500)
                .LerpAddVelocityMagnitude(4f, 6, EaseType.Linear)//.SyncRendererRotation()
                .SetCircleCollider(4f).SetCollisionGroup(0b0000_0010)
                .Build();

            // curvy laser test
            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(320, 240))
                .SetSprite("curvylaser_lime", Color.White, 100, StgBlendState.Additive, MathHelper.Pi)
                .SetMovement(2f, angleOffset, 0.1f)
                .SetSpawningCircle(1)
                .SetAngularVelocity(0.05f)
                .SetCollisionGroup(0b0000_0010)
                .MakeCurvyLaser(512, 16f)
                .Build();

            // curvy laser animation & collision test
            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(320, 240))
                .SetAnimation("lightning", Color.White, 100, StgBlendState.Additive, MathHelper.Pi)
                .SetMovement(2f, angleOffset, 0.1f)
                .SetSpawningCircle(1)
                .AddMovementAngle(1f).Delay(20)
                .AddMovementAngle(-1f).Delay(20)
                .AddMovementAngle(1f).Delay(20)
                .AddMovementAngle(-1f).Delay(20)
                .AddMovementAngle(1f).Delay(20)
                .AddMovementAngle(-1f)
                .SetCollisionGroup(0b0000_0010)
                .MakeCurvyLaser(128, 16f)
                .Build();

            // hierarchy test
            Span<Entity> parent = stackalloc Entity[1];
            bulletManager.SpawnBullet()
                .SetPosition(new Vector2(200, 200))
                .SetMovement(1f, angleOffset, 0)
                .SyncTransformRotation()
                .Build(parent);

            ref var transform = ref world.GetComponent<Transform>(parent[0]);

            transform.Scale = new Vector2(2, 1);

            const int ChildrenCount = 80;

            Span<Entity> children = stackalloc Entity[ChildrenCount];
            bulletManager.SpawnBullet()
                .SetSpawnAnimation("mist_red", 2f, 1f, 0, 12, EaseType.Linear)
                .SetAnimation("fireball_red", Color.White, 100, StgBlendState.Alpha)
                .SetSpawningCircle(ChildrenCount)
                .SetCollisionGroup(0b0000_0010)
                .Build(children);

            for (int i = 0; i < ChildrenCount; i++)
            {
                const float Delta = MathHelper.TwoPi / ChildrenCount;
                const int Radius = 100;
                var position = new Vector2(
                    Radius * MathF.Cos(Delta * i),
                    Radius * MathF.Sin(Delta * i));
                engine.SetParentTest(parent[0], children[i], position, Vector2.One, 0);
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
    private AnimationSystem animationSystem = null!;
    private RenderSystem renderSystem = null!;
    private CollisionSystem collisionSystem = null!;
    private LifetimeSystem lifetimeSystem = null!;
    private HierarchySystem hierarcySystem = null!;

    private Rectangle gameBounds = new(0, 0, 640, 480); // new(640 / 4, 480 / 4, 640 / 2, 480 / 2);

    private TestScript script = null!;

    private InputManager Input { get; } = InputManager.Instance;
    bool isPaused = false;
    bool shouldAdvance = false;

    private double fpsTimer;
    private int fpsCounter;
    private int currentFps;
    private int frameCounter;

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

        assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "curvylaser_lime");
        assetManager.Load<AnimationAsset>("bullet/bullet_animations.txt", "lightning");

        assetManager.Load<AnimationAsset>("player/reimu_animations.txt", "reimu_idle");
        assetManager.Load<AnimationAsset>("player/reimu_animations.txt", "reimu_left");
        assetManager.Load<AnimationAsset>("player/reimu_animations.txt", "reimu_right");
        assetManager.Load<AnimationAsset>("player/reimu_animations.txt", "reimu_transition_left");
        assetManager.Load<AnimationAsset>("player/reimu_animations.txt", "reimu_transition_right");

        assetManager.Load<AnimationAsset>("youmu/youmu_animations.txt", "youmu_idle");
        assetManager.Load<AnimationAsset>("youmu/youmu_animations.txt", "youmu_left");
        assetManager.Load<AnimationAsset>("youmu/youmu_animations.txt", "youmu_right");
        assetManager.Load<AnimationAsset>("youmu/youmu_animations.txt", "youmu_transition_left");
        assetManager.Load<AnimationAsset>("youmu/youmu_animations.txt", "youmu_transition_right");
        assetManager.Load<AnimationAsset>("youmu/youmu_animations.txt", "youmu_slash_right");
        assetManager.Load<AnimationAsset>("youmu/youmu_animations.txt", "youmu_slash_down");

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
        hierarcySystem = new HierarchySystem(world);

        script = new(bulletManager, world, this, assetManager);
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
            fpsTimer--;
        }

        if (!isPaused || shouldAdvance)
        {
            if (currentFps > 58)
                script.Update();
            animationSystem.Update();
            movementSystem.Update();
            hierarcySystem.Update();
            lifetimeSystem.Update();
            collisionSystem.Update();
        }

        shouldAdvance = false;
        Input.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        fpsCounter++;

        GraphicsDevice.Clear(new Color(15, 10, 30));
        stgBatch.BeginSorted(SamplerState.PointClamp, RasterizerState.CullCounterClockwise,
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
            $"FPS: {currentFps}\n" +
            $"F: {frameCounter}",
            new Vector2(572, 4), fpsColor, 200, StgBlendState.Alpha);

        stgBatch.End();

        frameCounter++;
        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        world.Dispose();
        renderSystem.Dispose();
        collisionSystem.Dispose();
        lifetimeSystem.Dispose();
        hierarcySystem.Dispose();
        base.UnloadContent();
    }

    // really bad test methods, no safeguarding whatsoever
    // todo: should these be in a separate class/system?
    public void SetParentTest(Entity parent, Entity children, Vector2 localPosition, Vector2 localScale, float rotation = 0)
    {
        world.AddComponent<Hierarchy>(children, new() {
            Parent = parent,
            LocalPosition = localPosition,
            LocalScale = localScale,
            LocalRotation = rotation
        });
    }

    public void SetParentTest(Entity parent, Entity children, bool keepWorldTransform)
    {
        if (!world.TryGetComponent<Transform>(children, out var transform))
            return;

        if (keepWorldTransform)
        {
            ref var parentTransform = ref world.GetComponent<Transform>(parent);
            world.AddComponent<Hierarchy>(children, new() {
                Parent = parent,
                LocalPosition = transform.Position - parentTransform.Position,
                LocalScale = transform.Scale / parentTransform.Scale,
                LocalRotation = transform.Rotation - parentTransform.Rotation
            });
        }
        else
        {
            world.AddComponent<Hierarchy>(children, new()
            {
                Parent = parent,
                LocalPosition = transform.Position,
                LocalScale = transform.Scale,
                LocalRotation = transform.Rotation
            });
        }
    }
}
