using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;

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
            new Vector2(float.NaN, float.NaN), new Vector2(float.NaN, float.NaN), float.NaN, frames, Easing.Linear));
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

    // why the fuck are we lerping velocity, that's acceleration's job
    [UnscopedRef]
    public ref BulletBuilder LerpVelocity(Vector2 start, Vector2 end, ushort frameDuration, EasingFunction easingFunction)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            start, end, float.NaN, frameDuration, easingFunction));
        currentCurveFrame += frameDuration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpVelocityTo(Vector2 end, ushort frameDuration, EasingFunction easingFunction)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(float.NaN, float.NaN), end, float.NaN, frameDuration, easingFunction));
        currentCurveFrame += frameDuration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpVelocityRelative(float speedIncrement, float rotateAngle, ushort frameDuration, EasingFunction easingFunction)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            Vector2.UnitX * speedIncrement, Vector2.Zero, rotateAngle, frameDuration, easingFunction));
        currentCurveFrame += frameDuration;
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
    public ref BulletBuilder SetSprite(SpriteAsset sprite, Color color, byte layer, StgBlendState blendState)
    {
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
