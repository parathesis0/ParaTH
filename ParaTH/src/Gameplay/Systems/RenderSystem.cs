using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed class RenderSystem(World world, StgBatch batch, Rectangle bounds) : IDisposable
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

    private struct DeferredCurvyLaserDrawData
    {
        public Texture2D Texture;
        public Rectangle SourceRect;
        public float TextureRotation;
        public UnsafePooledQueue<Vector2> LaserNodes;
        public float HalfWidth;
        public Color Color;
        public byte Layer;
        public StgBlendState BlendState;
    }

    private struct DrawSortKey : IComparable<DrawSortKey>
    {
        public uint SpawnId;
        public int Index;
        public bool IsCurvyLaser;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int CompareTo(DrawSortKey other)
        {
            return SpawnId.CompareTo(other.SpawnId);
        }
    }

    private readonly UnsafePooledList<DeferredDrawData> deferredDraws = new(16384);
    private readonly UnsafePooledList<DeferredCurvyLaserDrawData> deferredCurvyLaserDraws = new(256);
    private readonly UnsafePooledList<DrawSortKey> sortKeys = new(16384);

    [SkipLocalsInit]
    public void Update()
    {
        deferredDraws.Clear();
        deferredCurvyLaserDraws.Clear();
        sortKeys.Clear();

        var q = world.GetOrCreateQuery(descriptor);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            bool useSpriteRenderer = archetype.Has<SpriteRenderer>();
            bool hasSpawnAnim = archetype.Has<SpawnAnimation>();
            bool hasCurvyLaser = archetype.Has<CurvyLaser>();

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                var transforms = chunk.GetFilledComponentSpan<Transform>();
                var states = chunk.GetFilledComponentSpan<RenderState>();

                var spriteRenderers = useSpriteRenderer ?
                    chunk.GetFilledComponentSpan<SpriteRenderer>() : default;
                var animRenderers = !useSpriteRenderer ?
                    chunk.GetFilledComponentSpan<AnimationRenderer>() : default;
                var spawnAnims = hasSpawnAnim && !hasCurvyLaser ?
                    chunk.GetFilledComponentSpan<SpawnAnimation>() : default;
                var curvyLasers = hasCurvyLaser ?
                    chunk.GetFilledComponentSpan<CurvyLaser>() : default;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
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

                    if (hasCurvyLaser)
                    {
                        ref var laser = ref curvyLasers.UnsafeAt(i);

                        if (IsCurvyLaserVisible(laser.LaserNodes, laser.HalfWidth))
                        {
                            int currentIndex = deferredCurvyLaserDraws.Count;

                            deferredCurvyLaserDraws.Add(new DeferredCurvyLaserDrawData
                            {
                                Texture = dp.Texture,
                                SourceRect = dp.SourceRect,
                                TextureRotation = state.Rotation,
                                LaserNodes = laser.LaserNodes,
                                HalfWidth = laser.HalfWidth,
                                Color = dp.Color,
                                Layer = state.Layer,
                                BlendState = state.BlendState,
                            });

                            sortKeys.Add(new DrawSortKey
                            {
                                SpawnId = state.SpawnId,
                                Index = currentIndex,
                                IsCurvyLaser = true,
                            });
                        }
                    }
                    else
                    {
                        if (hasSpawnAnim)
                            ApplySpawnAnimation(ref spawnAnims.UnsafeAt(i), in state, ref dp);

                        ref var transform = ref transforms.UnsafeAt(i);

                        float halfW = dp.SourceRect.Width * 0.5f;
                        float halfH = dp.SourceRect.Height * 0.5f;
                        float radius = (halfW > halfH ? halfW : halfH) * 1.415f;

                        float px = transform.Position.X;
                        float py = transform.Position.Y;

                        var b = this.bounds;

                        if (px + radius > b.Left && px - radius < b.Right &&
                            py + radius > b.Top && py - radius < b.Bottom)
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
                                Index = currentIndex,
                            });
                        }
                    }
                }
            }
        }

        var keysSpan = sortKeys.AsSpan();

        if (keysSpan.Length > 1)
            keysSpan.Sort();

        var dataSpan = deferredDraws.AsSpan();
        var laserDataSpan = deferredCurvyLaserDraws.AsSpan();

        for (int i = 0; i < keysSpan.Length; i++)
        {
            ref var key = ref keysSpan.UnsafeAt(i);

            if (key.IsCurvyLaser)
            {
                ref var d = ref laserDataSpan.UnsafeAt(key.Index);
                int nodeCount = d.LaserNodes.Count;

                if (nodeCount == 0) continue;

                d.LaserNodes.AsSpans(out var first, out var second);

                if (second.Length == 0)
                {
                    batch.DrawCurvyLaser(
                        d.Texture, d.SourceRect, d.TextureRotation,
                        first, d.HalfWidth,
                        d.Color, d.Layer, d.BlendState);
                }
                else
                {
                    Span<Vector2> renderNodeSpan = stackalloc Vector2[nodeCount]; // hope this doesnt overflow
                    first.CopyTo(renderNodeSpan);
                    second.CopyTo(renderNodeSpan.Slice(first.Length));

                    batch.DrawCurvyLaser(
                        d.Texture, d.SourceRect, d.TextureRotation,
                        renderNodeSpan, d.HalfWidth,
                        d.Color, d.Layer, d.BlendState);
                }
            }
            else
            {
                ref var d = ref dataSpan.UnsafeAt(key.Index);
                batch.Draw(
                    d.Texture, d.Position, d.SourceRect, d.Color,
                    d.Rotation, d.Anchor, d.Scale,
                    SpriteEffects.None, d.Layer, d.BlendState);
            }
        }
    }

    // ────────────────── Visibility ──────────────────

    private bool IsCurvyLaserVisible(UnsafePooledQueue<Vector2> nodes, float hw)
    {
        nodes.AsSpans(out var first, out var second);
        var b = bounds;

        if (AnyNodeInBounds(first, hw, b))
            return true;

        if (second.Length > 0 && AnyNodeInBounds(second, hw, b))
            return true;

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool AnyNodeInBounds(Span<Vector2> nodes, float hw, Rectangle b)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            ref var n = ref nodes.UnsafeAt(i);
            if (n.X + hw > b.Left && n.X - hw < b.Right &&
                n.Y + hw > b.Top && n.Y - hw < b.Bottom)
            {
                return true;
            }
        }
        return false;
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

    public void Dispose()
    {
        deferredDraws.Clear();
        deferredCurvyLaserDraws.Clear();
        sortKeys.Clear();
    }
}
