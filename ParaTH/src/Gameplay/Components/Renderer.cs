using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

// 56 bytes, huge
public struct Renderer
{
    public Texture2D Texture;           // 8
    public Rectangle SourceRect;        // 4 + 4 + 4 + 4
    public Vector2 Anchor;              // 4 + 4
    public Vector2 Scale;               // 4 + 4
    public float Rotation;              // 4
    public Color Color;                 // 4
    public uint SpawnId;                // 4 for render order in the same layer, or else destroying entities fucks it up
    public byte Layer;                  // 1
    public StgBlendState BlendState;    // 1
    public bool IsVisible;              // 1
                                        // 1 padding
}
