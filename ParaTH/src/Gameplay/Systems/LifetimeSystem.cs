using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace ParaTH;

// todo: make this handle hierarcy
public sealed class LifetimeSystem(World world, Rectangle bounds) : IDisposable
{
    private readonly World world = world;
    private Rectangle bounds = bounds;

    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Lifetime>();

    private readonly UnsafePooledList<Entity> toDestroy = new(256);
    private readonly UnsafePooledList<Entity> curvyLaserToDestroy = new(16);

    public void Update()
    {
        var q = world.GetOrCreateQuery(descriptor);
        var toDestroy = this.toDestroy;
        var curvyLaserToDestroy = this.curvyLaserToDestroy;

        toDestroy.Clear();
        curvyLaserToDestroy.Clear();

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            bool hasSprite = archetype.Has<SpriteRenderer>();
            bool hasAnimation = archetype.Has<AnimationRenderer>();
            bool hasCurvyLaser = archetype.Has<CurvyLaser>();

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                var transforms = chunk.GetFilledComponentSpan<Transform>();
                var lifetimes = chunk.GetFilledComponentSpan<Lifetime>();

                var sprites = hasSprite ? chunk.GetFilledComponentSpan<SpriteRenderer>() : default;
                var animations = hasAnimation ? chunk.GetFilledComponentSpan<AnimationRenderer>() : default;
                var curvyLasers = hasCurvyLaser ? chunk.GetFilledComponentSpan<CurvyLaser>() : default;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    ref var transform = ref transforms.UnsafeAt(i);
                    ref var lifetime = ref lifetimes.UnsafeAt(i);

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
                                if (hasCurvyLaser)
                                    curvyLaserToDestroy.Add(chunk.Entities.UnsafeAt(i));
                                else
                                    toDestroy.Add(chunk.Entities.UnsafeAt(i));
                            }
                        }
                        else
                        {
                            if (hasCurvyLaser)
                                curvyLaserToDestroy.Add(chunk.Entities.UnsafeAt(i));
                            else
                                toDestroy.Add(chunk.Entities.UnsafeAt(i));
                        }
                    }
                }
            }
        }

        // hot path: regular bullets
        for (int i = 0; i < toDestroy.Count; i++)
            world.DestroyEntity(toDestroy[i]);

        // cold path: curvy lasers need manual queue disposal
        for (int i = 0; i < curvyLaserToDestroy.Count; i++)
        {
            var entity = curvyLaserToDestroy[i];
            ref var laser = ref world.GetComponent<CurvyLaser>(entity);
            laser.LaserNodes.Dispose();
            world.DestroyEntity(entity);
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
    }
}
