using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace ParaTH;

using DepthBuckets = UnsafePooledList<UnsafePooledList<LifetimeSystem.HierarchyPair>>;

// handles the destruction of offbound entities
// a parent children hierarchy is seen as a whole and will not be destroyed until all its entities are offscreen
[SkipLocalsInit]
public sealed class LifetimeSystem(World world, Rectangle bounds) : IDisposable
{
    private readonly World world = world;
    private Rectangle bounds = bounds;

    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Lifetime>();

    private readonly UnsafeBitset isReadyToDie = new(4);                                // bitset for checking whether an entity should die
    private readonly UnsafePooledList<Entity> potentialToDestroy = new(256);            // contains entites without curvy laser components
    private readonly UnsafePooledList<Entity> potentialCurvyLaserToDestroy = new(16);   // handled separately because curvy lasers owns resources that need manual disposal
    private readonly UnsafePooledList<Entity> potentialLaserToDestroy = new(16);        // same as curvy lasers but for static lasers
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
        var potentialToDestroy = this.potentialToDestroy;
        var potentialCurvyLaserToDestroy = this.potentialCurvyLaserToDestroy;
        var potentialLaserToDestroy = this.potentialLaserToDestroy;
        var hierarchyEntityBuckets = this.hierarchyEntityBuckets;
        var isReadyToDie = this.isReadyToDie;

        potentialToDestroy.Clear();
        potentialCurvyLaserToDestroy.Clear();
        potentialLaserToDestroy.Clear();
        isReadyToDie.Clear();
        isReadyToDie.EnsureCapacity(world.MaxEntityId);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            bool hasRenderer = archetype.Has<Renderer>();
            bool hasCurvyLaser = archetype.Has<CurvyLaser>();
            bool hasLaser = !hasCurvyLaser && archetype.Has<Laser>();
            bool hasHrc = archetype.Has<Hierarchy>();

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                chunk.GetFilledComponentSpan<Transform, Lifetime>(
                    out var transforms, out var lifetimes);

                var renderers = hasRenderer ? chunk.GetFilledComponentSpan<Renderer>() : default;
                var curvyLasers = hasCurvyLaser ? chunk.GetFilledComponentSpan<CurvyLaser>() : default;
                var lasers = hasLaser ? chunk.GetFilledComponentSpan<Laser>() : default;
                var hierarchies = hasHrc ? chunk.GetFilledComponentSpan<Hierarchy>() : default;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    ref var transform = ref transforms.UnsafeAt(i);
                    ref var lifetime = ref lifetimes.UnsafeAt(i);
                    var entity = chunk.Entities.UnsafeAt(i);

                    // calculate if an entity is offscreen
                    bool isOffscreen;
                    if (hasCurvyLaser)
                    {
                        isOffscreen = IsCurvyLaserOffscreen(ref curvyLasers.UnsafeAt(i));
                    }
                    else if (hasLaser)
                    {
                        isOffscreen = IsLaserOffscreen(ref lasers.UnsafeAt(i), transform.Rotation);
                    }
                    else if (hasRenderer)
                    {
                        float radius = CalculateSpriteRadius(ref transform, ref renderers.UnsafeAt(i));
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
                                isReadyToDie.Set(entity.Id);
                        }
                        else
                        {
                            isReadyToDie.Set(entity.Id);
                        }

                        if (isReadyToDie.IsSet(entity.Id))
                        {
                            if (hasCurvyLaser)
                                potentialCurvyLaserToDestroy.Add(entity);
                            else if (hasLaser)
                                potentialLaserToDestroy.Add(entity);
                            else
                                potentialToDestroy.Add(entity);
                        }
                    }

                    // if an entity has hierarchy, it is someone's children and could be someone's parent
                    // add to depth buckets for hierarchy processing
                    if (hasHrc)
                    {
                        ref var hierarchy = ref hierarchies.UnsafeAt(i);
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

                if (!isReadyToDie.IsSet(pair.Child.Id))
                    isReadyToDie.Unset(pair.Parent.Id);
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

                if (!isReadyToDie.IsSet(pair.Parent.Id))
                    isReadyToDie.Unset(pair.Child.Id);
            }

            bucket.Clear();
        }

        // hot path: regular bullets
        for (int i = 0; i < potentialToDestroy.Count; i++)
        {
            var entity = potentialToDestroy[i];
            if (isReadyToDie.IsSet(entity.Id))
                world.DestroyEntity(entity);
        }

        // cold path: curvy lasers
        for (int i = 0; i < potentialCurvyLaserToDestroy.Count; i++)
        {
            var entity = potentialCurvyLaserToDestroy[i];
            if (isReadyToDie.IsSet(entity.Id))
            {
                ref var laser = ref world.GetComponent<CurvyLaser>(entity);
                laser.LaserNodes.Dispose();
                world.DestroyEntity(entity);
            }
        }

        // cold path: static lasers
        for (int i = 0; i < potentialLaserToDestroy.Count; i++)
        {
            var entity = potentialLaserToDestroy[i];
            if (isReadyToDie.IsSet(entity.Id))
            {
                ref var laser = ref world.GetComponent<Laser>(entity);
                laser.LaserNodes.Dispose();
                world.DestroyEntity(entity);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsPointOffscreen(Vector2 position)
    {
        return position.X < bounds.Left ||
               position.X > bounds.Right ||
               position.Y < bounds.Top ||
               position.Y > bounds.Bottom;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsCircleOffscreen(Vector2 position, float radius)
    {
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

    private bool IsLaserOffscreen(ref Laser laser, float rotation)
    {
        var nodes = laser.LaserNodes;
        if (nodes == null || nodes.Count == 0)
            return true;

        float radius = laser.HalfWidth * 1.415f;
        var raw = nodes.AsSpan();

        Vector2 origin = raw.UnsafeAt(0);
        if (!IsCircleOffscreen(origin, radius))
            return false;

        float cos = MathF.Cos(rotation);
        float sin = MathF.Sin(rotation);

        for (int i = 1; i < raw.Length; i++)
        {
            Vector2 rel = raw.UnsafeAt(i) - origin;
            Vector2 rotated = new(
                origin.X + rel.X * cos - rel.Y * sin,
                origin.Y + rel.X * sin + rel.Y * cos);
            if (!IsCircleOffscreen(rotated, radius))
                return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float CalculateSpriteRadius(ref Transform tf, ref Renderer rnd)
    {
        float w = rnd.SourceRect.Width;
        float h = rnd.SourceRect.Height;
        float baseRadius = (w > h ? w : h) * 0.5f;
        float maxScale = tf.Scale.X > tf.Scale.Y ? tf.Scale.X : tf.Scale.Y;
        return baseRadius * maxScale * 1.415f;
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
        isReadyToDie.Dispose();
        potentialToDestroy.Dispose();
        potentialCurvyLaserToDestroy.Dispose();
        potentialLaserToDestroy.Dispose();

        for (int i = 0; i < hierarchyEntityBuckets.Count; i++)
            hierarchyEntityBuckets[i].Dispose();

        hierarchyEntityBuckets.Dispose();
    }
}
