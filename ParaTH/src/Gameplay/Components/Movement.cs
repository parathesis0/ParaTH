using Microsoft.Xna.Framework;

namespace ParaTH;

// 20 bytes
public struct Movement(Vector2 velocity, Vector2 acceleration, bool syncRotation)
{
    public Vector2 Velocity = velocity;                 // 8
    public Vector2 Acceleration = acceleration;         // 8
    public bool SyncRenderStateRotation = syncRotation; // 1
                                                        // 3 padding
}
