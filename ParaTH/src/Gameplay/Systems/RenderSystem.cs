using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed class RenderSystem(World world, StgBatch batch, Rectangle bounds)
{
    private Rectangle bounds = bounds;
    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, RenderState>()
        .WithAny<SpriteRenderer, AnimationRenderer>();

    private struct DrawParams
    {
        public Texture2D Texture;
        public Rectangle SourceRect;
        public Vector2 Anchor;
        public Vector2 Scale;
        public Color Color;
    }

    public void Update()
    {
        var q = world.GetOrCreateQuery(descriptor);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            bool useSpriteRenderer = archetype.Has<SpriteRenderer>();
            bool hasSpawnAnim = archetype.Has<SpawnAnimation>();
            // bool hasDeathAnim  = archetype.Has<DeathAnimation>();
            // bool hasDamageFlash = archetype.Has<DamageFlash>();

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                var transforms = chunk.GetFilledComponentSpan<Transform>();
                var states = chunk.GetFilledComponentSpan<RenderState>();

                var spriteRenderers = useSpriteRenderer ?
                    chunk.GetFilledComponentSpan<SpriteRenderer>() : default;
                var animRenderers = !useSpriteRenderer ?
                    chunk.GetFilledComponentSpan<AnimationRenderer>() : default;
                var spawnAnims = hasSpawnAnim ?
                    chunk.GetFilledComponentSpan<SpawnAnimation>() : default;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    ref var transform = ref transforms.UnsafeAt(i);
                    ref var state = ref states.UnsafeAt(i);

                    var dp = new DrawParams
                    {
                        Scale = state.Scale,
                        Color = state.Color,
                    };

                    if (useSpriteRenderer)
                        ResolveSprite(ref spriteRenderers.UnsafeAt(i), ref dp);
                    else
                        ResolveAnimation(ref animRenderers.UnsafeAt(i), ref dp);

                    if (hasSpawnAnim)
                        ApplySpawnAnimation(ref spawnAnims.UnsafeAt(i), in state, ref dp);

                    // if (hasDeathAnim)
                    //     ApplyDeathAnimation(ref deathAnims.UnsafeAt(i), in state, ref dp);
                    // if (hasDamageFlash)
                    //     ApplyDamageFlash(ref flashes.UnsafeAt(i), in state, ref dp);

                    // TODO: PLACEHOLDER
                    float halfW = dp.SourceRect.Width * 0.5f;
                    float halfH = dp.SourceRect.Height * 0.5f;
                    float radius = (halfW > halfH ? halfW : halfH) * 1.415f;

                    float px = transform.Position.X;
                    float py = transform.Position.Y;

                    var bounds = this.bounds;

                    if (px + radius > bounds.Left && px - radius < bounds.Right &&
                        py + radius > bounds.Top && py - radius < bounds.Bottom)
                    {
                        batch.Draw(
                            dp.Texture, transform.Position, dp.SourceRect, dp.Color,
                            state.Rotation, dp.Anchor, dp.Scale,
                            SpriteEffects.None, state.Layer, state.BlendState);
                    }
                }
            }
        }
    }

    // ────────────────── Renderers ──────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ResolveSprite(ref SpriteRenderer r, ref DrawParams dp)
    {
        dp.Texture = r.Sprite.Texture;
        dp.SourceRect = r.Sprite.SourceRect;
        dp.Anchor = r.Sprite.Anchor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ResolveAnimation(ref AnimationRenderer r, ref DrawParams dp)
    {
        dp.Texture = r.Animation.Texture;
        dp.SourceRect = r.CurrentFrame.SourceRect;
        dp.Anchor = r.CurrentFrame.Anchor;
    }

    // ────────────────── Effects ──────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ApplySpawnAnimation(
#pragma warning disable RCS1242 // Do not pass non-read-only struct by read-only reference
        ref SpawnAnimation anim, in RenderState state, ref DrawParams dp)
#pragma warning restore RCS1242 // Do not pass non-read-only struct by read-only reference
    {
        if (anim.Counter >= anim.Duration) return;

        dp.Texture = anim.Sprite.Texture;
        dp.SourceRect = anim.Sprite.SourceRect;
        dp.Anchor = anim.Sprite.Anchor;

        float t = (float)(anim.Counter + 1) / anim.Duration;
        dp.Scale.X = MathHelper.Lerp(
            anim.StartScale.X, state.Scale.X, Easing.Evaluate(anim.TypeX, t));
        dp.Scale.Y = MathHelper.Lerp(
            anim.StartScale.Y, state.Scale.Y, Easing.Evaluate(anim.TypeY, t));
        dp.Color.A = (byte)MathHelper.Lerp(
            (float)anim.StartAlpha * 255f, state.Color.A, t);
    }
}
