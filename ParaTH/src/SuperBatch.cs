using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed class SuperBatch : IDisposable 
{
    public GraphicsDevice GraphicsDevice { get; private set; }
    public bool IsDisposed { get; private set; }
    
    private DynamicVertexBuffer vertexBuffer = null!;
    private IndexBuffer indexBuffer = null!;
    
    private VertexPositionColor[] vertexInfo = null!;
    private short[] indexData = null!;
    
    private BasicEffect effect = null!;

    public SuperBatch(GraphicsDevice graphicsDevice)
    {
        GraphicsDevice = graphicsDevice;
    }
    
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
            effect.Dispose();
        }

        IsDisposed = true;
    }
}
