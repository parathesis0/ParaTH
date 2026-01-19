using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ParaTH;

public sealed class Engine : Game
{
    private DynamicVertexBuffer vertexBuffer = null!;
    private IndexBuffer indexBuffer = null!;
    
    private VertexPositionColor[] vertexInfo = null!;
    private short[] indexData = null!;
    
    private BasicEffect effect = null!;
    
    public Engine()
    {
        var gdm = new GraphicsDeviceManager(this);
        gdm.PreferredBackBufferWidth = 640;
        gdm.PreferredBackBufferHeight = 480;
        gdm.SynchronizeWithVerticalRetrace = false;
        Content.RootDirectory = "Content";
        
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromTicks(166667);
        IsMouseVisible = true;
    }
    
    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        vertexBuffer = new DynamicVertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
        indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 3, BufferUsage.WriteOnly);

        const float Size = 100;
        
        vertexInfo = 
        [
            new VertexPositionColor(new Vector3(    0, -Size, 0), Color.Red),
            new VertexPositionColor(new Vector3( Size,  Size, 0), Color.Green),
            new VertexPositionColor(new Vector3(-Size,  Size, 0), Color.Blue)
        ];
        var center = new Vector3(640f / 2, 480f / 2, 0);
        foreach (ref var vpc in vertexInfo.AsSpan())
            vpc.Position += center;
        
        indexData = [ 0, 1, 2 ];
        
        vertexBuffer.SetData(vertexInfo);
        indexBuffer.SetData(indexData);

        effect = new BasicEffect(GraphicsDevice);
        effect.VertexColorEnabled = true;
        effect.Projection = Matrix.CreateOrthographicOffCenter(0, 640, 480, 0, -1, 0);
        
        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        vertexBuffer.Dispose();
        indexBuffer.Dispose();
        effect.Dispose();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        GraphicsDevice.BlendState = BlendState.AlphaBlend;
        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        GraphicsDevice.DepthStencilState = DepthStencilState.None;
        
        GraphicsDevice.SetVertexBuffer(vertexBuffer);
        GraphicsDevice.Indices = indexBuffer;

        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            
            GraphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                3,
                0,
                1
            );
        }
        base.Draw(gameTime);
    }
}
