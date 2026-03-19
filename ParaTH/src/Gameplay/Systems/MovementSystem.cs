using Microsoft.Xna.Framework;

namespace ParaTH;

public sealed class MovementSystem(World world)
{
    private QueryDescriptor query = new QueryDescriptor()
        .WithAll<Transform, Movement>();

    public void Update()
    {
        var q = world.GetOrCreateQuery(query);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            if (archetype.Has<CurveMovement>())
                UpdateCurveBullet(archetype);
            else
                UpdateBasicBullet(archetype);
        }
    }

    private static void UpdateCurveBullet(Archetype archetype)
    {
        foreach (ref var chunk in archetype.GetChunksSpan())
        {
            chunk.GetFilledComponentSpan<Transform, Movement, CurveMovement>
                (out var transforms, out var movements, out var curves);

            for (int i = 0; i < chunk.EntityCount; i++)
            {
                ref var transform = ref transforms.UnsafeAt(i);
                ref var movement = ref movements.UnsafeAt(i);
                ref var curve = ref curves.UnsafeAt(i);

                // update velocity first
                movement.Velocity += movement.Acceleration;

                // update angular velocity
                var frames = curve.Frames;
                while (curve.CurrentIndex < frames.Length - 1 &&
                       curve.CurrentFrame >= frames[curve.CurrentIndex + 1].TriggerFrame)
                {
                    curve.CurrentIndex++;
                }

                var currentAngularVelocity = frames[curve.CurrentIndex].AngularVelocity;

                var sin = MathF.Sin(currentAngularVelocity);
                var cos = MathF.Cos(currentAngularVelocity);
                var x = movement.Velocity.X;
                var y = movement.Velocity.Y;

                curve.CurrentFrame++;

                movement.Velocity.X = x * cos - y * sin;
                movement.Velocity.Y = x * sin + y * cos;

                // update position
                transform.Position += movement.Velocity;
                if (movement.SyncTransformRotation)
                    transform.Rotation = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
            }
        }
    }

    private static void UpdateBasicBullet(Archetype archetype)
    {
        foreach (ref var chunk in archetype.GetChunksSpan())
        {
            chunk.GetFilledComponentSpan<Transform, Movement>
                (out var transforms, out var movements);

            for (int i = 0; i < chunk.EntityCount; i++)
            {
                ref var transform = ref transforms.UnsafeAt(i);
                ref var movement = ref movements.UnsafeAt(i);

                movement.Velocity += movement.Acceleration;
                transform.Position += movement.Velocity;
                if (movement.SyncTransformRotation)
                    transform.Rotation = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
            }
        }
    }
}
