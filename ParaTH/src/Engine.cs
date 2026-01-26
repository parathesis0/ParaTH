using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ParaTH;

public sealed class Engine : Game
{
    private StgBatch stgBatch = null!;
    private ParticleManager particleManager = null!;
    private Texture2D particleTexture = null!;
    private Texture2D whiteTexture = null!;
    private Matrix projection;
    private readonly Random rng = new();

    private int frameCount;
    private double fpsTimer;
    private int currentFps;

    private float invertRadius = 100f;
    private const float MinRadius = 20f;
    private const float MaxRadius = 400f;
    private const int CircleSegments = 64;
    private readonly Vector2[] circleVertices = new Vector2[CircleSegments];
    private readonly Vector2[] circleTexCoords = new Vector2[CircleSegments];
    private int prevScrollValue;

    public Engine()
    {
        var gdm = new GraphicsDeviceManager(this);
        gdm.PreferredBackBufferWidth = 1280;
        gdm.PreferredBackBufferHeight = 720;
        gdm.SynchronizeWithVerticalRetrace = false;
        gdm.GraphicsProfile = GraphicsProfile.HiDef;

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromTicks(166667);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        stgBatch = new StgBatch(GraphicsDevice);
        particleManager = new ParticleManager();

        particleTexture = new Texture2D(GraphicsDevice, 32, 32);
        var colorData = new Color[32 * 32];
        Vector2 center = new Vector2(15.5f, 15.5f);

        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float alpha = MathHelper.Clamp(1f - (dist / 16f), 0f, 1f);
                colorData[y * 32 + x] = new Color(1f, 1f, 1f, alpha);
            }
        }
        particleTexture.SetData(colorData);

        whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
        whiteTexture.SetData([Color.White]);

        for (int i = 0; i < CircleSegments; i++)
        {
            circleTexCoords[i] = new Vector2(0.5f, 0.5f);
        }

        projection = Matrix.CreateOrthographicOffCenter(0, 1280, 720, 0, 0, 1);
        prevScrollValue = Mouse.GetState().ScrollWheelValue;
    }

    private void UpdateCircleVertices(float cx, float cy, float radius)
    {
        for (int i = 0; i < CircleSegments; i++)
        {
            float angle = i * MathHelper.TwoPi / CircleSegments;
            circleVertices[i] = new Vector2(
                cx + MathF.Cos(angle) * radius,
                cy + MathF.Sin(angle) * radius
            );
        }
    }

    protected override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var kState = Keyboard.GetState();
        var mState = Mouse.GetState();

        if (kState.IsKeyDown(Keys.Escape)) Exit();

        int scrollDelta = mState.ScrollWheelValue - prevScrollValue;
        if (scrollDelta != 0)
        {
            invertRadius += scrollDelta * 0.1f;
            invertRadius = MathHelper.Clamp(invertRadius, MinRadius, MaxRadius);
        }
        prevScrollValue = mState.ScrollWheelValue;

        if (kState.IsKeyDown(Keys.Space))
            EmitBurst(640, 360, 50);

        if (mState.LeftButton == ButtonState.Pressed)
            EmitStream(mState.X, mState.Y, 20);

        particleManager.Update();

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

    private void EmitBurst(float cx, float cy, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = (float)(rng.NextDouble() * MathHelper.TwoPi);
            float speed = (float)(rng.NextDouble() * 4 + 2);

            particleManager.Emit(
                pos: new Vector2(cx, cy),
                vel: new Vector2(speed * MathF.Cos(angle), speed * MathF.Sin(angle)),
                acc: new Vector2(0f, 0f),
                rot: angle,
                omega: (float)(rng.NextDouble() - 0.5) * 0.2f,
                sizeX0: 0.8f, sizeY0: 0.8f,
                sizeX1: 0.1f, sizeY1: 0.1f,
                opacity0: 1f, opacity1: 0f,
                texture: particleTexture,
                offset: new Vector2(16, 16),
                color: new Color(rng.Next(200, 255), rng.Next(100, 200), rng.Next(50, 150)),
                layer: (byte)rng.Next(0, 100),
                blend: StgBlendState.Additive,
                duration: 90,
                sizeXCurve: 2, sizeYCurve: 2, opacityCurve: 6
            );
        }
    }

    private void EmitStream(float cx, float cy, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = (float)(rng.NextDouble() * MathHelper.TwoPi);
            float speed = (float)(rng.NextDouble() * 2 + 0.5);

            particleManager.Emit(
                pos: new Vector2(cx, cy),
                vel: new Vector2(speed * MathF.Cos(angle), speed * MathF.Sin(angle)),
                acc: new Vector2(0, 0.2f),
                rot: 0, omega: 0,
                sizeX0: 0.4f, sizeY0: 0.4f,
                sizeX1: 0.6f, sizeY1: 0.6f,
                opacity0: 0.8f, opacity1: 0f,
                texture: particleTexture,
                offset: new Vector2(16, 16),
                color: new Color(rng.Next(100, 180), rng.Next(150, 220), rng.Next(200, 255)),
                layer: (byte)rng.Next(0, 100),
                blend: StgBlendState.Additive,
                duration: 60,
                sizeXCurve: 0, sizeYCurve: 0, opacityCurve: 2
            );
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(5, 5, 10));

        stgBatch.Begin(SamplerState.PointClamp, RasterizerState.CullCounterClockwise, null, projection);
        particleManager.Draw(stgBatch);

        var mState = Mouse.GetState();
        UpdateCircleVertices(mState.X, mState.Y, invertRadius);

        stgBatch.DrawConvexPolygon(whiteTexture, circleVertices, circleTexCoords, Color.White, 255, StgBlendState.Invert);
        stgBatch.End();

        Window.Title = $"Particles: {particleManager.ParticleCount} | FPS: {currentFps} | " +
                       $"Radius: {invertRadius:F0} | [Space]=Burst [LMB]=Stream [Scroll]=Size";

        base.Draw(gameTime);
    }
}
