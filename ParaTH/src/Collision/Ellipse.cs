using Microsoft.Xna.Framework;

namespace ParaTH;

public struct Ellipse(Vector2 halfSize, float rotation)
{
    public Vector2 HalfSize = halfSize;
    public float Rotation = rotation;
}
