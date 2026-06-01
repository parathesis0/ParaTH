using Microsoft.Xna.Framework;

namespace ParaTH;

// Headless logic tests — no GraphicsDevice required.
// Covers: HierarchyManager (SetParent/Unparent/traversal/depth) + the three rotation flags:
//   - Movement.SyncTransformRotation
//   - Renderer.IsFixedRotation
//   - Hierarchy.PreserveTransformRotation
// combined with RotationController behaviour.
//
// run with:  dotnet run -- --test
public static class LogicTests
{
    private static int passed;
    private static int failed;

    // a fresh world sized like the game's, plus the systems we can run without graphics
    private static World NewWorld() => new(
        baseChunkByteSize: 16384,
        baseChunkEntityCount: 100,
        initialArchetypeCapacity: 8,
        initialEntityCapacity: 1024);

    public static int RunAll()
    {
        passed = 0;
        failed = 0;

        Console.WriteLine("== ParaTH logic tests ==");

        Test_SyncTransformRotation_FacesVelocity();
        Test_SyncTransformRotation_Off_KeepsRotation();

        Test_IsFixedRotation_Resolve();

        Test_PreserveTransformRotation_False_InheritsParent();
        Test_PreserveTransformRotation_True_KeepsOwn();

        Test_Hierarchy_WorldPositionStays_PreservesWorld();
        Test_Hierarchy_KeepLocal_StampsCurrentTransform();
        Test_Hierarchy_MultiLevelDepth();
        Test_Hierarchy_Reparent_UpdatesSubtreeDepth();
        Test_Hierarchy_Unparent_ReRootsChildren();
        Test_Hierarchy_ChildTraversal();
        Test_Lifetime_Hierarchy_AnyAliveSavesGroup();
        Test_Lifetime_Hierarchy_AllReadyDestroysGroup();

        Test_RotationController_RotationalVelocity();
        Test_RotationController_SetRotation_Immediate();
        Test_Integration_SpinningParent_OrbitsChild();

        Console.WriteLine($"== done: {passed} passed, {failed} failed ==");
        return failed;
    }

    #region SyncTransformRotation
    private static void Test_SyncTransformRotation_FacesVelocity()
    {
        using var world = NewWorld();
        var movement = new MovementSystem(world);

        // moving +X: transform should face angle 0; then switch to +Y -> PiOver2
        var e = world.CreateEntity(
            new Transform(new Vector2(100, 100), Vector2.One, rotation: 12.34f),
            new Movement(new Vector2(2, 0), Vector2.Zero, syncTransformRotation: true),
            new Lifetime(60));

        movement.Update();
        ApproxEq(world.GetComponent<Transform>(e).Rotation, 0f, "SyncTransformRotation +X -> angle 0");

        ref var mv = ref world.GetComponent<Movement>(e);
        mv.Velocity = new Vector2(0, 3);
        movement.Update();
        ApproxEq(world.GetComponent<Transform>(e).Rotation, MathHelper.PiOver2,
            "SyncTransformRotation +Y -> angle PiOver2");
    }

    private static void Test_SyncTransformRotation_Off_KeepsRotation()
    {
        using var world = NewWorld();
        var movement = new MovementSystem(world);

        const float StartRot = 0.777f;
        var e = world.CreateEntity(
            new Transform(new Vector2(100, 100), Vector2.One, rotation: StartRot),
            new Movement(new Vector2(2, 2), Vector2.Zero, syncTransformRotation: false),
            new Lifetime(60));

        movement.Update();
        ApproxEq(world.GetComponent<Transform>(e).Rotation, StartRot,
            "SyncTransformRotation off -> rotation unchanged by movement");
    }
    #endregion

    #region IsFixedRotation
    private static void Test_IsFixedRotation_Resolve()
    {
        // pure resolution helper (what RenderSystem feeds the sprite batch)
        var fixedR = new Renderer { IsFixedRotation = true, Rotation = 0.5f };
        ApproxEq(fixedR.ResolveRenderRotation(transformRotation: 10f), 0.5f,
            "IsFixedRotation true -> ignores transform rotation");

        var followR = new Renderer { IsFixedRotation = false, Rotation = 0.5f };
        ApproxEq(followR.ResolveRenderRotation(transformRotation: 10f), 10.5f,
            "IsFixedRotation false -> transform rotation + offset");
    }
    #endregion

    #region PreserveTransformRotation
    private static void Test_PreserveTransformRotation_False_InheritsParent()
    {
        using var world = NewWorld();
        var manager = new HierarchyManager(world);
        var hierarchySys = new HierarchySystem(world);

        var parent = world.CreateEntity(
            new Transform(new Vector2(50, 50), Vector2.One, rotation: 1.0f),
            new Movement(Vector2.Zero, Vector2.Zero, false),
            new Lifetime(60));

        // child rotation should be overwritten to parent.Rotation + LocalRotation
        var child = world.CreateEntity(
            new Transform(new Vector2(10, 0), Vector2.One, rotation: 0.2f));
        manager.SetParent(child, parent, worldPositionStays: false, preserveTransformRotation: false);

        hierarchySys.Update();
        ApproxEq(world.GetComponent<Transform>(child).Rotation, 1.0f + 0.2f,
            "PreserveTransformRotation false -> child rot = parent + local");
    }

    private static void Test_PreserveTransformRotation_True_KeepsOwn()
    {
        using var world = NewWorld();
        var manager = new HierarchyManager(world);
        var hierarchySys = new HierarchySystem(world);

        var parent = world.CreateEntity(
            new Transform(new Vector2(50, 50), Vector2.One, rotation: 1.0f),
            new Movement(Vector2.Zero, Vector2.Zero, false),
            new Lifetime(60));

        const float ChildRot = 5.0f;
        var child = world.CreateEntity(
            new Transform(new Vector2(10, 0), Vector2.One, rotation: ChildRot));
        manager.SetParent(child, parent, worldPositionStays: false, preserveTransformRotation: true);

        hierarchySys.Update();
        ApproxEq(world.GetComponent<Transform>(child).Rotation, ChildRot,
            "PreserveTransformRotation true -> child rot untouched");

        // position is still inherited even when rotation is preserved
        var expectedPos = new Vector2(
            50 + (10 * MathF.Cos(1.0f)),
            50 + (10 * MathF.Sin(1.0f)));
        ApproxEq(world.GetComponent<Transform>(child).Position, expectedPos,
            "PreserveTransformRotation true -> position still inherited");
    }
    #endregion

    #region Hierarchy manager
    private static void Test_Hierarchy_WorldPositionStays_PreservesWorld()
    {
        using var world = NewWorld();
        var manager = new HierarchyManager(world);
        var hierarchySys = new HierarchySystem(world);

        var parent = world.CreateEntity(
            new Transform(new Vector2(100, 100), new Vector2(2, 1), rotation: 0.5f),
            new Movement(Vector2.Zero, Vector2.Zero, false), new Lifetime(60));

        var childWorld = new Transform(new Vector2(150, 130), Vector2.One, rotation: 0.3f);
        var child = world.CreateEntity(
            childWorld, new Movement(Vector2.Zero, Vector2.Zero, false), new Lifetime(60));

        manager.SetParent(child, parent, worldPositionStays: true);
        hierarchySys.Update();

        ref var t = ref world.GetComponent<Transform>(child);
        ApproxEq(t.Position, childWorld.Position, "worldPositionStays -> world position preserved");
        ApproxEq(t.Scale, childWorld.Scale, "worldPositionStays -> world scale preserved");
        ApproxEq(t.Rotation, childWorld.Rotation, "worldPositionStays -> world rotation preserved");
    }

    private static void Test_Hierarchy_KeepLocal_StampsCurrentTransform()
    {
        using var world = NewWorld();
        var manager = new HierarchyManager(world);

        var parent = world.CreateEntity(
            new Transform(new Vector2(100, 100), Vector2.One, 0.5f),
            new Movement(Vector2.Zero, Vector2.Zero, false), new Lifetime(60));

        var child = world.CreateEntity(
            new Transform(new Vector2(150, 130), Vector2.One, 0.3f),
            new Movement(Vector2.Zero, Vector2.Zero, false), new Lifetime(60));

        manager.SetParent(child, parent, worldPositionStays: false);

        ref var h = ref world.GetComponent<Hierarchy>(child);
        ApproxEq(h.LocalPosition, new Vector2(150, 130), "keepLocal -> local position = old transform");
        ApproxEq(h.LocalRotation, 0.3f, "keepLocal -> local rotation = old transform");
    }

    private static void Test_Hierarchy_MultiLevelDepth()
    {
        using var world = NewWorld();
        var manager = new HierarchyManager(world);

        var root = MakeNode(world);
        var a = MakeNode(world);
        var b = MakeNode(world);
        var c = MakeNode(world);

        manager.SetParent(a, root);   // root has no Hierarchy -> a depth 0
        manager.SetParent(b, a);      // a depth 0 -> b depth 1
        manager.SetParent(c, b);      // b depth 1 -> c depth 2

        Check(world.GetComponent<Hierarchy>(a).Depth == 0, "depth: a == 0");
        Check(world.GetComponent<Hierarchy>(b).Depth == 1, "depth: b == 1");
        Check(world.GetComponent<Hierarchy>(c).Depth == 2, "depth: c == 2");
    }

    private static void Test_Hierarchy_Reparent_UpdatesSubtreeDepth()
    {
        using var world = NewWorld();
        var manager = new HierarchyManager(world);

        var root = MakeNode(world);
        var a = MakeNode(world);
        var b = MakeNode(world);
        var c = MakeNode(world);

        manager.SetParent(a, root); // a:0
        manager.SetParent(b, a);    // b:1
        manager.SetParent(c, b);    // c:2

        // reparent b (with subtree c) onto root -> b:0, c must cascade to 1
        manager.SetParent(b, root);

        Check(world.GetComponent<Hierarchy>(b).Depth == 0, "reparent: b -> 0");
        Check(world.GetComponent<Hierarchy>(c).Depth == 1, "reparent: c cascades -> 1");
        Check(manager.GetParent(c) == b, "reparent: c still child of b");
        Check(manager.GetChildCount(a) == 0, "reparent: a lost its child b");
    }

    private static void Test_Hierarchy_Unparent_ReRootsChildren()
    {
        using var world = NewWorld();
        var manager = new HierarchyManager(world);

        var a = MakeNode(world);
        var b = MakeNode(world);
        var c = MakeNode(world);

        manager.SetParent(b, a); // b:0
        manager.SetParent(c, b); // c:1

        manager.Unparent(b);

        Check(manager.GetParent(b) == default, "unparent: b has no parent");
        Check(world.HasComponent<Hierarchy>(b), "unparent: b keeps Hierarchy for child list");
        Check(manager.GetChildCount(a) == 0, "unparent: a has no children");
        Check(manager.GetParent(c) == b, "unparent: c still child of b");
        Check(world.GetComponent<Hierarchy>(c).Depth == 0, "unparent: c re-rooted to depth 0");
    }

    private static void Test_Hierarchy_ChildTraversal()
    {
        using var world = NewWorld();
        var manager = new HierarchyManager(world);

        var parent = MakeNode(world);
        var c0 = MakeNode(world);
        var c1 = MakeNode(world);
        var c2 = MakeNode(world);

        manager.SetParent(c0, parent);
        manager.SetParent(c1, parent);
        manager.SetParent(c2, parent);

        Check(manager.GetChildCount(parent) == 3, "traversal: child count == 3");

        int seen = 0;
        bool sawC0 = false, sawC1 = false, sawC2 = false;
        foreach (var child in manager.GetChildren(parent))
        {
            seen++;
            sawC0 |= child == c0;
            sawC1 |= child == c1;
            sawC2 |= child == c2;
            Check(manager.GetParent(child) == parent, "traversal: GetParent(child) == parent");
        }
        Check(seen == 3 && sawC0 && sawC1 && sawC2, "traversal: enumerated all 3 children");

        // index access (order is reverse-insertion since LinkChild pushes front, but all must be present)
        var byIndex = new[] { manager.GetChild(parent, 0), manager.GetChild(parent, 1), manager.GetChild(parent, 2) };
        Check(manager.GetChild(parent, 3) == default, "traversal: out-of-range -> default");
        Check(Array.IndexOf(byIndex, c0) >= 0 && Array.IndexOf(byIndex, c1) >= 0 && Array.IndexOf(byIndex, c2) >= 0,
            "traversal: GetChild covers all children");
    }
    #endregion

    #region Hierarchy lifetime
    private static void Test_Lifetime_Hierarchy_AnyAliveSavesGroup()
    {
        using var world = NewWorld();
        var manager = new HierarchyManager(world);
        var lifetime = new LifetimeSystem(world, new Rectangle(0, 0, 100, 100));

        var parent = MakeNode(world, new Vector2(200, 200), offscreenFramesToLive: 0);
        var child = MakeNode(world, new Vector2(50, 50), offscreenFramesToLive: 0);
        manager.SetParent(child, parent, worldPositionStays: true);

        lifetime.Update();

        Check(world.IsAlive(parent), "lifetime hierarchy: child saves offscreen parent");
        Check(world.IsAlive(child), "lifetime hierarchy: onscreen child remains alive");
    }

    private static void Test_Lifetime_Hierarchy_AllReadyDestroysGroup()
    {
        using var world = NewWorld();
        var manager = new HierarchyManager(world);
        var lifetime = new LifetimeSystem(world, new Rectangle(0, 0, 100, 100));

        var parent = MakeNode(world, new Vector2(200, 200), offscreenFramesToLive: 0);
        var child = MakeNode(world, new Vector2(220, 220), offscreenFramesToLive: 0);
        manager.SetParent(child, parent, worldPositionStays: true);

        lifetime.Update();

        Check(!world.IsAlive(parent), "lifetime hierarchy: ready parent destroyed");
        Check(!world.IsAlive(child), "lifetime hierarchy: ready child destroyed");
    }
    #endregion

    #region RotationController
    private static void Test_RotationController_RotationalVelocity()
    {
        using var world = NewWorld();
        var movement = new MovementSystem(world);

        var e = world.CreateEntity(
            new Transform(Vector2.Zero, Vector2.One, 0f),
            new Movement(Vector2.Zero, Vector2.Zero, false),
            new Lifetime(60));

        world.AddComponent(e, new RotationController
        {
            Instructions =
            [
                new RotationInstruction(0, 0.1f, 0, EaseType.Linear, RotationInstruction.Ops.SetRotationalVelocity)
            ],
            Index = -1
        });

        movement.Update();
        ApproxEq(world.GetComponent<Transform>(e).Rotation, 0.1f, "RotationController: RV after 1 frame");
        movement.Update();
        ApproxEq(world.GetComponent<Transform>(e).Rotation, 0.2f, "RotationController: RV after 2 frames");
    }

    private static void Test_RotationController_SetRotation_Immediate()
    {
        using var world = NewWorld();
        var movement = new MovementSystem(world);

        var e = world.CreateEntity(
            new Transform(Vector2.Zero, Vector2.One, 0f),
            new Movement(Vector2.Zero, Vector2.Zero, false),
            new Lifetime(60));

        world.AddComponent(e, new RotationController
        {
            Instructions =
            [
                new RotationInstruction(0, MathHelper.PiOver2, 0, EaseType.Linear, RotationInstruction.Ops.SetRotation)
            ],
            Index = -1
        });

        movement.Update();
        ApproxEq(world.GetComponent<Transform>(e).Rotation, MathHelper.PiOver2,
            "RotationController: SetRotation duration 0 applied immediately");
    }
    #endregion

    #region Integration
    private static void Test_Integration_SpinningParent_OrbitsChild()
    {
        using var world = NewWorld();
        var movement = new MovementSystem(world);
        var manager = new HierarchyManager(world);
        var hierarchySys = new HierarchySystem(world);

        var parent = world.CreateEntity(
            new Transform(new Vector2(320, 240), Vector2.One, 0f),
            new Movement(Vector2.Zero, Vector2.Zero, false),
            new Lifetime(600));
        world.AddComponent(parent, new RotationController
        {
            Instructions =
            [
                new RotationInstruction(0, 0.5f, 0, EaseType.Linear, RotationInstruction.Ops.SetRotationalVelocity)
            ],
            Index = -1
        });

        // child sits 50px to the parent's +X; parented keeping world pos
        var child = world.CreateEntity(new Transform(new Vector2(370, 240), Vector2.One, 0f));
        manager.SetParent(child, parent, worldPositionStays: true);

        // frame: movement spins parent (+0.5 rad), then hierarchy propagates to child
        movement.Update();
        hierarchySys.Update();

        float theta = 0.5f;
        var expected = new Vector2(320 + (50 * MathF.Cos(theta)), 240 + (50 * MathF.Sin(theta)));
        ApproxEq(world.GetComponent<Transform>(child).Position, expected,
            "integration: child orbits spinning parent");
        ApproxEq(world.GetComponent<Hierarchy>(child).LocalPosition, new Vector2(50, 0),
            "integration: child local stays fixed on orbit");
    }
    #endregion

    #region helpers
    private static Entity MakeNode(World world) => MakeNode(world, Vector2.Zero, 60);

    private static Entity MakeNode(World world, Vector2 position, short offscreenFramesToLive) => world.CreateEntity(
        new Transform(position, Vector2.One, 0f),
        new Movement(Vector2.Zero, Vector2.Zero, false),
        new Lifetime(offscreenFramesToLive));

    private static void Check(bool cond, string name)
    {
        if (cond) { passed++; Console.WriteLine($"  PASS  {name}"); }
        else { failed++; Console.WriteLine($"  FAIL  {name}"); }
    }

    private static void ApproxEq(float a, float b, string name, float eps = 1e-3f)
        => Check(MathF.Abs(a - b) <= eps, $"{name}  (got {a:0.####}, want {b:0.####})");

    private static void ApproxEq(Vector2 a, Vector2 b, string name, float eps = 1e-3f)
        => Check((a - b).Length() <= eps, $"{name}  (got {a}, want {b})");
    #endregion
}
