using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace ParaTH;

using DepthBuckets = UnsafePooledList<UnsafePooledList<LifetimeSystem.HierarchyPair>>;

// handles lifetime expiry and offbound destruction.
// offscreen expiry is soft: a parent-child hierarchy dies as a group only when all Lifetime nodes are ready.
// max-age expiry is hard: the expired node owns and destroys its whole subtree.
[SkipLocalsInit]
public sealed class LifetimeSystem(World world, Rectangle bounds) : IDisposable
{
    private readonly World world = world;
    private Rectangle bounds = bounds;

    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Lifetime>();

    private readonly UnsafeBitset lifetimeNodes = new(4);                               // entities participating in soft lifetime groups
    private readonly UnsafeBitset softReadyToDie = new(4);                              // offscreen expiry
    private readonly UnsafeBitset hardReadyToDie = new(4);                              // max-age expiry
    private readonly UnsafeBitset toDestroy = new(4);                                   // final destroy set, may include nodes without Lifetime
    private readonly UnsafePooledList<Entity> softDestroyCandidates = new(256);
    private readonly UnsafePooledList<Entity> hardDestroyCandidates = new(64);
    private readonly UnsafePooledList<Entity> destroyEntities = new(256);
    private readonly UnsafePooledList<Entity> subtreeStack = new(64);
    private readonly DepthBuckets hierarchyEntityBuckets = new(4);                      // depth-based buckets used for syncing parent and children's lifetimes
    private int maxDepthSeen = -1;

    public readonly struct HierarchyPair(Entity child, Entity parent) // is public for using alias
    {
        public readonly Entity Child = child;
        public readonly Entity Parent = parent;
    }

    public void Update()
    {
        var q = world.GetOrCreateQuery(descriptor);
        var softDestroyCandidates = this.softDestroyCandidates;
        var hardDestroyCandidates = this.hardDestroyCandidates;
        var hierarchyEntityBuckets = this.hierarchyEntityBuckets;
        var lifetimeNodes = this.lifetimeNodes;
        var softReadyToDie = this.softReadyToDie;
        var hardReadyToDie = this.hardReadyToDie;
        var toDestroy = this.toDestroy;

        softDestroyCandidates.Clear();
        hardDestroyCandidates.Clear();
        destroyEntities.Clear();
        lifetimeNodes.Clear();
        softReadyToDie.Clear();
        hardReadyToDie.Clear();
        toDestroy.Clear();

        int bitCapacity = world.MaxEntityId + 1;
        lifetimeNodes.EnsureCapacity(bitCapacity);
        softReadyToDie.EnsureCapacity(bitCapacity);
        hardReadyToDie.EnsureCapacity(bitCapacity);
        toDestroy.EnsureCapacity(bitCapacity);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            bool hasRenderer = archetype.Has<Renderer>();
            bool hasCurvyLaser = archetype.Has<CurvyLaser>();
            bool hasHrc = archetype.Has<Hierarchy>();

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                chunk.GetFilledComponentSpan<Transform, Lifetime>(
                    out var transforms, out var lifetimes);

                var renderers = hasRenderer ? chunk.GetFilledComponentSpan<Renderer>() : default;
                var curvyLasers = hasCurvyLaser ? chunk.GetFilledComponentSpan<CurvyLaser>() : default;
                var hierarchies = hasHrc ? chunk.GetFilledComponentSpan<Hierarchy>() : default;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    ref var transform = ref transforms.UnsafeAt(i);
                    ref var lifetime = ref lifetimes.UnsafeAt(i);
                    var entity = chunk.Entities.UnsafeAt(i);

                    lifetimeNodes.Set(entity.Id);

                    if (lifetime.MaxAliveFrames > 0 && lifetime.AliveFrames >= lifetime.MaxAliveFrames)
                    {
                        hardReadyToDie.Set(entity.Id);
                        hardDestroyCandidates.Add(entity);
                    }

                    if (lifetime.OffscreenFramesToLive >= 0)
                    {
                        // calculate if an entity is offscreen
                        bool isOffscreen;
                        if (hasCurvyLaser)
                        {
                            isOffscreen = IsCurvyLaserOffscreen(ref curvyLasers.UnsafeAt(i));
                        }
                        else if (hasRenderer)
                        {
                            float radius = CalculateSpriteRadius(ref renderers.UnsafeAt(i));
                            isOffscreen = IsCircleOffscreen(transform.Position, radius);
                        }
                        else
                        {
                            isOffscreen = IsPointOffscreen(transform.Position);
                        }

                        // decrement ttl or put on kill list
                        if (isOffscreen)
                        {
                            if (lifetime.OffscreenFramesToLive > 0)
                            {
                                lifetime.OffscreenFramesToLive--;
                                if (lifetime.OffscreenFramesToLive <= 0)
                                    softReadyToDie.Set(entity.Id);
                            }
                            else
                            {
                                softReadyToDie.Set(entity.Id);
                            }

                            if (softReadyToDie.IsSet(entity.Id))
                                softDestroyCandidates.Add(entity);
                        }
                    }

                    lifetime.AliveFrames++;

                    // only parented hierarchy nodes participate in group lifetime propagation
                    if (hasHrc)
                    {
                        ref var hierarchy = ref hierarchies.UnsafeAt(i);
                        if (hierarchy.Parent == default)
                            continue;

                        var depth = hierarchy.Depth;
                        var parent = hierarchy.Parent;

                        if (depth > maxDepthSeen)
                        {
                            for (int d = maxDepthSeen + 1; d <= depth; d++)
                                hierarchyEntityBuckets.Add(new UnsafePooledList<HierarchyPair>(64));

                            maxDepthSeen = depth;
                        }

                        hierarchyEntityBuckets[depth].Add(new HierarchyPair(entity, parent));
                    }
                }
            }
        }

        // process hierarchy from deepest to shallowest (children save parents)
        for (int depth = maxDepthSeen; depth >= 0; depth--)
        {
            var bucket = hierarchyEntityBuckets[depth];
            var bucketSpan = bucket.AsSpan();

            for (int i = 0; i < bucketSpan.Length; i++)
            {
                var pair = bucketSpan.UnsafeAt(i);

                if (CanSaveSoftGroup(pair.Child))
                    softReadyToDie.Unset(pair.Parent.Id);
            }
        }

        // process hierarchy from shallowest to deepest (parents save children)
        for (int depth = 0; depth <= maxDepthSeen; depth++)
        {
            var bucket = hierarchyEntityBuckets[depth];
            var bucketSpan = bucket.AsSpan();

            for (int i = 0; i < bucketSpan.Length; i++)
            {
                var pair = bucketSpan.UnsafeAt(i);

                if (CanSaveSoftGroup(pair.Parent))
                    softReadyToDie.Unset(pair.Child.Id);
            }

            bucket.Clear();
        }

        for (int i = 0; i < hardDestroyCandidates.Count; i++)
        {
            var entity = hardDestroyCandidates[i];
            if (hardReadyToDie.IsSet(entity.Id))
                MarkSubtreeForDestroy(entity);
        }

        for (int i = 0; i < softDestroyCandidates.Count; i++)
        {
            var entity = softDestroyCandidates[i];
            if (softReadyToDie.IsSet(entity.Id))
                MarkSubtreeForDestroy(entity);
        }

        UnlinkDestroyRoots();
        DestroyMarkedEntities();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool CanSaveSoftGroup(Entity entity)
        {
            int id = entity.Id;
            return lifetimeNodes.IsSet(id) && !hardReadyToDie.IsSet(id) && !softReadyToDie.IsSet(id);
        }
    }

    private void MarkSubtreeForDestroy(Entity root)
    {
        var stack = subtreeStack;
        stack.Clear();
        stack.Add(root);

        while (stack.Count > 0)
        {
            var entity = stack[^1];
            stack.RemoveLast();

            if (toDestroy.IsSet(entity.Id))
                continue;

            toDestroy.Set(entity.Id);
            destroyEntities.Add(entity);

            if (!world.HasComponent<Hierarchy>(entity))
                continue;

            ref var hierarchy = ref world.GetComponent<Hierarchy>(entity);
            var child = hierarchy.FirstChild;
            while (child != default)
            {
                stack.Add(child);
                child = world.GetComponent<Hierarchy>(child).NextSibling;
            }
        }
    }

    private void UnlinkDestroyRoots()
    {
        for (int i = 0; i < destroyEntities.Count; i++)
        {
            var entity = destroyEntities[i];
            if (!world.HasComponent<Hierarchy>(entity))
                continue;

            ref var hierarchy = ref world.GetComponent<Hierarchy>(entity);
            var parent = hierarchy.Parent;
            if (parent == default || toDestroy.IsSet(parent.Id))
                continue;

            UnlinkFromParent(parent, ref hierarchy);
        }
    }

    private void UnlinkFromParent(Entity parent, ref Hierarchy childHierarchy)
    {
        var prev = childHierarchy.PrevSibling;
        var next = childHierarchy.NextSibling;

        if (prev != default)
            world.GetComponent<Hierarchy>(prev).NextSibling = next;
        else
            world.GetComponent<Hierarchy>(parent).FirstChild = next;

        if (next != default)
            world.GetComponent<Hierarchy>(next).PrevSibling = prev;

        childHierarchy.Parent = default;
        childHierarchy.PrevSibling = default;
        childHierarchy.NextSibling = default;

        ref var parentHierarchy = ref world.GetComponent<Hierarchy>(parent);
        if (parentHierarchy.ChildCount > 0)
            parentHierarchy.ChildCount--;

        if (parentHierarchy.Parent == default && parentHierarchy.FirstChild == default)
            world.RemoveComponent<Hierarchy>(parent);
    }

    private void DestroyMarkedEntities()
    {
        for (int i = 0; i < destroyEntities.Count; i++)
        {
            var entity = destroyEntities[i];

            if (world.HasComponent<CurvyLaser>(entity))
                world.GetComponent<CurvyLaser>(entity).LaserNodes.Dispose();

            world.DestroyEntity(entity);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsPointOffscreen(Vector2 position)
    {
        var bounds = this.bounds;
        return position.X < bounds.Left ||
               position.X > bounds.Right ||
               position.Y < bounds.Top ||
               position.Y > bounds.Bottom;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsCircleOffscreen(Vector2 position, float radius)
    {
        var bounds = this.bounds;
        return position.X + radius < bounds.Left ||
               position.X - radius > bounds.Right ||
               position.Y + radius < bounds.Top ||
               position.Y - radius > bounds.Bottom;
    }

    private bool IsCurvyLaserOffscreen(ref CurvyLaser laser)
    {
        var nodes = laser.LaserNodes;
        if (nodes == null || nodes.Count == 0)
            return true;

        float radius = laser.HalfWidth * 1.415f;
        nodes.AsSpans(out var first, out var second);
        int totalCount = first.Length + second.Length;
        Span<Vector2> allNodes = stackalloc Vector2[totalCount];
        first.CopyTo(allNodes);
        second.CopyTo(allNodes[first.Length..]);

        for (int j = 0; j < allNodes.Length; j++)
        {
            if (!IsCircleOffscreen(allNodes[j], radius))
                return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float CalculateSpriteRadius(ref Renderer rnd)
    {
        float w = rnd.SourceRect.Width * MathF.Abs(rnd.Scale.X);
        float h = rnd.SourceRect.Height * MathF.Abs(rnd.Scale.Y);
        float baseRadius = (w > h ? w : h) * 0.5f;
        return baseRadius * 1.415f;
    }

    public void SetBounds(Rectangle newBounds)
    {
        bounds = newBounds;
    }

    public void TrimExcess()
    {
        for (int i = 0; i < hierarchyEntityBuckets.Count; i++)
            hierarchyEntityBuckets[i].Dispose();

        hierarchyEntityBuckets.Clear();
        maxDepthSeen = -1;
    }

    public void Dispose()
    {
        lifetimeNodes.Dispose();
        softReadyToDie.Dispose();
        hardReadyToDie.Dispose();
        toDestroy.Dispose();
        softDestroyCandidates.Dispose();
        hardDestroyCandidates.Dispose();
        destroyEntities.Dispose();
        subtreeStack.Dispose();

        for (int i = 0; i < hierarchyEntityBuckets.Count; i++)
            hierarchyEntityBuckets[i].Dispose();

        hierarchyEntityBuckets.Dispose();
    }
}
