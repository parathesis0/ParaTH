using Microsoft.Xna.Framework;

namespace ParaTH;

public struct SpriteRenderer(SpriteAsset sprite, Color color, byte layer, StgBlendState blendState)
{
    public SpriteAsset Sprite = sprite;
    public Color Color = color;
    public byte Layer = layer;
    public StgBlendState BlendState = blendState;
}
