using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed record SpriteAsset : Asset
{
    public readonly Texture2D Texture;
    public readonly Rectangle SourceRect;
    public readonly Vector2 Anchor;

    public SpriteAsset(uint id, Texture2D texture, Rectangle sourceRect, Vector2 anchor) : base(id)
    {
        Texture = texture;
        SourceRect = sourceRect;
        Anchor = anchor;
    }
}
