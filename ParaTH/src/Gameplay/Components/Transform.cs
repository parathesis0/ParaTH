using Microsoft.Xna.Framework;

namespace ParaTH;

public struct Transform(Vector2 position, Vector2 scale, float rotation)
{
    public Vector2 Position = position;
    public Vector2 Scale = scale;
    public float Rotation = rotation;
}
