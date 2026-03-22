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

                    float angleDelta = (endAngle - startAngle) % (MathHelper.TwoPi);
                    if (angleDelta >  MathHelper.Pi)
                        angleDelta -= MathHelper.Pi * 2;
                    if (angleDelta < -MathHelper.Pi)
                        angleDelta += MathHelper.Pi * 2;

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
                    baseVelocity.Normalize(); // if you get NaN here its your fault
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
                var t = inst.Duration == 0 ? 1 : (float)(relativeTick + 1) / inst.Duration;
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
