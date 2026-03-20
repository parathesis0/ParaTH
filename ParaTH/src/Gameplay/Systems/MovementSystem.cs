using Microsoft.Xna.Framework;

namespace ParaTH;

public sealed class MovementSystem(World world)
{
    private QueryDescriptor query = new QueryDescriptor()
        .WithAll<Transform, Movement>();

    public void Update()
    {
        var q = world.GetOrCreateQuery(query);

        // not scalable, find better way
        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            if (archetype.Has<CurveMovementController>())
                UpdateCurveBullet(archetype);
            else
                UpdateBasicBullet(archetype);
        }
    }

    private static void UpdateCurveBullet(Archetype archetype)
    {
        foreach (ref var chunk in archetype.GetChunksSpan())
        {
            chunk.GetFilledComponentSpan<Transform, Movement, CurveMovementController>
                (out var transforms, out var movements, out var curves);

            for (int i = 0; i < chunk.EntityCount; i++)
            {
                ref var transform = ref transforms.UnsafeAt(i);
                ref var movement = ref movements.UnsafeAt(i);
                ref var curve = ref curves.UnsafeAt(i);

                // update velocity first
                movement.Velocity += movement.Acceleration;

                // update curve movement controller
                var angularVelocity = UpdateCurveMovementController(ref curve);

                // rotate velocity based on angular velocity
                var sin = MathF.Sin(angularVelocity);
                var cos = MathF.Cos(angularVelocity);
                var x = movement.Velocity.X;
                var y = movement.Velocity.Y;
                movement.Velocity.X = x * cos - y * sin;
                movement.Velocity.Y = x * sin + y * cos;

                // update position
                transform.Position += movement.Velocity;
                if (movement.SyncTransformRotation)
                    transform.Rotation = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
            }
        }
    }

    private static float UpdateCurveMovementController(ref CurveMovementController curve)
    {
        // handle instruction advance
        var instructions = curve.Instructions;
        while (curve.CurrentIndex < instructions.Length - 1 &&
               curve.TickCount >= instructions.UnsafeAt(curve.CurrentIndex + 1).TriggerFrame)
        {
            curve.CurrentIndex++;
        }

        // handle looping
        ref var instructionRef = ref instructions.UnsafeAt(curve.CurrentIndex);
        if (instructionRef.TargetIndex > 0 && instructionRef.LoopRepeatTimes > 0)
        {
            curve.CurrentIndex = instructionRef.TargetIndex;
            curve.TickCount = instructions.UnsafeAt(curve.CurrentIndex).TriggerFrame;
            if (instructionRef.LoopRepeatTimes != CurveMovementInstruction.Infinite)
                instructionRef.LoopRepeatTimes--;
        }

        // get and return the current angular velocity
        var currentInstruction = instructions.UnsafeAt(curve.CurrentIndex);
        var currentAngularVelocity = currentInstruction.AngularVelocity;

        curve.TickCount++;

        return currentAngularVelocity;
    }

    // wip, not tested. def mistakes
    private static void UpdateVelocityController(ref VelocityController vCon, ref Movement movement)
    {
        var insts = vCon.Instructions;
        while (vCon.CurrentIndex < insts.Length - 1 &&
               vCon.TickCount >= insts.UnsafeAt(vCon.CurrentIndex + 1).TriggerFrame)
        {
            // reached new frame, init it
            vCon.CurrentIndex++;
            vCon.TickCount = 0;

            if (vCon.CurrentInstrucion.IsRelative)
            {
                ref var instRef = ref vCon.CurrentInstrucionRef;
                instRef.StartVelocity = movement.Velocity;
                instRef.EndVelocity += movement.Velocity;
            }
        }

        // assume no acceleration during the lerp
        var inst = vCon.Instructions[vCon.CurrentIndex];
        var t = inst.Duration == 0 ? 1 : (float)(vCon.TickCount / inst.Duration);
        t = inst.Ease(t);
        var targetVelocity = Vector2.Lerp(inst.StartVelocity, inst.EndVelocity, t);

        vCon.TickCount++;

        movement.Velocity = targetVelocity;
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
