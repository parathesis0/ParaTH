using FontStashSharp;
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
    private TestAnimationPlayer animator1 = null!;
    private TestAnimationPlayer animator2 = null!;
    private SoundAsset sound1 = null!;
    private SoundAsset sound2 = null!;
    private SongAsset song1 = null!;
    private SongAsset song2 = null!;

    private float rotation = 0f;

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
        assetManager.RegisterLoader(new FontAssetLoader());

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
        animator1 = new TestAnimationPlayer();
        animator2 = new TestAnimationPlayer();
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

            rotation += 0.1f;

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
            rotation,
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
            rotation,
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

        const string fontPath1 = "fonts/bumpitup_modified.ttf";
        const string fontName1 = "test_font";
        const string fontPath2 = "fonts/mspgothic.ttf";
        const string fontName2 = "touhou_font";

        var font1 = assetManager.Load<FontAsset>(fontPath1, fontName1).GetFont(24);
        var font2 = assetManager.Load<FontAsset>(fontPath2, fontName2).GetFont(24);

        stgBatch.DrawString(font2, "THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG",
            new Vector2(160, 320), Color.White, 128, StgBlendState.Alpha);

        var str = "！！！哈利路大旋風！！！";
        var anchor = font2.MeasureString(str) / 2;

        stgBatch.DrawString(font2, str,
            new Vector2(100, 100), Color.White, 128, StgBlendState.Alpha, rotation, anchor);
        stgBatch.DrawString(font2, "StgBatch DrawString Test!",
            new Vector2(160, 360), Color.Yellow, 128, StgBlendState.Alpha);

        stgBatch.End();

        base.Draw(gameTime);
    }
}
