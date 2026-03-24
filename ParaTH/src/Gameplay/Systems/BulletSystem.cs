using Microsoft.Xna.Framework;

namespace ParaTH;

public sealed class BulletSystem(World world)
{
    private QueryDescriptor query = new QueryDescriptor()
        .WithAll<Transform, Movement, Lifetime, SpawnAnimation>();

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
            chunk.GetFilledComponentSpan<Transform, Movement, Lifetime, SpawnAnimation, BulletController>(
                out var transforms, out var movements, out var lifetimes, out var spawnAnims, out var controllers);

            for (int i = 0; i < chunk.EntityCount; i++)
            {
                ref var transform = ref transforms.UnsafeAt(i);
                ref var movement = ref movements.UnsafeAt(i);
                ref var lifetime = ref lifetimes.UnsafeAt(i);
                ref var spawnAnim = ref spawnAnims.UnsafeAt(i);
                ref var controller = ref controllers.UnsafeAt(i);

                var oldPosition = transform.Position;

                var currentFrame = lifetime.AliveFrames;

                if (controller.AccelerationInstructions.Length != 0)
                    UpdateAccelerationController(ref controller, currentFrame, ref movement);

                movement.Velocity += movement.Acceleration;

                if (controller.CurveInstructions.Length != 0)
                    UpdateCurveMovement(ref controller, currentFrame, ref movement);

                if (controller.VelocityInstructions.Length != 0)
                    UpdateVelocityController(ref controller, currentFrame, ref movement);

                if (controller.PositionInstructions.Length != 0)
                    UpdatePositionController(ref controller, currentFrame, ref transform);

                UpdateSpawnAnimation(ref spawnAnim, currentFrame, out var velocityMultiplier);

                transform.Position += movement.Velocity * velocityMultiplier;

                var newPosition = transform.Position;
                var delta = newPosition - oldPosition;
                if (movement.SyncTransformRotation && delta.Length() >= float.Epsilon)
                    transform.Rotation = MathF.Atan2(delta.Y, delta.X);

                lifetime.AliveFrames++;
            }
        }
    }

    private static void UpdatePositionController(ref BulletController ctrl, ushort currentFrame, ref Transform transform)
    {
        // handle instruction advance
        var insts = ctrl.PositionInstructions;
        while (ctrl.PositionIndex < insts.Length - 1 &&
               currentFrame >= insts.UnsafeAt(ctrl.PositionIndex + 1).TriggerFrame)
        {
            // reached new instruction, init it
            ctrl.PositionIndex++;

            var op = ctrl.PositionInstructions.UnsafeAt(ctrl.PositionIndex).Op;
            var inst = ctrl.PositionInstructions.UnsafeAt(ctrl.PositionIndex);

            switch (op)
            {
                case PositionInstruction.Ops.Set:
                    ctrl.PositionStartValue = transform.Position;
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
        if (ctrl.PositionIndex >= 0)
        {
            var inst = ctrl.PositionInstructions.UnsafeAt(ctrl.PositionIndex);
            int relativeTick = currentFrame - inst.TriggerFrame;
            if (relativeTick < inst.Duration)
            {
                var t = (float)(relativeTick + 1) / inst.Duration;
                t = Easing.Evaluate(inst.EaseType, t);
                transform.Position = Vector2.Lerp(ctrl.PositionStartValue, ctrl.PositionEndValue, t);
            }
        }
    }

    private static void UpdateVelocityController(ref BulletController ctrl, ushort currentFrame, ref Movement movement)
    {
        // handle instruction advance
        var insts = ctrl.VelocityInstructions;
        while (ctrl.VelocityIndex < insts.Length - 1 &&
               currentFrame >= insts.UnsafeAt(ctrl.VelocityIndex + 1).TriggerFrame)
        {
            // reached new instruction, init it
            ctrl.VelocityIndex++;

            var op = ctrl.VelocityInstructions.UnsafeAt(ctrl.VelocityIndex).Op;
            var inst = ctrl.VelocityInstructions.UnsafeAt(ctrl.VelocityIndex);

            // handle ops
            switch (op)
            {
                case VelocityInstruction.Ops.SetVelocity:
                {
                    ctrl.VelocityStartValue = movement.Velocity;
                    ctrl.VelocityEndValue = inst.Params; // newVelocity
                    break;
                }
                case VelocityInstruction.Ops.SetMagnitude:
                {
                    var direction = movement.Velocity;
                    direction.Normalize(); // if you get NaN here its your fault
                    ctrl.VelocityStartValue = movement.Velocity;
                    ctrl.VelocityEndValue = direction * inst.Params.X; // newMagnitude
                    break;
                }
                case VelocityInstruction.Ops.SetAngle:
                {
                    // ctrl.VelocityStartValue: X = startAngle, Y = currentMagnitude
                    // ctrl.VelocityEndValue: X = endAngle (calculated to use nearest lerp)
                    var currentMagnitude = movement.Velocity.Length();
                    var startAngle = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
                    var endAngle = inst.Params.X; // newAngle

                    float angleDelta = MathHelper.WrapAngle(endAngle - startAngle);
                    ctrl.VelocityStartValue = new Vector2(startAngle, currentMagnitude);
                    ctrl.VelocityEndValue.X = startAngle + angleDelta;
                    break;
                }
                case VelocityInstruction.Ops.AddVelocity:
                {
                    ctrl.VelocityStartValue = movement.Velocity;
                    ctrl.VelocityEndValue = movement.Velocity + inst.Params; // velocityDelta
                    break;
                }
                case VelocityInstruction.Ops.AddMagnitude:
                {
                    var magnitude = movement.Velocity.Length();
                    ctrl.VelocityStartValue = movement.Velocity;
                    ctrl.VelocityEndValue = movement.Velocity / magnitude * (magnitude + inst.Params.X); // magnitudeDelta
                    break;
                }
                case VelocityInstruction.Ops.AddAngle:
                {
                    // ctrl.VelocityStartValue: X = startAngle, Y = currentMagnitude
                    // ctrl.VelocityEndValue: X = endAngle (use warp lerp)
                    var currentMagnitude = movement.Velocity.Length();
                    var baseAngle = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
                    ctrl.VelocityStartValue = new Vector2(baseAngle, currentMagnitude);
                    ctrl.VelocityEndValue.X = baseAngle + inst.Params.X; // angleDelta
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
                else
                {
                    movement.Velocity = ctrl.VelocityEndValue;
                }
            }
        }

        // modify velocity directly
        // takes over acceleration during the lerp
        if (ctrl.VelocityIndex >= 0)
        {
            var inst = ctrl.VelocityInstructions.UnsafeAt(ctrl.VelocityIndex);
            int relativeTick = currentFrame - inst.TriggerFrame;
            if (relativeTick < inst.Duration)
            {
                // duration won't be zero here for it is handled in the while loop
                var t = (float)(relativeTick + 1) / inst.Duration;
                t = Easing.Evaluate(inst.EaseType, t);
                if (inst.Op == VelocityInstruction.Ops.SetAngle ||
                    inst.Op == VelocityInstruction.Ops.AddAngle)
                {
                    // lerp angle
                    float lerpedAngle = float.Lerp(ctrl.VelocityStartValue.X, ctrl.VelocityEndValue.X, t);
                    float magnitude = ctrl.VelocityStartValue.Y;

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
    }

    private static void UpdateAccelerationController(ref BulletController ctrl, ushort currentFrame, ref Movement movement)
    {
        var insts = ctrl.AccelerationInstructions;
        while (ctrl.AccelerationIndex < insts.Length - 1 &&
               currentFrame >= insts.UnsafeAt(ctrl.AccelerationIndex + 1).TriggerFrame)
        {
            ctrl.AccelerationIndex++;
            var inst = insts.UnsafeAt(ctrl.AccelerationIndex);

            switch (inst.Op)
            {
                case AccelerationInstruction.Ops.SetAcceleration:
                {
                    movement.Acceleration = inst.Params;    // newAcceleration
                    break;
                }
                case AccelerationInstruction.Ops.SetMagnitude:
                {
                    var direction = movement.Acceleration;
                    direction.Normalize();
                    movement.Acceleration = direction * inst.Params.X; // newMagnitude
                    break;
                }
                case AccelerationInstruction.Ops.SetAngle:
                {
                    var magnitude = movement.Acceleration.Length();
                    movement.Acceleration = new Vector2(
                        magnitude * MathF.Cos(inst.Params.X),   // newAngle
                        magnitude * MathF.Sin(inst.Params.X));  // newAngle
                    break;
                }
                case AccelerationInstruction.Ops.AddAcceleration:
                {
                    movement.Acceleration += inst.Params; // newAcceleration
                    break;
                }
                case AccelerationInstruction.Ops.AddMagnitude:
                {
                    var magnitude = movement.Acceleration.Length();
                    movement.Acceleration = movement.Acceleration / magnitude * (magnitude + inst.Params.X); // magnitudeDelta
                    break;
                }
                case AccelerationInstruction.Ops.AddAngle:
                {
                    var magnitude = movement.Acceleration.Length();
                    var angle = MathF.Atan2(movement.Acceleration.Y, movement.Acceleration.X) + inst.Params.X; // angleDelta
                    movement.Acceleration = new Vector2(
                        magnitude * MathF.Cos(angle),
                        magnitude * MathF.Sin(angle));
                    break;
                }
            }
        }
    }

    private static void UpdateCurveMovement(ref BulletController ctrl, ushort currentFrame, ref Movement movement)
    {
        var insts = ctrl.CurveInstructions;
        // handle instruction advance
        while (ctrl.CurveIndex < insts.Length - 1 &&
               currentFrame >= insts.UnsafeAt(ctrl.CurveIndex + 1).TriggerFrame)
        {
            ctrl.CurveIndex++;
        }

        var currentAngularVelocity = insts.UnsafeAt(ctrl.CurveIndex).AngularVelocity;

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
    }

    private static void UpdateSpawnAnimation(ref SpawnAnimation spawnAnim, ushort currentFrame, out float velocityMultiplier)
    {
        bool playingSpawnAnimation = currentFrame < spawnAnim.Duration;
        if (playingSpawnAnimation)
        {
            velocityMultiplier = spawnAnim.SpawningVelocityMultiplier;
            spawnAnim.Counter++;
            return;
        }

        velocityMultiplier = 1f;
    }

    private static void UpdateBasicBullet(Archetype archetype)
    {
        foreach (ref var chunk in archetype.GetChunksSpan())
        {
            chunk.GetFilledComponentSpan<Transform, Movement, Lifetime, SpawnAnimation>(
                out var transforms, out var movements, out var lifetimes, out var spawnAnims);

            for (int i = 0; i < chunk.EntityCount; i++)
            {
                ref var transform = ref transforms.UnsafeAt(i);
                ref var movement = ref movements.UnsafeAt(i);
                ref var lifetime = ref lifetimes.UnsafeAt(i);
                ref var spawnAnim = ref spawnAnims.UnsafeAt(i);

                var oldPosition = transform.Position;

                movement.Velocity += movement.Acceleration;

                UpdateSpawnAnimation(ref spawnAnim, lifetime.AliveFrames, out var velocityMultiplier);

                transform.Position += movement.Velocity * velocityMultiplier;

                var newPosition = transform.Position;
                var delta = newPosition - oldPosition;
                if (movement.SyncTransformRotation && delta.Length() >= float.Epsilon)
                    transform.Rotation = MathF.Atan2(delta.Y, delta.X);

                lifetime.AliveFrames++;
            }
        }
    }
}
