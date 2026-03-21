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
    // the most unholy lines of code you will ever read
    [UnscopedRef]
    public ref BulletBuilder DelayVelocity(ushort frames)
    {
        currentVelocityFrame += frames;
        velocityInstructions.Add(new(currentVelocityFrame,
            Vector2.Zero, new Vector2(float.NaN, float.NaN), frames, Easing.Linear));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocity(Vector2 newVelocity)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            Vector2.Zero, newVelocity, 0, Easing.Linear));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddVelocity(Vector2 delta)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            delta, new Vector2(float.NaN, 0), 0, Easing.Linear));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocityRelative(float newVelocityMagnitude, float angleDelta)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(newVelocityMagnitude, angleDelta), new Vector2(0, float.NaN), 0, Easing.Linear));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddVelocityRelative(float velocityDelta, float angleDelta)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(velocityDelta, angleDelta), new Vector2(float.NaN, float.NaN), 0, Easing.Linear));
        return ref this;
    }

    // why the fuck are we lerping velocity, just use acceleration
    [UnscopedRef]
    public ref BulletBuilder LerpVelocity(Vector2 start, Vector2 end, ushort frameDuration, EasingFunction easing)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            start, end, frameDuration, easing));
        currentVelocityFrame += frameDuration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpAddVelocity(Vector2 delta, ushort frameDuration, EasingFunction easing)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            delta, new Vector2(float.NaN, 0), frameDuration, easing));
        currentVelocityFrame += frameDuration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpVelocityRelative(float newVelocityMagnitude, float angleDelta, ushort frameDuration, EasingFunction easing)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(newVelocityMagnitude, angleDelta), new Vector2(0, float.NaN), frameDuration, easing));
        currentVelocityFrame += frameDuration;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder LerpAddVelocityRelative(float velocityDelta, float angleDelta, ushort frameDuration, EasingFunction easing)
    {
        velocityInstructions.Add(new(currentVelocityFrame,
            new Vector2(velocityDelta, angleDelta), new Vector2(float.NaN, float.NaN), frameDuration, easing));
        currentVelocityFrame += frameDuration;
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
