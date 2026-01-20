using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace ParaTH;

public sealed class Engine : Game
{
    // 渲染器
    private SpriteBatch spriteBatch = null!;
    private SuperBatch superBatch = null!;

    // 资源
    private Texture2D[] textures = null!; // 纹理数组
    private const int TextureCount = 16;  // 纹理数量
    private Matrix projection;

    // 测试状态
    private enum RenderMode { SpriteBatch, SuperBatch }
    private RenderMode currentMode = RenderMode.SpriteBatch;
    private int spriteCount = 5000;
    private float rotation = 0f;
    private Random rng = new Random();

    // 性能统计
    private Stopwatch stopwatch = new Stopwatch();
    private double lastDrawTimeMs = 0;
    private int frameCount = 0;
    private double fpsTimer = 0;
    private int currentFps = 0;

    // 预分配数组
    private Vector2[] tempVertices = new Vector2[4];
    private Vector2[] tempUVs = [Vector2.Zero, Vector2.UnitX, Vector2.One, Vector2.UnitY];

    public Engine()
    {
        var gdm = new GraphicsDeviceManager(this);
        gdm.PreferredBackBufferWidth = 1280;
        gdm.PreferredBackBufferHeight = 720;
        gdm.SynchronizeWithVerticalRetrace = false;
        gdm.GraphicsProfile = GraphicsProfile.HiDef; // 确保支持较新的特性
        IsFixedTimeStep = false;
        Content.RootDirectory = "Content";
    }

    protected override void LoadContent()
    {
        var effect = Content.Load<Effect>("ParaSpriteEffect");

        spriteBatch = new SpriteBatch(GraphicsDevice);
        superBatch = new SuperBatch(GraphicsDevice, effect);

        // 生成 16 张不同颜色的纹理
        textures = new Texture2D[TextureCount];
        for (int i = 0; i < TextureCount; i++)
        {
            textures[i] = new Texture2D(GraphicsDevice, 32, 32); // 32x32 大小
            Color[] data = new Color[32 * 32];

            // 生成随机亮色
            Color c = new Color(rng.Next(50, 255), rng.Next(50, 255), rng.Next(50, 255));
            Array.Fill(data, c);
            textures[i].SetData(data);
        }

        projection = Matrix.CreateOrthographicOffCenter(0, 1280, 720, 0, 0, 1);
    }

    protected override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        rotation += dt;

        var kState = Keyboard.GetState();
        if (kState.IsKeyDown(Keys.Escape)) Exit();

        if (kState.IsKeyDown(Keys.F1)) currentMode = RenderMode.SpriteBatch;
        if (kState.IsKeyDown(Keys.F2)) currentMode = RenderMode.SuperBatch;

        if (kState.IsKeyDown(Keys.Up) && spriteCount < 100000) spriteCount += 100;
        if (kState.IsKeyDown(Keys.Down) && spriteCount > 100) spriteCount -= 100;

        fpsTimer += dt;
        frameCount++;
        if (fpsTimer >= 1.0)
        {
            currentFps = frameCount;
            frameCount = 0;
            fpsTimer = 0;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(20, 20, 30));

        stopwatch.Restart();

        if (currentMode == RenderMode.SpriteBatch)
        {
            DrawWithSpriteBatch();
        }
        else
        {
            DrawWithSuperBatch();
        }

        stopwatch.Stop();
        lastDrawTimeMs = stopwatch.Elapsed.TotalMilliseconds;


        string infoText = $"{currentMode} | Count: {spriteCount} | Tex: {TextureCount} | FPS: {currentFps} | CPU: {lastDrawTimeMs:F2}ms";
        Window.Title = infoText;

        base.Draw(gameTime);
    }

    private void DrawWithSpriteBatch()
    {
        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

        var origin = new Vector2(16, 16); // 32x32 中心
        Vector2 scale = new Vector2(0.6f, 0.6f); // 缩放一点

        for (int i = 0; i < spriteCount; i++)
        {
            float x = (i % 100) * 12 + 40;
            float y = (i / 100f) * 12 + 40;

            // 随机层级
            byte layer = (byte)rng.Next(0, 256);

            // 纹理选择：故意打乱，让相邻的索引使用不同的纹理
            // 例如：0号用Tex0, 1号用Tex1...
            Texture2D tex = textures[i % TextureCount];

            spriteBatch.Draw(
                tex,
                new Vector2(x, y),
                null,
                Color.White,
                rotation + i * 0.01f,
                origin,
                scale,
                SpriteEffects.None,
                layer / 255f
            );
        }

        spriteBatch.End();
    }

    private void DrawWithSuperBatch()
    {
        superBatch.Begin(projection);

        float size = 10f; // 视觉大小的一半

        for (int i = 0; i < spriteCount; i++)
        {
            float cx = (i % 100) * 12 + 40;
            float cy = (i / 100f) * 12 + 40;

            // 保持和 SpriteBatch 一样的随机种子逻辑是不可能的，因为 rng.Next 在每帧都变
            // 但大概率分布是一样的
            byte layer = (byte)rng.Next(0, 256);

            // 同样的纹理选择逻辑
            Texture2D tex = textures[i % TextureCount];

            float rot = rotation + i * 0.01f;
            float cos = MathF.Cos(rot);
            float sin = MathF.Sin(rot);

            // 手动计算顶点 (保持你的逻辑)
            tempVertices[0].X = cx + (-size * cos - -size * sin);
            tempVertices[0].Y = cy + (-size * sin + -size * cos);
            tempVertices[1].X = cx + (size * cos - -size * sin);
            tempVertices[1].Y = cy + (size * sin + -size * cos);
            tempVertices[2].X = cx + (size * cos - size * sin);
            tempVertices[2].Y = cy + (size * sin + size * cos);
            tempVertices[3].X = cx + (-size * cos - size * sin);
            tempVertices[3].Y = cy + (-size * sin + size * cos);

            superBatch.DrawConvexPolygon(
                tex,
                tempVertices,
                tempUVs,
                Color.White,
                layer
            );
        }

        superBatch.End();
    }
}
