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

                if (controller.AccelerationInstructions is not null)
                    UpdateAccelerationController(ref controller, ref movement);

                movement.Velocity += movement.Acceleration;

                if (controller.CurveInstructions is not null)
                    UpdateCurveMovement(ref controller, ref movement);

                if (controller.VelocityInstructions is not null)
                    UpdateVelocityController(ref controller, ref movement);

                if (controller.PositionInstructions is not null)
                    UpdatePositionController(ref controller, ref transform);

                transform.Position += movement.Velocity;

                var newPosition = transform.Position;

                var delta = newPosition - oldPosition;

                if (movement.SyncTransformRotation && delta.Length() >= float.Epsilon)
                    transform.Rotation = MathF.Atan2(delta.Y, delta.X);
            }
        }
    }

    private static void UpdatePositionController(ref BulletController ctrl, ref Transform transform)
    {
        // handle instruction advance
        var insts = ctrl.PositionInstructions;
        while (ctrl.PositionIndex < insts.Length - 1 &&
               ctrl.PositionTick >= insts.UnsafeAt(ctrl.PositionIndex + 1).TriggerFrame)
        {
            // reached new instruction, init it
            ctrl.PositionIndex++;

            var op = ctrl.PositionInstructions.UnsafeAt(ctrl.PositionIndex).Op;
            var inst = ctrl.PositionInstructions.UnsafeAt(ctrl.PositionIndex);

            switch (op)
            {
                case PositionInstruction.Ops.Delay:
                    break;
                case PositionInstruction.Ops.Set:
                    break;
                case PositionInstruction.Ops.Add: // inst.EndValue = positionDelta
                    ctrl.PositionStartValue = transform.Position;
                    ctrl.PositionEndValue = transform.Position + inst.Params;
                    break;
            }

            // instruction takes 0 frame
            // update position immediately to ensure the next op can get the correct value
            if (inst.Duration == 0)
                transform.Position = ctrl.PositionEndValue;
        }

        // modify position directly
        // takes over velocity during the lerp
        if (ctrl.PositionIndex >= 0 &&
            ctrl.PositionInstructions.UnsafeAt(ctrl.PositionIndex).Op != PositionInstruction.Ops.Delay)
        {
            var inst = ctrl.PositionInstructions.UnsafeAt(ctrl.PositionIndex);
            int relativeTick = ctrl.PositionTick - inst.TriggerFrame;
            if (relativeTick < inst.Duration)
            {
                var t = (float)(relativeTick + 1) / inst.Duration;
                t = inst.Ease(t);
                transform.Position = Vector2.Lerp(ctrl.PositionStartValue, ctrl.PositionEndValue, t);
            }
        }

        ctrl.PositionTick++;
    }

    private static void UpdateVelocityController(ref BulletController ctrl, ref Movement movement)
    {
        // handle instruction advance
        var insts = ctrl.VelocityInstructions;
        while (ctrl.VelocityIndex < insts.Length - 1 &&
               ctrl.VelocityTick >= insts.UnsafeAt(ctrl.VelocityIndex + 1).TriggerFrame)
        {
            // reached new instruction, init it
            ctrl.VelocityIndex++;

            var op = ctrl.VelocityInstructions.UnsafeAt(ctrl.VelocityIndex).Op;
            var inst = ctrl.VelocityInstructions.UnsafeAt(ctrl.VelocityIndex);

            // handle ops
            switch (op)
            {
                case VelocityInstruction.Ops.Delay:
                {
                    break;
                }
                case VelocityInstruction.Ops.SetVelocity:
                {
                    ctrl.VelocityStartValue = movement.Velocity;
                    ctrl.VelocityEndValue = inst.Params; // params: vec2 newVelocity
                    break;
                }
                case VelocityInstruction.Ops.SetMagnitude:
                {
                    var currentVelocity = movement.Velocity;
                    currentVelocity.Normalize(); // if you get NaN here its your fault
                    ctrl.VelocityStartValue = movement.Velocity;
                    ctrl.VelocityEndValue = currentVelocity * inst.Params.X; // params: float newMagnitude
                    break;
                }
                case VelocityInstruction.Ops.SetAngle:
                {
                    // reuses fields:
                    // ctrl.VelocityStartValue: X = startAngle, Y = currentMagnitude
                    // ctrl.VelocityEndValue: X = endAngle (calculated to use nearest warp)
                    var currentMagnitude = movement.Velocity.Length();
                    var startAngle = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
                    var endAngle = inst.Params.X; // params: float newAngle

                    float angleDelta = MathHelper.WrapAngle(endAngle - startAngle);
                    ctrl.VelocityStartValue = new Vector2(startAngle, currentMagnitude);
                    ctrl.VelocityEndValue.X = startAngle + angleDelta;
                    break;
                }
                case VelocityInstruction.Ops.AddVelocity:
                {
                    ctrl.VelocityStartValue = movement.Velocity;
                    ctrl.VelocityEndValue = movement.Velocity + inst.Params; // params: vec2 velocityDelta
                    break;
                }
                case VelocityInstruction.Ops.AddMagnitude:
                {
                    var baseVelocity = movement.Velocity;
                    var baseMagnitude = baseVelocity.Length();
                    baseVelocity /= baseMagnitude; // if you get NaN here its your fault
                    ctrl.VelocityStartValue = movement.Velocity;
                    ctrl.VelocityEndValue = baseVelocity * (baseMagnitude + inst.Params.X); // params: float magnitudeDelta
                    break;
                }
                case VelocityInstruction.Ops.AddAngle:
                {
                    // reuses fields:
                    // ctrl.VelocityStartValue: X = startAngle, Y = currentMagnitude
                    // ctrl.VelocityEndValue: X = endAngle (use warp lerp)
                    var currentMagnitude = movement.Velocity.Length();
                    var baseAngle = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
                    var angleDelta = inst.Params.X; // params: float newAngle
                    ctrl.VelocityStartValue = new Vector2(baseAngle, currentMagnitude);
                    ctrl.VelocityEndValue.X = baseAngle + angleDelta;
                    break;
                }
            }

            // instruction takes 0 frame
            // update the Velocity immediately to ensure the next op can get the correct value
            if (inst.Duration == 0)
            {
                if (op == VelocityInstruction.Ops.SetAngle ||
                    op == VelocityInstruction.Ops.AddAngle)
                {
                    float magnitude = ctrl.VelocityStartValue.Y;
                    movement.Velocity = new Vector2(
                        magnitude * MathF.Cos(ctrl.VelocityEndValue.X),
                        magnitude * MathF.Sin(ctrl.VelocityEndValue.X));
                }
                else if (op != VelocityInstruction.Ops.Delay)
                {
                    movement.Velocity = ctrl.VelocityEndValue;
                }
            }
        }

        // modify velocity directly
        // takes over acceleration during the lerp
        if (ctrl.VelocityIndex >= 0 &&
            ctrl.VelocityInstructions.UnsafeAt(ctrl.VelocityIndex).Op != VelocityInstruction.Ops.Delay)
        {
            var inst = ctrl.VelocityInstructions.UnsafeAt(ctrl.VelocityIndex);
            int relativeTick = ctrl.VelocityTick - inst.TriggerFrame;
            if (relativeTick < inst.Duration)
            {
                // duration won't be zero here for it is handled in the while loop
                var t = (float)(relativeTick + 1) / inst.Duration;
                t = inst.Ease(t);
                if (inst.Op == VelocityInstruction.Ops.SetAngle ||
                    inst.Op == VelocityInstruction.Ops.AddAngle)
                {
                    // lerp angle
                    float lerpedAngle = float.Lerp(ctrl.VelocityStartValue.X, ctrl.VelocityEndValue.X, t);
                    float magnitude = inst.Params.Y;

                    movement.Velocity = new Vector2(
                        magnitude * MathF.Cos(lerpedAngle),
                        magnitude * MathF.Sin(lerpedAngle));
                }
                else // vector lerp logic
                {
                    movement.Velocity = Vector2.Lerp(ctrl.VelocityStartValue, ctrl.VelocityEndValue, t);
                }
            }
        }

        ctrl.VelocityTick++;
    }

    private static void UpdateAccelerationController(ref BulletController ctrl, ref Movement movement)
    {
        // handle instruction advance
        var insts = ctrl.AccelerationInstructions;
        while (ctrl.AccelerationIndex < insts.Length - 1 &&
               ctrl.AccelerationTick >= insts.UnsafeAt(ctrl.AccelerationIndex + 1).TriggerFrame)
        {
            // reached new instruction, init it
            ctrl.AccelerationIndex++;

            var op = ctrl.AccelerationInstructions.UnsafeAt(ctrl.AccelerationIndex).Op;
            var inst = ctrl.AccelerationInstructions.UnsafeAt(ctrl.AccelerationIndex);

            // handle ops
            switch (op)
            {
                case AccelerationInstruction.Ops.Delay:
                {
                    break;
                }
                case AccelerationInstruction.Ops.SetAcceleration:
                {
                    ctrl.AccelerationStartValue = movement.Acceleration;
                    ctrl.AccelerationEndValue = inst.Params; // params: vec2 newAcceleration
                    break;
                }
                case AccelerationInstruction.Ops.SetMagnitude:
                {
                    var currentAcceleration = movement.Acceleration;
                    currentAcceleration.Normalize(); // if you get NaN here its your fault
                    ctrl.AccelerationStartValue = movement.Acceleration;
                    ctrl.AccelerationEndValue = currentAcceleration * inst.Params.X; // params: float newMagnitude
                    break;
                }
                case AccelerationInstruction.Ops.SetAngle:
                {
                    // reuses fields:
                    // ctrl.AccelerationStartValue: X = startAngle, Y = currentMagnitude
                    // ctrl.AccelerationEndValue: X = endAngle (calculated to use nearest warp)
                    var currentMagnitude = movement.Acceleration.Length();
                    var startAngle = MathF.Atan2(movement.Acceleration.Y, movement.Acceleration.X);
                    var endAngle = inst.Params.X; // params: float newAngle

                    float angleDelta = MathHelper.WrapAngle(endAngle - startAngle);
                    ctrl.AccelerationStartValue = new Vector2(startAngle, currentMagnitude);
                    ctrl.AccelerationEndValue.X = startAngle + angleDelta;
                    break;
                }
                case AccelerationInstruction.Ops.AddAcceleration:
                {
                    ctrl.AccelerationStartValue = movement.Acceleration;
                    ctrl.AccelerationEndValue = movement.Acceleration + inst.Params; // params: vec2 accelerationDelta
                    break;
                }
                case AccelerationInstruction.Ops.AddMagnitude:
                {
                    var baseAcceleration = movement.Acceleration;
                    var baseMagnitude = baseAcceleration.Length();
                    baseAcceleration /= baseMagnitude; // if you get NaN here its your fault
                    ctrl.AccelerationStartValue = movement.Acceleration;
                    ctrl.AccelerationEndValue = baseAcceleration * (baseMagnitude + inst.Params.X); // params: float magnitudeDelta
                    break;
                }
                case AccelerationInstruction.Ops.AddAngle:
                {
                    // reuses fields:
                    // ctrl.AccelerationStartValue: X = startAngle, Y = currentMagnitude
                    // ctrl.AccelerationEndValue: X = endAngle (use warp lerp)
                    var currentMagnitude = movement.Acceleration.Length();
                    var baseAngle = MathF.Atan2(movement.Acceleration.Y, movement.Acceleration.X);
                    var angleDelta = inst.Params.X; // params: float newAngle
                    ctrl.AccelerationStartValue = new Vector2(baseAngle, currentMagnitude);
                    ctrl.AccelerationEndValue.X = baseAngle + angleDelta;
                    break;
                }
            }

            // instruction takes 0 frame
            // update the Acceleration immediately to ensure the next op can get the correct value
            if (inst.Duration == 0)
            {
                if (op == AccelerationInstruction.Ops.SetAngle ||
                    op == AccelerationInstruction.Ops.AddAngle)
                {
                    float magnitude = ctrl.AccelerationStartValue.Y;
                    movement.Acceleration = new Vector2(
                        magnitude * MathF.Cos(ctrl.AccelerationEndValue.X),
                        magnitude * MathF.Sin(ctrl.AccelerationEndValue.X));
                }
                else if (op != AccelerationInstruction.Ops.Delay)
                {
                    movement.Acceleration = ctrl.AccelerationEndValue;
                }
            }
        }

        // modify acceleration directly
        if (ctrl.AccelerationIndex >= 0 &&
            ctrl.AccelerationInstructions.UnsafeAt(ctrl.AccelerationIndex).Op != AccelerationInstruction.Ops.Delay)
        {
            var inst = ctrl.AccelerationInstructions.UnsafeAt(ctrl.AccelerationIndex);
            int relativeTick = ctrl.AccelerationTick - inst.TriggerFrame;
            if (relativeTick < inst.Duration)
            {
                // duration won't be zero here for it is handled in the while loop
                var t = (float)(relativeTick + 1) / inst.Duration;
                t = inst.Ease(t);
                if (inst.Op == AccelerationInstruction.Ops.SetAngle ||
                    inst.Op == AccelerationInstruction.Ops.AddAngle)
                {
                    // lerp angle
                    float lerpedAngle = float.Lerp(ctrl.AccelerationStartValue.X, ctrl.AccelerationEndValue.X, t);
                    float magnitude = inst.Params.Y;

                    movement.Acceleration = new Vector2(
                        magnitude * MathF.Cos(lerpedAngle),
                        magnitude * MathF.Sin(lerpedAngle));
                }
                else // vector lerp logic
                {
                    movement.Acceleration = Vector2.Lerp(ctrl.AccelerationStartValue, ctrl.AccelerationEndValue, t);
                }
            }
        }

        ctrl.AccelerationTick++;
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
