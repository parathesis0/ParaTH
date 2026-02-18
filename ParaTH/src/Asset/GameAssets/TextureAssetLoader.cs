using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace ParaTH;

public sealed class TextureAssetLoader(GraphicsDevice graphicsDevice) : IAssetLoader<TextureAsset>
{
    private readonly GraphicsDevice graphicsDevice = graphicsDevice;

    public void ParseAndLoad(string fullPath, AssetPool pool)
    {
        var name = Path.GetFileNameWithoutExtension(fullPath);
        if (pool.Contains(name)) return;

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: '{fullPath}'");

        using var stream = File.OpenRead(fullPath);
        var tex = Texture2D.FromStream(graphicsDevice, stream);
        var texAsset = new TextureAsset(tex);

        Debug.Assert(texAsset is not null, "Texture is null!");

        pool.Add(name, texAsset);
    }
}
