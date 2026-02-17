using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using FontStashSharp;

namespace ParaTH;

[Obsolete("Trash written by Opus 4.6")]
public sealed class AssetSystem : IDisposable
{
    private readonly GraphicsDevice graphicsDevice;
    private readonly string assetRoot;
    private readonly AssetPool pool = new();

    private readonly Dictionary<string, TextureAsset> texturesByPath = [];
    private readonly Dictionary<string, FontSystem> fontSystems = [];
    private readonly Dictionary<(string, float), FontAsset> fontCache = [];
    private readonly HashSet<string> loadedFiles = [];

    public AssetSystem(GraphicsDevice graphicsDevice, string assetRoot)
    {
        this.graphicsDevice = graphicsDevice;
        this.assetRoot = Path.GetFullPath(assetRoot);
    }

    private string FullPath(string relativePath) => Path.Combine(assetRoot, relativePath);

    private static string Normalize(string path) => path.Replace('\\', '/');

    public void Load(string path)
    {
        string key = Normalize(path);
        if (!loadedFiles.Add(key))
            return;

        string ext = Path.GetExtension(path).ToLowerInvariant();
        switch (ext)
        {
            case ".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif" or ".tga" or ".dds":
                EnsureTexture(path);
                break;
            case ".wav":
                LoadSound(path);
                break;
            case ".ttf" or ".otf":
                LoadFontInternal(path);
                break;
            case ".txt":
                LoadDefinition(path);
                break;
            default:
                throw new NotSupportedException($"Unsupported asset format: {ext}");
        }
    }

    public T Get<T>(string name) where T : Asset => pool.Get<T>(name);

    public bool TryGet<T>(string name, out T? asset) where T : Asset => pool.TryGet(name, out asset);

    public FontAsset GetFont(string name, float size)
    {
        var cacheKey = (name, size);
        if (fontCache.TryGetValue(cacheKey, out var cached))
            return cached;

        if (!fontSystems.TryGetValue(name, out var fs))
            throw new KeyNotFoundException($"Font '{name}' has not been loaded");

        var asset = new FontAsset(fs.GetFont(size));
        fontCache[cacheKey] = asset;
        return asset;
    }

    private TextureAsset EnsureTexture(string relativePath)
    {
        string key = Normalize(relativePath);
        if (texturesByPath.TryGetValue(key, out var existing))
            return existing;

        using var stream = File.OpenRead(FullPath(relativePath));
        var tex = Texture2D.FromStream(graphicsDevice, stream);

        var asset = new TextureAsset(tex);
        texturesByPath[key] = asset;

        string name = Path.GetFileNameWithoutExtension(relativePath);
        if (!pool.Contains(name))
            pool.Add(name, asset);

        return asset;
    }

    private void LoadSound(string relativePath)
    {
        using var stream = File.OpenRead(FullPath(relativePath));
        var sfx = SoundEffect.FromStream(stream);
        string name = Path.GetFileNameWithoutExtension(relativePath);
        pool.Add(name, new SoundAsset(sfx));
    }

    private void LoadFontInternal(string relativePath)
    {
        string name = Path.GetFileNameWithoutExtension(relativePath);
        if (fontSystems.ContainsKey(name))
            return;

        var fs = new FontSystem();
        fs.AddFont(File.ReadAllBytes(FullPath(relativePath)));
        fontSystems[name] = fs;
    }

    private void LoadDefinition(string relativePath)
    {
        string fullPath = FullPath(relativePath);
        using var reader = new StreamReader(fullPath);

        string? header = null;
        while ((header = reader.ReadLine()) != null)
        {
            header = header.Trim();
            if (header.Length > 0 && header[0] != '#')
                break;
        }

        switch (header)
        {
            case "@sprites":
                ParseSprites(reader);
                break;
            case "@animations":
                ParseAnimations(reader);
                break;
            case "@songs":
                ParseSongs(reader);
                break;
            default:
                throw new FormatException($"Unknown definition header: {header}");
        }
    }

    private void ParseSprites(StreamReader reader)
    {
        TextureAsset? tex = null;

        while (reader.ReadLine() is { } raw)
        {
            var line = raw.Trim();
            if (line.Length == 0 || line[0] == '#') continue;

            if (line.StartsWith("texture "))
            {
                tex = EnsureTexture(line[8..].Trim());
                continue;
            }

            if (tex is null)
                throw new FormatException(
                    "Sprite definition: 'texture' must appear before sprite entries");

            // name  x  y  w  h  ax  ay
            var p = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
            if (p.Length < 7)
                throw new FormatException($"Invalid sprite entry: {line}");

            pool.Add(p[0], new SpriteAsset(
                tex.Texture,
                new Rectangle(int.Parse(p[1]), int.Parse(p[2]),
                              int.Parse(p[3]), int.Parse(p[4])),
                new Vector2(float.Parse(p[5]), float.Parse(p[6]))));
        }
    }

    private void ParseAnimations(StreamReader reader)
    {
        TextureAsset? tex = null;
        string? animName = null;
        AnimationAsset.PlayType playType = default;
        List<AnimationAsset.Frame>? frames = null;

        void Flush()
        {
            if (animName is null || tex is null || frames is null || frames.Count == 0)
                return;
            pool.Add(animName,
                new AnimationAsset(tex, playType, frames.ToArray()));
            animName = null;
            frames = null;
        }

        while (reader.ReadLine() is { } raw)
        {
            var line = raw.Trim();
            if (line.Length == 0 || line[0] == '#') continue;

            if (line.StartsWith("texture "))
            {
                tex = EnsureTexture(line[8..].Trim());
                continue;
            }

            if (line.StartsWith("animation "))
            {
                Flush();
                var p = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
                animName = p[1];
                playType = Enum.Parse<AnimationAsset.PlayType>(p[2], ignoreCase: true);
                frames = [];
                continue;
            }

            if (frames is null)
                throw new FormatException("Frame data before 'animation' declaration");

            // x  y  w  h  ax  ay  dur
            var f = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
            if (f.Length < 7)
                throw new FormatException($"Invalid frame entry: {line}");

            frames.Add(new AnimationAsset.Frame(
                new Rectangle(int.Parse(f[0]), int.Parse(f[1]),
                              int.Parse(f[2]), int.Parse(f[3])),
                new Vector2(float.Parse(f[4]), float.Parse(f[5])),
                uint.Parse(f[6])));
        }

        Flush();
    }

    private void ParseSongs(StreamReader reader)
    {
        while (reader.ReadLine() is { } raw)
        {
            var line = raw.Trim();
            if (line.Length == 0 || line[0] == '#') continue;

            // name  path  start  end
            var p = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
            if (p.Length < 4)
                throw new FormatException($"Invalid song entry: {line}");

            string name = p[0];
            string songPath = FullPath(p[1]);
            uint startMs = uint.Parse(p[2]);
            uint endMs = uint.Parse(p[3]);

            var song = Song.FromUri(name, new Uri(songPath));
            pool.Add(name, new SongAsset(song, new LoopRange(startMs, endMs)));
        }
    }

    public void Dispose()
    {
        foreach (var tex in texturesByPath.Values)
            tex.Texture.Dispose();
        foreach (var fs in fontSystems.Values)
            fs.Dispose();

        texturesByPath.Clear();
        fontSystems.Clear();
        fontCache.Clear();
        loadedFiles.Clear();
        pool.Clear();
    }
}
