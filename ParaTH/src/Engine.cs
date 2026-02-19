using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

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
    private SoundAsset sound1 = null!;
    private SoundAsset sound2 = null!;
    private SongAsset song1 = null!;
    private SongAsset song2 = null!;

    private int frameCount;
    private bool isPaused;
    private bool stepRequested;
    private KeyboardState currKb;
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
        assetManager.RegisterLoader(new SoundAssetLoader());
        assetManager.RegisterLoader(new SongAssetLoader(assetManager));

        stgBatch = new StgBatch(GraphicsDevice);
        projection = Matrix.CreateOrthographicOffCenter(0, 1280, 720, 0, 0, 1);

        string path1; string path2;
        string name1; string name2;

        path1 = "bullet/bullet_sprites.txt";
        name1 = "heart_pink";
        path2 = "bullet/bullet_sprites.txt";
        name2 = "heart_black";
        sprite1 = assetManager.Load<SpriteAsset>(path1, name1);
        sprite2 = assetManager.Load<SpriteAsset>(path2, name2);

        path1 = "bullet/fire_animations.txt";
        name1 = "fireball_red";
        path2 = "bullet/fire_animations.txt";
        name2 = "fireball_purple";
        animation1 = assetManager.Load<AnimationAsset>(path1, name1);
        animation2 = assetManager.Load<AnimationAsset>(path2, name2);
        animator1 = new AnimationPlayer();
        animator2 = new AnimationPlayer();
        animator1.Play(animation1);
        animator2.Play(animation2);

        path1 = "sfx/pichuun.wav";
        name1 = "pichuun";
        path2 = "sfx/boom.wav";
        name2 = "boom";
        sound1 = assetManager.Load<SoundAsset>(path1, name1);
        sound2 = assetManager.Load<SoundAsset>(path2, name2);

        path1 = "bgm/songs.txt";
        name1 = "stage";
        path2 = "bgm/songs.txt";
        name2 = "boss";
        song1 = assetManager.Load<SongAsset>(path1, name1);
        song2 = assetManager.Load<SongAsset>(path2, name2);
    }

    private bool IsKeyPressed(Keys key) => prevKb.IsKeyUp(key) && currKb.IsKeyDown(key);

    protected override void Update(GameTime gameTime)
    {
        currKb = Keyboard.GetState();

        if (IsKeyPressed(Keys.P))
            isPaused = !isPaused;

        if (IsKeyPressed(Keys.K))
        {
            if (!isPaused) isPaused = true;
            stepRequested = true;
        }

        if (IsKeyPressed(Keys.D1))
            sound1.SoundEffect.Play();
        if (IsKeyPressed(Keys.D2))
            sound2.SoundEffect.Play();

        if (IsKeyPressed(Keys.D3))
            MediaPlayer.Play(song1.Song);
        if (IsKeyPressed(Keys.D4))
            MediaPlayer.Play(song2.Song);
        if (IsKeyPressed(Keys.D5))
            MediaPlayer.Stop();

        prevKb = currKb;

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
