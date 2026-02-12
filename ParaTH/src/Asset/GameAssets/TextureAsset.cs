using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed record TextureAsset : Asset
{
    public readonly Texture2D Texture;

    public TextureAsset(uint id, Texture2D texture) : base(id)
    {
        Texture = texture;
    }
}
