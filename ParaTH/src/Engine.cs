using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ParaTH;

public sealed class Engine : Game
{
    private AssetManager assetManager = null!;
    private StgBatch stgBatch = null!;
    private Matrix projection;
    private SpriteAsset sprite1 = null!;
    private SpriteAsset sprite2 = null!;
    private AnimationAsset animation1 = null!;
    private AnimationAsset animation2 = null!;
    private AnimationPlayer animator1 = null!;
    private AnimationPlayer animator2 = null!;

    private int frameCount;
    private bool isPaused;
    private bool stepRequested;
    private KeyboardState prevKb;

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
        assetManager = new AssetManager("Asset");

        assetManager.RegisterLoader(new TextureAssetLoader(GraphicsDevice));
        assetManager.RegisterLoader(new SpriteAssetLoader(assetManager));
        assetManager.RegisterLoader(new AnimationAssetLoader(assetManager));

        stgBatch = new StgBatch(GraphicsDevice);
        projection = Matrix.CreateOrthographicOffCenter(0, 1280, 720, 0, 0, 1);

        string path;
        string name1;
        string name2;

        path = "bullet/bullet_sprites.txt";
        name1 = "heart_pink";
        name2 = "heart_black";
        sprite1 = assetManager.Load<SpriteAsset>(path, name1);
        sprite2 = assetManager.Load<SpriteAsset>(path, name2);

        path = "bullet/fire_animations.txt";
        name1 = "fireball_red";
        name2 = "fireball_red";
        animation1 = assetManager.Load<AnimationAsset>(path, name1);
        animation2 = assetManager.Load<AnimationAsset>(path, name2);

        animator1 = new AnimationPlayer();
        animator2 = new AnimationPlayer();

        animator1.Play(animation1);
        animator2.Play(animation2);
    }


    protected override void Update(GameTime gameTime)
    {
        var kb = Keyboard.GetState();

        if (prevKb.IsKeyUp(Keys.P) && kb.IsKeyDown(Keys.P))
            isPaused = !isPaused;

        if (prevKb.IsKeyUp(Keys.K) && kb.IsKeyDown(Keys.K))
        {
            if (!isPaused) isPaused = true;
            stepRequested = true;
        }

        prevKb = kb;

        if (!isPaused || stepRequested)
        {
            stepRequested = false;
            frameCount++;

            animator1.Update();
            animator2.Update();
        }

        Window.Title = isPaused
            ? $"Frame {frameCount}  [PAUSED — K: step, P: resume]"
            : $"Frame {frameCount}";

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        stgBatch.Begin(SamplerState.PointClamp, RasterizerState.CullCounterClockwise, null, projection);

        stgBatch.Draw(
            sprite1.Texture,
            new Vector2(32, 32),
            sprite1.SourceRect,
            Color.White,
            0,
            sprite1.Anchor,
            Vector2.One * 2,
            SpriteEffects.None,
            0,
            StgBlendState.Alpha
        );

        stgBatch.Draw(
            sprite2.Texture,
            new Vector2(96, 32),
            sprite2.SourceRect,
            Color.White,
            0,
            sprite2.Anchor,
            Vector2.One * 2,
            SpriteEffects.None,
            0,
            StgBlendState.Alpha
        );

        stgBatch.Draw(
            animator1.CurrentAsset!.Texture,
            new Vector2(160, 32),
            animator1.CurrentFrame.SourceRect,
            Color.White,
            0,
            animator1.CurrentFrame.Anchor,
            Vector2.One * 2,
            SpriteEffects.None,
            0,
            StgBlendState.Alpha
        );

        stgBatch.Draw(
            animator2.CurrentAsset!.Texture,
            new Vector2(224, 32),
            animator2.CurrentFrame.SourceRect,
            Color.White,
            0,
            animator2.CurrentFrame.Anchor,
            Vector2.One * 2,
            SpriteEffects.None,
            0,
            StgBlendState.Additive
        );

        stgBatch.End();

        base.Draw(gameTime);
    }
}
