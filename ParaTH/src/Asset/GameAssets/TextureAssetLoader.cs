using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed class TextureAssetLoader(GraphicsDevice graphicsDevice) : IAssetLoader<TextureAsset>
{
    private readonly GraphicsDevice graphicsDevice = graphicsDevice;

    public Asset ParseAndLoad(string fullPath, string assetName, AssetPool pool)
    {
        using var stream = File.OpenRead(fullPath);
        var tex = Texture2D.FromStream(graphicsDevice, stream);

        // premultiply
        var data = new Color[tex.Width * tex.Height];
        tex.GetData(data);
        for (int i = 0; i < data.Length; i++)
        {
            byte a = data[i].A;
            data[i].R = (byte)(data[i].R * a / 255);
            data[i].G = (byte)(data[i].G * a / 255);
            data[i].B = (byte)(data[i].B * a / 255);
        }
        tex.SetData(data);

        var texAsset = new TextureAsset(tex);

        pool.Add(assetName, texAsset);
        return texAsset;
    }
}
