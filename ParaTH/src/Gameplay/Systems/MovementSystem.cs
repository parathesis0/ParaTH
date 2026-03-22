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
            ref var instRef = ref ctrl.PositionInstructions.UnsafeAt(ctrl.PositionIndex);

            switch (op)
            {
                case PositionInstruction.Ops.Delay:
                    break;
                case PositionInstruction.Ops.Set:
                    break;
                case PositionInstruction.Ops.Add: // EndValue = positionDelta
                    instRef.StartValue = transform.Position;
                    instRef.EndValue += transform.Position;
                    break;
            }

            // instruction takes 0 frame
            // update position immediately to ensure the next op can get the correct value
            if (instRef.Duration == 0)
                transform.Position = instRef.EndValue;
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
                transform.Position = Vector2.Lerp(inst.StartValue, inst.EndValue, t);
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
            ref var instRef = ref ctrl.VelocityInstructions.UnsafeAt(ctrl.VelocityIndex);

            // handle ops
            switch (op)
            {
                case VelocityInstruction.Ops.Delay:
                {
                    break;
                }
                case VelocityInstruction.Ops.SetVelocity:
                {
                    instRef.EndValue = instRef.ParamsOrStartValue; // params: vec2 newVelocity
                    instRef.ParamsOrStartValue = movement.Velocity; // StartValue
                    break;
                }
                case VelocityInstruction.Ops.SetMagnitude:
                {
                    var currentVelocity = movement.Velocity;
                    currentVelocity.Normalize(); // if you get NaN here its your fault
                    instRef.EndValue = currentVelocity * instRef.ParamsOrStartValue.X; // params: float newMagnitude
                    instRef.ParamsOrStartValue = movement.Velocity; // StartValue
                    break;
                }
                case VelocityInstruction.Ops.SetAngle:
                {
                    // reuses fields:
                    // ParamsOrStartValue: X = startAngle, Y = currentMagnitude
                    // EndValue: X = endAngle (calculated to use nearest warp)
                    var currentMagnitude = movement.Velocity.Length();
                    var startAngle = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
                    var endAngle = instRef.ParamsOrStartValue.X; // params: float newAngle

                    float angleDelta = MathHelper.WrapAngle(endAngle - startAngle);
                    instRef.ParamsOrStartValue = new Vector2(startAngle, currentMagnitude);
                    instRef.EndValue.X = startAngle + angleDelta;
                    break;
                }
                case VelocityInstruction.Ops.AddVelocity:
                {
                    instRef.EndValue = movement.Velocity + instRef.ParamsOrStartValue; // params: vec2 velocityDelta
                    instRef.ParamsOrStartValue = movement.Velocity; // StartValue
                    break;
                }
                case VelocityInstruction.Ops.AddMagnitude:
                {
                    var baseVelocity = movement.Velocity;
                    var baseMagnitude = baseVelocity.Length();
                    baseVelocity /= baseMagnitude; // if you get NaN here its your fault
                    instRef.EndValue = baseVelocity * (baseMagnitude + instRef.ParamsOrStartValue.X); // params: float magnitudeDelta
                    instRef.ParamsOrStartValue = movement.Velocity; // StartValue
                    break;
                }
                case VelocityInstruction.Ops.AddAngle:
                {
                    // reuses fields:
                    // ParamsOrStartValue: X = startAngle, Y = currentMagnitude
                    // EndValue: X = endAngle (use warp lerp)
                    var currentMagnitude = movement.Velocity.Length();
                    var baseAngle = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
                    var angleDelta = instRef.ParamsOrStartValue.X; // params: float newAngle
                    instRef.ParamsOrStartValue = new Vector2(baseAngle, currentMagnitude);
                    instRef.EndValue.X = baseAngle + angleDelta;
                    break;
                }
            }

            // instruction takes 0 frame
            // update the Velocity immediately to ensure the next op can get the correct value
            if (instRef.Duration == 0)
            {
                if (op == VelocityInstruction.Ops.SetAngle ||
                    op == VelocityInstruction.Ops.AddAngle)
                {
                    float magnitude = instRef.ParamsOrStartValue.Y;
                    movement.Velocity = new Vector2(
                        magnitude * MathF.Cos(instRef.EndValue.X),
                        magnitude * MathF.Sin(instRef.EndValue.X));
                }
                else if (op != VelocityInstruction.Ops.Delay)
                {
                    movement.Velocity = instRef.EndValue;
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
                    float lerpedAngle = float.Lerp(inst.ParamsOrStartValue.X, inst.EndValue.X, t);
                    float magnitude = inst.ParamsOrStartValue.Y;

                    movement.Velocity = new Vector2(
                        magnitude * MathF.Cos(lerpedAngle),
                        magnitude * MathF.Sin(lerpedAngle));
                }
                else // vector lerp logic
                {
                    movement.Velocity = Vector2.Lerp(inst.ParamsOrStartValue, inst.EndValue, t); // StartValue;
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
            ref var instRef = ref ctrl.AccelerationInstructions.UnsafeAt(ctrl.AccelerationIndex);

            // handle ops
            switch (op)
            {
                case AccelerationInstruction.Ops.Delay:
                {
                    break;
                }
                case AccelerationInstruction.Ops.SetAcceleration:
                {
                    instRef.EndValue = instRef.ParamsOrStartValue; // params: vec2 newAcceleration
                    instRef.ParamsOrStartValue = movement.Acceleration; // StartValue
                    break;
                }
                case AccelerationInstruction.Ops.SetMagnitude:
                {
                    var currentAcceleration = movement.Acceleration;
                    currentAcceleration.Normalize(); // if you get NaN here its your fault
                    instRef.EndValue = currentAcceleration * instRef.ParamsOrStartValue.X; // params: float newMagnitude
                    instRef.ParamsOrStartValue = movement.Acceleration; // StartValue
                    break;
                }
                case AccelerationInstruction.Ops.SetAngle:
                {
                    // reuses fields:
                    // ParamsOrStartValue: X = startAngle, Y = currentMagnitude
                    // EndValue: X = endAngle (calculated to use nearest warp)
                    var currentMagnitude = movement.Acceleration.Length();
                    var startAngle = MathF.Atan2(movement.Acceleration.Y, movement.Acceleration.X);
                    var endAngle = instRef.ParamsOrStartValue.X; // params: float newAngle

                    float angleDelta = MathHelper.WrapAngle(endAngle - startAngle);
                    instRef.ParamsOrStartValue = new Vector2(startAngle, currentMagnitude);
                    instRef.EndValue.X = startAngle + angleDelta;
                    break;
                }
                case AccelerationInstruction.Ops.AddAcceleration:
                {
                    instRef.EndValue = movement.Acceleration + instRef.ParamsOrStartValue; // params: vec2 accelerationDelta
                    instRef.ParamsOrStartValue = movement.Acceleration; // StartValue
                    break;
                }
                case AccelerationInstruction.Ops.AddMagnitude:
                {
                    var baseAcceleration = movement.Acceleration;
                    var baseMagnitude = baseAcceleration.Length();
                    baseAcceleration /= baseMagnitude; // if you get NaN here its your fault
                    instRef.EndValue = baseAcceleration * (baseMagnitude + instRef.ParamsOrStartValue.X); // params: float magnitudeDelta
                    instRef.ParamsOrStartValue = movement.Acceleration; // StartValue
                    break;
                }
                case AccelerationInstruction.Ops.AddAngle:
                {
                    // reuses fields:
                    // ParamsOrStartValue: X = startAngle, Y = currentMagnitude
                    // EndValue: X = endAngle (use warp lerp)
                    var currentMagnitude = movement.Acceleration.Length();
                    var baseAngle = MathF.Atan2(movement.Acceleration.Y, movement.Acceleration.X);
                    var angleDelta = instRef.ParamsOrStartValue.X; // params: float newAngle
                    instRef.ParamsOrStartValue = new Vector2(baseAngle, currentMagnitude);
                    instRef.EndValue.X = baseAngle + angleDelta;
                    break;
                }
            }

            // instruction takes 0 frame
            // update the Acceleration immediately to ensure the next op can get the correct value
            if (instRef.Duration == 0)
            {
                if (op == AccelerationInstruction.Ops.SetAngle ||
                    op == AccelerationInstruction.Ops.AddAngle)
                {
                    float magnitude = instRef.ParamsOrStartValue.Y;
                    movement.Acceleration = new Vector2(
                        magnitude * MathF.Cos(instRef.EndValue.X),
                        magnitude * MathF.Sin(instRef.EndValue.X));
                }
                else if (op != AccelerationInstruction.Ops.Delay)
                {
                    movement.Acceleration = instRef.EndValue;
                }
            }
        }

        // modify acceleration directly
        // takes over acceleration during the lerp
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
                    float lerpedAngle = float.Lerp(inst.ParamsOrStartValue.X, inst.EndValue.X, t);
                    float magnitude = inst.ParamsOrStartValue.Y;

                    movement.Acceleration = new Vector2(
                        magnitude * MathF.Cos(lerpedAngle),
                        magnitude * MathF.Sin(lerpedAngle));
                }
                else // vector lerp logic
                {
                    movement.Acceleration = Vector2.Lerp(inst.ParamsOrStartValue, inst.EndValue, t); // StartValue;
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
