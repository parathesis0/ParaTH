using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed unsafe class SuperBatch : IDisposable
{
    #region Private Consts
    private const int MaxVertices = 65536;
    private const int BucketCount = 256;
    private const int BucketInitialSize = 64;
    #endregion

    #region Private Containers
    private struct DrawCommand(int textureIndex, int indexStart, int indexCount)
    {
        public readonly int TextureIndex = textureIndex;
        public int IndexStart = indexStart;
        public readonly int IndexCount = indexCount;
    }

    private class CommandBucket
    {
        public DrawCommand[] Commands = Empty;
        public int Count = 0;

        private static readonly DrawCommand[] Empty = [];

        public void Add(DrawCommand command)
        {
            if (Count == Commands.Length)
                Array.Resize(ref Commands, Count == 0 ? BucketInitialSize : Count * 2);

            Commands[Count++] = command;
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
    private BlendState blendState = null!;
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
    public SuperBatch(GraphicsDevice graphicsDevice)
    {
        GraphicsDevice = graphicsDevice;

        rawVertices = new VertexPositionColorTexture[MaxVertices];
        rawIndices = new short[MaxVertices * 3];
        sortedIndices = new short[MaxVertices * 3];
        textureInfo = new Texture2D[MaxVertices / 3];

        buckets = new CommandBucket[BucketCount];
        for (int i = 0; i < BucketCount; i++)
            buckets[i] = new CommandBucket();

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
        using var stream = assembly.GetManifestResourceStream("ParaSpriteEffect") ??
                           throw new InvalidOperationException("ParaSpriteEffect not found.");
        byte[] effectCode = new byte[stream.Length];
        stream.ReadExactly(effectCode, 0, effectCode.Length);

        effect = new Effect(GraphicsDevice, effectCode);
        matrixParameter = effect.Parameters["MatrixTransform"];

        hasBegun = false;
    }
    #endregion

    #region Dispose Methods
    ~SuperBatch()
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
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            RasterizerState.CullCounterClockwise,
            null,
            transformMatrix
        );
    }

    public void Begin(
        BlendState blendState,
        SamplerState samplerState,
        RasterizerState rasterizerState,
        Effect? customEffect,
        Matrix transformMatrix)
    {
        if (hasBegun)
            throw new InvalidOperationException("Begin called before calling End.");
        hasBegun = true;

        this.blendState = blendState;
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
            throw new InvalidOperationException("End called before calling Begin.");
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
        byte layerDepth)
    {
        if (!hasBegun)
            throw new InvalidOperationException("Draw called before Begin.");

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

        buckets[layerDepth].Add(new DrawCommand(commandCount, startIndex, 6));
        textureInfo[commandCount++] = texture;
    }

    public void DrawConvexPolygon(
        Texture2D texture,
        Vector2[] vertices,
        Vector2[] textureCoords,
        Color color,
        byte layerDepth)
    {
        if (!hasBegun)
            throw new InvalidOperationException("Draw called before Begin.");

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
            currVertex->Position.X = vertices[i].X;
            currVertex->Position.Y = vertices[i].Y;
            currVertex->Position.Z = 0;
            currVertex->Color = color;
            currVertex->TextureCoordinate = textureCoords[i];
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

        buckets[layerDepth].Add(new DrawCommand(commandCount, startIndex, iCount));
        textureInfo[commandCount++] = texture;
    }
    #endregion

    #region Private Methods
    private void FlushBatch()
    {
        if (vertexCount == 0) return;

        int sortedIndexPtrOffset = 0;

        for (int i = 0; i < BucketCount; i++)
        {
            ref var bucket = ref buckets[i];

            if (bucket.Count == 0) continue;

            var cmds = bucket.Commands;
            int count = bucket.Count;

            for (int j = 0; j < count; j++)
            {
                ref var cmd = ref cmds[j];

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
            buckets[i].Count = 0;
    }

    private void PrepRenderState()
    {
        GraphicsDevice.BlendState = blendState;
        GraphicsDevice.SamplerStates[0] = samplerState;
        GraphicsDevice.RasterizerState = rasterizerState;

        matrixParameter.SetValue(transformMatrix);

        GraphicsDevice.SetVertexBuffer(vertexBuffer);
        GraphicsDevice.Indices = indexBuffer;

        effect.CurrentTechnique.Passes[0].Apply();
    }

    private void DrawAllBuckets()
    {
        for (int i = 0; i < BucketCount; i++)
        {
            ref var bucket = ref buckets[i];
            if (bucket.Count == 0) continue;

            ref var firstCmd = ref bucket.Commands[0];
            Texture2D currentTexture = textureInfo[firstCmd.TextureIndex];
            int batchStartIndex = firstCmd.IndexStart;
            int batchIndexCount = firstCmd.IndexCount;

            for (int j = 1; j < bucket.Count; j++)
            {
                ref var cmd = ref bucket.Commands[j];
                Texture2D cmdTexture = textureInfo[cmd.TextureIndex];

                if (cmdTexture == currentTexture)
                {
                    batchIndexCount += cmd.IndexCount;
                }
                else
                {
                    DrawPrimitives(currentTexture, batchStartIndex, batchIndexCount / 3);

                    currentTexture = cmdTexture;
                    batchStartIndex = cmd.IndexStart;
                    batchIndexCount = cmd.IndexCount;
                }
            }

            DrawPrimitives(currentTexture, batchStartIndex, batchIndexCount / 3);
        }
    }

    private void DrawPrimitives(Texture2D texture, int startIndex, int primitiveCount)
    {
        if (primitiveCount <= 0) return;

        if (customEffect is not null)
        {
            foreach (var pass in customEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.Textures[0] = texture;
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
            GraphicsDevice.Textures[0] = texture;
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
