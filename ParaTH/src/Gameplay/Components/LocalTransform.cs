using Microsoft.Xna.Framework;

namespace ParaTH;

public struct LocalTransform(Entity parent, Vector2 localPosition, Vector2 localScale, float localRotation)
{
    public Entity Parent = parent;
    public Vector2 LocalPosition = localPosition;
    public Vector2 LocalScale = localScale;
    public float LocalRotation = localRotation;
    public int Depth = 0;
}

