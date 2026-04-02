using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

// todo: impl curvy laser drawing
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

    private struct DeferredDrawData
    {
        public Texture2D Texture;
        public Rectangle SourceRect;
        public Vector2 Position;
        public Vector2 Anchor;
        public Vector2 Scale;
        public Color Color;
        public float Rotation;
        public byte Layer;
        public StgBlendState BlendState;
    }

    private struct DrawSortKey : IComparable<DrawSortKey>
    {
        public uint SpawnId;
        public int Index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int CompareTo(DrawSortKey other)
        {
            return SpawnId.CompareTo(other.SpawnId);
        }
    }

    private readonly UnsafePooledList<DeferredDrawData> deferredDraws = new(16384);
    private readonly UnsafePooledList<DrawSortKey> sortKeys = new(16384);

    public void Update()
    {
        deferredDraws.Clear();
        sortKeys.Clear();

        var q = world.GetOrCreateQuery(descriptor);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            bool useSpriteRenderer = archetype.Has<SpriteRenderer>();
            bool hasSpawnAnim = archetype.Has<SpawnAnimation>();

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

                    float halfW = dp.SourceRect.Width * 0.5f;
                    float halfH = dp.SourceRect.Height * 0.5f;
                    float radius = (halfW > halfH ? halfW : halfH) * 1.415f;

                    float px = transform.Position.X;
                    float py = transform.Position.Y;

                    var bounds = this.bounds;

                    if (px + radius > bounds.Left && px - radius < bounds.Right &&
                        py + radius > bounds.Top && py - radius < bounds.Bottom)
                    {
                        int currentIndex = deferredDraws.Count;

                        deferredDraws.Add(new DeferredDrawData
                        {
                            Texture = dp.Texture,
                            SourceRect = dp.SourceRect,
                            Position = transform.Position,
                            Anchor = dp.Anchor,
                            Scale = dp.Scale,
                            Color = dp.Color,
                            Rotation = state.Rotation,
                            Layer = state.Layer,
                            BlendState = state.BlendState
                        });

                        sortKeys.Add(new DrawSortKey
                        {
                            SpawnId = state.SpawnId,
                            Index = currentIndex
                        });
                    }
                }
            }
        }

        var keysSpan = sortKeys.AsSpan();
        var dataSpan = deferredDraws.AsSpan();

        if (keysSpan.Length > 1)
            keysSpan.Sort();

        for (int i = 0; i < keysSpan.Length; i++)
        {
            ref var d = ref dataSpan.UnsafeAt(keysSpan.UnsafeAt(i).Index);
            batch.Draw(
                d.Texture, d.Position, d.SourceRect, d.Color,
                d.Rotation, d.Anchor, d.Scale,
                SpriteEffects.None, d.Layer, d.BlendState);
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
#pragma warning disable RCS1242
        ref SpawnAnimation anim, in RenderState state, ref DrawParams dp)
#pragma warning restore RCS1242
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
