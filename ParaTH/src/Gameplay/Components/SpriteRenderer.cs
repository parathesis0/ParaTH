using Microsoft.Xna.Framework;

namespace ParaTH;

// 16 bytes
public struct SpriteRenderer(SpriteAsset sprite, Color color, byte layer, StgBlendState blendState)
{
    public SpriteAsset Sprite = sprite;             // 8
    public Color Color = color;                     // 4
    public byte Layer = layer;                      // 1
    public StgBlendState BlendState = blendState;   // 1
                                                    // 2 padding
}
