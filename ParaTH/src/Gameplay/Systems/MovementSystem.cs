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
            if (archetype.Has<BulletController>())
                UpdateControlledBullet(archetype);
            else
                UpdateBasicBullet(archetype);
        }
    }

    private static void UpdateControlledBullet(Archetype archetype)
    {
        foreach (ref var chunk in archetype.GetChunksSpan())
        {
            chunk.GetFilledComponentSpan<Transform, Movement, BulletController>(
                out var transforms, out var movements, out var controllers);

            for (int i = 0; i < chunk.EntityCount; i++)
            {
                ref var transform = ref transforms.UnsafeAt(i);
                ref var movement = ref movements.UnsafeAt(i);
                ref var controller = ref controllers.UnsafeAt(i);

                var oldPosition = transform.Position;

                // update velocity first
                movement.Velocity += movement.Acceleration;

                // update curve movement controller
                if (controller.CurveInstructions is not null)
                    UpdateCurveMovement(ref controller, ref movement);

                // update velocity control
                if (controller.VelocityInstructions is not null)
                    UpdateVelocityController(ref controller, ref movement);

                // update position
                transform.Position += movement.Velocity;

                var newPosition = transform.Position;

                var delta = newPosition - oldPosition;

                if (movement.SyncTransformRotation && delta.Length() >= float.Epsilon)
                    transform.Rotation = MathF.Atan2(delta.Y, delta.X);
            }
        }
    }

    private static void UpdateCurveMovement(ref BulletController ctrl, ref Movement movement)
    {
        var insts = ctrl.CurveInstructions;

        // handle instruction advance
        while (ctrl.CurveIndex < insts.Length - 1 &&
               ctrl.CurveTick >= insts.UnsafeAt(ctrl.CurveIndex + 1).TriggerFrame)
        {
            ctrl.CurveIndex++;
        }

        float currentAngularVelocity = 0f;
        ref var instRef = ref insts.UnsafeAt(ctrl.CurveIndex);

        // only apply once we've reached the current instruction's trigger frame
        if (ctrl.CurveTick >= instRef.TriggerFrame)
        {
            // handle looping
            if (instRef.TargetIndex >= 0 && instRef.LoopRepeatTimes > 0)
            {
                ctrl.CurveIndex = instRef.TargetIndex;
                ctrl.CurveTick = insts.UnsafeAt(ctrl.CurveIndex).TriggerFrame;
                if (instRef.LoopRepeatTimes != CurveInstruction.Infinite)
                    instRef.LoopRepeatTimes--;
            }

            currentAngularVelocity = insts.UnsafeAt(ctrl.CurveIndex).AngularVelocity;
        }

        // rotate velocity based on angular velocity
        if (currentAngularVelocity != 0f)
        {
            var sin = MathF.Sin(currentAngularVelocity);
            var cos = MathF.Cos(currentAngularVelocity);
            var x = movement.Velocity.X;
            var y = movement.Velocity.Y;
            movement.Velocity.X = x * cos - y * sin;
            movement.Velocity.Y = x * sin + y * cos;
        }

        ctrl.CurveTick++;
    }

    // painful to write and debug. all this shit to save 4 bytes
    private static void UpdateVelocityController(ref BulletController ctrl, ref Movement movement)
    {
        // handle instruction advance
        var insts = ctrl.VelocityInstructions;
        while (ctrl.VelocityIndex < insts.Length - 1 &&
               ctrl.VelocityTick >= insts.UnsafeAt(ctrl.VelocityIndex + 1).TriggerFrame)
        {
            // reached new instruction, init it
            ctrl.VelocityIndex++;
            ctrl.VelocityTick = 0;

            ref var instRef = ref ctrl.VelocityInstructions.UnsafeAt(ctrl.VelocityIndex);

            if (instRef.IsRelative && instRef.IsAdd) // AddRelative
            {
                // calculate relative velocity vector
                var currentAngle = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
                var angle = currentAngle + instRef.AngleDelta;
                var magnitute = instRef.NewVelocityMagnitude;

                var velocityDelta = new Vector2(
                    magnitute * MathF.Cos(angle),
                    magnitute * MathF.Sin(angle));

                instRef.StartVelocity = movement.Velocity;
                instRef.EndVelocity = instRef.StartVelocity + velocityDelta;
            }
            else if (instRef.IsRelative) // SetRelative.
            {
                // calculate relative velocity vector
                var currentAngle = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
                var angle = currentAngle + instRef.AngleDelta;
                var magnitute = instRef.NewVelocityMagnitude;

                var velocityDelta = new Vector2(
                    magnitute * MathF.Cos(angle),
                    magnitute * MathF.Sin(angle));

                // you could merge this branch with the previous one but it's confusing enough
                instRef.StartVelocity = movement.Velocity;
                instRef.EndVelocity = velocityDelta;
            }
            else if (instRef.IsAdd) // Add
            {
                // assignment has to be done in this order because AddEndVelocity is stored in StartVelocity
                instRef.EndVelocity = movement.Velocity + instRef.AddEndVelocity;
                instRef.StartVelocity = movement.Velocity;
            }
            // Set, no value juggling here
        }

        // lerp velocity directly
        // takes over acceleration during the lerp
        var inst = ctrl.VelocityInstructions.UnsafeAt(ctrl.VelocityIndex);

        if (!inst.IsDelay)
        {
            var t = inst.Duration == 0 ? 1 : (float)(ctrl.VelocityTick + 1) / inst.Duration;
            t = inst.Ease(t);
            var targetVelocity = Vector2.Lerp(inst.StartVelocity, inst.EndVelocity, t);
            movement.Velocity = targetVelocity;
        }

        ctrl.VelocityTick++;
    }

    private static void UpdateBasicBullet(Archetype archetype)
    {
        foreach (ref var chunk in archetype.GetChunksSpan())
        {
            chunk.GetFilledComponentSpan<Transform, Movement>(
                out var transforms, out var movements);

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
