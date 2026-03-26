using Microsoft.Xna.Framework;

namespace ParaTH;

// 20 bytes
public struct RenderState(Vector2 scale, float rotation, Color color, byte layer, StgBlendState blendState)
{
    public Vector2 Scale = scale;                   // 8
    public float Rotation = rotation;               // 4
    public Color Color = color;                     // 4
    public byte Layer = layer;                      // 1
    public StgBlendState BlendState = blendState;   // 1
                                                    // 2 padding, todo: put RendererType here?
}

