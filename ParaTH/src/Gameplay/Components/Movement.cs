using Microsoft.Xna.Framework;

namespace ParaTH;

public struct Movement(Vector2 velocity, Vector2 acceleration, bool syncTransformRotation)
{
    public Vector2 Velocity = velocity;
    public Vector2 Acceleration = acceleration;
    public bool SyncTransformRotation = syncTransformRotation;
}
