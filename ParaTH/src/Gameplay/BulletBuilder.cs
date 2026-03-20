using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;

namespace ParaTH;

public ref struct BulletBuilder(BulletManager bulletManager)
{
    private readonly BulletManager manager = bulletManager;

    private Transform transform = new(Vector2.Zero, Vector2.One, 0);
    private Movement movement;
    private VelocityController velocityController;
    private CurveMovementController curveMovementController;
    private SpriteRenderer spriteRenderer;

    private ushort currentVelocityControllerFrame = 0;
    private readonly List<VelocityInstruction> velocityInstructions = [ default ];

    private ushort currentCurveMovementControllerFrame = 0;
    private readonly List<CurveMovementInstruction> curveMovementInstructions = [ default ];

    #region Essentials
    [UnscopedRef]
    public ref BulletBuilder SetEssentials(
        Vector2 position, string spriteName, Color color, byte layer, StgBlendState blendState)
    {
        var sprite = manager.Asset.Get<SpriteAsset>(spriteName);

        transform.Position = position;
        spriteRenderer.Sprite = sprite;
        spriteRenderer.Color = color;
        spriteRenderer.Layer = layer;
        spriteRenderer.BlendState = blendState;

        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder DelayAll(byte frames)
    {
        // expand
        currentCurveMovementControllerFrame += frames;
        return ref this;
    }
    #endregion

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
        currentVelocityControllerFrame += frames;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocity(Vector2 newVelocity)
    {
        throw new NotImplementedException();
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocityRelative(Vector2 velocityDelta)
    {
        throw new NotImplementedException();
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocityRelative(float speedIncrement, float rotateAngle)
    {
        throw new NotImplementedException();
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpVelocity(Vector2 start, Vector2 end, ushort frameDuration, EasingFunction easingFunction)
    {
        velocityInstructions.Add(new(easingFunction, start, end, currentVelocityControllerFrame, frameDuration, false));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpVelocityRelative(Vector2 end, ushort frameDuration, EasingFunction easingFunction)
    {
        velocityInstructions.Add(new(easingFunction, Vector2.Zero, end, currentVelocityControllerFrame, frameDuration, true));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpVelocityRelative(float speedIncrement, float rotateAngle, ushort frameDuration, EasingFunction easingFunction)
    {
        var velocityVector = new Vector2(
            speedIncrement * MathF.Cos(rotateAngle),
            speedIncrement * MathF.Sin(rotateAngle));

        velocityInstructions.Add(new(easingFunction, Vector2.Zero, velocityVector, currentVelocityControllerFrame, frameDuration, true));
        return ref this;
    }
    #endregion

    #region Angular Velocity
    [UnscopedRef]
    public ref BulletBuilder DelayAngularVelocity(ushort frames)
    {
        currentCurveMovementControllerFrame += frames;
        return ref this;
    }
    [UnscopedRef]
    public ref BulletBuilder SetAngularVelocity(float angularVelocity)
    {
        curveMovementInstructions.Add(new(angularVelocity, currentCurveMovementControllerFrame, -1, 0));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AngularVelocityLoopFrom(sbyte index, byte repeatTimes)
    {
        curveMovementInstructions.Add(new(0, currentCurveMovementControllerFrame, index, repeatTimes));
        return ref this;
    }
    #endregion

    [UnscopedRef]
    public Entity Build()
    {
        // unscalable, find better way
        var hasVelocityController = velocityInstructions.Count > 1;
        var hasCurveMovement = curveMovementInstructions.Count > 1;

        if (hasCurveMovement && hasVelocityController)
        {
            curveMovementController.Instructions = [.. curveMovementInstructions];
            velocityController.Instructions = [.. velocityInstructions];
            return manager.World.CreateEntity(transform, movement, curveMovementController, velocityController, spriteRenderer);
        }

        if (hasVelocityController)
        {
            velocityController.Instructions = [.. velocityInstructions];
            return manager.World.CreateEntity(transform, movement, velocityController, spriteRenderer);
        }

        if (hasCurveMovement)
        {
            curveMovementController.Instructions = [.. curveMovementInstructions];
            return manager.World.CreateEntity(transform, movement, curveMovementController, spriteRenderer);
        }

        return manager.World.CreateEntity(transform, movement, spriteRenderer);
    }
}
