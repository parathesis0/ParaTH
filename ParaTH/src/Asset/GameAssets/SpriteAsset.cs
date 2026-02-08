using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed class SpriteAsset(uint id, Texture2D texture, Rectangle sourceRect, Vector2 anchor)
    : Asset(id)
{
    public readonly Texture2D Texture = texture;
    public readonly Rectangle SourceRect = sourceRect;
    public readonly Vector2 Anchor = anchor;
}
