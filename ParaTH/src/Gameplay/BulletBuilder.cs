using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ParaTH;

public enum RendererType : byte
{
    None,
    SpriteRenderer,
    AnimationRenderer,
}

public enum SpawningType : byte
{
    None,
    Circle,
    Spread
}

public ref struct BulletBuilder(BulletManager bulletManager)
{
    private readonly BulletManager manager = bulletManager;

    // mandatory components
    private Transform transform = new(Vector2.Zero, Vector2.One, 0);
    private Movement movement;
    private Lifetime lifetime;

    // optional component, won't be added if RendererType.None
    private RenderState renderState;
    private RendererType activeRenderer = RendererType.None;
    // optional components, can only be one of the following
    private SpriteRenderer spriteRenderer;
    private AnimationRenderer animationRenderer;

    // optional component, instruction is shared
    private ushort currentFrame = 0;
    private readonly UnsafePooledList<PositionInstruction> positionInstructions = new(4);
    private readonly UnsafePooledList<VelocityInstruction> velocityInstructions = new(4);
    private readonly UnsafePooledList<AccelerationInstruction> accelerationInstructions = new(4);
    private readonly UnsafePooledList<CurveInstruction> curveInstructions = new(4);

    // optional
    private SpawnAnimation spawnAnimation;
    private Collider collider = new() { IsActive = true };
    private int curvyLaserLength = 0;
    private float curvyLaserHalfWidth = 0;

    // spawn settings
    private int way = 1;
    private int layer = 1;
    private float layerVelocityDelta = 0;
    private float layerAccelerationDelta = 0;
    private float layerAngleOffset = 0;
    private float distanceToCenter = 0;
    private float totalSpread = 0;
    private float spreadDelta = 0;
    private SpawningType spawningType = SpawningType.None;

    #region Delay
    [UnscopedRef]
    public ref BulletBuilder Delay(ushort frames)
    {
        currentFrame += frames;
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
    public ref BulletBuilder SyncRenderStateRotation()
    {
        movement.SyncRenderStateRotation = true;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SyncTransformRotation()
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
        curveInstructions.Add(new(currentFrame,
            newAngularVelocity, CurveInstruction.Ops.Set));
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder AddAngularVelocity(float angularVelocityDelta)
    {
        curveInstructions.Add(new(currentFrame,
            angularVelocityDelta, CurveInstruction.Ops.Add));
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
    public ref BulletBuilder SetSprite(string spriteName, Color color, byte layer, StgBlendState blendState,
                                       float rotation = MathHelper.PiOver2, Vector2? scale = null)
    {
        scale ??= Vector2.One;
        var sprite = manager.AssetManager.Get<SpriteAsset>(spriteName);
        spriteRenderer.Sprite = sprite;
        renderState.Color = color;
        renderState.Layer = layer;
        renderState.BlendState = blendState;
        renderState.Rotation = rotation;
        renderState.Scale = scale.Value;

        activeRenderer = RendererType.SpriteRenderer;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetAnimation(string animationName, Color color, byte layer, StgBlendState blendState,
                                          float rotation = MathHelper.PiOver2, Vector2? scale = null)
    {
        scale ??= Vector2.One;
        var animation = manager.AssetManager.Get<AnimationAsset>(animationName);
        animationRenderer.Animation = animation;
        animationRenderer.IsPlaying = true;
        renderState.Color = color;
        renderState.Layer = layer;
        renderState.BlendState = blendState;
        renderState.Rotation = rotation;
        renderState.Scale = scale.Value;

        activeRenderer = RendererType.AnimationRenderer;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetSpawnAnimation(string spriteName, Vector2 startScale, float startAlpha,
                                               float spawningVelocityMultiplier, byte duration, EaseType easeX, EaseType easeY)
    {
        var sprite = manager.AssetManager.Get<SpriteAsset>(spriteName);
        spawnAnimation.Sprite = sprite;
        spawnAnimation.StartScale = startScale;
        spawnAnimation.StartAlpha = (Half)startAlpha;
        spawnAnimation.VelocityMultiplier = (Half)spawningVelocityMultiplier;
        spawnAnimation.Duration = duration;
        spawnAnimation.TypeX = easeX;
        spawnAnimation.TypeY = easeY;

        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetSpawnAnimation(string spriteName, float startScale, float startAlpha,
                                               float spawningVelocityMultiplier, byte duration, EaseType ease)
    {
        SetSpawnAnimation(spriteName, new Vector2(startScale, startScale), startAlpha, spawningVelocityMultiplier, duration, ease, ease);
        return ref this;
    }
    #endregion

    #region Collider
    [UnscopedRef]
    public ref BulletBuilder SetCollisionGroup(byte groupMask)
    {
        collider.GroupMask = groupMask;

        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetTargetGroup(byte targetMask)
    {
        collider.TargetGroupMask = targetMask;

        return ref this;
    }
    [UnscopedRef]
    public ref BulletBuilder SetObbCollider(Vector2 halfSize, float rotation = 0f)
    {
        collider.ShapeType = ShapeType.ObbRect;
        collider.ObbRect.HalfSize = halfSize;
        collider.ObbRect.Rotation = rotation;

        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetCircleCollider(float radius)
    {
        collider.ShapeType = ShapeType.Circle;
        collider.Circle.Radius = radius;

        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetEllipseCollider(Vector2 halfSize, float rotation = 0f)
    {
        collider.ShapeType = ShapeType.Ellipse;
        collider.Ellipse.HalfSize = halfSize;
        collider.Ellipse.Rotation = rotation;

        return ref this;
    }
    #endregion

    #region Spawning Control
    [UnscopedRef]
    public ref BulletBuilder SetSpawningNone(
        int layer = 1,
        float layerVelocityDelta = 0,
        float layerAccelerationDelta = 0,
        float distanceToCenter = 0)
    {
        this.spawningType = SpawningType.None;
        this.way = 1;
        this.layer = layer;
        this.layerVelocityDelta = layerVelocityDelta;
        this.layerAccelerationDelta = layerAccelerationDelta;
        this.distanceToCenter = distanceToCenter;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetSpawningCircle(
        int way,
        int layer = 1,
        float layerVelocityDelta = 0,
        float layerAccelerationDelta = 0,
        float layerAngleOffset = 0,
        float distanceToCenter = 0)
    {
        this.spawningType = SpawningType.Circle;
        this.way = way;
        this.layer = layer;
        this.layerVelocityDelta = layerVelocityDelta;
        this.layerAccelerationDelta = layerAccelerationDelta;
        this.layerAngleOffset = layerAngleOffset;
        this.distanceToCenter = distanceToCenter;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetSpawningSpreadByTotal(
        int way,
        float totalSpread,
        int layer = 1,
        float layerVelocityDelta = 0,
        float layerAccelerationDelta = 0,
        float distanceToCenter = 0)
    {
        this.spawningType = SpawningType.Spread;
        this.way = way;
        this.totalSpread = totalSpread;
        this.spreadDelta = 0;
        this.layer = layer;
        this.layerVelocityDelta = layerVelocityDelta;
        this.layerAccelerationDelta = layerAccelerationDelta;
        this.distanceToCenter = distanceToCenter;
        return ref this;
    }

    [UnscopedRef]
    public ref BulletBuilder SetSpawningSpreadByDelta(
        int way,
        float spreadDelta,
        int layer = 1,
        float layerVelocityDelta = 0,
        float layerAccelerationDelta = 0,
        float distanceToCenter = 0)
    {
        this.spawningType = SpawningType.Spread;
        this.way = way;
        this.totalSpread = 0;
        this.spreadDelta = spreadDelta;
        this.layer = layer;
        this.layerVelocityDelta = layerVelocityDelta;
        this.layerAccelerationDelta = layerAccelerationDelta;
        this.distanceToCenter = distanceToCenter;
        return ref this;
    }

    #endregion

    #region Curvy Laser
    [UnscopedRef]
    public ref BulletBuilder MakeCurvyLaser(int length, float halfWidth)
    {
        collider.ShapeType = ShapeType.CurvyLaser;
        curvyLaserLength = length;
        curvyLaserHalfWidth = halfWidth;
        return ref this;
    }
    #endregion

    [SkipLocalsInit]
    public void Build(scoped Span<Entity> outputEntities = default)
    {
        int amount = way * layer;
        if (amount <= 0)
            return;

        bool hasRds = activeRenderer != RendererType.None;
        bool hasSpr = activeRenderer == RendererType.SpriteRenderer;
        bool hasAni = activeRenderer == RendererType.AnimationRenderer;

        bool hasPos = positionInstructions.Count > 0;
        bool hasVel = velocityInstructions.Count > 0;
        bool hasAcc = accelerationInstructions.Count > 0;
        bool hasCur = curveInstructions.Count > 0;
        bool hasSpw = spawnAnimation.Duration > 0;
        bool hasCol = collider.ShapeType != ShapeType.None;
        bool hasCls = curvyLaserLength > 0;

        int typeCount = 3 + Unsafe.As<bool, byte>(ref hasRds)
                          + Unsafe.As<bool, byte>(ref hasSpr)
                          + Unsafe.As<bool, byte>(ref hasAni)
                          + Unsafe.As<bool, byte>(ref hasPos)
                          + Unsafe.As<bool, byte>(ref hasVel)
                          + Unsafe.As<bool, byte>(ref hasAcc)
                          + Unsafe.As<bool, byte>(ref hasCur)
                          + Unsafe.As<bool, byte>(ref hasSpw)
                          + Unsafe.As<bool, byte>(ref hasCol)
                          + Unsafe.As<bool, byte>(ref hasCls);

        var types = new ComponentTypeInfo[typeCount];
        int tIdx = 0;
        types[tIdx++] = Component<Transform>.TypeInfo;
        types[tIdx++] = Component<Movement>.TypeInfo;
        types[tIdx++] = Component<Lifetime>.TypeInfo;

        if (hasRds) types[tIdx++] = Component<RenderState>.TypeInfo;
        if (hasSpr) types[tIdx++] = Component<SpriteRenderer>.TypeInfo;
        if (hasAni) types[tIdx++] = Component<AnimationRenderer>.TypeInfo;
        if (hasPos) types[tIdx++] = Component<PositionController>.TypeInfo;
        if (hasVel) types[tIdx++] = Component<VelocityController>.TypeInfo;
        if (hasAcc) types[tIdx++] = Component<AccelerationController>.TypeInfo;
        if (hasCur) types[tIdx++] = Component<CurveController>.TypeInfo;
        if (hasSpw) types[tIdx++] = Component<SpawnAnimation>.TypeInfo;
        if (hasCol) types[tIdx++] = Component<Collider>.TypeInfo;
        if (hasCls) types[tIdx++] = Component<CurvyLaser>.TypeInfo;

        using var pooledEntities = ScopedPooledArray<Entity>.Rent(amount);
        Span<Entity> entities = pooledEntities.AsSpan();

        using var pooledTransforms = ScopedPooledArray<Transform>.Rent(amount);
        Span<Transform> tfs = pooledTransforms.AsSpan();

        using var pooledMovements = ScopedPooledArray<Movement>.Rent(amount);
        Span<Movement> mvs = pooledMovements.AsSpan();

        using var pooledLifetimes = ScopedPooledArray<Lifetime>.Rent(amount);
        Span<Lifetime> lts = pooledLifetimes.AsSpan();

        using var pooledRenderStates = hasRds ? ScopedPooledArray<RenderState>.Rent(amount) : default;
        Span<RenderState> rss = hasRds ? pooledRenderStates.AsSpan() : default;

        using var pooledSpriteRenderers = hasSpr ? ScopedPooledArray<SpriteRenderer>.Rent(amount) : default;
        Span<SpriteRenderer> srs = hasSpr ? pooledSpriteRenderers.AsSpan() : default;

        using var pooledAnimationRenderers = hasAni ? ScopedPooledArray<AnimationRenderer>.Rent(amount) : default;
        Span<AnimationRenderer> ars = hasAni ? pooledAnimationRenderers.AsSpan() : default;

        using var pooledPcs = hasPos ? ScopedPooledArray<PositionController>.Rent(amount) : default;
        Span<PositionController> pcs = hasPos ? pooledPcs.AsSpan() : default;

        using var pooledVcs = hasVel ? ScopedPooledArray<VelocityController>.Rent(amount) : default;
        Span<VelocityController> vcs = hasVel ? pooledVcs.AsSpan() : default;

        using var pooledAcs = hasAcc ? ScopedPooledArray<AccelerationController>.Rent(amount) : default;
        Span<AccelerationController> acs = hasAcc ? pooledAcs.AsSpan() : default;

        using var pooledCcs = hasCur ? ScopedPooledArray<CurveController>.Rent(amount) : default;
        Span<CurveController> ccs = hasCur ? pooledCcs.AsSpan() : default;

        using var pooledSas = hasSpw ? ScopedPooledArray<SpawnAnimation>.Rent(amount) : default;
        Span<SpawnAnimation> sas = hasSpw ? pooledSas.AsSpan() : default;

        using var pooledCls = hasCol ? ScopedPooledArray<Collider>.Rent(amount) : default;
        Span<Collider> cls = hasCol ? pooledCls.AsSpan() : default;

        using var pooledLzs = hasCls ? ScopedPooledArray<CurvyLaser>.Rent(amount) : default;
        Span<CurvyLaser> lzs = hasCls ? pooledLzs.AsSpan() : default;

        float baseVcMag = movement.Velocity.Length();
        float baseVcAngle = baseVcMag > 0 ? MathF.Atan2(movement.Velocity.Y, movement.Velocity.X) : 0;
        float baseAccMag = movement.Acceleration.Length();
        float baseAccAngle = baseAccMag > 0 ? MathF.Atan2(movement.Acceleration.Y, movement.Acceleration.X) : baseVcAngle;

        var sharedPcs = hasPos ? positionInstructions.ToArray() : null;
        var sharedVcs = hasVel ? velocityInstructions.ToArray() : null;
        var sharedAcs = hasAcc ? accelerationInstructions.ToArray() : null;
        var sharedCcs = hasCur ? curveInstructions.ToArray() : null;

        uint baseSpawnId = manager.GlobalSpawnCounter;
        manager.GlobalSpawnCounter += (uint)amount;

        for (int i = 0; i < amount; i++)
        {
            int l = i / way;
            int w = i % way;

            float angle = baseVcAngle;
            if (spawningType == SpawningType.Circle)
            {
                angle += (MathF.PI * 2f / way) * w + l * layerAngleOffset;
            }
            else if (spawningType == SpawningType.Spread && way > 1)
            {
                float total = totalSpread > 0 ? totalSpread : spreadDelta * (way - 1);
                float delta = total / (way - 1);
                angle += -total / 2f + delta * w;
            }

            float currentMag = baseVcMag + l * layerVelocityDelta;
            float currentAccMag = baseAccMag + l * layerAccelerationDelta;
            float currentAccAngle = baseAccAngle + (angle - baseVcAngle);

            var dir = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            var accDir = new Vector2(MathF.Cos(currentAccAngle), MathF.Sin(currentAccAngle));

            tfs[i] = new Transform(transform.Position + dir * distanceToCenter, transform.Scale, transform.Rotation);
            mvs[i] = new Movement(dir * currentMag, accDir * currentAccMag, movement.SyncRenderStateRotation, movement.SyncTransformRotation);
            lts[i] = lifetime;

            if (hasRds) { rss[i] = renderState; rss[i].SpawnId = baseSpawnId + (uint)i; }
            if (hasSpr) srs[i] = spriteRenderer;
            if (hasAni) ars[i] = animationRenderer;

            if (hasPos) pcs[i] = new PositionController { Instructions = sharedPcs!, Index = -1 };
            if (hasVel) vcs[i] = new VelocityController { Instructions = sharedVcs!, Index = -1 };
            if (hasAcc) acs[i] = new AccelerationController { Instructions = sharedAcs!, Index = -1 };
            if (hasCur) ccs[i] = new CurveController { Instructions = sharedCcs!, Index = -1 };
            if (hasSpw) sas[i] = spawnAnimation;
            if (hasCol) cls[i] = collider;
            if (hasCls) lzs[i] = new CurvyLaser { LaserNodes = new(curvyLaserLength), Length = curvyLaserLength, HalfWidth = curvyLaserHalfWidth };
        }

        manager.World.ReserveEntityBulk(entities, types, out Archetype archetype, out Slot start, out Slot end);

        archetype.SetRangeWithSpanBulk(start, end, tfs, mvs, lts);

        if (hasRds) archetype.SetRangeWithSpanBulk(start, end, rss);
        if (hasSpr) archetype.SetRangeWithSpanBulk(start, end, srs);
        if (hasAni) archetype.SetRangeWithSpanBulk(start, end, ars);
        if (hasPos) archetype.SetRangeWithSpanBulk(start, end, pcs);
        if (hasVel) archetype.SetRangeWithSpanBulk(start, end, vcs);
        if (hasAcc) archetype.SetRangeWithSpanBulk(start, end, acs);
        if (hasCur) archetype.SetRangeWithSpanBulk(start, end, ccs);
        if (hasSpw) archetype.SetRangeWithSpanBulk(start, end, sas);
        if (hasCol) archetype.SetRangeWithSpanBulk(start, end, cls);
        if (hasCls) archetype.SetRangeWithSpanBulk(start, end, lzs);

        if (!outputEntities.IsEmpty)
            entities.CopyTo(outputEntities);

        positionInstructions.Dispose();
        velocityInstructions.Dispose();
        accelerationInstructions.Dispose();
        curveInstructions.Dispose();
    }
}
