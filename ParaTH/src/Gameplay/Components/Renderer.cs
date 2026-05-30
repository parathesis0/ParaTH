using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

// 64 bytes, huge
public struct Renderer
{
    public Texture2D Texture;           // 8
    public Rectangle SourceRect;        // 4 + 4 + 4 + 4
    public Vector2 Anchor;              // 4 + 4
    public Vector2 Scale;               // 4 + 4
    public float Rotation;              // 4, fixed angle or offset based on RotationMode
    public Color Color;                 // 4
    public uint SpawnId;                // 4 for render order in the same layer, or else destroying entities fucks it up
    public byte Layer;                  // 1
    public StgBlendState BlendState;    // 1
    public bool IsFixedRotation;        // 1
    public bool IsVisible;              // 1
                                        // 4 padding

    // resolves the angle the sprite is drawn at.
    // IsFixedRotation: the renderer's own Rotation is an absolute world angle, ignoring the entity's facing.
    // otherwise: renderer.Rotation is an offset added on top of the entity's Transform.Rotation.
    public readonly float ResolveRenderRotation(float transformRotation)
        => IsFixedRotation ? Rotation : transformRotation + Rotation;
}
