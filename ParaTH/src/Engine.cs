using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ParaTH;

public sealed class Engine : Game
{
    private SuperBatch superBatch = null!;
    private ParticleManager particleManager = null!;
    private Texture2D particleTexture = null!;
    private Matrix projection;
    private Random rng = new Random();

    private int frameCount;
    private double fpsTimer;
    private int currentFps;
    private KeyboardState prevKeyState;

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
    }

    protected override void LoadContent()
    {
        superBatch = new SuperBatch(GraphicsDevice);
        particleManager = new ParticleManager();

        // 创建圆形粒子纹理
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

        projection = Matrix.CreateOrthographicOffCenter(0, 1280, 720, 0, 0, 1);
    }

    protected override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var kState = Keyboard.GetState();

        if (kState.IsKeyDown(Keys.Escape)) Exit();

        // 按空格发射爆发粒子
        if (kState.IsKeyDown(Keys.Space))
            EmitBurst(640, 360, 50);

        // 鼠标位置持续发射
        var mState = Mouse.GetState();
        if (mState.LeftButton == ButtonState.Pressed)
            EmitStream(mState.X, mState.Y, 5);

        particleManager.Update();

        // FPS
        fpsTimer += dt;
        frameCount++;
        if (fpsTimer >= 1.0)
        {
            currentFps = frameCount;
            frameCount = 0;
            fpsTimer = 0;
        }

        prevKeyState = kState;
        base.Update(gameTime);
    }

    private void EmitBurst(float cx, float cy, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = (float)(rng.NextDouble() * MathHelper.TwoPi);
            float speed = (float)(rng.NextDouble() * 4 + 2);

            particleManager.Emit(
                x: cx,
                y: cy,
                vx: MathF.Cos(angle) * speed,
                vy: MathF.Sin(angle) * speed,
                ax: 0,
                ay: 0.08f,
                rot: angle,
                omega: (float)(rng.NextDouble() - 0.5) * 0.2f,
                sizeX0: 0.8f,
                sizeY0: 0.8f,
                sizeX1: 0.1f,
                sizeY1: 0.1f,
                opacity0: 1f,
                opacity1: 0f,
                texture: particleTexture,
                offsetX: 16,
                offsetY: 16,
                color: new Color(
                    rng.Next(200, 255),
                    rng.Next(100, 200),
                    rng.Next(50, 150)),
                layer: (byte)rng.Next(0, 255),
                duration: 90,
                sizeXCurve: 2,  // OutQuad
                sizeYCurve: 2,
                opacityCurve: 6 // OutCubic
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
                x: cx + (float)(rng.NextDouble() - 0.5) * 10,
                y: cy + (float)(rng.NextDouble() - 0.5) * 10,
                vx: MathF.Cos(angle) * speed,
                vy: MathF.Sin(angle) * speed - 1f,
                ax: 0,
                ay: -0.02f,
                rot: 0,
                omega: 0,
                sizeX0: 0.4f,
                sizeY0: 0.4f,
                sizeX1: 0.6f,
                sizeY1: 0.6f,
                opacity0: 0.8f,
                opacity1: 0f,
                texture: particleTexture,
                offsetX: 16,
                offsetY: 16,
                color: new Color(
                    rng.Next(100, 180),
                    rng.Next(150, 220),
                    rng.Next(200, 255)),
                layer: (byte)rng.Next(0, 255),
                duration: 60,
                sizeXCurve: 0,
                sizeYCurve: 0,
                opacityCurve: 2
            );
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(5, 5, 10));

        superBatch.Begin(BlendState.Additive, SamplerState.PointClamp, RasterizerState.CullCounterClockwise, null, projection);
        particleManager.Draw(superBatch);
        superBatch.End();

        Window.Title = $"Particles: {particleManager.ParticleCount} | FPS: {currentFps} | [Space]=Burst [LMB]=Stream";

        base.Draw(gameTime);
    }
}
