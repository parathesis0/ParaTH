using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed class TextureAsset(uint id, Texture2D texture) : Asset(id)
{
    public readonly Texture2D Texture = texture;
}
