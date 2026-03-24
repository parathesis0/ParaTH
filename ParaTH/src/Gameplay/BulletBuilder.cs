using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;

namespace ParaTH;

// todos:
// group spawning in spread/circle/fan etc
// collision, managed despawning
// reusing built instructions
public ref struct BulletBuilder(BulletManager bulletManager)
{
    private readonly BulletManager manager = bulletManager;

    private Transform transform = new(Vector2.Zero, Vector2.One, 0);
    private Movement movement;
    private SpriteRenderer spriteRenderer;
    private Lifetime lifetime;
    private SpawnAnimation spawnAnimation;

    private ushort currentFrame = 0;
    private readonly List<PositionInstruction> positionInstructions = [];
    private readonly List<VelocityInstruction> velocityInstructions = [];
    private readonly List<AccelerationInstruction> accelerationInstructions = [];
    private readonly List<CurveInstruction> curveInstructions = [];
    private float currentAngularVelocity = 0f;

    [UnscopedRef]
    public ref BulletBuilder Delay(ushort frames)
    {
        currentFrame += frames;
        return ref this;
    }

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
    // convert from magnitude angle to vector for conveniences sake
    [UnscopedRef]
    public ref BulletBuilder SetVelocity(float velocityMagnitude, float angle)
    {
        var velocityVector = new Vector2(
            velocityMagnitude * MathF.Cos(angle),
            velocityMagnitude * MathF.Sin(angle));
        return ref SetVelocity(velocityVector);
    }

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
            movement.Velocity = movement.Velocity / magnitude * (magnitude + magnitudeDelta);
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
    // convert from magnitude angle to vector for conveniences sake
    [UnscopedRef]
    public ref BulletBuilder SetAcceleration(float accelerationMagnitude, float angle)
    {
        var accelerationVector = new Vector2(
            accelerationMagnitude * MathF.Cos(angle),
            accelerationMagnitude * MathF.Sin(angle));
        return ref SetAcceleration(accelerationVector);
    }
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

    #region Movement
    // ways to set velocity and acceleration at once to save effort
    [UnscopedRef]
    public ref BulletBuilder SetMovement(Vector2 velocity, Vector2 acceleration)
    {
        SetVelocity(velocity);
        SetAcceleration(acceleration);
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetMovement(float velocityMagnitude, float angle, Vector2 acceleration)
    {
        SetVelocity(velocityMagnitude, angle);
        SetAcceleration(acceleration);
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetMovement(float velocityMagnitude, float angle, float accelerationMagnitude)
    {
        var cos = MathF.Cos(angle);
        var sin = MathF.Sin(angle);
        var velocityVector = new Vector2(
            velocityMagnitude * cos,
            velocityMagnitude * sin);
        var accelerationVector = new Vector2(
            accelerationMagnitude * cos,
            accelerationMagnitude * sin);
        SetVelocity(velocityVector);
        SetAcceleration(accelerationVector);
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddMovementAngle(float angleDelta)
    {
        AddVelocityAngle(angleDelta);
        AddAccelerationAngle(angleDelta);
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AutoSyncTransformRotation()
    {
        movement.SyncTransformRotation = true;
        return ref this;
    }
    #endregion

    #region Angular Velocity
    // not compatable with VelocityAngle controls
    [UnscopedRef]
    public ref BulletBuilder SetAngularVelocity(float newAngularVelocity)
    {
        currentAngularVelocity = newAngularVelocity;
        curveInstructions.Add(new(currentFrame, newAngularVelocity));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddAngularVelocity(float angularVelocityDelta)
    {
        currentAngularVelocity += angularVelocityDelta;
        curveInstructions.Add(new(currentFrame, currentAngularVelocity));
        return ref this;
    }
    #endregion

    #region Lifetime
    [UnscopedRef]
    public ref BulletBuilder SetOffscreenLifeTime(short frames)
    {
        lifetime.OffscreenFramesToLive = frames;
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

    [UnscopedRef]
    public ref BulletBuilder SetSpawnAnimation(string spriteName, float startScaleMultiplier, float startAlphaMultiplier,
                                               float spawningVelocityMultiplier, byte duration, EaseType easeType)
    {
        var sprite = manager.AssetManager.Get<SpriteAsset>(spriteName);

        spawnAnimation.Sprite = sprite;
        spawnAnimation.StartScaleMultiplier = (Half)startScaleMultiplier;
        spawnAnimation.StartAlphaMultiplier = (Half)startAlphaMultiplier;

        spawnAnimation.SpawningVelocityMultiplierFixed = spawningVelocityMultiplier == 0f
            ? (byte)0
            : (byte)(SpawnAnimation.FixedPointScale / spawningVelocityMultiplier);

        spawnAnimation.Duration = duration;
        spawnAnimation.Type = easeType;

        return ref this;
    }
    #endregion

    // todo: make this t4 template generated
    public Entity Build()
    {
        byte mask = 0;
        if (positionInstructions.Count > 0) mask |= 1;
        if (velocityInstructions.Count > 0) mask |= 2;
        if (accelerationInstructions.Count > 0) mask |= 4;
        if (curveInstructions.Count > 0) mask |= 8;

        PositionController pc = default;
        VelocityController vc = default;
        AccelerationController ac = default;
        CurveController cc = default;

        if ((mask & 1) != 0) pc = new PositionController { Instructions = [.. positionInstructions], Index = -1 };
        if ((mask & 2) != 0) vc = new VelocityController { Instructions = [.. velocityInstructions], Index = -1 };
        if ((mask & 4) != 0) ac = new AccelerationController { Instructions = [.. accelerationInstructions], Index = -1 };
        if ((mask & 8) != 0) cc = new CurveController { Instructions = [.. curveInstructions], Index = -1 };

        return mask switch
        {
            0  => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation),
            1  => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, pc),
            2  => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, vc),
            3  => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, pc, vc),
            4  => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, ac),
            5  => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, pc, ac),
            6  => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, vc, ac),
            7  => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, pc, vc, ac),
            8  => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, cc),
            9  => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, pc, cc),
            10 => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, vc, cc),
            11 => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, pc, vc, cc),
            12 => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, ac, cc),
            13 => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, pc, ac, cc),
            14 => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, vc, ac, cc),
            15 => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation, pc, vc, ac, cc),
            _  => manager.World.CreateEntity(transform, movement, spriteRenderer, lifetime, spawnAnimation) // unreachable
        };
    }
}
