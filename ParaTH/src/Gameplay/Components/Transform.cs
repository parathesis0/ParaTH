using Microsoft.Xna.Framework;

namespace ParaTH;

// 20 bytes
public struct Transform(Vector2 position, Vector2 scale, float rotation)
{
    public Vector2 Position = position; // 4 + 4
    public Vector2 Scale = scale;       // 4 + 4
    public float Rotation = rotation;   // 4
}
