using Microsoft.Xna.Framework;

namespace ParaTH;

// 32 bytes
public struct Sprite(SpriteAsset sprite, Vector2 scale, float rotation, Color color, byte layer, StgBlendState blendState)
{
    public SpriteAsset sprite = sprite;             // 8
    public Vector2 Scale = scale;                   // 8
    public float Rotation = rotation;               // 4
    public Color Color = color;                     // 4
    public uint SpawnId = 0;                        // 4 for render order in the same layer, or else destroying entities fucks it up
    public byte Layer = layer;                      // 1
    public StgBlendState BlendState = blendState;   // 1
    public bool IsVisible = true;                   // 1
                                                    // 1 padding
}
