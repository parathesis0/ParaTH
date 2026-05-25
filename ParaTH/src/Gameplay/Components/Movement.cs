using Microsoft.Xna.Framework;

namespace ParaTH;

// 20 bytes
public struct Movement(
    Vector2 velocity, Vector2 acceleration, bool syncTransformRotation)
{
    public Vector2 Velocity = velocity;                         // 4 + 4
    public Vector2 Acceleration = acceleration;                 // 4 + 4
    public bool SyncTransformRotation = syncTransformRotation;  // 1
                                                                // 3 padding
}
