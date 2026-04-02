using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace ParaTH;

// fixme: this implies that something must something to render to be properly despawned.
// which is not the case for invisible anchor bullets. fix that.
// todo: impl CurvyLaser. wait for all its nodes + halfwidth * 1.415 to go offscreen. manually dispose its queue.
public sealed class LifetimeSystem(World world, Rectangle bounds)
{
    private readonly World world = world;
    private Rectangle bounds = bounds;

    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Lifetime>()
        .WithAny<SpriteRenderer, AnimationRenderer>();

    public void Update()
    {
        var q = world.GetOrCreateQuery(descriptor);

        using var toDestroy = new UnsafePooledList<Entity>(256);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            bool useSprite = archetype.Has<SpriteRenderer>();

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                var transforms = chunk.GetFilledComponentSpan<Transform>();
                var lifetimes = chunk.GetFilledComponentSpan<Lifetime>();

                var sprites = useSprite ?
                    chunk.GetFilledComponentSpan<SpriteRenderer>() : default;
                var animations = !useSprite ?
                    chunk.GetFilledComponentSpan<AnimationRenderer>() : default;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    ref var transform = ref transforms.UnsafeAt(i);
                    ref var lifetime = ref lifetimes.UnsafeAt(i);

                    float radius = CalculatePessimisticRadius(
                        useSprite,
                        ref transform,
                        ref useSprite ? ref sprites.UnsafeAt(i) : ref Unsafe.NullRef<SpriteRenderer>(),
                        ref !useSprite ? ref animations.UnsafeAt(i) : ref Unsafe.NullRef<AnimationRenderer>()
                    );

                    bool isOffscreen =
                        transform.Position.X + radius < bounds.Left ||
                        transform.Position.X - radius > bounds.Right ||
                        transform.Position.Y + radius < bounds.Top ||
                        transform.Position.Y - radius > bounds.Bottom;

                    if (isOffscreen)
                    {
                        if (lifetime.OffscreenFramesToLive > 0)
                        {
                            lifetime.OffscreenFramesToLive--;
                            if (lifetime.OffscreenFramesToLive <= 0)
                                toDestroy.Add(chunk.Entities.UnsafeAt(i));
                        }
                        else
                        {
                            toDestroy.Add(chunk.Entities.UnsafeAt(i));
                        }
                    }
                }
            }
        }

        for (int i = 0; i < toDestroy.Count; i++)
            world.DestroyEntity(toDestroy[i]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float CalculatePessimisticRadius(
        bool useSprite,
        ref Transform tf,
        ref SpriteRenderer sr,
        ref AnimationRenderer ar)
    {
        float w, h;
        if (useSprite)
        {
            w = sr.Sprite.SourceRect.Width;
            h = sr.Sprite.SourceRect.Height;
        }
        else
        {
            var frame = ar.CurrentFrame;
            w = frame.SourceRect.Width;
            h = frame.SourceRect.Height;
        }

        float baseRadius = (w > h ? w : h) * 0.5f;
        float maxScale = tf.Scale.X > tf.Scale.Y ? tf.Scale.X : tf.Scale.Y;
        return baseRadius * maxScale * 1.415f;
    }

    public void SetBounds(Rectangle newBounds) => bounds = newBounds;
}
