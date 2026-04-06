using Microsoft.Xna.Framework;

namespace ParaTH;

// 20 bytes
public struct Movement(
    Vector2 velocity, Vector2 acceleration, bool syncRenderRotation, bool syncTransformRotation)
{
    public Vector2 Velocity = velocity;                         // 8
    public Vector2 Acceleration = acceleration;                 // 8
    public bool SyncRenderStateRotation = syncRenderRotation;   // 1
    public bool SyncTransformRotation = syncTransformRotation;  // 1
                                                                // 2 padding
}
