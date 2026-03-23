using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;

namespace ParaTH;

// todo: group spawning in spread/circle/fan etc
public ref struct BulletBuilder(BulletManager bulletManager)
{
    private readonly BulletManager manager = bulletManager;

    private Transform transform = new(Vector2.Zero, Vector2.One, 0);
    private Movement movement;
    private SpriteRenderer spriteRenderer;
    private BulletController controller;

    private ushort currentFrame = 0;
    private readonly List<PositionInstruction> positionInstructions = [];
    private readonly List<VelocityInstruction> velocityInstructions = [];
    private readonly List<AccelerationInstruction> accelerationInstructions = [];
    private readonly List<CurveInstruction> curveInstructions = [];

    [UnscopedRef]
    public ref BulletBuilder Delay(ushort frames)
    {
        currentFrame += frames;
        return ref this;
    }

    #region Basic Movement
    // todo: make this better
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
    public ref BulletBuilder SetPosition(Vector2 newPosition)
    {
        if (currentFrame == 0)
        {
            transform.Position = newPosition;
            return ref this;
        }

        positionInstructions.Add(new(currentFrame,
            newPosition, 0, EaseType.Linear, PositionInstruction.Ops.Set));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddPosition(Vector2 positionDelta)
    {
        if (currentFrame == 0)
        {
            transform.Position += positionDelta;
            return ref this;
        }

        positionInstructions.Add(new(currentFrame,
            positionDelta, 0, EaseType.Linear, PositionInstruction.Ops.Add));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpToPosition(Vector2 newPosition, ushort duration, EaseType easeType)
    {
        positionInstructions.Add(new(currentFrame,
            newPosition, duration, easeType, PositionInstruction.Ops.Set));
        currentFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpAddPosition(Vector2 poisitionDelta, ushort duration, EaseType easeType)
    {
        positionInstructions.Add(new(currentFrame,
            poisitionDelta, duration, easeType, PositionInstruction.Ops.Add));
        currentFrame += duration;
        return ref this;
    }
    #endregion

    #region Velocity Control
    // todo: add Set/LerpVelocityAngleToPlayer
    [UnscopedRef]
    public ref BulletBuilder SetVelocity(Vector2 newVelocity)
    {
        if (currentFrame == 0)
        {
            movement.Velocity = newVelocity;
            return ref this;
        }

        velocityInstructions.Add(new(currentFrame,
            newVelocity, 0, EaseType.Linear, VelocityInstruction.Ops.SetVelocity));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocityMagnitude(float newMagnitude)
    {
        if (currentFrame == 0)
        {
            movement.Velocity.Normalize();
            movement.Velocity *= newMagnitude;
            return ref this;
        }

        velocityInstructions.Add(new(currentFrame,
            new Vector2(newMagnitude, 0), 0, EaseType.Linear, VelocityInstruction.Ops.SetMagnitude));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocityAngle(float newAngle)
    {
        if (currentFrame == 0)
        {
            var magnitude = movement.Velocity.Length();
            movement.Velocity = new Vector2(
                magnitude * MathF.Cos(newAngle),
                magnitude * MathF.Sin(newAngle));
            return ref this;
        }

        velocityInstructions.Add(new(currentFrame,
            new Vector2(newAngle, 0), 0, EaseType.Linear, VelocityInstruction.Ops.SetAngle));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddVelocity(Vector2 velocityDelta)
    {
        if (currentFrame == 0)
        {
            movement.Velocity += velocityDelta;
            return ref this;
        }

        velocityInstructions.Add(new(currentFrame,
            velocityDelta, 0, EaseType.Linear, VelocityInstruction.Ops.AddVelocity));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddVelocityMagnitude(float magnitudeDelta)
    {
        if (currentFrame == 0)
        {
            var magnitude = movement.Velocity.Length();
            movement.Velocity /= magnitude;
            movement.Velocity *= (magnitude + magnitudeDelta);
            return ref this;
        }

        velocityInstructions.Add(new(currentFrame,
            new Vector2(magnitudeDelta, 0), 0, EaseType.Linear, VelocityInstruction.Ops.AddMagnitude));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddVelocityAngle(float angleDelta)
    {
        if (currentFrame == 0)
        {
            var magnitude = movement.Velocity.Length();
            var angle = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X);
            movement.Velocity = new Vector2(
                magnitude * MathF.Cos(angle + angleDelta),
                magnitude * MathF.Sin(angle + angleDelta));
            return ref this;
        }

        velocityInstructions.Add(new(currentFrame,
            new Vector2(angleDelta, 0), 0, EaseType.Linear, VelocityInstruction.Ops.AddAngle));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpToVelocity(Vector2 newVelocity, ushort duration, EaseType easeType)
    {
        velocityInstructions.Add(new(currentFrame,
            newVelocity, duration, easeType, VelocityInstruction.Ops.SetVelocity));
        currentFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpToVelocityMagnitude(float newMagnitude, ushort duration, EaseType easeType)
    {
        velocityInstructions.Add(new(currentFrame,
            new Vector2(newMagnitude, 0), duration, easeType, VelocityInstruction.Ops.SetMagnitude));
        currentFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpToVelocityAngle(float newAngle, ushort duration, EaseType easeType)
    {
        velocityInstructions.Add(new(currentFrame,
            new Vector2(newAngle, 0), duration, easeType, VelocityInstruction.Ops.SetAngle));
        currentFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpAddVelocity(Vector2 velocityDelta, ushort duration, EaseType easeType)
    {
        velocityInstructions.Add(new(currentFrame,
            velocityDelta, duration, easeType, VelocityInstruction.Ops.AddVelocity));
        currentFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpAddVelocityMagnitude(float magnitudeDelta, ushort duration, EaseType easeType)
    {
        velocityInstructions.Add(new(currentFrame,
            new Vector2(magnitudeDelta, 0), duration, easeType, VelocityInstruction.Ops.AddMagnitude));
        currentFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpAddVelocityAngle(float angleDelta, ushort duration, EaseType easeType)
    {
        velocityInstructions.Add(new(currentFrame,
            new Vector2(angleDelta, 0), duration, easeType, VelocityInstruction.Ops.AddAngle));
        currentFrame += duration;
        return ref this;
    }
    #endregion

    #region Acceleration Control
    // todo: add Set/LerpAccelerationAngleToPlayer
    [UnscopedRef]
    public ref BulletBuilder SetAcceleration(Vector2 newAcceleration)
    {
        if (currentFrame == 0)
        {
            movement.Acceleration = newAcceleration;
            return ref this;
        }

        accelerationInstructions.Add(new(currentFrame,
            newAcceleration, AccelerationInstruction.Ops.SetAcceleration));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetAccelerationMagnitude(float newMagnitude)
    {
        if (currentFrame == 0)
        {
            movement.Acceleration.Normalize();
            movement.Acceleration *= newMagnitude;
            return ref this;
        }

        accelerationInstructions.Add(new(currentFrame,
            new Vector2(newMagnitude, 0), AccelerationInstruction.Ops.SetMagnitude));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetAccelerationAngle(float newAngle)
    {
        if (currentFrame == 0)
        {
            var magnitude = movement.Acceleration.Length();
            movement.Acceleration = new Vector2(
                magnitude * MathF.Cos(newAngle),
                magnitude * MathF.Sin(newAngle));
            return ref this;
        }

        accelerationInstructions.Add(new(currentFrame,
            new Vector2(newAngle, 0), AccelerationInstruction.Ops.SetAngle));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddAcceleration(Vector2 accelerationDelta)
    {
        if (currentFrame == 0)
        {
            movement.Acceleration += accelerationDelta;
            return ref this;
        }

        accelerationInstructions.Add(new(currentFrame,
            accelerationDelta, AccelerationInstruction.Ops.AddAcceleration));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddAccelerationMagnitude(float magnitudeDelta)
    {
        if (currentFrame == 0)
        {
            var magnitude = movement.Acceleration.Length();
            movement.Acceleration /= magnitude;
            movement.Acceleration *= (magnitude + magnitudeDelta);
            return ref this;
        }

        accelerationInstructions.Add(new(currentFrame,
            new Vector2(magnitudeDelta, 0), AccelerationInstruction.Ops.AddMagnitude));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddAccelerationAngle(float angleDelta)
    {
        if (currentFrame == 0)
        {
            var magnitude = movement.Acceleration.Length();
            var angle = MathF.Atan2(movement.Acceleration.Y, movement.Acceleration.X);
            movement.Acceleration = new Vector2(
                magnitude * MathF.Cos(angle + angleDelta),
                magnitude * MathF.Sin(angle + angleDelta));
            return ref this;
        }

        accelerationInstructions.Add(new(currentFrame,
            new Vector2(angleDelta, 0), AccelerationInstruction.Ops.AddAngle));
        return ref this;
    }
    #endregion

    #region Angular Velocity
    [UnscopedRef]
    public ref BulletBuilder SetAngularVelocity(float angularVelocity)
    {
        curveInstructions.Add(new(currentFrame, angularVelocity));
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
            var posInsts = InitializeInstructions(positionInstructions);
            var velInsts = InitializeInstructions(velocityInstructions);
            var accelInsts = InitializeInstructions(accelerationInstructions);
            var curveInsts = InitializeInstructions(curveInstructions);

            // index is -1 so the first instruction can get inited
            controller.PositionInstructions = posInsts;
            controller.PositionIndex = -1;
            controller.VelocityInstructions = velInsts;
            controller.VelocityIndex = -1;
            controller.AccelerationInstructions = accelInsts;
            controller.AccelerationIndex = -1;
            controller.CurveInstructions = curveInsts;
            controller.CurveIndex = -1;

            return manager.World.CreateEntity(transform, movement, spriteRenderer, controller);
        }

        return manager.World.CreateEntity(transform, movement, spriteRenderer);
    }

    private static T[] InitializeInstructions<T>(List<T> instructions)
    {
        return [.. instructions];
    }
}
