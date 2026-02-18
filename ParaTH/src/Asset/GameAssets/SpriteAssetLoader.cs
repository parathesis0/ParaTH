using Microsoft.Xna.Framework;

namespace ParaTH;

public sealed class SpriteAssetLoader(AssetManager assetManager) : IAssetLoader<SpriteAsset>
{
    public void ParseAndLoad(string fullPath, AssetPool pool)
    {
        var lines = File.ReadAllLines(fullPath);

        if (lines.Length == 0 || lines[0].Trim() != "@sprites")
            throw new FormatException($"Invalid sprite asset file: '{fullPath}'");

        TextureAsset? tex = null;

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            if (line.StartsWith("texture"))
            {
                var texturePath = line["texture ".Length..].Trim();

                var texName = Path.GetFileNameWithoutExtension(texturePath);

                if (!assetManager.TryGet(texName, out tex))
                {
                    assetManager.Load<TextureAsset>(texturePath);
                    tex = assetManager.Get<TextureAsset>(texName);
                }

                continue;
            }

            if (tex is null)
            {
                throw new FormatException(
                    $"No texture declared before sprite definitions in '{fullPath}'");
            }

            var p = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);

            if (p.Length != 7)
            {
                throw new FormatException(
                    $"Invalid sprite definition at line {i + 1} in '{fullPath}': " +
                    $"expected 7 fields, got {p.Length}");
            }

            var name = p[0];
            var x    = int.Parse(p[1]);
            var y    = int.Parse(p[2]);
            var w    = int.Parse(p[3]);
            var h    = int.Parse(p[4]);
            var ax   = int.Parse(p[5]);
            var ay   = int.Parse(p[6]);

            var sprite = new SpriteAsset(
                tex.Texture,
                new Rectangle(x, y, w, h),
                new Vector2(ax, ay)
            );

            pool.Add(name, sprite);
        }
    }
}
