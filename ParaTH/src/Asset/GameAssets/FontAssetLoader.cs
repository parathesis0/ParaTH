using FontStashSharp;

namespace ParaTH;

public sealed class FontAssetLoader : IAssetLoader<FontAsset>
{
    public Asset ParseAndLoad(string fullPath, string assetName, AssetPool pool)
    {
        var fontData = File.ReadAllBytes(fullPath);
        var fontSystem = new FontSystem();
        fontSystem.AddFont(fontData);

        var fontAsset = new FontAsset(fontSystem);
        pool.Add(assetName, fontAsset);
        return fontAsset;
    }
}
