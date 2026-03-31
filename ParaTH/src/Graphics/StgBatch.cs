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
        if (!hasBegun)
            HelperThrow("Draw called before Begin.");

        if (vertexCount + 4 > rawVertices.Length ||
            indexCount + 6 > rawIndices.Length)
        {
            FlushBatch();
        }

        Rectangle source = sourceRectangle ??
                           new Rectangle(0, 0, texture.Width, texture.Height);

        float width = source.Width * scale.X;
        float height = source.Height * scale.Y;

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

        float u0 = source.X * invTexWidth;
        float v0 = source.Y * invTexHeight;
        float u1 = (source.X + source.Width) * invTexWidth;
        float v1 = (source.Y + source.Height) * invTexHeight;

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

    public void DrawCurvyLaser(
        Texture2D texture,
        Rectangle? sourceRectangle,
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
        Rectangle source = sourceRectangle ?? new Rectangle(0, 0, texture.Width, texture.Height);
        float invTexWidth = 1f / texture.Width;
        float invTexHeight = 1f / texture.Height;

        float u0 = source.X * invTexWidth;
        float v0 = source.Y * invTexHeight;
        float u1 = (source.X + source.Width) * invTexWidth;
        float v1 = (source.Y + source.Height) * invTexHeight;

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

        // generate vertices, use miter join for consistent width at bends
        int startVertex = vertexCount;
        int startIndex = indexCount;

        VertexPositionColorTexture* currVertex = vPtr + vertexCount;
        float invTotalArc = 1f / totalArc;

        // head node, perpendicular
        {
            Vector2 currentPos = pts[0];
            Vector2 segDir = pts[1] - pts[0];
            float len = segDir.Length();
            if (len > 1e-6f) segDir /= len;
            else segDir = Vector2.UnitX;
            float expandX = -segDir.Y * halfThickness;
            float expandY = segDir.X * halfThickness;

            // uv lerp with arc length
            float t = cumArc[0] * invTotalArc;
            Vector2 topUV = Vector2.Lerp(uvTL, uvTR, t);
            Vector2 bottomUV = Vector2.Lerp(uvBL, uvBR, t);

            currVertex->Position.X = currentPos.X + expandX;
            currVertex->Position.Y = currentPos.Y + expandY;
            currVertex->Position.Z = 0;
            currVertex->Color = color;
            currVertex->TextureCoordinate = topUV;
            currVertex++;

            currVertex->Position.X = currentPos.X - expandX;
            currVertex->Position.Y = currentPos.Y - expandY;
            currVertex->Position.Z = 0;
            currVertex->Color = color;
            currVertex->TextureCoordinate = bottomUV;
            currVertex++;
        }

        // middle nodes, bisector and miter join
        for (int i = 1; i < pointsCount - 1; i++)
        {
            Vector2 currentPos = pts[i];

            Vector2 toPrev = pts[i - 1] - currentPos;
            Vector2 toNext = pts[i + 1] - currentPos;

            float lenPrev = toPrev.Length();
            float lenNext = toNext.Length();

            Vector2 normPrev = lenPrev > 1e-6f ? toPrev / lenPrev : Vector2.UnitX;
            Vector2 normNext = lenNext > 1e-6f ? toNext / lenNext : Vector2.UnitX;

            Vector2 bisect = normPrev + normNext;
            float bisectLen = bisect.Length();

            float expandX, expandY;

            if (bisectLen < 0.00002f || bisectLen > 1.99998f)
            {
                // u bend, just use perpendicular
                expandX = normPrev.Y * halfThickness;
                expandY = -normPrev.X * halfThickness;
            }
            else
            {
                bisect /= bisectLen;

                float crossVal = bisect.X * normPrev.Y - bisect.Y * normPrev.X;
                float absCross = MathF.Abs(crossVal);

                if (absCross < 1e-6f)
                {
                    expandX = normPrev.Y * halfThickness;
                    expandY = -normPrev.X * halfThickness;
                }
                else
                {
                    float expandDelta = halfThickness / absCross;
                    expandX = bisect.X * expandDelta;
                    expandY = bisect.Y * expandDelta;
                }
            }

            // uv lerp with arc length
            float t = cumArc[i] * invTotalArc;
            Vector2 topUV = Vector2.Lerp(uvTL, uvTR, t);
            Vector2 bottomUV = Vector2.Lerp(uvBL, uvBR, t);

            currVertex->Position.X = currentPos.X + expandX;
            currVertex->Position.Y = currentPos.Y + expandY;
            currVertex->Position.Z = 0;
            currVertex->Color = color;
            currVertex->TextureCoordinate = topUV;
            currVertex++;

            currVertex->Position.X = currentPos.X - expandX;
            currVertex->Position.Y = currentPos.Y - expandY;
            currVertex->Position.Z = 0;
            currVertex->Color = color;
            currVertex->TextureCoordinate = bottomUV;
            currVertex++;
        }

        // tail node, perpendicular
        {
            int i = pointsCount - 1;
            Vector2 currentPos = pts[i];
            Vector2 segDir = pts[i] - pts[i - 1];
            float len = segDir.Length();
            if (len > 1e-6f) segDir /= len;
            else segDir = Vector2.UnitX;
            float expandX = -segDir.Y * halfThickness;
            float expandY = segDir.X * halfThickness;

            // uv lerp with arc length
            float t = cumArc[i] * invTotalArc;
            Vector2 topUV = Vector2.Lerp(uvTL, uvTR, t);
            Vector2 bottomUV = Vector2.Lerp(uvBL, uvBR, t);

            currVertex->Position.X = currentPos.X + expandX;
            currVertex->Position.Y = currentPos.Y + expandY;
            currVertex->Position.Z = 0;
            currVertex->Color = color;
            currVertex->TextureCoordinate = topUV;
            currVertex++;

            currVertex->Position.X = currentPos.X - expandX;
            currVertex->Position.Y = currentPos.Y - expandY;
            currVertex->Position.Z = 0;
            currVertex->Color = color;
            currVertex->TextureCoordinate = bottomUV;
            currVertex++;
        }

        // cross fix
        VertexPositionColorTexture* baseVert = vPtr + startVertex;
        for (int i = 0; i < pointsCount - 1; i++)
        {
            int tl = i * 2, bl = i * 2 + 1;
            int tr = i * 2 + 2, br = i * 2 + 3;

            float ex = baseVert[tr].Position.X - baseVert[tl].Position.X;
            float ey = baseVert[tr].Position.Y - baseVert[tl].Position.Y;
            float fx = baseVert[br].Position.X - baseVert[bl].Position.X;
            float fy = baseVert[br].Position.Y - baseVert[bl].Position.Y;
            float dot1 = ex * fx + ey * fy;

            float gx = baseVert[br].Position.X - baseVert[tl].Position.X;
            float gy = baseVert[br].Position.Y - baseVert[tl].Position.Y;
            float hx = baseVert[tr].Position.X - baseVert[bl].Position.X;
            float hy = baseVert[tr].Position.Y - baseVert[bl].Position.Y;
            float dot2 = gx * hx + gy * hy;

            if (dot2 > dot1)
            {
                // cross happens, swap nodes
                (baseVert[br].Position, baseVert[tr].Position) = (baseVert[tr].Position, baseVert[br].Position);
            }
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
