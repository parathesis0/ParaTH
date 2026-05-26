using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

// handles rendering and transient visual states
[SkipLocalsInit]
public sealed class RenderSystem(World world, StgBatch batch, Rectangle bounds) : IDisposable
{
    private Rectangle bounds = bounds;
    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Renderer>();

    private QueryDescriptor colliderDescriptor = new QueryDescriptor()
        .WithAll<Transform, Collider>();

    // debug: draw green collider outlines on top of everything
    public bool DebugDrawColliders;

    private const int DebugCircleSides = 16;
    private const byte DebugLayer = 255;
    private static readonly Color DebugColor = new(0, 255, 0, 128);

    // 64 bytes
    private struct DeferredDrawData
    {
        public Texture2D Texture;           // 8
        public Rectangle SourceRect;        // 16: 4 + 4 + 4 + 4
        public Vector2 Position;            // 8: 4 + 4
        public Vector2 Anchor;              // 8: 4 + 4
        public Vector2 Scale;               // 8: 4 + 4
        public Color Color;                 // 4
        public float Rotation;              // 4
        public byte Layer;                  // 1
        public StgBlendState BlendState;    // 1
                                            // 6 padding
    }

    // 64 bytes
    private struct DeferredCurvyLaserDrawData
    {
        public Texture2D Texture;                       // 8
        public UnsafePooledQueue<Vector2> LaserNodes;   // 8
        public SpriteAsset SourceSprite;                // 8
        public Vector2 SourceScale;                     // 8: 4 + 4
        public Rectangle SourceRect;                    // 16: 4 + 4 + 4 + 4
        public float TextureRotation;                   // 4
        public float HalfWidth;                         // 4
        public Color Color;                             // 4
        public byte Layer;                              // 1
        public StgBlendState BlendState;                // 1
                                                        // 2 padding
    }

    // packed to 8 bytes for faster sorting & swapping
    private struct DrawSortKey : IComparable<DrawSortKey>
    {
        // [63:56] Layer (8)  [55:24] SpawnId (32)  [20] IsCurvyLaser (1)  [19:0] Index (20)
        private const int IndexBits = 20;
        private const ulong IndexMask = (1UL << IndexBits) - 1;
        private const int LaserBit = 20;
        private const ulong LaserFlag = 1UL << LaserBit;
        private const int SpawnIdShift = 24;
        private const int LayerShift = 56;

        private ulong packed;

        public int Index
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (int)(packed & IndexMask);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => packed = (packed & ~IndexMask) | ((uint)value & IndexMask);
        }

        public bool IsCurvyLaser
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (packed & LaserFlag) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => packed = value ? (packed | LaserFlag) : (packed & ~LaserFlag);
        }

        public uint SpawnId
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => packed = (packed & ~(0xFFFF_FFFFUL << SpawnIdShift)) | ((ulong)value << SpawnIdShift);
        }

        public byte Layer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => packed = (packed & ~(0xFFUL << LayerShift)) | ((ulong)value << LayerShift);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int CompareTo(DrawSortKey other)
        {
            return packed.CompareTo(other.packed);
        }
    }

    private readonly UnsafePooledList<DeferredDrawData> deferredDraws = new(16384);
    private readonly UnsafePooledList<DeferredCurvyLaserDrawData> deferredCurvyLaserDraws = new(256);
    private readonly UnsafePooledList<DrawSortKey> sortKeys = new(16384);

    public void Update()
    {
        var deferredDraws = this.deferredDraws;
        var deferredCurvyLaserDraws = this.deferredCurvyLaserDraws;
        var sortKeys = this.sortKeys;

        deferredDraws.Clear();
        deferredCurvyLaserDraws.Clear();
        sortKeys.Clear();

        int maxLaserLength = 0;

        var q = world.GetOrCreateQuery(descriptor);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            bool hasSpawnEffect = archetype.Has<SpawnEffect>();
            bool hasCurvyLaser = archetype.Has<CurvyLaser>();
            bool hasLaserSourceRenderer = hasCurvyLaser && archetype.Has<LaserSourceRenderer>();

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                chunk.GetFilledComponentSpan<Transform, Renderer>(
                    out var transforms, out var renderers);

                var spawnAnims = hasSpawnEffect ?
                    chunk.GetFilledComponentSpan<SpawnEffect>() : default;
                var curvyLasers = hasCurvyLaser ?
                    chunk.GetFilledComponentSpan<CurvyLaser>() : default;
                var laserSources = hasLaserSourceRenderer ?
                    chunk.GetFilledComponentSpan<LaserSourceRenderer>() : default;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    ref var renderer = ref renderers.UnsafeAt(i);

                    if (!hasCurvyLaser)
                    {
                        ref var transform = ref transforms.UnsafeAt(i);

                        var dd = new DeferredDrawData
                        {
                            Texture = renderer.Texture,
                            SourceRect = renderer.SourceRect,
                            Position = transform.Position,
                            Anchor = renderer.Anchor,
                            Scale = renderer.Scale,
                            Color = renderer.Color,
                            Rotation = ResolveRotation(in renderer, transform.Rotation),
                            Layer = renderer.Layer,
                            BlendState = renderer.BlendState,
                        };

                        if (hasSpawnEffect)
                            ApplySpawnEffect(ref spawnAnims.UnsafeAt(i), in renderer, ref dd);

                        float halfW = dd.SourceRect.Width * MathF.Abs(dd.Scale.X) * 0.5f;
                        float halfH = dd.SourceRect.Height * MathF.Abs(dd.Scale.Y) * 0.5f;
                        float radius = (halfW > halfH ? halfW : halfH) * 1.415f;

                        float px = dd.Position.X;
                        float py = dd.Position.Y;

                        var b = this.bounds;

                        if (px + radius > b.Left && px - radius < b.Right &&
                            py + radius > b.Top && py - radius < b.Bottom)
                        {
                            int currentIndex = deferredDraws.Count;
                            deferredDraws.Add(dd);

                            sortKeys.Add(new DrawSortKey
                            {
                                SpawnId = renderer.SpawnId,
                                Layer = renderer.Layer,
                                Index = currentIndex,
                            });

                        }
                    }
                    else
                    {
                        ref var laser = ref curvyLasers.UnsafeAt(i);

                        maxLaserLength = Math.Max(maxLaserLength, laser.MaxNodes);

                        if (IsCurvyLaserVisible(laser.LaserNodes, laser.HalfWidth))
                        {
                            int currentIndex = deferredCurvyLaserDraws.Count;

                            SpriteAsset sourceSprite = null!;
                            Vector2 sourceScale = default;

                            if (hasLaserSourceRenderer && laser.IsSpawning)
                            {
                                ref var glow = ref laserSources.UnsafeAt(i);
                                sourceSprite = glow.Sprite;
                                sourceScale = glow.Scale;
                            }

                            deferredCurvyLaserDraws.Add(new DeferredCurvyLaserDrawData
                            {
                                Texture = renderer.Texture,
                                SourceRect = renderer.SourceRect,
                                TextureRotation = ResolveRotation(in renderer, transforms.UnsafeAt(i).Rotation),
                                LaserNodes = laser.LaserNodes,
                                HalfWidth = laser.HalfWidth,
                                Color = renderer.Color,
                                Layer = renderer.Layer,
                                BlendState = renderer.BlendState,
                                SourceSprite = sourceSprite,
                                SourceScale = sourceScale
                            });

                            sortKeys.Add(new DrawSortKey
                            {
                                SpawnId = renderer.SpawnId,
                                Layer = renderer.Layer,
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

                d.LaserNodes.AsSpans(out var first, out var second);

                if (second.Length == 0)
                {
                    batch.DrawStrip(
                        d.Texture, d.SourceRect, d.TextureRotation,
                        first, d.HalfWidth,
                        d.Color, d.Layer, d.BlendState);
                }
                else
                {
                    Span<Vector2> renderNodeSpan = laserNodeBuffer.Slice(0, nodeCount);
                    first.CopyTo(renderNodeSpan);
                    second.CopyTo(renderNodeSpan.Slice(first.Length));

                    batch.DrawStrip(
                        d.Texture, d.SourceRect, d.TextureRotation,
                        renderNodeSpan, d.HalfWidth,
                        d.Color, d.Layer, d.BlendState);
                }

                if (d.SourceSprite is not null)
                {
                    Vector2 headPosition = d.LaserNodes.PeekHead();

                    batch.Draw(
                        d.SourceSprite.Texture,
                        headPosition,
                        d.SourceSprite.SourceRect,
                        d.Color,
                        0f,
                        d.SourceSprite.Anchor,
                        d.SourceScale,
                        SpriteEffects.None,
                        d.Layer,
                        d.BlendState
                    );
                }
            }
        }

        if (DebugDrawColliders)
            DrawColliderDebugOverlay();
    }

    // ────────────────── Debug Collider Draw ──────────────────

    private void DrawColliderDebugOverlay()
    {
        var q = world.GetOrCreateQuery(colliderDescriptor);
        Span<Vector2> verts = stackalloc Vector2[DebugCircleSides];

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            bool hasCurvyLaser = archetype.Has<CurvyLaser>();

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                chunk.GetFilledComponentSpan<Transform, Collider>(
                    out var transforms, out var colliders);

                var curvyLasers = hasCurvyLaser ?
                    chunk.GetFilledComponentSpan<CurvyLaser>() : default;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    ref var collider = ref colliders.UnsafeAt(i);
                    if (!collider.IsActive)
                        continue;

                    ref var transform = ref transforms.UnsafeAt(i);

                    if (hasCurvyLaser)
                        DrawCurvyLaserColliderDebug(ref curvyLasers.UnsafeAt(i), verts);
                    else
                        DrawColliderDebug(ref collider, transform.Position, transform.Rotation, verts);
                }
            }
        }
    }

    private void DrawColliderDebug(ref Collider c, Vector2 pos, float rot, Span<Vector2> verts)
    {
        switch (c.ShapeType)
        {
            case ShapeType.Circle:
                BuildCirclePolygon(pos, c.Circle.Radius, verts);
                batch.DrawConvexPolygon(batch.WhitePixel, verts, default, DebugColor, DebugLayer, StgBlendState.Alpha);
                break;
            case ShapeType.ObbRect:
                Span<Vector2> quad = stackalloc Vector2[4];
                BuildObbPolygon(pos, rot, c.ObbRect.HalfSize, quad);
                batch.DrawConvexPolygon(batch.WhitePixel, quad, default, DebugColor, DebugLayer, StgBlendState.Alpha);
                break;
            case ShapeType.Ellipse:
                BuildEllipsePolygon(pos, rot, c.Ellipse.HalfSize, verts);
                batch.DrawConvexPolygon(batch.WhitePixel, verts, default, DebugColor, DebugLayer, StgBlendState.Alpha);
                break;
        }
    }

    private void DrawCurvyLaserColliderDebug(ref CurvyLaser laser, Span<Vector2> verts)
    {
        float radius = laser.HalfWidth;
        laser.LaserNodes.AsSpans(out var first, out var second);
        DrawNodeCircles(first, radius, verts);
        DrawNodeCircles(second, radius, verts);
    }

    private void DrawNodeCircles(Span<Vector2> nodes, float radius, Span<Vector2> verts)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            BuildCirclePolygon(nodes.UnsafeAt(i), radius, verts);
            batch.DrawConvexPolygon(batch.WhitePixel, verts, default, DebugColor, DebugLayer, StgBlendState.Alpha);
        }
    }

    private static void BuildCirclePolygon(Vector2 center, float radius, Span<Vector2> verts)
    {
        int n = verts.Length;
        float step = MathF.PI * 2f / n;
        for (int i = 0; i < n; i++)
        {
            float t = i * step;
            verts.UnsafeAt(i) = new Vector2(
                center.X + MathF.Cos(t) * radius,
                center.Y + MathF.Sin(t) * radius);
        }
    }

    private static void BuildEllipsePolygon(Vector2 center, float rotation, Vector2 halfSize, Span<Vector2> verts)
    {
        int n = verts.Length;
        float step = MathF.PI * 2f / n;
        float cos = MathF.Cos(rotation);
        float sin = MathF.Sin(rotation);

        for (int i = 0; i < n; i++)
        {
            float t = i * step;
            float lx = MathF.Cos(t) * halfSize.X;
            float ly = MathF.Sin(t) * halfSize.Y;
            verts.UnsafeAt(i) = new Vector2(
                center.X + lx * cos - ly * sin,
                center.Y + lx * sin + ly * cos);
        }
    }

    private static void BuildObbPolygon(Vector2 center, float rotation, Vector2 halfSize, Span<Vector2> verts)
    {
        float cos = MathF.Cos(rotation);
        float sin = MathF.Sin(rotation);
        float hx = halfSize.X;
        float hy = halfSize.Y;

        // counter-clockwise winding
        verts.UnsafeAt(0) = new Vector2(center.X + (-hx) * cos - (-hy) * sin, center.Y + (-hx) * sin + (-hy) * cos);
        verts.UnsafeAt(1) = new Vector2(center.X + ( hx) * cos - (-hy) * sin, center.Y + ( hx) * sin + (-hy) * cos);
        verts.UnsafeAt(2) = new Vector2(center.X + ( hx) * cos - ( hy) * sin, center.Y + ( hx) * sin + ( hy) * cos);
        verts.UnsafeAt(3) = new Vector2(center.X + (-hx) * cos - ( hy) * sin, center.Y + (-hx) * sin + ( hy) * cos);
    }

    // ────────────────── Visibility ──────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
#pragma warning disable RCS1242 // Do not pass non-read-only struct by read-only reference
    private static float ResolveRotation(in Renderer renderer, float transformRotation)
#pragma warning restore RCS1242 // Do not pass non-read-only struct by read-only reference
    {
        return renderer.RotationMode switch
        {
            RendererRotationMode.FixedWorld => renderer.Rotation,
            RendererRotationMode.FollowTransform => transformRotation + renderer.Rotation,
            _ => default,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ApplySpawnEffect(
#pragma warning disable RCS1242 // Do not pass non-read-only struct by read-only reference
        ref SpawnEffect effect, in Renderer renderer, ref DeferredDrawData dd)
#pragma warning restore RCS1242 // Do not pass non-read-only struct by read-only reference
    {
        if (effect.Counter >= effect.Duration) return;

        dd.Texture = effect.Sprite.Texture;
        dd.SourceRect = effect.Sprite.SourceRect;
        dd.Anchor = effect.Sprite.Anchor;

        float t = (float)(effect.Counter + 1) / effect.Duration;
        dd.Scale.X = MathHelper.Lerp(
            effect.StartScale.X, renderer.Scale.X, Easing.Evaluate(effect.TypeX, t));
        dd.Scale.Y = MathHelper.Lerp(
            effect.StartScale.Y, renderer.Scale.Y, Easing.Evaluate(effect.TypeY, t));
        dd.Color.A = (byte)MathHelper.Lerp(
            (float)effect.StartAlpha * 255f, renderer.Color.A, t);
    }

    public void Dispose()
    {
        deferredDraws.Clear();
        deferredCurvyLaserDraws.Clear();
        sortKeys.Clear();
    }
}
