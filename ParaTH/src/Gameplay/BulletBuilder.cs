using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using static ParaTH.AnimationAsset;

namespace ParaTH;

public ref struct BulletBuilder(BulletManager bulletManager)
{
    private readonly BulletManager manager = bulletManager;

    private Transform transform = new(Vector2.Zero, Vector2.One, 0);
    private Movement movement;
    private SpriteRenderer spriteRenderer;
    private BulletController controller;

    private ushort currentPositionFrame = 0;
    private readonly List<PositionInstruction> positionInstructions = [];

    private ushort currentVelocityFrame = 0;
    private readonly List<VelocityInstruction> velocityInstructions = [];

    private ushort currentAccelerationFrame = 0;
    private readonly List<AccelerationInstruction> accelerationInstructions = [];

    private ushort currentCurveFrame = 0;
    private readonly List<CurveInstruction> curveInstructions = [];

    [UnscopedRef]
    public ref BulletBuilder DelayAll(ushort frames)
    {
        // expand
        currentPositionFrame += frames;
        currentVelocityFrame += frames;
        currentAccelerationFrame += frames;
        currentCurveFrame += frames;
        return ref this;
    }

    #region Basic Movement
    [UnscopedRef]
    public ref BulletBuilder SetMovement(Vector2 velocity, Vector2 acceleration)
    {
        movement.Velocity = velocity;
        movement.Acceleration = acceleration;

        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetMovement(float velocity, Vector2 acceleration, float angle)
    {
        var velocityVector = new Vector2(
            velocity * MathF.Cos(angle),
            velocity * MathF.Sin(angle));

        movement.Velocity = velocityVector;
        movement.Acceleration = acceleration;

        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetMovement(float velocity, float acceleration, float angle)
    {
        var cos = MathF.Cos(angle);
        var sin = MathF.Sin(angle);

        var velocityVector = new Vector2(
            velocity * cos,
            velocity * sin);
        var accelerationVector = new Vector2(
            acceleration * cos,
            acceleration * sin);

        movement.Velocity = velocityVector;
        movement.Acceleration = accelerationVector;

        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AutoSyncTransformRotation()
    {
        movement.SyncTransformRotation = true;
        return ref this;
    }
    #endregion

    #region Position Control
    [UnscopedRef]
    public ref BulletBuilder DelayPosition(ushort frames)
    {
        positionInstructions.Add(new(currentPositionFrame,
            Vector2.Zero, frames, Easing.Linear, PositionInstruction.Ops.Delay));
        currentPositionFrame += frames;
        return ref this;
    }
    [UnscopedRef]
    public ref BulletBuilder SetPosition(Vector2 newPosition)
    {
        if (currentPositionFrame == 0)
        {
            transform.Position = newPosition;
            return ref this;
        }

        positionInstructions.Add(new(currentPositionFrame,
            newPosition, 0, Easing.Linear, PositionInstruction.Ops.Set));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddPosition(Vector2 positionDelta)
    {
        if (currentPositionFrame == 0)
        {
            transform.Position += positionDelta;
            return ref this;
        }

        positionInstructions.Add(new(currentPositionFrame,
            positionDelta, 0, Easing.Linear, PositionInstruction.Ops.Add));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpToPosition(Vector2 newPosition, ushort duration, EasingFunction easing)
    {
        positionInstructions.Add(new(currentPositionFrame,
            newPosition, duration, easing, PositionInstruction.Ops.Set));
        currentPositionFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpAddPosition(Vector2 poisitionDelta, ushort duration, EasingFunction easing)
    {
        positionInstructions.Add(new(currentPositionFrame,
            poisitionDelta, duration, easing, PositionInstruction.Ops.Add));
        currentPositionFrame += duration;
        return ref this;
    }
    #endregion

    #region Velocity Control
    // todo: add Set/LerpVelocityAngleToPlayer
    [UnscopedRef]
    public ref BulletBuilder DelayVelocity(ushort frames)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            Vector2.Zero, frames, Easing.Linear, VelocityInstruction.Ops.Delay));
        currentVelocityFrame += frames;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocity(Vector2 newVelocity)
    {
        if (currentVelocityFrame == 0)
        {
            movement.Velocity = newVelocity;
            return ref this;
        }

        velocityInstructions.Add(new(currentVelocityFrame,
            newVelocity, 0, Easing.Linear, VelocityInstruction.Ops.SetVelocity));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocityMagnitude(float newMagnitude)
    {
        if (currentVelocityFrame == 0)
        {
            movement.Velocity.Normalize();
            movement.Velocity *= newMagnitude;
            return ref this;
        }

        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(newMagnitude, 0), 0, Easing.Linear, VelocityInstruction.Ops.SetMagnitude));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocityAngle(float newAngle)
    {
        if (currentVelocityFrame == 0)
        {
            var magnitude = movement.Velocity.Length();
            movement.Velocity = new Vector2(
                magnitude * MathF.Cos(newAngle),
                magnitude * MathF.Sin(newAngle));
            return ref this;
        }

        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(newAngle, 0), 0, Easing.Linear, VelocityInstruction.Ops.SetAngle));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddVelocity(Vector2 velocityDelta)
    {
        if (currentVelocityFrame == 0)
        {
            movement.Velocity += velocityDelta;
            return ref this;
        }

        velocityInstructions.Add(new(currentVelocityFrame,
            velocityDelta, 0, Easing.Linear, VelocityInstruction.Ops.AddVelocity));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddVelocityMagnitude(float magnitudeDelta)
    {
        if (currentVelocityFrame == 0)
        {
            var magnitude = movement.Velocity.Length();
            movement.Velocity /= magnitude;
            movement.Velocity *= (magnitude + magnitudeDelta);
            return ref this;
        }

        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(magnitudeDelta, 0), 0, Easing.Linear, VelocityInstruction.Ops.AddMagnitude));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddVelocityAngle(float angleDelta)
    {
        if (currentVelocityFrame == 0)
        {
            var magnitude = movement.Velocity.Length();
            var angle = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
            movement.Velocity = new Vector2(
                magnitude * MathF.Cos(angle + angleDelta),
                magnitude * MathF.Sin(angle + angleDelta));
            return ref this;
        }

        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(angleDelta, 0), 0, Easing.Linear, VelocityInstruction.Ops.AddAngle));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpToVelocity(Vector2 newVelocity, ushort duration, EasingFunction easing)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            newVelocity, duration, easing, VelocityInstruction.Ops.SetVelocity));
        currentVelocityFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpToVelocityMagnitude(float newMagnitude, ushort duration, EasingFunction easing)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(newMagnitude, 0), duration, easing, VelocityInstruction.Ops.SetMagnitude));
        currentVelocityFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpToVelocityAngle(float newAngle, ushort duration, EasingFunction easing)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(newAngle, 0), duration, easing, VelocityInstruction.Ops.SetAngle));
        currentVelocityFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpAddVelocity(Vector2 velocityDelta, ushort duration, EasingFunction easing)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            velocityDelta, duration, easing, VelocityInstruction.Ops.AddVelocity));
        currentVelocityFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpAddVelocityMagnitude(float magnitudeDelta, ushort duration, EasingFunction easing)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(magnitudeDelta, 0), duration, easing, VelocityInstruction.Ops.AddMagnitude));
        currentVelocityFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpAddVelocityAngle(float angleDelta, ushort duration, EasingFunction easing)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(angleDelta, 0), duration, easing, VelocityInstruction.Ops.AddAngle));
        currentVelocityFrame += duration;
        return ref this;
    }
    #endregion

    #region Acceleration Control
    // todo: add Set/LerpAccelerationAngleToPlayer
    [UnscopedRef]
    public ref BulletBuilder DelayAcceleration(ushort frames)
    {
        accelerationInstructions.Add(new(currentAccelerationFrame,
            Vector2.Zero, frames, Easing.Linear, AccelerationInstruction.Ops.Delay));
        currentAccelerationFrame += frames;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetAcceleration(Vector2 newAcceleration)
    {
        if (currentAccelerationFrame == 0)
        {
            movement.Acceleration = newAcceleration;
            return ref this;
        }

        accelerationInstructions.Add(new(currentAccelerationFrame,
            newAcceleration, 0, Easing.Linear, AccelerationInstruction.Ops.SetAcceleration));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetAccelerationMagnitude(float newMagnitude)
    {
        if (currentAccelerationFrame == 0)
        {
            movement.Acceleration.Normalize();
            movement.Acceleration *= newMagnitude;
            return ref this;
        }

        accelerationInstructions.Add(new(currentAccelerationFrame,
            new Vector2(newMagnitude, 0), 0, Easing.Linear, AccelerationInstruction.Ops.SetMagnitude));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetAccelerationAngle(float newAngle)
    {
        if (currentAccelerationFrame == 0)
        {
            var magnitude = movement.Acceleration.Length();
            movement.Acceleration = new Vector2(
                magnitude * MathF.Cos(newAngle),
                magnitude * MathF.Sin(newAngle));
            return ref this;
        }

        accelerationInstructions.Add(new(currentAccelerationFrame,
            new Vector2(newAngle, 0), 0, Easing.Linear, AccelerationInstruction.Ops.SetAngle));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddAcceleration(Vector2 accelerationDelta)
    {
        if (currentAccelerationFrame == 0)
        {
            movement.Acceleration += accelerationDelta;
            return ref this;
        }

        accelerationInstructions.Add(new(currentAccelerationFrame,
            accelerationDelta, 0, Easing.Linear, AccelerationInstruction.Ops.AddAcceleration));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddAccelerationMagnitude(float magnitudeDelta)
    {
        if (currentAccelerationFrame == 0)
        {
            var magnitude = movement.Acceleration.Length();
            movement.Acceleration /= magnitude;
            movement.Acceleration *= (magnitude + magnitudeDelta);
            return ref this;
        }

        accelerationInstructions.Add(new(currentAccelerationFrame,
            new Vector2(magnitudeDelta, 0), 0, Easing.Linear, AccelerationInstruction.Ops.AddMagnitude));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddAccelerationAngle(float angleDelta)
    {
        if (currentAccelerationFrame == 0)
        {
            var magnitude = movement.Acceleration.Length();
            var angle = MathF.Atan2(movement.Acceleration.Y, movement.Acceleration.X);
            movement.Acceleration = new Vector2(
                magnitude * MathF.Cos(angle + angleDelta),
                magnitude * MathF.Sin(angle + angleDelta));
            return ref this;
        }

        accelerationInstructions.Add(new(currentAccelerationFrame,
            new Vector2(angleDelta, 0), 0, Easing.Linear, AccelerationInstruction.Ops.AddAngle));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpToAcceleration(Vector2 newAcceleration, ushort duration, EasingFunction easing)
    {
        accelerationInstructions.Add(new(currentAccelerationFrame,
            newAcceleration, duration, easing, AccelerationInstruction.Ops.SetAcceleration));
        currentAccelerationFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpToAccelerationMagnitude(float newMagnitude, ushort duration, EasingFunction easing)
    {
        accelerationInstructions.Add(new(currentAccelerationFrame,
            new Vector2(newMagnitude, 0), duration, easing, AccelerationInstruction.Ops.SetMagnitude));
        currentAccelerationFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpToAccelerationAngle(float newAngle, ushort duration, EasingFunction easing)
    {
        accelerationInstructions.Add(new(currentAccelerationFrame,
            new Vector2(newAngle, 0), duration, easing, AccelerationInstruction.Ops.SetAngle));
        currentAccelerationFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpAddAcceleration(Vector2 accelerationDelta, ushort duration, EasingFunction easing)
    {
        accelerationInstructions.Add(new(currentAccelerationFrame,
            accelerationDelta, duration, easing, AccelerationInstruction.Ops.AddAcceleration));
        currentAccelerationFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpAddAccelerationMagnitude(float magnitudeDelta, ushort duration, EasingFunction easing)
    {
        accelerationInstructions.Add(new(currentAccelerationFrame,
            new Vector2(magnitudeDelta, 0), duration, easing, AccelerationInstruction.Ops.AddMagnitude));
        currentAccelerationFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpAddAccelerationAngle(float angleDelta, ushort duration, EasingFunction easing)
    {
        accelerationInstructions.Add(new(currentAccelerationFrame,
            new Vector2(angleDelta, 0), duration, easing, AccelerationInstruction.Ops.AddAngle));
        currentAccelerationFrame += duration;
        return ref this;
    }
    #endregion

    #region Angular Velocity
    [UnscopedRef]
    public ref BulletBuilder DelayAngularVelocity(ushort frames)
    {
        currentCurveFrame += frames;
        return ref this;
    }
    [UnscopedRef]
    public ref BulletBuilder SetAngularVelocity(float angularVelocity)
    {
        curveInstructions.Add(new(currentCurveFrame, angularVelocity, -1, 0));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AngularVelocityLoopFrom(sbyte index, byte repeatTimes)
    {
        curveInstructions.Add(new(currentCurveFrame, 0, index, repeatTimes));
        return ref this;
    }
    #endregion

    #region Visual WIP
    [UnscopedRef]
    public ref BulletBuilder SetSprite(string spriteName, Color color, byte layer, StgBlendState blendState)
    {
        var sprite = manager.AssetManager.Get<SpriteAsset>(spriteName);

        spriteRenderer.Sprite = sprite;
        spriteRenderer.Color = color;
        spriteRenderer.Layer = layer;
        spriteRenderer.BlendState = blendState;

        return ref this;
    }
    #endregion

    [UnscopedRef]
    public Entity Build()
    {
        bool needController = positionInstructions.Count > 0 ||
                              velocityInstructions.Count > 0 ||
                              accelerationInstructions.Count > 0 ||
                              curveInstructions.Count > 0;

        if (needController)
        {
            controller.PositionInstructions = InitializeInstructionsWith(positionInstructions);
            controller.PositionIndex = -1; // so that the first instruction can get inited
            controller.VelocityInstructions = InitializeInstructionsWith(velocityInstructions);
            controller.VelocityIndex = -1; // so that the first instruction can get inited
            controller.AccelerationInstructions = InitializeInstructionsWith(accelerationInstructions);
            controller.AccelerationIndex = -1; // so that the first instruction can get inited
            controller.CurveInstructions = InitializeInstructionsWith(curveInstructions);

            return manager.World.CreateEntity(transform, movement, spriteRenderer, controller);
        }

        return manager.World.CreateEntity(transform, movement, spriteRenderer);
    }

    private static T[] InitializeInstructionsWith<T>(List<T> instructions)
    {
        return instructions.Count > 0 ? [.. instructions] : null!;
    }
}
