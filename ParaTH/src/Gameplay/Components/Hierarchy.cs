using Microsoft.Xna.Framework;

namespace ParaTH;

// 28 bytes
public struct Hierarchy(Entity parent, Vector2 localPosition, Vector2 localScale, float localRotation)
{
    public Vector2 LocalPosition = localPosition;   // 8
    public Vector2 LocalScale = localScale;         // 8
    public float LocalRotation = localRotation;     // 4
    public Entity Parent = parent;                  // 4
    public int Depth = 0;                           // 4
}

