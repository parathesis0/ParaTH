using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed class LifetimeSystem : IDisposable
{
    private readonly World world;
    private Rectangle bounds;

    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Lifetime>();

    private readonly UnsafePooledList<Entity> toDestroy = new(256);
    private readonly UnsafePooledList<Entity> curvyLaserToDestroy = new(16);

    private struct HierarchyNode : IComparable<HierarchyNode>
    {
        public Entity Entity;
        public Entity Parent;
        public int Depth;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int CompareTo(HierarchyNode other)
        {
            return Depth.CompareTo(other.Depth);
        }
    }
    private readonly UnsafePooledList<HierarchyNode> hierarchyNodes = new(256);

    public LifetimeSystem(World world, Rectangle bounds)
    {
        this.world = world;
        this.bounds = bounds;
    }

    public void Update()
    {
        var q = world.GetOrCreateQuery(descriptor);
        var toDestroy = this.toDestroy;
        var curvyLaserToDestroy = this.curvyLaserToDestroy;
        var hierarchyNodes = this.hierarchyNodes;

        toDestroy.Clear();
        curvyLaserToDestroy.Clear();
        hierarchyNodes.Clear();

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

                    if (hasHrc)
                    {
                        hierarchyNodes.Add(new HierarchyNode
                        {
                            Entity = entity,
                            Parent = hierarchies.UnsafeAt(i).Parent,
                            Depth = hierarchies.UnsafeAt(i).Depth
                        });
                    }

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

                    if (isOffscreen)
                    {
                        if (lifetime.OffscreenFramesToLive > 0)
                        {
                            lifetime.OffscreenFramesToLive--;
                            if (lifetime.OffscreenFramesToLive <= 0)
                            {
                                lifetime.IsReadyToDie = true;
                                if (hasCurvyLaser) curvyLaserToDestroy.Add(entity);
                                else toDestroy.Add(entity);
                            }
                        }
                        else
                        {
                            lifetime.IsReadyToDie = true;
                            if (hasCurvyLaser) curvyLaserToDestroy.Add(entity);
                            else toDestroy.Add(entity);
                        }
                    }
                }
            }
        }

        var hNodesSpan = hierarchyNodes.AsSpan();
        if (hNodesSpan.Length > 0)
        {
            if (hNodesSpan.Length > 1) // fuck you
                hNodesSpan.Sort();

            for (int i = hNodesSpan.Length - 1; i >= 0; i--)
            {
                ref var node = ref hNodesSpan.UnsafeAt(i);
                if (!world.IsAlive(node.Parent)) continue;

                ref var childLt = ref world.GetComponent<Lifetime>(node.Entity);
                if (!childLt.IsReadyToDie)
                {
                    ref var parentLt = ref world.GetComponent<Lifetime>(node.Parent);
                    parentLt.IsReadyToDie = false;
                }
            }

            for (int i = 0; i < hNodesSpan.Length; i++)
            {
                ref var node = ref hNodesSpan.UnsafeAt(i);
                if (!world.IsAlive(node.Parent)) continue;

                ref var parentLt = ref world.GetComponent<Lifetime>(node.Parent);
                if (!parentLt.IsReadyToDie)
                {
                    ref var childLt = ref world.GetComponent<Lifetime>(node.Entity);
                    childLt.IsReadyToDie = false;
                }
            }
        }

        // hot path: regular bullets
        for (int i = 0; i < toDestroy.Count; i++)
        {
            var entity = toDestroy[i];
            if (world.GetComponent<Lifetime>(entity).IsReadyToDie)
                world.DestroyEntity(entity);
        }

        // cold path: curvy lasers
        for (int i = 0; i < curvyLaserToDestroy.Count; i++)
        {
            var entity = curvyLaserToDestroy[i];
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

    public void SetBounds(Rectangle newBounds) => bounds = newBounds;

    public void Dispose()
    {
        toDestroy.Dispose();
        curvyLaserToDestroy.Dispose();
        hierarchyNodes.Dispose();
    }
}
