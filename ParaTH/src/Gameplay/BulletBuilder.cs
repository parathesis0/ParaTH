using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;

namespace ParaTH;

public ref struct BulletBuilder(BulletManager bulletManager)
{
    private readonly BulletManager manager = bulletManager;

    private Transform transform = new(Vector2.Zero, Vector2.One, 0);
    private Movement movement;
    private CurveMovement curveMovement;
    private SpriteRenderer spriteRenderer;

    private int currentCurveMovementFrame = 0;
    private readonly List<CurveMovementFrame> curveMovementFrames = [default];

    #region Basics
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
    #endregion

    #region Velocity
    [UnscopedRef]
    public ref BulletBuilder SetVelocity(Vector2 velocity, Vector2 acceleration)
    {
        movement.Velocity = velocity;
        movement.Acceleration = acceleration;

        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocity(float velocity, Vector2 acceleration, float angle)
    {
        var velocityVector = new Vector2(
            velocity * MathF.Cos(angle),
            velocity * MathF.Sin(angle));

        movement.Velocity = velocityVector;
        movement.Acceleration = acceleration;

        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetVelocity(float velocity, float acceleration, float angle)
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
    public ref BulletBuilder ToggleTransformRotationSync()
    {
        movement.SyncTransformRotation = !movement.SyncTransformRotation;
        return ref this;
    }
    #endregion

    #region Angular Velocity
    [UnscopedRef]
    public ref BulletBuilder AngularVelocityDelay(int frames)
    {
        currentCurveMovementFrame += frames;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetAngularVelocity(float angularVelocity)
    {
        curveMovementFrames.Add(new(angularVelocity, currentCurveMovementFrame));
        return ref this;
    }
    #endregion

    [UnscopedRef]
    public Entity Build()
    {
        var hasCurveMovement = curveMovementFrames.Count > 0;

        if (hasCurveMovement)
        {
            curveMovement.Frames = [.. curveMovementFrames];
            return manager.World.CreateEntity(transform, movement, curveMovement, spriteRenderer);
        }

        return manager.World.CreateEntity(transform, movement, spriteRenderer);
    }
}
