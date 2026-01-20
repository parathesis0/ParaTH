using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed class SuperBatch : IDisposable
{
    #region Private Consts
    private const int MaxVertices = 65536;
    private const int BucketCount = 256;
    private const int BucketInitialSize = 64;
    #endregion

    #region Private Containers
    private struct DrawCommand(int textureIndex, int indexStart, int indexCount)
    {
        public int TextureIndex = textureIndex;
        public int IndexStart = indexStart;
        public int IndexCount = indexCount;
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

    private int vertexCount;
    private int indexCount;
    private int commandCount;

    private bool hasBegun;
    private BlendState blendState = null!;
    private SamplerState samplerState = null!;
    private RasterizerState rasterizerState = null!;
    private Matrix transformMatrix;
    private Effect effect;
    private EffectParameter matrixParameter;
    #endregion

    #region Public Properties
    public GraphicsDevice GraphicsDevice { get; private set; }
    public bool IsDisposed { get; private set; }
    #endregion

    #region Public Constructor
    public SuperBatch(GraphicsDevice graphicsDevice, Effect shaderEffect) // todo make this load fucking internally
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

        effect = shaderEffect.Clone();
        matrixParameter = effect.Parameters["MatrixTransform"];

        hasBegun = false;
    }
    #endregion

    #region Public Begin Methods
    public void Begin(Matrix transformMatrix)
    {
        Begin(
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            RasterizerState.CullCounterClockwise,
            transformMatrix
        );
    }

    public void Begin(
        BlendState blendState,
        SamplerState samplerState,
        RasterizerState rasterizerState,
        Matrix transformMatrix)
    {
        if (hasBegun)
            throw new InvalidOperationException("Begin called before calling End.");
        hasBegun = true;

        this.blendState = blendState;
        this.samplerState = samplerState;
        this.rasterizerState = rasterizerState;

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
    }
    #endregion

    #region Public Draw Methods
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

        if (vertexCount + vCount > rawVertices.Length ||
            indexCount + iCount > rawIndices.Length)
        {
            FlushBatch();
        }

        int startVertex = vertexCount;
        int startIndex = indexCount;

        // Write vertices
        for (int i = 0; i < vCount; i++)
        {
            rawVertices[vertexCount].Position = new Vector3(vertices[i], 0);
            rawVertices[vertexCount].Color = color;
            rawVertices[vertexCount].TextureCoordinate = textureCoords[i];
            vertexCount++;
        }

        // Write indices, fan triangulation
        for (int i = 0; i < triCount; i++)
        {
            rawIndices[indexCount++] = (short)(startVertex);
            rawIndices[indexCount++] = (short)(startVertex + i + 1);
            rawIndices[indexCount++] = (short)(startVertex + i + 2);
        }

        buckets[layerDepth].Add(new DrawCommand(commandCount, startIndex, iCount));
        textureInfo[commandCount++] = texture;
    }
    #endregion

    #region Private Methods
    private void FlushBatch()
    {
        if (vertexCount == 0) return;

        int sortedIndexPtr = 0;

        for (int i = 0; i < BucketCount; i++)
        {
            ref var bucket = ref buckets[i];

            for (int j = 0; j < bucket.Count; j++)
            {
                ref var cmd = ref bucket.Commands[j];
                Array.Copy(rawIndices, cmd.IndexStart, sortedIndices, sortedIndexPtr, cmd.IndexCount);
                cmd.IndexStart = sortedIndexPtr;
                sortedIndexPtr += cmd.IndexCount;
            }
        }

        int totalIndexCount = sortedIndexPtr;

        vertexBuffer.SetData(rawVertices, 0, vertexCount, SetDataOptions.Discard);
        indexBuffer.SetData(sortedIndices, 0, totalIndexCount, SetDataOptions.Discard);

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

        if (disposing)
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            effect.Dispose();
        }

        IsDisposed = true;
    }
    #endregion
}
