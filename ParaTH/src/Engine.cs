using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace ParaTH;

public sealed class Engine : Game
{
    private SpriteBatch spriteBatch = null!;
    private SuperBatch superBatch = null!;

    private Texture2D[] textures = null!;
    private const int TextureCount = 16;
    private Matrix projection;

    private enum RenderMode { SpriteBatch, SuperBatch }
    private RenderMode currentMode = RenderMode.SpriteBatch;
    private int spriteCount = 5000;
    private float rotation = 0f;
    private Random rng = new Random();

    private Stopwatch stopwatch = new Stopwatch();
    private double lastDrawTimeMs = 0;
    private int frameCount = 0;
    private double fpsTimer = 0;
    private int currentFps = 0;

    public Engine()
    {
        var gdm = new GraphicsDeviceManager(this);
        gdm.PreferredBackBufferWidth = 1280;
        gdm.PreferredBackBufferHeight = 720;
        gdm.SynchronizeWithVerticalRetrace = false;
        gdm.GraphicsProfile = GraphicsProfile.HiDef;
        IsFixedTimeStep = false;
        Content.RootDirectory = "Content";
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        superBatch = new SuperBatch(GraphicsDevice);

        textures = new Texture2D[TextureCount];
        for (int i = 0; i < TextureCount; i++)
        {
            textures[i] = new Texture2D(GraphicsDevice, 32, 32);
            var data = new Color[32 * 32];

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

        var origin = new Vector2(16, 16);
        Vector2 scale = new Vector2(0.6f, 0.6f);

        for (int i = 0; i < spriteCount; i++)
        {
            float x = (i % 100) * 12 + 40;
            float y = (i / 100f) * 12 + 40;

            byte layer = (byte)rng.Next(0, 256);

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

        var origin = new Vector2(16, 16);
        Vector2 scale = new Vector2(0.6f, 0.6f);

        for (int i = 0; i < spriteCount; i++)
        {
            float x = (i % 100) * 12 + 40;
            float y = (i / 100f) * 12 + 40;

            byte layer = (byte)rng.Next(0, 256);

            Texture2D tex = textures[i % TextureCount];

            superBatch.Draw(
                tex,
                new Vector2(x, y),
                null,
                Color.White,
                rotation + i * 0.01f,
                origin,
                scale,
                SpriteEffects.None,
                layer
            );
        }

        superBatch.End();
    }
}
