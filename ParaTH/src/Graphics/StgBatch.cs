using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ParaTH;

public sealed unsafe class StgBatch : IDisposable
{
    #region Private Consts
    private const int MaxVertices = 65536;
    private const int BucketCount = 256;
    private const int BucketInitialSize = 64;
    #endregion

    #region Private Containers
    private struct DrawCommand(int textureIndex, int indexStart, int indexCount, StgBlendState blendState)
    {
        public readonly int TextureIndex = textureIndex;
        public int IndexStart = indexStart;
        public readonly int IndexCount = indexCount;
        public readonly StgBlendState BlendState = blendState;
    }

    private class CommandBucket
    {
        public DrawCommand[] Commands = [];
        public int Count = 0;

        public void Add(DrawCommand command)
        {
            if (Count == Commands.Length)
                Array.Resize(ref Commands, Count == 0 ? BucketInitialSize : Count * 2);

            Commands.UnsafeAt(Count++) = command;
        }
    }
    #endregion

    #region Private Fields
    private readonly DynamicVertexBuffer vertexBuffer;
    private readonly DynamicIndexBuffer indexBuffer;

    private readonly VertexPositionColorTexture[] rawVertices;
    private readonly short[] rawIndices;
    private readonly short[] sortedIndices;
    private readonly Texture2D[] textureInfo;
    private readonly CommandBucket[] buckets;

    private GCHandle vHandle;
    private GCHandle iHandle;
    private GCHandle sHandle;

    private readonly VertexPositionColorTexture* vPtr;
    private readonly short* iPtr;
    private readonly short* sPtr;

    private int vertexCount;
    private int indexCount;
    private int commandCount;

    private bool hasBegun;
    private SamplerState samplerState = null!;
    private RasterizerState rasterizerState = null!;
    private Matrix transformMatrix;
    private Effect? customEffect;

    private readonly Effect effect;
    private readonly EffectParameter matrixParameter;
    #endregion

    #region Public Properties
    public GraphicsDevice GraphicsDevice { get; }
    public bool IsDisposed { get; private set; }
    #endregion

    #region Public Constructor
    public StgBatch(GraphicsDevice graphicsDevice)
    {
        GraphicsDevice = graphicsDevice;

        rawVertices = new VertexPositionColorTexture[MaxVertices];
        rawIndices = new short[MaxVertices * 3];
        sortedIndices = new short[MaxVertices * 3];
        textureInfo = new Texture2D[MaxVertices / 3];

        buckets = new CommandBucket[BucketCount];
        for (int i = 0; i < BucketCount; i++)
            buckets.UnsafeAt(i) = new CommandBucket();

        vertexBuffer = new DynamicVertexBuffer(
            GraphicsDevice,
            typeof(VertexPositionColorTexture),
            MaxVertices,
            BufferUsage.WriteOnly
        );
        indexBuffer = new DynamicIndexBuffer(
            GraphicsDevice,
            IndexElementSize.SixteenBits,
            MaxVertices * 3,
            BufferUsage.WriteOnly
        );

        vHandle = GCHandle.Alloc(rawVertices, GCHandleType.Pinned);
        iHandle = GCHandle.Alloc(rawIndices, GCHandleType.Pinned);
        sHandle = GCHandle.Alloc(sortedIndices, GCHandleType.Pinned);

        vPtr = (VertexPositionColorTexture*)vHandle.AddrOfPinnedObject();
        iPtr = (short*)iHandle.AddrOfPinnedObject();
        sPtr = (short*)sHandle.AddrOfPinnedObject();

        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("StgSpriteEffect");
        if (stream is null)
            HelperThrow("StgSpriteEffect not found.");
        byte[] effectCode = new byte[stream.Length];
        stream.ReadExactly(effectCode, 0, effectCode.Length);

        effect = new Effect(GraphicsDevice, effectCode);
        matrixParameter = effect.Parameters["MatrixTransform"];

        hasBegun = false;
    }
    #endregion

    #region Dispose Methods
    ~StgBatch()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (IsDisposed) return;

        if (vHandle.IsAllocated) vHandle.Free();
        if (iHandle.IsAllocated) iHandle.Free();
        if (sHandle.IsAllocated) sHandle.Free();

        if (disposing)
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            effect.Dispose();
        }

        IsDisposed = true;
    }
    #endregion

    #region Public Begin Methods
    public void Begin(Matrix transformMatrix)
    {
        Begin(
            SamplerState.PointClamp,
            RasterizerState.CullCounterClockwise,
            null,
            transformMatrix
        );
    }

    public void Begin(
        SamplerState samplerState,
        RasterizerState rasterizerState,
        Effect? customEffect,
        Matrix transformMatrix)
    {
        if (hasBegun)
            HelperThrow("Begin called before calling End.");
        hasBegun = true;

        this.samplerState = samplerState;
        this.rasterizerState = rasterizerState;

        this.customEffect = customEffect;
        this.transformMatrix = transformMatrix;
    }
    #endregion

    #region Public End Method
    public void End()
    {
        if (!hasBegun)
            HelperThrow("End called before calling Begin.");
        hasBegun = false;

        FlushBatch();

        customEffect = null;
    }
    #endregion

    #region Public Draw Methods
    // all this to save a branch and a 16 bytes assignment?
    public void Draw(
        Texture2D texture,
        Vector2 position,
        Rectangle? sourceRectangle,
        Color color,
        float rotation,
        Vector2 origin,
        Vector2 scale,
        SpriteEffects spriteEffects,
        byte layerDepth,
        StgBlendState blendState)
    {
        Rectangle source = sourceRectangle ??
                           new Rectangle(0, 0, texture.Width, texture.Height);

        Draw(texture, position, source, color, rotation, origin, scale, spriteEffects, layerDepth, blendState);
    }

    public void Draw(
        Texture2D texture,
        Vector2 position,
        Rectangle sourceRectangle,
        Color color,
        float rotation,
        Vector2 origin,
        Vector2 scale,
        SpriteEffects spriteEffects,
        byte layerDepth,
        StgBlendState blendState)
    {
        if (!hasBegun)
            HelperThrow("Draw called before Begin.");

        if (vertexCount + 4 > rawVertices.Length ||
            indexCount + 6 > rawIndices.Length)
        {
            FlushBatch();
        }

        float width = sourceRectangle.Width * scale.X;
        float height = sourceRectangle.Height * scale.Y;

        float ox = origin.X * scale.X;
        float oy = origin.Y * scale.Y;

        float tlX = -ox;
        float tlY = -oy;
        float trX = width - ox;
        float trY = -oy;
        float brX = width - ox;
        float brY = height - oy;
        float blX = -ox;
        float blY = height - oy;

        Vector2 tl, tr, br, bl;
        if (rotation != 0)
        {
            float cos = MathF.Cos(rotation);
            float sin = MathF.Sin(rotation);

            tl = new Vector2(
                position.X + tlX * cos - tlY * sin,
                position.Y + tlX * sin + tlY * cos
            );
            tr = new Vector2(
                position.X + trX * cos - trY * sin,
                position.Y + trX * sin + trY * cos
            );
            br = new Vector2(
                position.X + brX * cos - brY * sin,
                position.Y + brX * sin + brY * cos
            );
            bl = new Vector2(
                position.X + blX * cos - blY * sin,
                position.Y + blX * sin + blY * cos
            );
        }
        else
        {
            tl = new Vector2(position.X + tlX, position.Y + tlY);
            tr = new Vector2(position.X + trX, position.Y + trY);
            br = new Vector2(position.X + brX, position.Y + brY);
            bl = new Vector2(position.X + blX, position.Y + blY);
        }

        float invTexWidth = 1f / texture.Width;
        float invTexHeight = 1f / texture.Height;

        float u0 = sourceRectangle.X * invTexWidth;
        float v0 = sourceRectangle.Y * invTexHeight;
        float u1 = (sourceRectangle.X + sourceRectangle.Width) * invTexWidth;
        float v1 = (sourceRectangle.Y + sourceRectangle.Height) * invTexHeight;

        if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            (u0, u1) = (u1, u0);

        if (spriteEffects.HasFlag(SpriteEffects.FlipVertically))
            (v0, v1) = (v1, v0);

        int startVertex = vertexCount;
        int startIndex = indexCount;

        VertexPositionColorTexture* currVertex = vPtr + vertexCount;

        currVertex->Position.X = tl.X;
        currVertex->Position.Y = tl.Y;
        currVertex->Position.Z = 0;
        currVertex->Color = color;
        currVertex->TextureCoordinate.X = u0;
        currVertex->TextureCoordinate.Y = v0;
        currVertex++;

        currVertex->Position.X = tr.X;
        currVertex->Position.Y = tr.Y;
        currVertex->Position.Z = 0;
        currVertex->Color = color;
        currVertex->TextureCoordinate.X = u1;
        currVertex->TextureCoordinate.Y = v0;
        currVertex++;

        currVertex->Position.X = br.X;
        currVertex->Position.Y = br.Y;
        currVertex->Position.Z = 0;
        currVertex->Color = color;
        currVertex->TextureCoordinate.X = u1;
        currVertex->TextureCoordinate.Y = v1;
        currVertex++;

        currVertex->Position.X = bl.X;
        currVertex->Position.Y = bl.Y;
        currVertex->Position.Z = 0;
        currVertex->Color = color;
        currVertex->TextureCoordinate.X = u0;
        currVertex->TextureCoordinate.Y = v1;

        short* currIndex = iPtr + indexCount;
        *(currIndex++) = (short)(startVertex);
        *(currIndex++) = (short)(startVertex + 1);
        *(currIndex++) = (short)(startVertex + 2);
        *(currIndex++) = (short)(startVertex);
        *(currIndex++) = (short)(startVertex + 2);
        *(currIndex)   = (short)(startVertex + 3);

        vertexCount += 4;
        indexCount += 6;

        buckets.UnsafeAt(layerDepth).Add(new DrawCommand(commandCount, startIndex, 6, blendState));
        textureInfo.UnsafeAt(commandCount++) = texture;
    }

    public void DrawConvexPolygon(
        Texture2D texture,
        Vector2[] vertices,
        Vector2[] textureCoords,
        Color color,
        byte layerDepth,
        StgBlendState blendState)
    {
        if (!hasBegun)
            HelperThrow("Draw called before Begin.");

        if (vertices.Length < 3) return;

        int vCount = vertices.Length;
        int triCount = vCount - 2;
        int iCount = triCount * 3;

        if (vertexCount + vCount > MaxVertices ||
            indexCount + iCount > MaxVertices * 3)
        {
            FlushBatch();
        }

        int startVertex = vertexCount;
        int startIndex = indexCount;

        VertexPositionColorTexture* currVertex = vPtr + vertexCount;

        for (int i = 0; i < vCount; i++)
        {
            currVertex->Position.X = vertices.UnsafeAt(i).X;
            currVertex->Position.Y = vertices.UnsafeAt(i).Y;
            currVertex->Position.Z = 0;
            currVertex->Color = color;
            currVertex->TextureCoordinate = textureCoords.UnsafeAt(i);
            currVertex++;
        }

        short* currIndex = iPtr + indexCount;
        short baseV = (short)startVertex;

        for (int i = 0; i < triCount; i++)
        {
            *(currIndex++) = baseV;
            *(currIndex++) = (short)(baseV + i + 1);
            *(currIndex++) = (short)(baseV + i + 2);
        }

        vertexCount += vCount;
        indexCount += iCount;

        buckets.UnsafeAt(layerDepth).Add(new DrawCommand(commandCount, startIndex, iCount, blendState));
        textureInfo.UnsafeAt(commandCount++) = texture;
    }

    // this looks good enough, do not modify unless it lags really really bad
    [SkipLocalsInit]
    public void DrawCurvyLaser(
        Texture2D texture,
        Rectangle sourceRectangle,
        float textureRotation,
        ReadOnlySpan<Vector2> nodes,
        float halfThickness,
        Color color,
        byte layerDepth,
        StgBlendState blendState)
    {
        if (!hasBegun)
            HelperThrow("Draw called before Begin.");

        int rawCount = nodes.Length;
        if (rawCount < 2)
            return;

        // filter out overlapping nodes and calculate arc length
        const float kMinSegLen = 0.5f;

        Vector2* pts = stackalloc Vector2[rawCount];
        float* cumArc = stackalloc float[rawCount];

        pts[0] = nodes.UnsafeAt(0);
        cumArc[0] = 0f;
        int pointsCount = 1;
        float totalArc = 0f;

        for (int i = 1; i < rawCount; i++)
        {
            float d = Vector2.Distance(pts[pointsCount - 1], nodes.UnsafeAt(i));
            if (d >= kMinSegLen)
            {
                totalArc += d;
                pts[pointsCount] = nodes.UnsafeAt(i);
                cumArc[pointsCount] = totalArc;
                pointsCount++;
            }
        }

        // don't draw if laser gets crushed to one point
        if (pointsCount < 2 || totalArc < kMinSegLen)
            return;

        int vCount = pointsCount * 2;
        int iCount = (pointsCount - 1) * 6;

        if (vertexCount + vCount > MaxVertices ||
            indexCount + iCount > MaxVertices * 3)
        {
            FlushBatch();
        }

        // transform uv
        float invTexWidth = 1f / texture.Width;
        float invTexHeight = 1f / texture.Height;

        float u0 = sourceRectangle.X * invTexWidth;
        float v0 = sourceRectangle.Y * invTexHeight;
        float u1 = (sourceRectangle.X + sourceRectangle.Width) * invTexWidth;
        float v1 = (sourceRectangle.Y + sourceRectangle.Height) * invTexHeight;

        float cu = (u0 + u1) * 0.5f;
        float cv = (v0 + v1) * 0.5f;
        float halfU = (u1 - u0) * 0.5f;
        float halfV = (v1 - v0) * 0.5f;

        float cos = MathF.Cos(textureRotation);
        float sin = MathF.Sin(textureRotation);

        Vector2 uvTL = new(cu - halfU * cos + halfV * sin, cv - halfU * sin - halfV * cos);
        Vector2 uvBL = new(cu - halfU * cos - halfV * sin, cv - halfU * sin + halfV * cos);
        Vector2 uvTR = new(cu + halfU * cos + halfV * sin, cv + halfU * sin - halfV * cos);
        Vector2 uvBR = new(cu + halfU * cos - halfV * sin, cv + halfU * sin + halfV * cos);

        // generate vertices — consistent normal-based miter, no cross fix needed
        int startVertex = vertexCount;
        int startIndex = indexCount;

        VertexPositionColorTexture* currVertex = vPtr + vertexCount;
        float invTotalArc = 1f / totalArc;

        // miter limit: max expand = halfThickness / kMiterMinDot
        // 0.25 → max 4x, handles bends up to ~150°
        const float kMiterMinDot = 0.25f;

        // head node
        {
            Vector2 currentPos = pts[0];
            float expandX, expandY;

            // head: outgoing perpendicular
            Vector2 dir = pts[1] - pts[0];
            float len = dir.Length();
            if (len > 1e-6f) dir /= len; else dir = Vector2.UnitX;
            // left normal = (-dir.Y, dir.X)
            expandX = -dir.Y * halfThickness;
            expandY = dir.X * halfThickness;

            // uv
            float t = cumArc[0] * invTotalArc;
            Vector2 topUV = Vector2.Lerp(uvTL, uvTR, t);
            Vector2 bottomUV = Vector2.Lerp(uvBL, uvBR, t);

            // left vertex  (+ expand)
            currVertex->Position.X = currentPos.X + expandX;
            currVertex->Position.Y = currentPos.Y + expandY;
            currVertex->Position.Z = 0;
            currVertex->Color = color;
            currVertex->TextureCoordinate = topUV;
            currVertex++;

            // right vertex (- expand)
            currVertex->Position.X = currentPos.X - expandX;
            currVertex->Position.Y = currentPos.Y - expandY;
            currVertex->Position.Z = 0;
            currVertex->Color = color;
            currVertex->TextureCoordinate = bottomUV;
            currVertex++;
        }

        // middle nodes
        for (int i = 1; i < pointsCount - 1; i++)
        {
            Vector2 currentPos = pts[i];
            float expandX, expandY;

            // middle: miter join
            Vector2 dIn = pts[i] - pts[i - 1];  // incoming direction
            Vector2 dOut = pts[i + 1] - pts[i]; // outgoing direction
            float lenIn = dIn.Length();
            float lenOut = dOut.Length();
            if (lenIn > 1e-6f) dIn /= lenIn; else dIn = Vector2.UnitX;
            if (lenOut > 1e-6f) dOut /= lenOut; else dOut = Vector2.UnitX;

            Vector2 nIn = new(-dIn.Y, dIn.X);
            Vector2 nOut = new(-dOut.Y, dOut.X);

            Vector2 miter = nIn + nOut;
            float miterLen = miter.Length();

            if (miterLen < 1e-4f)
            {
                // u turn, fallback to in normal
                expandX = nIn.X * halfThickness;
                expandY = nIn.Y * halfThickness;
            }
            else
            {
                miter /= miterLen;                              // normalize
                float d = miter.X * nIn.X + miter.Y * nIn.Y;    // dot(miter, nIn)
                d = MathF.Max(d, kMiterMinDot);                 // miter limit clamp
                float scale = halfThickness / d;
                expandX = miter.X * scale;
                expandY = miter.Y * scale;
            }

            // uv
            float t = cumArc[i] * invTotalArc;
            Vector2 topUV = Vector2.Lerp(uvTL, uvTR, t);
            Vector2 bottomUV = Vector2.Lerp(uvBL, uvBR, t);

            // left vertex  (+ expand)
            currVertex->Position.X = currentPos.X + expandX;
            currVertex->Position.Y = currentPos.Y + expandY;
            currVertex->Position.Z = 0;
            currVertex->Color = color;
            currVertex->TextureCoordinate = topUV;
            currVertex++;

            // right vertex (- expand)
            currVertex->Position.X = currentPos.X - expandX;
            currVertex->Position.Y = currentPos.Y - expandY;
            currVertex->Position.Z = 0;
            currVertex->Color = color;
            currVertex->TextureCoordinate = bottomUV;
            currVertex++;
        }

        // tail node
        {
            int i = pointsCount - 1;
            Vector2 currentPos = pts[i];
            float expandX, expandY;

            // tail: incoming perpendicular
            Vector2 dir = pts[i] - pts[i - 1];
            float len = dir.Length();
            if (len > 1e-6f) dir /= len; else dir = Vector2.UnitX;
            expandX = -dir.Y * halfThickness;
            expandY = dir.X * halfThickness;

            // uv
            float t = cumArc[i] * invTotalArc;
            Vector2 topUV = Vector2.Lerp(uvTL, uvTR, t);
            Vector2 bottomUV = Vector2.Lerp(uvBL, uvBR, t);

            // left vertex  (+ expand)
            currVertex->Position.X = currentPos.X + expandX;
            currVertex->Position.Y = currentPos.Y + expandY;
            currVertex->Position.Z = 0;
            currVertex->Color = color;
            currVertex->TextureCoordinate = topUV;
            currVertex++;

            // right vertex (- expand)
            currVertex->Position.X = currentPos.X - expandX;
            currVertex->Position.Y = currentPos.Y - expandY;
            currVertex->Position.Z = 0;
            currVertex->Color = color;
            currVertex->TextureCoordinate = bottomUV;
            currVertex++;
        }

        // generate indices
        short* currIndex = iPtr + indexCount;
        short baseV = (short)startVertex;

        for (int i = 0; i < pointsCount - 1; i++)
        {
            short t0 = (short)(baseV + i * 2);
            short b0 = (short)(baseV + i * 2 + 1);
            short t1 = (short)(baseV + i * 2 + 2);
            short b1 = (short)(baseV + i * 2 + 3);

            *(currIndex++) = t0;
            *(currIndex++) = b0;
            *(currIndex++) = t1;

            *(currIndex++) = b0;
            *(currIndex++) = b1;
            *(currIndex++) = t1;
        }

        vertexCount += vCount;
        indexCount += iCount;

        buckets.UnsafeAt(layerDepth).Add(new DrawCommand(commandCount, startIndex, iCount, blendState));
        textureInfo.UnsafeAt(commandCount++) = texture;
    }
    #endregion

    #region Private Methods
    [MethodImpl(MethodImplOptions.NoInlining)]
    [DoesNotReturn]
    [StackTraceHidden]
    private static void HelperThrow(string? msg)
    {
        throw new InvalidOperationException(msg);
    }

    private void FlushBatch()
    {
        if (vertexCount == 0) return;

        int sortedIndexPtrOffset = 0;

        for (int i = 0; i < BucketCount; i++)
        {
            ref var bucket = ref buckets.UnsafeAt(i);

            if (bucket.Count == 0) continue;

            var cmds = bucket.Commands;
            int count = bucket.Count;

            for (int j = 0; j < count; j++)
            {
                ref var cmd = ref cmds.UnsafeAt(j);

                int bytesToCopy = cmd.IndexCount * sizeof(short);

                short* src = iPtr + cmd.IndexStart;
                short* dst = sPtr + sortedIndexPtrOffset;

                Buffer.MemoryCopy(src, dst, bytesToCopy, bytesToCopy);

                cmd.IndexStart = sortedIndexPtrOffset;
                sortedIndexPtrOffset += cmd.IndexCount;
            }
        }

        int totalIndexCount = sortedIndexPtrOffset;
        int vertexDataBytes = vertexCount * sizeof(VertexPositionColorTexture);
        vertexBuffer.SetDataPointerEXT(
            0,
            (IntPtr)vPtr,
            vertexDataBytes,
            SetDataOptions.Discard
        );
        int indexDataBytes = totalIndexCount * sizeof(short);
        indexBuffer.SetDataPointerEXT(
            0,
            (IntPtr)sPtr,
            indexDataBytes,
            SetDataOptions.Discard
        );

        PrepRenderState();
        DrawAllBuckets();

        vertexCount = 0;
        indexCount = 0;
        commandCount = 0;
        for (int i = 0; i < BucketCount; i++)
            buckets.UnsafeAt(i).Count = 0;
    }

    private void PrepRenderState()
    {
        GraphicsDevice.SamplerStates[0] = samplerState;
        GraphicsDevice.RasterizerState = rasterizerState;

        matrixParameter.SetValue(transformMatrix);

        GraphicsDevice.SetVertexBuffer(vertexBuffer);
        GraphicsDevice.Indices = indexBuffer;

        effect.CurrentTechnique.Passes[0].Apply();
    }

    private static BlendState GetBlendState(StgBlendState state)
    {
        return state switch
        {
            StgBlendState.Alpha => StgBlendStates.Alpha,
            StgBlendState.Additive => StgBlendStates.Additive,
            StgBlendState.Subtract => StgBlendStates.Subtract,
            StgBlendState.ReverseSubtract => StgBlendStates.ReverseSubtract,
            StgBlendState.Invert => StgBlendStates.Invert,
            _ => default! // unreachable
        };
    }

    private void DrawAllBuckets()
    {
        Texture2D? currentTexture = null;
        StgBlendState currentBlendState = default;
        int batchStartIndex = 0;
        int batchIndexCount = 0;

        for (int i = 0; i < BucketCount; i++)
        {
            ref var bucket = ref buckets.UnsafeAt(i);
            if (bucket.Count == 0) continue;

            for (int j = 0; j < bucket.Count; j++)
            {
                ref var cmd = ref bucket.Commands.UnsafeAt(j);
                Texture2D cmdTexture = textureInfo.UnsafeAt(cmd.TextureIndex);

                if (currentTexture is null)
                {
                    currentTexture = cmdTexture;
                    currentBlendState = cmd.BlendState;
                    batchStartIndex = cmd.IndexStart;
                    batchIndexCount = cmd.IndexCount;
                }
                else if (cmdTexture == currentTexture && cmd.BlendState == currentBlendState)
                {
                    batchIndexCount += cmd.IndexCount;
                }
                else
                {
                    DrawPrimitives(currentTexture, batchStartIndex, batchIndexCount / 3, currentBlendState);

                    currentTexture = cmdTexture;
                    currentBlendState = cmd.BlendState;
                    batchStartIndex = cmd.IndexStart;
                    batchIndexCount = cmd.IndexCount;
                }
            }
        }

        if (batchIndexCount > 0)
            DrawPrimitives(currentTexture!, batchStartIndex, batchIndexCount / 3, currentBlendState);
    }

    private void DrawPrimitives(Texture2D texture, int startIndex, int primitiveCount, StgBlendState blendState)
    {
        if (primitiveCount <= 0) return;

        GraphicsDevice.BlendState = GetBlendState(blendState);
        GraphicsDevice.Textures[0] = texture;

        if (customEffect is not null)
        {
            foreach (var pass in customEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    vertexCount,
                    startIndex,
                    primitiveCount
                );
            }
        }
        else
        {
            GraphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                vertexCount,
                startIndex,
                primitiveCount
            );
        }
    }
    #endregion
}
