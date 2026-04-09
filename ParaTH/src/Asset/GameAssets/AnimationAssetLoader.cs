using Microsoft.Xna.Framework;

namespace ParaTH;

using static AnimationAsset;

public sealed class AnimationAssetLoader(AssetManager assetManager) : IAssetLoader<AnimationAsset>
{
    private readonly HashSet<string> parsedFiles = [];

    private static void FlushIfNotNull(string? animName, PlayType? type, List<Frame>? frames,
        TextureAsset tex, AssetPool pool)
    {
        if (animName is null || type is null || frames is null || frames.Count == 0)
            return;

        var animation = new AnimationAsset(tex.Texture, type.Value, [.. frames]);

        pool.Add(animName, animation);
    }

    public Asset ParseAndLoad(string fullPath, string assetName, AssetPool pool)
    {
        if (parsedFiles.Contains(fullPath))
        {
            throw new KeyNotFoundException(
                $"Animation '{assetName}' not found in already-parsed file '{fullPath}'");
        }
        parsedFiles.Add(fullPath);

        var lines = File.ReadAllLines(fullPath);

        if (lines.Length == 0 || lines[0].Trim() != "@animations")
            throw new FormatException($"Invalid animation asset file: '{fullPath}'");

        TextureAsset? tex = null;
        string? animName = null;
        PlayType? type = null;
        List<Frame>? frames = null;

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            if (line.StartsWith("texture "))
            {
                var texPath = line["texture ".Length..].Trim();
                var texName = Path.GetFileNameWithoutExtension(texPath);
                tex = assetManager.Load<TextureAsset>(texPath, texName);

                continue;
            }

            if (tex is null)
            {
                throw new FormatException(
                    $"No texture declared before animation definitions in '{fullPath}'");
            }

            if (line.StartsWith("animation "))
            {
                FlushIfNotNull(animName, type, frames, tex, pool);

                // animation  name  looptype
                var p = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
                if (p.Length != 3)
                {
                    throw new FormatException(
                        $"Invalid animation definition at line {i + 1} in '{fullPath}': " +
                        $"expected 3 fields, got {p.Length}");
                }

                animName = p[1];
                type = Enum.Parse<PlayType>(p[2], ignoreCase: true);
                frames = [];
                continue;
            }

            if (frames is null)
            {
                throw new FormatException(
                    $"Frame data before 'animation' declaration in '{fullPath}'");
            }

            // x  y  w  h  ax  ay  dur
            var f = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
            if (f.Length != 7)
            {
                throw new FormatException(
                    $"Invalid frame definition at line {i + 1} in '{fullPath}': " +
                    $"expected 7 fields, got {f.Length}");
            }

            var x   = int.Parse(f[0]);
            var y   = int.Parse(f[1]);
            var w   = int.Parse(f[2]);
            var h   = int.Parse(f[3]);
            var ax  = int.Parse(f[4]);
            var ay  = int.Parse(f[5]);
            var dur = int.Parse(f[6]);

            var frame = new Frame(
                new Rectangle(x, y, w, h),
                new Vector2(ax, ay),
                dur
            );
            frames.Add(frame);
        }

        FlushIfNotNull(animName, type, frames, tex!, pool);

        return pool.Get<AnimationAsset>(assetName);
    }
}
