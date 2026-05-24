using Microsoft.Xna.Framework;

namespace ParaTH;

// 24 bytes
public struct LaserSourceRenderer(SpriteAsset sprite, Vector2 scale)
{
    public SpriteAsset Sprite = sprite;     // 8
    public Vector2 Scale = scale;           // 8
    public Vector2 LocalOffset;             // 8: emit-end offset in laser local frame; unused for CurvyLaser
}
