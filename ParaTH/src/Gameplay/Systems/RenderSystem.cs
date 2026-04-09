using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

// handle rendering and transient visual states
public sealed class RenderSystem(World world, StgBatch batch, Rectangle bounds) : IDisposable
{
    private Rectangle bounds = bounds;
    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Renderer>();

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
        private const int FlagMask = unchecked((int)0x80000000);
        private const int IndexMask = 0x7FFFFFFF;
        private int packedSpawnIdAndFlag;
        public uint SpawnId;

        public int Index
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => packedSpawnIdAndFlag & IndexMask;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => packedSpawnIdAndFlag = (packedSpawnIdAndFlag & FlagMask) | (value & IndexMask);
        }

        public bool IsCurvyLaser
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (packedSpawnIdAndFlag & FlagMask) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => packedSpawnIdAndFlag = value ? (packedSpawnIdAndFlag | FlagMask) : (packedSpawnIdAndFlag & IndexMask);
        }

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
        var deferredDraws = this.deferredDraws;
        var deferredCurvyLaserDraws = this.deferredCurvyLaserDraws;
        var sortKeys = this.sortKeys;

        deferredDraws.Clear();
        deferredCurvyLaserDraws.Clear();
        sortKeys.Clear();

        // used for stackallocing curvy laser buffer
        int maxLaserLength = 0;

        var q = world.GetOrCreateQuery(descriptor);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            bool hasSpawnEffect = archetype.Has<SpawnEffect>();
            bool hasCurvyLaser = archetype.Has<CurvyLaser>();

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                chunk.GetFilledComponentSpan<Transform, Renderer>(
                    out var transforms, out var states);

                var spawnAnims = hasSpawnEffect ?
                    chunk.GetFilledComponentSpan<SpawnEffect>() : default;
                var curvyLasers = hasCurvyLaser ?
                    chunk.GetFilledComponentSpan<CurvyLaser>() : default;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    ref var state = ref states.UnsafeAt(i);

                    var dp = new DrawParams
                    {
                        Texture = state.Texture,
                        SourceRect = state.SourceRect,
                        Anchor = state.Anchor,
                        Scale = state.Scale,
                        Color = state.Color,
                    };

                    if (!hasCurvyLaser)
                    {
                        if (hasSpawnEffect)
                            ApplySpawnEffect(ref spawnAnims.UnsafeAt(i), in state, ref dp);

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
                    else
                    {
                        // curvy lasers don't use spawn effecct
                        ref var laser = ref curvyLasers.UnsafeAt(i);

                        maxLaserLength = Math.Max(maxLaserLength, laser.Length);

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
                }
            }
        }

        var keysSpan = sortKeys.AsSpan();

        keysSpan.Sort();

        var dataSpan = deferredDraws.AsSpan();
        var laserDataSpan = deferredCurvyLaserDraws.AsSpan();

        Span<Vector2> laserNodeBuffer = stackalloc Vector2[maxLaserLength];

        for (int i = 0; i < keysSpan.Length; i++)
        {
            ref var key = ref keysSpan.UnsafeAt(i);

            if (!key.IsCurvyLaser)
            {
                ref var d = ref dataSpan.UnsafeAt(key.Index);
                batch.Draw(
                    d.Texture, d.Position, d.SourceRect, d.Color,
                    d.Rotation, d.Anchor, d.Scale,
                    SpriteEffects.None, d.Layer, d.BlendState);
            }
            else
            {
                ref var d = ref laserDataSpan.UnsafeAt(key.Index);
                int nodeCount = d.LaserNodes.Count;

                if (nodeCount == 0)
                    continue;

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
                    Span<Vector2> renderNodeSpan = laserNodeBuffer.Slice(nodeCount);
                    first.CopyTo(renderNodeSpan);
                    second.CopyTo(renderNodeSpan.Slice(first.Length));

                    batch.DrawCurvyLaser(
                        d.Texture, d.SourceRect, d.TextureRotation,
                        renderNodeSpan, d.HalfWidth,
                        d.Color, d.Layer, d.BlendState);
                }
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

    // ────────────────── Effects ──────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ApplySpawnEffect(
#pragma warning disable RCS1242
        ref SpawnEffect effect, in Renderer state, ref DrawParams dp)
#pragma warning restore RCS1242
    {
        if (effect.Counter >= effect.Duration) return;

        dp.Texture = effect.Sprite.Texture;
        dp.SourceRect = effect.Sprite.SourceRect;
        dp.Anchor = effect.Sprite.Anchor;

        float t = (float)(effect.Counter + 1) / effect.Duration;
        dp.Scale.X = MathHelper.Lerp(
            effect.StartScale.X, state.Scale.X, Easing.Evaluate(effect.TypeX, t));
        dp.Scale.Y = MathHelper.Lerp(
            effect.StartScale.Y, state.Scale.Y, Easing.Evaluate(effect.TypeY, t));
        dp.Color.A = (byte)MathHelper.Lerp(
            (float)effect.StartAlpha * 255f, state.Color.A, t);
    }

    public void Dispose()
    {
        deferredDraws.Clear();
        deferredCurvyLaserDraws.Clear();
        sortKeys.Clear();
    }
}
