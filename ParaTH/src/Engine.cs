using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace ParaTH;

public sealed class Engine : Game
{
    private SpriteBatch spriteBatch = null!;
    private SuperBatch superBatch = null!;

    // "Old" way: Array of separate textures
    private Texture2D[] textures = null!;

    // "New" way: Single texture with regions
    private TextureAtlas atlas = null!;
    private TextureRegion[] atlasRegions = null!; // Cached array for fast access during Draw

    private const int TextureCount = 16;
    private Matrix projection;

    private enum RenderMode
    {
        SpriteBatch_MultiTex,
        SpriteBatch_Atlas,
        SuperBatch_MultiTex,
        SuperBatch_Atlas
    }

    private RenderMode currentMode = RenderMode.SpriteBatch_MultiTex;
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
        gdm.SynchronizeWithVerticalRetrace = false; // Unlock FPS for testing
        gdm.GraphicsProfile = GraphicsProfile.HiDef;
        IsFixedTimeStep = false;
        Content.RootDirectory = "Content";
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        superBatch = new SuperBatch(GraphicsDevice);

        // 1. Setup Individual Textures (Bad for batching)
        textures = new Texture2D[TextureCount];

        // 2. Setup Atlas Texture (128x128 fits 16 32x32 textures) (Good for batching)
        int atlasDimension = 128;
        Texture2D atlasTexture = new Texture2D(GraphicsDevice, atlasDimension, atlasDimension);
        Color[] atlasData = new Color[atlasDimension * atlasDimension];
        atlas = new TextureAtlas(atlasTexture);
        atlasRegions = new TextureRegion[TextureCount];

        int currentX = 0;
        int currentY = 0;

        for (int i = 0; i < TextureCount; i++)
        {
            // --- Create Individual Texture ---
            textures[i] = new Texture2D(GraphicsDevice, 32, 32);
            var colorData = new Color[32 * 32];
            Color c = new Color(rng.Next(50, 255), rng.Next(50, 255), rng.Next(50, 255));
            Array.Fill(colorData, c);
            textures[i].SetData(colorData);

            // --- Paint into Atlas ---
            // We manually copy the color data into the specific rect of the master atlas array
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    int index = (currentY + y) * atlasDimension + (currentX + x);
                    atlasData[index] = c;
                }
            }

            // Define the region
            string name = $"tex_{i}";
            atlas.CreateRegion(name, currentX, currentY, 32, 32, 16, 16); // 16,16 is origin offset
            atlasRegions[i] = atlas.GetRegion(name);

            // Move to next slot in 4x4 grid
            currentX += 32;
            if (currentX >= atlasDimension)
            {
                currentX = 0;
                currentY += 32;
            }
        }

        // Upload the full atlas data to GPU
        atlasTexture.SetData(atlasData);

        projection = Matrix.CreateOrthographicOffCenter(0, 1280, 720, 0, 0, 1);
    }

    protected override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        rotation += dt;

        var kState = Keyboard.GetState();
        if (kState.IsKeyDown(Keys.Escape)) Exit();

        // Switch Modes
        if (kState.IsKeyDown(Keys.F1)) currentMode = RenderMode.SpriteBatch_MultiTex;
        if (kState.IsKeyDown(Keys.F2)) currentMode = RenderMode.SpriteBatch_Atlas;
        if (kState.IsKeyDown(Keys.F3)) currentMode = RenderMode.SuperBatch_MultiTex;
        if (kState.IsKeyDown(Keys.F4)) currentMode = RenderMode.SuperBatch_Atlas;

        if (kState.IsKeyDown(Keys.Up) && spriteCount < 1000000) spriteCount += 1000;
        if (kState.IsKeyDown(Keys.Down) && spriteCount > 100) spriteCount -= 1000;

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

        switch (currentMode)
        {
            case RenderMode.SpriteBatch_MultiTex:
                DrawSB_MultiTex();
                break;
            case RenderMode.SpriteBatch_Atlas:
                DrawSB_Atlas();
                break;
            case RenderMode.SuperBatch_MultiTex:
                DrawSuper_MultiTex();
                break;
            case RenderMode.SuperBatch_Atlas:
                DrawSuper_Atlas();
                break;
        }

        stopwatch.Stop();
        lastDrawTimeMs = stopwatch.Elapsed.TotalMilliseconds;

        string modeName = currentMode.ToString().Replace("SpriteBatch_", "SB: ").Replace("SuperBatch_", "Super: ");
        string infoText = $"{modeName} | Sprites: {spriteCount} | FPS: {currentFps} | Draw Time: {lastDrawTimeMs:F2}ms";
        Window.Title = infoText;

        base.Draw(gameTime);
    }

    // --- SPRITE BATCH TESTS ---

    private void DrawSB_MultiTex()
    {
        // SpriteSortMode.BackToFront checks LayerDepth.
        // With multiple textures, this causes constant Texture Swapping (high draw calls).
        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

        var origin = new Vector2(16, 16);
        Vector2 scale = new Vector2(0.6f, 0.6f);

        for (int i = 0; i < spriteCount; i++)
        {
            CalcSprite(i, out Vector2 pos, out float rot, out byte layer);

            // Switching texture object frequently
            Texture2D tex = textures[i % TextureCount];

            spriteBatch.Draw(tex, pos, null, Color.White, rot, origin, scale, SpriteEffects.None, layer / 255f);
        }

        spriteBatch.End();
    }

    private void DrawSB_Atlas()
    {
        // Even with BackToFront sorting, because we only use ONE texture object (atlas.Texture),
        // SpriteBatch can batch everything into a single Draw call (or very few).
        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

        Vector2 scale = new Vector2(0.6f, 0.6f);

        for (int i = 0; i < spriteCount; i++)
        {
            CalcSprite(i, out Vector2 pos, out float rot, out byte layer);

            // Use region from array
            var region = atlasRegions[i % TextureCount];

            spriteBatch.Draw(
                atlas.Texture,
                pos,
                region.Bounds, // Source Rectangle
                Color.White,
                rot,
                region.Offset, // Origin from Atlas logic
                scale,
                SpriteEffects.None,
                layer / 255f
            );
        }

        spriteBatch.End();
    }

    // --- SUPER BATCH TESTS ---

    private void DrawSuper_MultiTex()
    {
        superBatch.Begin(BlendState.Additive, SamplerState.PointClamp, RasterizerState.CullCounterClockwise, null, projection);
        var origin = new Vector2(16, 16);
        Vector2 scale = new Vector2(0.6f, 0.6f);

        for (int i = 0; i < spriteCount; i++)
        {
            CalcSprite(i, out Vector2 pos, out float rot, out byte layer);
            Texture2D tex = textures[i % TextureCount];

            superBatch.Draw(tex, pos, null, Color.White, rot, origin, scale, SpriteEffects.None, layer);
        }
        superBatch.End();
    }

    private void DrawSuper_Atlas()
    {
        superBatch.Begin(BlendState.Additive, SamplerState.PointClamp, RasterizerState.CullCounterClockwise, null, projection);
        Vector2 scale = new Vector2(0.6f, 0.6f);

        for (int i = 0; i < spriteCount; i++)
        {
            CalcSprite(i, out Vector2 pos, out float rot, out byte layer);
            var region = atlasRegions[i % TextureCount];

            // Assuming SuperBatch has a Draw overload accepting sourceRectangle
            superBatch.Draw(
                atlas.Texture,
                pos,
                region.Bounds,
                Color.White,
                rot,
                region.Offset,
                scale,
                SpriteEffects.None,
                layer
            );
        }
        superBatch.End();
    }

    // Helper to keep math identical across tests
    private void CalcSprite(int i, out Vector2 pos, out float rot, out byte layer)
    {
        float x = (i % 100) * 12 + 40;
        float y = (i / 100f) * 12 + 40;

        // Pseudo-random layer based on index to ensure deterministic layers per frame
        // (using Random here inside Draw causes jitter if not re-seeded,
        // so we use a simple hash of 'i' for stability or just 'i' logic)
        layer = (byte)((i * 13) % 255);

        pos = new Vector2(x, y);
        rot = rotation + i * 0.01f;
    }
}
