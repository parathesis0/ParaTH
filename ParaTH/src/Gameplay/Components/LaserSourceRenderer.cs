using Microsoft.Xna.Framework;

namespace ParaTH;

// 16 bytes
public struct LaserSourceRenderer(SpriteAsset sprite, Vector2 scale)
{
    public SpriteAsset Sprite = sprite; // 8
    public Vector2 Scale = scale;       // 8
}
