using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ParaTH;

// sorry ;(
public sealed class TestScript(BulletFactory bulletManager, World world, Engine engine, AssetManager asset)
{
    Entity[] reimu = null!;
    Entity[] youmu = null!;

    Entity sweepEmitter; // Pattern D: RotationController-driven sweeping emitter

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
                {
                    //bulletManager.Create()
                    //    .SetPosition(new Vector2(200, 200))
                    //    .SetSprite("heart_pink", Color.White, 100, StgBlendState.Additive)
                    //    .SetSpawnEffect("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                    //    .LerpAddPosition(delta, 120, EaseType.OutQuad)
                    //    .LerpAddPosition(-delta, 120, EaseType.InQuad)
                    //    .SetVelocity(delta / 30f)
                    //    .SyncRendererRotation()
                    //    .Build();
                }

                // velocity test
                {
                    //bulletManager.Create()
                    //    .SetPosition(new Vector2(320, 240))
                    //    .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
                    //    .SetSpawnEffect("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                    //    .SetVelocity(2f, angle)
                    //    .SyncRendererRotation()
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
                }

                // acceleration test
                {
                    //bulletManager.Create()
                    //    .SetPosition(new Vector2(320, 240))
                    //    .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
                    //    .SetSpawnEffect("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                    //    .SetVelocity(2f, angle)
                    //    .SyncRendererRotation()
                    //    .Delay(60)
                    //    .SetAcceleration(Vector2.UnitY * 0.05f)
                    //    .Build();
                }

                // curve Test
                {
                    //bulletManager.Create()
                    //    .SetPosition(new Vector2(320, 240))
                    //    .SetSprite("heart_pink", Color.White, 100, StgBlendState.Alpha)
                    //    .SetSpawnEffect("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                    //    .SetVelocity(2f, angle)
                    //    .SyncRendererRotation()
                    //    .SetAngularVelocity(MathHelper.Pi / 60).Delay(30)
                    //    .SetAngularVelocity(-MathHelper.Pi / 60).Delay(30)
                    //    .SetAngularVelocity(MathHelper.Pi / 60).Delay(30)
                    //    .SetAngularVelocity(-MathHelper.Pi / 60).Delay(30)
                    //    .SetAngularVelocity(MathHelper.Pi / 60).Delay(30)
                    //    .SetAngularVelocity(-MathHelper.Pi / 60).Delay(30)
                    //    .SetAngularVelocity(0)
                    //    .Build();
                }

                // spawnAnimation test
                {
                    //bulletManager.Create()
                    //    .SetPosition(new Vector2(320, 240))
                    //    .SetSpawnEffect("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                    //    .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
                    //    .SetAnimation("fireball_red", Color.White, 100, StgBlendState.Alpha)
                    //    .SetVelocity(2f, angle).LerpAddVelocityMagnitude(12f, 120, EaseType.Linear)
                    //    .SyncRendererRotation()
                    //    .Build();
                }
            }
        }

        //if (counter == 10)
        //{
        //    // target test
        //    bulletManager.Create()
        //        .SetPosition(new Vector2(400, 400))
        //        .SetSprite("bigball_red", Color.White, 90, StgBlendState.Additive)
        //        .SetCircleCollider(16f).SetCollisionGroup(0b0000_0001).SetTargetGroup(0b0000_0010)
        //        .Build();

        //    // player sprite test
        //    bulletManager.Create()
        //        .SetPosition(new Vector2(200, 300))
        //        .SetAnimation("reimu_idle", Color.White, 90, StgBlendState.Alpha, 0f)
        //        .Build();
        //    bulletManager.Create()
        //        .SetPosition(new Vector2(250, 300))
        //        .SetAnimation("reimu_left", Color.White, 90, StgBlendState.Alpha, 0f)
        //        .Build();
        //    bulletManager.Create()
        //        .SetPosition(new Vector2(300, 300))
        //        .SetAnimation("reimu_right", Color.White, 90, StgBlendState.Alpha, 0f)
        //        .Build();
        //    bulletManager.Create()
        //        .SetPosition(new Vector2(350, 300))
        //        .SetAnimation("reimu_transition_left", Color.White, 90, StgBlendState.Alpha, 0f)
        //        .Build();
        //    bulletManager.Create()
        //        .SetPosition(new Vector2(400, 300))
        //        .SetAnimation("reimu_transition_right", Color.White, 90, StgBlendState.Alpha, 0f)
        //        .Build();

        //    // walk animatior test
        //    reimu = new Entity[1];

        //    bulletManager.Create()
        //        .SetPosition(new Vector2(200, 200))
        //        .SetSprite("bigball_red", Color.White, 90, StgBlendState.Additive, 0)
        //        .LerpAddPosition(new Vector2(160, 0), 80, EaseType.Linear)
        //        .LerpAddPosition(new Vector2(0, 160), 80, EaseType.Linear)
        //        .LerpAddPosition(new Vector2(-160, 0), 80, EaseType.Linear)
        //        .LerpAddPosition(new Vector2(0, -160), 80, EaseType.Linear)
        //        .LerpAddPosition(new Vector2(54, 0), 12, EaseType.Linear)
        //        .LerpAddPosition(new Vector2(-108, 0), 24, EaseType.Linear)
        //        .LerpAddPosition(new Vector2(54, 0), 12, EaseType.Linear)
        //        .Build(reimu);

        //    world.AddComponent<WalkAnimator>(reimu[0], new WalkAnimator(
        //        asset.Get<AnimationAsset>("reimu_idle"),
        //        asset.Get<AnimationAsset>("reimu_left"),
        //        asset.Get<AnimationAsset>("reimu_right"),
        //        asset.Get<AnimationAsset>("reimu_transition_left"),
        //        asset.Get<AnimationAsset>("reimu_transition_right")));

        //    // mixed animation test
        //    youmu = new Entity[1];

        //    bulletManager.Create()
        //        .SetPosition(new Vector2(100, 100))
        //        .SetSprite("bigball_red", Color.White, 90, StgBlendState.Additive, 0)
        //        .LerpAddPosition(new Vector2(160, 40), 80, EaseType.SmoothStep)
        //        .LerpAddPosition(new Vector2(-200, 40), 80, EaseType.SmoothStep)
        //        .Delay(100)
        //        .LerpAddPosition(new Vector2(40, -80), 80, EaseType.SmoothStep)
        //        .Build(youmu);

        //    world.AddComponent<SpriteAnimator>(youmu[0], new SpriteAnimator(
        //        asset.Get<AnimationAsset>("youmu_slash_right"), false));

        //    world.AddComponent<WalkAnimator>(youmu[0], new WalkAnimator(
        //        asset.Get<AnimationAsset>("youmu_idle"),
        //        asset.Get<AnimationAsset>("youmu_left"),
        //        asset.Get<AnimationAsset>("youmu_right"),
        //        asset.Get<AnimationAsset>("youmu_transition_left"),
        //        asset.Get<AnimationAsset>("youmu_transition_right")));
        //}

        //// really ugly test code. will be better after we have coroutine
        //if (counter == 200)
        //{
        //    var youmu = this.youmu[0];

        //    ref var walkAnim = ref world.GetComponent<WalkAnimator>(youmu);
        //    walkAnim.IsActive = false;
        //    ref var animation = ref world.GetComponent<SpriteAnimator>(youmu);
        //    animation.IsActive = true;
        //}

        //if (counter == 240)
        //{
        //    var youmu = this.youmu[0];

        //    ref var walkAnim = ref world.GetComponent<WalkAnimator>(youmu);
        //    walkAnim.IsActive = false;
        //    ref var animation = ref world.GetComponent<SpriteAnimator>(youmu);
        //    animation.Animation = asset.Get<AnimationAsset>("youmu_slash_down");
        //    animation.Counter = 0;
        //    animation.FrameIndex = 0;
        //    animation.IsReverse = false;
        //    animation.IsActive = true;
        //}

        //if (counter == 270)
        //{
        //    var youmu = this.youmu[0];

        //    ref var walkAnim = ref world.GetComponent<WalkAnimator>(youmu);
        //    walkAnim.IsActive = true;
        //    ref var animation = ref world.GetComponent<SpriteAnimator>(youmu);
        //    animation.IsActive = false;
        //}

        if (counter % 1 == 0)
        {
            // spawn control test
            {
                //bulletManager.Create()
                //    .SetPosition(new Vector2(320, 240))
                //    .SetSpawnEffect("heart_pink", 2, 0, 0, 11, EaseType.Linear)
                //    .SetSprite("arrow_pink", Color.White, 100, StgBlendState.Alpha)
                //    .SetAnimation("fireball_red", Color.White, 100, StgBlendState.Alpha)
                //    .SetVelocity(2f, 0)
                //    .SetSpawningCircle(8, 4, 0.5f, 0, 0.01f, 100)
                //    .SetSpawningSpreadByDelta(9, MathHelper.Pi / 8, 3)
                //    .SetSpawningSpreadByTotal(9, MathHelper.Pi, 3, 0.1f)
                //    .Build();
            }

            // spawnAnimation test
            {
                //bulletManager.Create()
                //    .SetPosition(new Vector2(320, 240))
                //    .SetSpawnEffect("mist_red", 2, 0, 0, 11, EaseType.Linear)
                //    .SetSprite("heart_red", Color.White, 100, StgBlendState.Alpha)
                //    .SetVelocity(2f, angleOffset).SetSpawningCircle(500)
                //    .LerpAddVelocityMagnitude(4f, 6, EaseType.Linear)//.SyncRendererRotation()
                //    .SetCircleCollider(4f).SetCollisionGroup(0b0000_0010)
                //    .Build();
            }

            // curvy laser test
            {
                //bulletManager.Create()
                //    .SetPosition(new Vector2(320, 240))
                //    .SetSprite("curvylaser_lime", Color.White, 100, StgBlendState.Additive, MathHelper.Pi)
                //    .SetMovement(2f, angleOffset, 0.1f)
                //    .SetSpawningCircle(1)
                //    .SetAngularVelocity(0.05f)
                //    .SetCollisionGroup(0b0000_0010)
                //    .MakeCurvyLaser(512, 16f)
                //    .Build();
            }

            // curvy laser animation & collision test
            {
                //bulletManager.Create()
                //    .SetPosition(new Vector2(320, 240))
                //    .SetAnimation("lightning", Color.White, 100, StgBlendState.Additive, MathHelper.Pi)
                //    .SetMovement(2f, angleOffset, 0.1f)
                //    .SetSpawningCircle(1)
                //    .AddMovementAngle(1f).Delay(20)
                //    .AddMovementAngle(-1f).Delay(20)
                //    .AddMovementAngle(1f).Delay(20)
                //    .AddMovementAngle(-1f).Delay(20)
                //    .AddMovementAngle(1f).Delay(20)
                //    .AddMovementAngle(-1f)
                //    .SetCollisionGroup(0b0000_0010)
                //    .MakeCurvyLaser(128, 16f)
                //    .Build();
            }

            // hierarchy test
            {
                //Span<Entity> parent = stackalloc Entity[1];
                //bulletManager.Create()
                //    .SetPosition(new Vector2(200, 200))
                //    .SetMovement(1f, angleOffset, 0)
                //    .SyncTransformRotation()
                //    .Build(parent);

                //ref var transform = ref world.GetComponent<Transform>(parent[0]);

                //transform.Scale = new Vector2(2, 1);

                //const int ChildrenCount = 80;

                //Span<Entity> children = stackalloc Entity[ChildrenCount];
                //bulletManager.Create()
                //    .SetSpawnEffect("mist_red", 2f, 1f, 0, 12, EaseType.Linear)
                //    .SetAnimation("fireball_red", Color.White, 100, StgBlendState.Alpha)
                //    .SetSpawningCircle(ChildrenCount)
                //    .Build(children);

                //for (int i = 0; i < ChildrenCount; i++)
                //{
                //    const float Delta = MathHelper.TwoPi / ChildrenCount;
                //    const int Radius = 100;
                //    var position = new Vector2(
                //        Radius * MathF.Cos(Delta * i),
                //        Radius * MathF.Sin(Delta * i));
                //    engine.SetParentTest(parent[0], children[i], position, Vector2.One, 0);
                //}
            }

            // laser glow test
            {
                //bulletManager.Create()
                //    .SetPosition(new Vector2(320, 240))
                //    .SetSprite("scale_lightpink", Color.White, 100, StgBlendState.Additive, 0)
                //    .SetMovement(4f, angleOffset, 0f)
                //    .SetLaserSourceSprite("lasersource_pink", Vector2.One)
                //    .SetSpawningCircle(10, distanceToCenter: 50)
                //    .SetCollisionGroup(0b0000_0010)
                //    .MakeCurvyLaser(64, 8f)
                //    .Build();
            }

            // laser test
            //{
            //    bulletManager.Create()
            //        .SetPosition(new Vector2(320, 240))
            //        .SetSprite("mediumball_blue", Color.White, 100, StgBlendState.Additive, 0)
            //        .SetLaserSourceSprite("lasersource_blue", Vector2.One)
            //        .SetSpawningCircle(4)
            //        .MakeLaser(16, 0, 100)
            //        .AppendLaserNode(100, MathHelper.PiOver2)
            //        .SetCollisionGroup(0b0000_0010)
            //        .Build();
            //}

            // ============================================================
            // Flag demonstration patterns.
            // Exercises Movement.SyncTransformRotation, Renderer.IsFixedRotation,
            // Hierarchy.PreserveTransformRotation, driven by RotationController.
            // ============================================================

            // one-shot anchored patterns (spin/orbit in place, never go offscreen)
            if (counter == 0)
            {
                SetupPatternB_IsFixedRotation();
                SetupPatternC_PreserveTransformRotation();
                SetupPatternD_SweepingEmitter();
            }

            // Pattern A: curving arrows, sync-rotation ON vs OFF, refreshed periodically
            if (counter % 90 == 0)
                FirePatternA_SyncTransformRotation();

            // Pattern D: emitter sweeps via RotationController; fire along its current facing
            if (counter % 4 == 0)
                FirePatternD_Sweep();
        }

        counter++;
    }

    // ----------------------------------------------------------------
    // Pattern A — Movement.SyncTransformRotation
    // Two arrows launched together on the same curving path (CurveController rotates
    // their velocity). The SYNC arrow turns to face its velocity each frame; the
    // NO-SYNC arrow keeps its launch rotation. Side-by-side the difference is obvious.
    // ----------------------------------------------------------------
    private void FirePatternA_SyncTransformRotation()
    {
        Vector2 origin = new(320, 60);

        // SYNC ON: transform.Rotation is overwritten to atan2(velocity) every frame
        bulletManager.Create()
            .SetPosition(origin - new Vector2(20, 0))
            .SetSprite("arrow_green", Color.White, 100, StgBlendState.Alpha, rotation: 0f)
            .SetVelocity(2.5f, MathHelper.PiOver2)
            .SetAngularVelocity(0.03f)            // curve the velocity so facing changes
            .SyncTransformRotation()
            .SetOffscreenLifeTime(0)
            .Build();

        // SYNC OFF: same curving path, but rotation stays fixed at launch
        bulletManager.Create()
            .SetPosition(origin + new Vector2(20, 0))
            .SetSprite("arrow_yellow", Color.White, 100, StgBlendState.Alpha, rotation: 0f)
            .SetVelocity(2.5f, MathHelper.PiOver2)
            .SetAngularVelocity(0.03f)
            .SetOffscreenLifeTime(0)
            .Build();
    }

    // ----------------------------------------------------------------
    // Pattern B — Renderer.IsFixedRotation
    // A ring of rice bullets, each spun in place by a RotationController. Alternating
    // bullets set IsFixedRotation: FIXED ones keep their sprite angle while the transform
    // spins underneath; NON-FIXED ones visibly rotate with the transform.
    // ----------------------------------------------------------------
    private void SetupPatternB_IsFixedRotation()
    {
        Vector2 center = new(480, 360);
        const int Count = 10;
        const float Radius = 55;

        for (int i = 0; i < Count; i++)
        {
            float a = MathHelper.TwoPi / Count * i;
            var pos = center + new Vector2(Radius * MathF.Cos(a), Radius * MathF.Sin(a));
            bool isFixed = (i & 1) == 0;

            bulletManager.Create()
                .SetPosition(pos)
                .SetSprite(isFixed ? "rice_lightgreen" : "rice_orange",
                           Color.White, 100, StgBlendState.Alpha, rotation: 0f)
                .SetRendererRotation(isFixed)     // IsFixedRotation = true on greens
                .SetRotationalVelocity(0.05f)     // RotationController spins the transform
                .Build();
        }
    }

    // ----------------------------------------------------------------
    // Pattern C — Hierarchy.PreserveTransformRotation
    // A spinning parent (RotationController) with kunai children orbiting it. Children all
    // orbit (position follows parent rotation), but PRESERVE children keep their own world
    // rotation while NON-PRESERVE children rotate with the parent.
    // ----------------------------------------------------------------
    private void SetupPatternC_PreserveTransformRotation()
    {
        Vector2 center = new(160, 360);

        Span<Entity> parentSpan = stackalloc Entity[1];
        bulletManager.Create()
            .SetPosition(center)
            .SetSprite("bigball_red", Color.White, 90, StgBlendState.Additive, rotation: 0f)
            .SetRotationalVelocity(0.02f)         // parent spins -> children orbit
            .Build(parentSpan);
        var parent = parentSpan[0];

        const int Count = 8;
        const float Radius = 75;
        Span<Entity> childSpan = stackalloc Entity[1];
        for (int i = 0; i < Count; i++)
        {
            float a = MathHelper.TwoPi / Count * i;
            var pos = center + new Vector2(Radius * MathF.Cos(a), Radius * MathF.Sin(a));
            bool preserve = (i & 1) == 0;

            bulletManager.Create()
                .SetPosition(pos)
                .SetSprite(preserve ? "kunai_lightblue" : "kunai_lightred",
                           Color.White, 100, StgBlendState.Alpha, rotation: 0f)
                .Build(childSpan);

            // worldPositionStays bakes the orbit offset from current world pos;
            // preserveTransformRotation decides whether the child keeps its own rotation.
            engine.Hierarchy.SetParent(childSpan[0], parent,
                worldPositionStays: true, preserveTransformRotation: preserve);
        }
    }

    // ----------------------------------------------------------------
    // Pattern D — Integration: RotationController-driven sweeping emitter
    // A turret pinned at center spins via RotationController. Each tick we read its
    // current facing and fire a 3-way spread of velocity-facing rice along it, producing
    // a sweeping fan. Ties RotationController + SyncTransformRotation together.
    // ----------------------------------------------------------------
    private void SetupPatternD_SweepingEmitter()
    {
        Span<Entity> span = stackalloc Entity[1];
        bulletManager.Create()
            .SetPosition(new Vector2(320, 240))
            .SetSprite("mediumball_blue", Color.White, 95, StgBlendState.Additive, rotation: 0f)
            .SetRotationalVelocity(0.04f)         // turret sweep speed
            .Build(span);
        sweepEmitter = span[0];
    }

    private void FirePatternD_Sweep()
    {
        if (!world.IsAlive(sweepEmitter))
            return;

        ref var emitter = ref world.GetComponent<Transform>(sweepEmitter);
        float facing = emitter.Rotation;
        Vector2 pos = emitter.Position;

        bulletManager.Create()
            .SetPosition(pos)
            .SetSprite("rice_lightcyan", Color.White, 100, StgBlendState.Additive, rotation: 0f)
            .SetVelocity(3f, facing)
            .SetSpawningSpreadByDelta(3, MathHelper.Pi / 16f)   // 3-way fan around facing
            .SyncTransformRotation()                            // each rice faces its own velocity
            .SetOffscreenLifeTime(0)
            .Build();
    }
}

public sealed class Engine : Game
{
    private AssetManager assetManager = null!;
    private StgBatch stgBatch = null!;
    private Matrix projection;
    private World world = null!;

    private BulletFactory bulletFactory = null!;

    private MovementSystem movementSystem = null!;
    private AnimationSystem animationSystem = null!;
    private RenderSystem renderSystem = null!;
    private CollisionSystem collisionSystem = null!;
    private LifetimeSystem lifetimeSystem = null!;
    private HierarchySystem hierarcySystem = null!;
    private HierarchyManager hierarchyManager = null!;

    // exposed so scripts can do runtime Unity-like reparenting
    public HierarchyManager Hierarchy => hierarchyManager;

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
        assetManager.Load<SpriteAsset>("bullet/laser_sprites_test.txt", "longlaser_lightred");

        assetManager.Load<AnimationAsset>("bullet/bullet_animations.txt", "fireball_red");

        assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "curvylaser_lime");
        assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "curvylaser_lightred");
        assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "curvylaser_lightblue");
        assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "curvylaser_lightpink");
        assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "lasersource_yellow");
        assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "lasersource_red");
        assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "lasersource_blue");
        assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "lasersource_pink");
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

        bulletFactory = new BulletFactory(world, assetManager);

        movementSystem = new MovementSystem(world);
        animationSystem = new AnimationSystem(world);
        renderSystem = new RenderSystem(world, stgBatch, gameBounds)
        {
            DebugDrawColliders = false
        };
        collisionSystem = new CollisionSystem(world);
        lifetimeSystem = new LifetimeSystem(world, gameBounds);
        hierarcySystem = new HierarchySystem(world);
        hierarchyManager = new HierarchyManager(world);

        script = new(bulletFactory, world, this, assetManager);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Input.IsKeyPressed(Keys.P))
            isPaused = !isPaused;

        if (Input.IsKeyPressed(Keys.K))
            shouldAdvance = true;

        if (Input.IsKeyPressed(Keys.D))
            renderSystem.DebugDrawColliders = !renderSystem.DebugDrawColliders;

        fpsTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (fpsTimer >= 1.0)
        {
            currentFps = fpsCounter;
            fpsCounter = 0;
            fpsTimer--;
        }

        if (!isPaused || shouldAdvance)
        {
            //if (currentFps > 58)
                script.Update();
            animationSystem.Update();
            movementSystem.Update();
            hierarcySystem.Update();
            lifetimeSystem.Update();
            collisionSystem.Update();

            frameCounter++;
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

        stgBatch.End();

        stgBatch.Begin(SamplerState.PointClamp, RasterizerState.CullCounterClockwise,
                     null, projection);

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

        // test strip laser
        //var sprite = assetManager.Load<SpriteAsset>("bullet/bullet_sprites.txt", "mediumball_pink");
        //stgBatch.DrawStrip(
        //    sprite.Texture,
        //    sprite.SourceRect,
        //    0,
        //    [Vector2.Zero, new Vector2(320, 240)],
        //    16,
        //    Color.White,
        //    100,
        //    StgBlendState.Additive);

        stgBatch.End();

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        world.Dispose();
        renderSystem.Dispose();
        collisionSystem.Dispose();
        lifetimeSystem.Dispose();
        hierarcySystem.Dispose();
        hierarchyManager.Dispose();
        base.UnloadContent();
    }

    // really bad test methods, no safeguarding whatsoever
    // todo: should these be in a separate class/system?
    public void SetParentTest(Entity parent, Entity children, Vector2 localPosition, Vector2 localScale, float rotation = 0)
    {
        // route through the manager so Depth + parent->child links stay correct.
        // keep-local mode, then stamp the explicit local TRS the caller asked for.
        hierarchyManager.SetParent(children, parent, worldPositionStays: false);
        ref var hierarchy = ref world.GetComponent<Hierarchy>(children);
        hierarchy.LocalPosition = localPosition;
        hierarchy.LocalScale = localScale;
        hierarchy.LocalRotation = rotation;
    }

    public void SetParentTest(Entity parent, Entity children, bool keepWorldTransform)
    {
        if (!world.HasComponent<Transform>(children))
            return;

        hierarchyManager.SetParent(parent: parent, child: children, worldPositionStays: keepWorldTransform);
    }
}
