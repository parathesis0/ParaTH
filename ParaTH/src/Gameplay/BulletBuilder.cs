using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using static ParaTH.AnimationAsset;

namespace ParaTH;

public ref struct BulletBuilder(BulletManager bulletManager, Vector2 position)
{
    private readonly BulletManager manager = bulletManager;

    private Transform transform = new(position, Vector2.One, 0);
    private Movement movement;
    private SpriteRenderer spriteRenderer;
    private BulletController controller;

    private ushort currentVelocityFrame = 0;
    private readonly List<VelocityInstruction> velocityInstructions = [];

    private ushort currentCurveFrame = 0;
    private readonly List<CurveInstruction> curveInstructions = [];

    [UnscopedRef]
    public ref BulletBuilder DelayAll(byte frames)
    {
        // expand
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
    public ref BulletBuilder EnableSyncTransformRotation()
    {
        movement.SyncTransformRotation = !movement.SyncTransformRotation;
        return ref this;
    }
    #endregion

    #region Velocity Control
    [UnscopedRef]
    public ref BulletBuilder DelayVelocity(ushort frames)
    {
        currentVelocityFrame += frames;
        velocityInstructions.Add(new(currentVelocityFrame,
            Vector2.Zero, frames, Easing.Linear, VelocityInstruction.Ops.Delay));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocity(Vector2 newVelocity)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            newVelocity, 0, Easing.Linear, VelocityInstruction.Ops.SetVelocity));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocityMagnitude(float newMagnitude)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(newMagnitude, 0), 0, Easing.Linear, VelocityInstruction.Ops.SetMagnitude));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocityAngle(float newAngle)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(newAngle, 0), 0, Easing.Linear, VelocityInstruction.Ops.SetAngle));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddVelocity(Vector2 velocityDelta)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            velocityDelta, 0, Easing.Linear, VelocityInstruction.Ops.AddVelocity));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddVelocityMagnitude(float magnitudeDelta)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(magnitudeDelta, 0), 0, Easing.Linear, VelocityInstruction.Ops.AddMagnitude));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddVelocityAngle(float angleDelta)
    {
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
        bool needController = velocityInstructions.Count > 0 ||
                              curveInstructions.Count > 0;

        if (needController)
        {
            controller.VelocityInstructions = InitializeInstructionsWith(velocityInstructions);
            controller.VelocityIndex = -1; // so that the first instruction can get inited
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
