using Microsoft.Xna.Framework;

namespace ParaTH;

// handles everything movement related, updating entities' position
public sealed class MovementSystem(World world)
{
    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Movement, Lifetime>();

    public void Update()
    {
        var q = world.GetOrCreateQuery(descriptor);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            bool hasPos = archetype.Has<PositionController>();
            bool hasVel = archetype.Has<VelocityController>();
            bool hasAcc = archetype.Has<AccelerationController>();
            bool hasCur = archetype.Has<CurveController>();
            bool hasRen = archetype.Has<RenderState>();     // for syncing rotation
            bool hasSpw = archetype.Has<SpawnEffect>();  // this one has to stay here, spawnAnimation affects velocity
            bool hasCls = archetype.Has<CurvyLaser>();      // techically should have a separate system dedicated to this
            bool hasHrc = archetype.Has<Hierarchy>();       // if an entity has this, use its local position

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                chunk.GetFilledComponentSpan<Transform, Movement, Lifetime>(
                    out var transforms, out var movements, out var lifetimes);

                var posSpan = hasPos ? chunk.GetFilledComponentSpan<PositionController>() : default;
                var velSpan = hasVel ? chunk.GetFilledComponentSpan<VelocityController>() : default;
                var accSpan = hasAcc ? chunk.GetFilledComponentSpan<AccelerationController>() : default;
                var curSpan = hasCur ? chunk.GetFilledComponentSpan<CurveController>() : default;
                var renSpan = hasRen ? chunk.GetFilledComponentSpan<RenderState>() : default;
                var spwSpan = hasSpw ? chunk.GetFilledComponentSpan<SpawnEffect>() : default;
                var clsSpan = hasCls ? chunk.GetFilledComponentSpan<CurvyLaser>() : default;
                var hrcSpan = hasHrc ? chunk.GetFilledComponentSpan<Hierarchy>() : default;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    ref var transform = ref transforms.UnsafeAt(i);
                    ref var movement = ref movements.UnsafeAt(i);
                    ref var lifetime = ref lifetimes.UnsafeAt(i);

                    ref Vector2 position = ref (hasHrc ? ref hrcSpan.UnsafeAt(i).LocalPosition : ref transform.Position);

                    var currentFrame = lifetime.AliveFrames;
                    var oldPosition = position;

                    if (hasCls)
                        UpdateCurvyLaser(ref clsSpan.UnsafeAt(i), oldPosition);

                    if (hasAcc)
                        UpdateAccelerationController(ref accSpan.UnsafeAt(i), currentFrame, ref movement);

                    movement.Velocity += movement.Acceleration;

                    if (hasCur)
                        UpdateCurveMovement(ref curSpan.UnsafeAt(i), currentFrame, ref movement);

                    if (hasVel)
                        UpdateVelocityController(ref velSpan.UnsafeAt(i), currentFrame, ref movement);

                    if (hasPos)
                        UpdatePositionController(ref posSpan.UnsafeAt(i), currentFrame, ref position);

                    var velocityMultiplier = 1.0f;
                    if (hasSpw)
                    {
                        ref var spw = ref spwSpan.UnsafeAt(i);
                        velocityMultiplier = spw.IsPlaying ? (float)spw.VelocityMultiplier : 1.0f;
                    }

                    position += movement.Velocity * velocityMultiplier;

                    var newPosition = position;
                    var delta = newPosition - oldPosition;

                    var angle = 0f;
                    var velocityNotZero = delta.LengthSquared() >= float.Epsilon;
                    if ((movement.SyncTransformRotation || movement.SyncRenderStateRotation) && velocityNotZero)
                        angle = MathF.Atan2(delta.Y, delta.X);
                    if (movement.SyncTransformRotation && velocityNotZero)
                        transform.Rotation = angle;
                    if (movement.SyncRenderStateRotation && velocityNotZero)
                    {
                        ref var ren = ref renSpan.UnsafeAt(i);
                        ren.Rotation = angle;
                    }

                    lifetime.AliveFrames++;
                }
            }
        }
    }

    private static void UpdatePositionController(ref PositionController ctrl, ushort currentFrame, ref Vector2 position)
    {
        // handle instruction advance
        var insts = ctrl.Instructions;
        while (ctrl.Index < insts.Length - 1 && currentFrame >= insts.UnsafeAt(ctrl.Index + 1).TriggerFrame)
        {
            // reached new instruction, init it
            ctrl.Index++;
            var inst = insts.UnsafeAt(ctrl.Index);

            switch (inst.Op)
            {
                case PositionInstruction.Ops.Set:
                    ctrl.StartValue = position;
                    ctrl.EndValue = inst.Params;
                    break;
                case PositionInstruction.Ops.Add:
                    ctrl.StartValue = position;
                    ctrl.EndValue = position + inst.Params; // positionDelta
                    break;
            }

            // instruction takes 0 frame
            // update position immediately to ensure the next op can get the correct value
            if (inst.Duration == 0)
                position = ctrl.EndValue;
        }

        // modify position directly
        // takes over velocity during the lerp
        if (ctrl.Index >= 0)
        {
            var inst = insts.UnsafeAt(ctrl.Index);
            int relativeTick = currentFrame - inst.TriggerFrame;
            if (relativeTick < inst.Duration)
            {
                var t = (float)(relativeTick + 1) / inst.Duration;
                t = Easing.Evaluate(inst.EaseType, t);
                position = Vector2.Lerp(ctrl.StartValue, ctrl.EndValue, t);
            }
        }
    }

    private static void UpdateVelocityController(ref VelocityController ctrl, ushort currentFrame, ref Movement movement)
    {
        // handle instruction advance
        var insts = ctrl.Instructions;
        while (ctrl.Index < insts.Length - 1 && currentFrame >= insts.UnsafeAt(ctrl.Index + 1).TriggerFrame)
        {
            // reached new instruction, init it
            ctrl.Index++;
            var inst = insts.UnsafeAt(ctrl.Index);
            var op = inst.Op;

            // handle ops
            switch (op)
            {
                case VelocityInstruction.Ops.SetVelocity:
                    ctrl.StartValue = movement.Velocity;
                    ctrl.EndValue = inst.Params; // newVelocity
                    break;
                case VelocityInstruction.Ops.SetMagnitude:
                    var direction = movement.Velocity;
                    direction.Normalize(); // if you get NaN here its your fault
                    ctrl.StartValue = movement.Velocity;
                    ctrl.EndValue = direction * inst.Params.X; // newMagnitude
                    break;
                case VelocityInstruction.Ops.SetAngle:
                    // ctrl.StartValue: X = startAngle, Y = currentMagnitude
                    // ctrl.EndValue: X = endAngle (calculated to use nearest lerp)
                    var currentMagnitude = movement.Velocity.Length();
                    var startAngle = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
                    var endAngle = inst.Params.X; // newAngle
                    float angleDelta = MathHelper.WrapAngle(endAngle - startAngle);
                    ctrl.StartValue = new Vector2(startAngle, currentMagnitude);
                    ctrl.EndValue.X = startAngle + angleDelta;
                    break;
                case VelocityInstruction.Ops.AddVelocity:
                    ctrl.StartValue = movement.Velocity;
                    ctrl.EndValue = movement.Velocity + inst.Params; // velocityDelta
                    break;
                case VelocityInstruction.Ops.AddMagnitude:
                    var magnitude = movement.Velocity.Length();
                    ctrl.StartValue = movement.Velocity;
                    ctrl.EndValue = movement.Velocity / magnitude * (magnitude + inst.Params.X); // magnitudeDelta
                    break;
                case VelocityInstruction.Ops.AddAngle:
                    // ctrl.StartValue: X = startAngle, Y = currentMagnitude
                    // ctrl.EndValue: X = endAngle (use warp lerp)
                    var currentMag = movement.Velocity.Length();
                    var baseAngle = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
                    ctrl.StartValue = new Vector2(baseAngle, currentMag);
                    ctrl.EndValue.X = baseAngle + inst.Params.X; // angleDelta
                    break;
            }

            // instruction takes 0 frame
            // update the Velocity immediately to ensure the next op can get the correct value
            if (inst.Duration == 0)
            {
                if (op == VelocityInstruction.Ops.SetAngle || op == VelocityInstruction.Ops.AddAngle)
                {
                    float mag = ctrl.StartValue.Y;
                    movement.Velocity = new Vector2(mag * MathF.Cos(ctrl.EndValue.X), mag * MathF.Sin(ctrl.EndValue.X));
                }
                else
                {
                    movement.Velocity = ctrl.EndValue;
                }
            }
        }

        // modify velocity directly
        // takes over acceleration during the lerp
        if (ctrl.Index >= 0)
        {
            var inst = insts.UnsafeAt(ctrl.Index);
            int relativeTick = currentFrame - inst.TriggerFrame;
            if (relativeTick < inst.Duration)
            {
                // duration won't be zero here for it is handled in the while loop
                var t = (float)(relativeTick + 1) / inst.Duration;
                t = Easing.Evaluate(inst.EaseType, t);
                if (inst.Op == VelocityInstruction.Ops.SetAngle || inst.Op == VelocityInstruction.Ops.AddAngle)
                {
                    // lerp angle
                    float lerpedAngle = float.Lerp(ctrl.StartValue.X, ctrl.EndValue.X, t);
                    float magnitude = ctrl.StartValue.Y;
                    movement.Velocity = new Vector2(magnitude * MathF.Cos(lerpedAngle), magnitude * MathF.Sin(lerpedAngle));
                }
                else // vector lerp logic
                {
                    movement.Velocity = Vector2.Lerp(ctrl.StartValue, ctrl.EndValue, t);
                }
            }
        }
    }

    private static void UpdateAccelerationController(ref AccelerationController ctrl, ushort currentFrame, ref Movement movement)
    {
        var insts = ctrl.Instructions;
        while (ctrl.Index < insts.Length - 1 && currentFrame >= insts.UnsafeAt(ctrl.Index + 1).TriggerFrame)
        {
            ctrl.Index++;
            var inst = insts.UnsafeAt(ctrl.Index);

            switch (inst.Op)
            {
                case AccelerationInstruction.Ops.SetAcceleration:
                    movement.Acceleration = inst.Params; // newAcceleration
                    break;
                case AccelerationInstruction.Ops.SetMagnitude:
                    var direction = movement.Acceleration;
                    direction.Normalize();
                    movement.Acceleration = direction * inst.Params.X; // newMagnitude
                    break;
                case AccelerationInstruction.Ops.SetAngle:
                    var magnitude = movement.Acceleration.Length();
                    movement.Acceleration = new Vector2(magnitude * MathF.Cos(inst.Params.X), magnitude * MathF.Sin(inst.Params.X)); // newAngle
                    break;
                case AccelerationInstruction.Ops.AddAcceleration:
                    movement.Acceleration += inst.Params; // newAcceleration
                    break;
                case AccelerationInstruction.Ops.AddMagnitude:
                    var mag2 = movement.Acceleration.Length();
                    movement.Acceleration = movement.Acceleration / mag2 * (mag2 + inst.Params.X); // magnitudeDelta
                    break;
                case AccelerationInstruction.Ops.AddAngle:
                    var mag3 = movement.Acceleration.Length();
                    var angle = MathF.Atan2(movement.Acceleration.Y, movement.Acceleration.X) + inst.Params.X; // angleDelta
                    movement.Acceleration = new Vector2(mag3 * MathF.Cos(angle), mag3 * MathF.Sin(angle));
                    break;
            }
        }
    }

    private static void UpdateCurveMovement(ref CurveController ctrl, ushort currentFrame, ref Movement movement)
    {
        var insts = ctrl.Instructions;
        // handle instruction advance
        while (ctrl.Index < insts.Length - 1 && currentFrame >= insts.UnsafeAt(ctrl.Index + 1).TriggerFrame)
        {
            ctrl.Index++;
            var inst = insts.UnsafeAt(ctrl.Index);
            switch (inst.Op)
            {
                case CurveInstruction.Ops.Set:
                    ctrl.CurrentAngularVelocity = inst.AngularVelocity;
                    break;
                case CurveInstruction.Ops.Add:
                    ctrl.CurrentAngularVelocity += inst.AngularVelocity;
                    break;
            }
        }

        if (ctrl.Index >= 0)
        {
            var currentAngularVelocity = ctrl.CurrentAngularVelocity;
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
    }

    private static void UpdateCurvyLaser(ref CurvyLaser curvyLaser, Vector2 currentPos)
    {
        var nodes = curvyLaser.LaserNodes;
        curvyLaser.LaserNodes.Enqueue(currentPos);

        while (nodes.Count > curvyLaser.Length)
            nodes.Dequeue();
    }
}
