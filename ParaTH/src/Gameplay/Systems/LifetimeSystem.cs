using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace ParaTH;

using DepthBuckets = UnsafePooledList<UnsafePooledList<Entity>>;

public sealed class LifetimeSystem(World world, Rectangle bounds) : IDisposable
{
    private readonly World world = world;
    private Rectangle bounds = bounds;

    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Lifetime>();

    private readonly UnsafePooledList<Entity> potentialToDestroy = new(256);
    private readonly UnsafePooledList<Entity> potentialCurvyLaserToDestroy = new(16);   // handled separately because curvy lasers resources that need manual disposal
    private readonly DepthBuckets hierarchyEntityBuckets = new();                       // depth-based buckets used for syncing parent and children's lifetimes
    private int maxDepthSeen = -1;

    public void Update()
    {
        var q = world.GetOrCreateQuery(descriptor);
        var potentialToDestroy = this.potentialToDestroy;
        var potentialCurvyLaserToDestroy = this.potentialCurvyLaserToDestroy;
        var hierarchyEntityBuckets = this.hierarchyEntityBuckets;

        potentialToDestroy.Clear();
        potentialCurvyLaserToDestroy.Clear();

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            bool hasSprite = archetype.Has<SpriteRenderer>();
            bool hasAnimation = archetype.Has<AnimationRenderer>();
            bool hasCurvyLaser = archetype.Has<CurvyLaser>();
            bool hasHrc = archetype.Has<Hierarchy>();

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                var transforms = chunk.GetFilledComponentSpan<Transform>();
                var lifetimes = chunk.GetFilledComponentSpan<Lifetime>();

                var sprites = hasSprite ? chunk.GetFilledComponentSpan<SpriteRenderer>() : default;
                var animations = hasAnimation ? chunk.GetFilledComponentSpan<AnimationRenderer>() : default;
                var curvyLasers = hasCurvyLaser ? chunk.GetFilledComponentSpan<CurvyLaser>() : default;
                var hierarchies = hasHrc ? chunk.GetFilledComponentSpan<Hierarchy>() : default;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    ref var transform = ref transforms.UnsafeAt(i);
                    ref var lifetime = ref lifetimes.UnsafeAt(i);
                    var entity = chunk.Entities.UnsafeAt(i);

                    lifetime.IsReadyToDie = false;

                    // calculate if an entity is offscreen
                    bool isOffscreen;
                    if (hasCurvyLaser)
                    {
                        isOffscreen = IsCurvyLaserOffscreen(ref curvyLasers.UnsafeAt(i));
                    }
                    else if (hasSprite)
                    {
                        float radius = CalculateSpriteRadius(ref transform, ref sprites.UnsafeAt(i));
                        isOffscreen = IsCircleOffscreen(transform.Position, radius);
                    }
                    else if (hasAnimation)
                    {
                        float radius = CalculateAnimationRadius(ref transform, ref animations.UnsafeAt(i));
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
                                lifetime.IsReadyToDie = true;
                        }
                        else
                        {
                            lifetime.IsReadyToDie = true;
                        }

                        if (lifetime.IsReadyToDie)
                        {
                            if (hasCurvyLaser)
                                potentialCurvyLaserToDestroy.Add(entity);
                            else
                                potentialToDestroy.Add(entity);
                        }
                    }

                    // if an entity has hierarchy, it is someone's children and could be someone's parent
                    // add to depth buckets for hierarchy processing
                    if (hasHrc)
                    {
                        int depth = hierarchies.UnsafeAt(i).Depth;

                        if (depth > maxDepthSeen)
                        {
                            for (int d = maxDepthSeen + 1; d <= depth; d++)
                                hierarchyEntityBuckets.Add(new UnsafePooledList<Entity>(64));

                            maxDepthSeen = depth;
                        }

                        hierarchyEntityBuckets[depth].Add(entity);
                    }
                }
            }
        }

        // todo: a lot of random memory accesses, ew
        // process hierarchy from deepest to shallowest (children save parents)
        for (int depth = maxDepthSeen; depth >= 0; depth--)
        {
            var bucket = hierarchyEntityBuckets[depth];
            var bucketSpan = bucket.AsSpan();

            for (int i = 0; i < bucketSpan.Length; i++)
            {
                var childEntity = bucketSpan.UnsafeAt(i);
                ref var hrc = ref world.GetComponent<Hierarchy>(childEntity);

                if (!world.IsAlive(hrc.Parent))
                    continue;

                ref var childLt = ref world.GetComponent<Lifetime>(childEntity);
                if (!childLt.IsReadyToDie)
                {
                    ref var parentLt = ref world.GetComponent<Lifetime>(hrc.Parent);
                    parentLt.IsReadyToDie = false;
                }
            }
        }

        // process hierarchy from shallowest to deepest (parents save children)
        for (int depth = 0; depth <= maxDepthSeen; depth++)
        {
            var bucket = hierarchyEntityBuckets[depth];
            var bucketSpan = bucket.AsSpan();

            for (int i = 0; i < bucketSpan.Length; i++)
            {
                var childEntity = bucketSpan.UnsafeAt(i);
                ref var hrc = ref world.GetComponent<Hierarchy>(childEntity);

                if (!world.IsAlive(hrc.Parent))
                    continue;

                ref var parentLt = ref world.GetComponent<Lifetime>(hrc.Parent);
                if (!parentLt.IsReadyToDie)
                {
                    ref var childLt = ref world.GetComponent<Lifetime>(childEntity);
                    childLt.IsReadyToDie = false;
                }
            }

            bucket.Clear();
        }

        // hot path: regular bullets
        for (int i = 0; i < potentialToDestroy.Count; i++)
        {
            var entity = potentialToDestroy[i];
            if (world.GetComponent<Lifetime>(entity).IsReadyToDie)
                world.DestroyEntity(entity);
        }

        // cold path: curvy lasers
        for (int i = 0; i < potentialCurvyLaserToDestroy.Count; i++)
        {
            var entity = potentialCurvyLaserToDestroy[i];
            if (world.GetComponent<Lifetime>(entity).IsReadyToDie)
            {
                ref var laser = ref world.GetComponent<CurvyLaser>(entity);
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

    [SkipLocalsInit]
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
    private static float CalculateSpriteRadius(ref Transform tf, ref SpriteRenderer sr)
    {
        float w = sr.Sprite.SourceRect.Width;
        float h = sr.Sprite.SourceRect.Height;
        float baseRadius = (w > h ? w : h) * 0.5f;
        float maxScale = tf.Scale.X > tf.Scale.Y ? tf.Scale.X : tf.Scale.Y;
        return baseRadius * maxScale * 1.415f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float CalculateAnimationRadius(ref Transform tf, ref AnimationRenderer ar)
    {
        var frame = ar.CurrentFrame;
        float w = frame.SourceRect.Width;
        float h = frame.SourceRect.Height;
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
        potentialToDestroy.Dispose();
        potentialCurvyLaserToDestroy.Dispose();

        for (int i = 0; i < hierarchyEntityBuckets.Count; i++)
            hierarchyEntityBuckets[i].Dispose();

        hierarchyEntityBuckets.Dispose();
    }
}
