using Microsoft.Xna.Framework;

namespace ParaTH;

public struct ObbRect(Vector2 halfSize, float rotation)
{
    public Vector2 HalfSize = halfSize;
    public float Rotation = rotation;
}
