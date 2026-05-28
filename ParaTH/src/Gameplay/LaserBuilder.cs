using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ParaTH;

// this is a fucking piece of shit, rewrite
internal struct StraightLaserConfig
{
    public SpriteAsset Sprite;
    public float Length;
    public float RenderHalfWidth;
    public Color Color;
    public byte Layer;
    public StgBlendState BlendState;
}

[SkipLocalsInit]
public ref struct LaserBuilder(BulletFactory bulletFactory)
{
    private readonly BulletFactory factory = bulletFactory;

    private Transform sourceTransform = new(Vector2.Zero, Vector2.One, 0);
    private Movement sourceMovement;
    private Lifetime lifetime;
    private Collider collider;
    private LaserSourceRenderer sourceRenderer;
    private StraightLaserConfig laser;
    private bool hasLaser;

    private bool hasSourceHierarchy;
    private Hierarchy sourceHierarchy;

    private ushort currentFrame = 0;
    private readonly UnsafePooledList<RotationInstruction> rotationInstructions = new(4);

    private int way = 1;
    private int layer = 1;
    private float layerVelocityDelta = 0;
    private float layerAccelerationDelta = 0;
    private float layerAngleOffset = 0;
    private float distanceToCenter = 0;
    private float totalSpread = 0;
    private float spreadDelta = 0;
    private SpawningType spawningType = SpawningType.None;

    // TEMP
    [UnscopedRef]
    public ref LaserBuilder SetPosition(Vector2 position)
    {
        sourceTransform.Position = position;
        return ref this;
    }

    #region Rotation
    [UnscopedRef]
    public ref LaserBuilder Delay(ushort frames)
    {
        currentFrame += frames;
        return ref this;
    }

    [UnscopedRef]
    public ref LaserBuilder SetRotation(float newRotation)
    {
        if (currentFrame == 0)
        {
            sourceTransform.Rotation = newRotation;
            return ref this;
        }

        rotationInstructions.Add(new(currentFrame,
            newRotation, 0, EaseType.Linear, RotationInstruction.Ops.SetRotation));
        return ref this;
    }

    [UnscopedRef]
    public ref LaserBuilder AddRotation(float rotationDelta)
    {
        if (currentFrame == 0)
        {
            sourceTransform.Rotation += rotationDelta;
            return ref this;
        }

        rotationInstructions.Add(new(currentFrame,
            rotationDelta, 0, EaseType.Linear, RotationInstruction.Ops.AddRotation));
        return ref this;
    }

    [UnscopedRef]
    public ref LaserBuilder LerpToRotation(float newRotation, ushort duration, EaseType easeType)
    {
        rotationInstructions.Add(new(currentFrame,
            newRotation, duration, easeType, RotationInstruction.Ops.SetRotation));
        currentFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref LaserBuilder LerpAddRotation(float rotationDelta, ushort duration, EaseType easeType)
    {
        rotationInstructions.Add(new(currentFrame,
            rotationDelta, duration, easeType, RotationInstruction.Ops.AddRotation));
        currentFrame += duration;
        return ref this;
    }

    [UnscopedRef]
    public ref LaserBuilder SetRotationalVelocity(float newRotationalVelocity)
    {
        rotationInstructions.Add(new(currentFrame,
            newRotationalVelocity, 0, EaseType.Linear, RotationInstruction.Ops.SetRotationalVelocity));
        return ref this;
    }

    [UnscopedRef]
    public ref LaserBuilder AddRotationalVelocity(float rotationalVelocityDelta)
    {
        rotationInstructions.Add(new(currentFrame,
            rotationalVelocityDelta, 0, EaseType.Linear, RotationInstruction.Ops.AddRotationalVelocity));
        return ref this;
    }
    #endregion

    #region Hierarchy
    [UnscopedRef]
    public ref LaserBuilder SetParent(Entity parent, Vector2 localPosition, Vector2 localScale,
                                      float localRotation = 0, bool preserveTransformRotation = false)
    {
        hasSourceHierarchy = true;
        sourceHierarchy = new Hierarchy(parent, localPosition, localScale, localRotation, preserveTransformRotation)
        {
            Depth = GetChildDepth(factory.World, parent)
        };
        sourceTransform.Position = localPosition;
        sourceTransform.Scale = localScale;
        sourceTransform.Rotation = localRotation;
        return ref this;
    }
    #endregion

    #region Main
    [UnscopedRef]
    public ref LaserBuilder MakeLaser(string spriteName, float length, float halfWidth, float rotation,
                                      Color color, byte layer, StgBlendState blendState)
    {
        laser.Sprite = factory.AssetManager.Get<SpriteAsset>(spriteName);
        laser.Length = length;
        laser.RenderHalfWidth = halfWidth;
        laser.Color = color;
        laser.Layer = layer;
        laser.BlendState = blendState;
        sourceTransform.Rotation = rotation;
        if (hasSourceHierarchy)
            sourceHierarchy.LocalRotation = rotation;
        hasLaser = true;
        return ref this;
    }

    [UnscopedRef]
    public ref LaserBuilder SetLaserSourceSprite(string spriteName, Vector2 scale)
    {
        sourceRenderer.Sprite = factory.AssetManager.Get<SpriteAsset>(spriteName);
        sourceRenderer.Scale = scale;
        return ref this;
    }
    #endregion

    #region Collision
    [UnscopedRef]
    public ref LaserBuilder SetCollisionGroup(byte groupMask)
    {
        collider.IsActive = true;
        collider.GroupMask = groupMask;
        return ref this;
    }

    [UnscopedRef]
    public ref LaserBuilder SetTargetGroup(byte targetMask)
    {
        collider.IsActive = true;
        collider.TargetGroupMask = targetMask;
        return ref this;
    }
    #endregion

    #region Lifetime
    [UnscopedRef]
    public ref LaserBuilder SetOffscreenLifeTime(short frames)
    {
        lifetime.OffscreenFramesToLive = frames;
        return ref this;
    }
    #endregion

    #region Spawning
    [UnscopedRef]
    public ref LaserBuilder SetSpawningNone(
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
    public ref LaserBuilder SetSpawningCircle(
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
    public ref LaserBuilder SetSpawningSpreadByTotal(
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
    public ref LaserBuilder SetSpawningSpreadByDelta(
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

    public readonly void Build(scoped Span<Entity> outputEntities = default)
    {
        if (!hasLaser)
        {
            DisposeInstructions();
            return;
        }

        BuildCore(factory, sourceTransform, sourceMovement, lifetime,
            collider, sourceRenderer, laser,
            rotationInstructions,
            spawningType, way, layer, layerVelocityDelta, layerAccelerationDelta,
            layerAngleOffset, distanceToCenter, totalSpread, spreadDelta,
            hasSourceHierarchy, sourceHierarchy, outputEntities);

        DisposeInstructions();
    }

    private static void BuildCore(
        BulletFactory factory,
        Transform baseSourceTransform,
        Movement baseSourceMovement,
        Lifetime lifetime,
        Collider baseCollider,
        LaserSourceRenderer sourceRenderer,
        StraightLaserConfig laser,
        UnsafePooledList<RotationInstruction> rotationInstructions,
        SpawningType spawningType,
        int way,
        int layer,
        float layerVelocityDelta,
        float layerAccelerationDelta,
        float layerAngleOffset,
        float distanceToCenter,
        float totalSpread,
        float spreadDelta,
        bool hasSourceHierarchy,
        Hierarchy baseSourceHierarchy,
        scoped Span<Entity> outputEntities)
    {
        int amount = way * layer;
        if (amount <= 0 || laser.Sprite is null)
            return;

        bool hasSourceRenderer = sourceRenderer.Sprite is not null;
        bool hasRotCtr = rotationInstructions.Count > 0;
        bool hasCollider = baseCollider.IsActive;

        int sourceTypeCount = 3 + Unsafe.As<bool, byte>(ref hasSourceRenderer)
                                + Unsafe.As<bool, byte>(ref hasRotCtr)
                                + Unsafe.As<bool, byte>(ref hasSourceHierarchy);

        Span<ComponentTypeInfo> sourceTypes = stackalloc ComponentTypeInfo[sourceTypeCount];
        int idx = 0;
        sourceTypes.UnsafeAt(idx++) = Component<Transform>.TypeInfo;
        sourceTypes.UnsafeAt(idx++) = Component<Movement>.TypeInfo;
        sourceTypes.UnsafeAt(idx++) = Component<Lifetime>.TypeInfo;
        if (hasSourceRenderer) sourceTypes.UnsafeAt(idx++) = Component<Renderer>.TypeInfo;
        if (hasRotCtr) sourceTypes.UnsafeAt(idx++) = Component<RotationController>.TypeInfo;
        if (hasSourceHierarchy) sourceTypes.UnsafeAt(idx++) = Component<Hierarchy>.TypeInfo;

        int bodyTypeCount = 4 + Unsafe.As<bool, byte>(ref hasCollider);
        Span<ComponentTypeInfo> bodyTypes = stackalloc ComponentTypeInfo[bodyTypeCount];
        idx = 0;
        bodyTypes.UnsafeAt(idx++) = Component<Transform>.TypeInfo;
        bodyTypes.UnsafeAt(idx++) = Component<Lifetime>.TypeInfo;
        bodyTypes.UnsafeAt(idx++) = Component<Renderer>.TypeInfo;
        bodyTypes.UnsafeAt(idx++) = Component<Hierarchy>.TypeInfo;
        if (hasCollider) bodyTypes.UnsafeAt(idx++) = Component<Collider>.TypeInfo;

        using var sourceEntities = ScopedPooledArray<Entity>.Rent(amount);
        using var bodyEntities = ScopedPooledArray<Entity>.Rent(amount);

        using var sourceTransforms = ScopedPooledArray<Transform>.Rent(amount);
        using var sourceMovements = ScopedPooledArray<Movement>.Rent(amount);
        using var sourceLifetimes = ScopedPooledArray<Lifetime>.Rent(amount);
        using var bodyTransforms = ScopedPooledArray<Transform>.Rent(amount);
        using var bodyLifetimes = ScopedPooledArray<Lifetime>.Rent(amount);
        using var bodyRenderers = ScopedPooledArray<Renderer>.Rent(amount);
        using var bodyHierarchies = ScopedPooledArray<Hierarchy>.Rent(amount);

        using var sourceRenderers = hasSourceRenderer ? ScopedPooledArray<Renderer>.Rent(amount) : default;
        using var sourceHierarchies = hasSourceHierarchy ? ScopedPooledArray<Hierarchy>.Rent(amount) : default;
        using var rotCtrs = hasRotCtr ? ScopedPooledArray<RotationController>.Rent(amount) : default;
        using var bodyColliders = hasCollider ? ScopedPooledArray<Collider>.Rent(amount) : default;

        float baseVelMag = baseSourceMovement.Velocity.Length();
        float baseVelAngle = baseVelMag > 0 ? MathF.Atan2(baseSourceMovement.Velocity.Y, baseSourceMovement.Velocity.X) : 0;
        float baseAccMag = baseSourceMovement.Acceleration.Length();
        float baseAccAngle = baseAccMag > 0 ? MathF.Atan2(baseSourceMovement.Acceleration.Y, baseSourceMovement.Acceleration.X) : baseVelAngle;

        var sharedRotInstr = hasRotCtr ? rotationInstructions.ToArray() : null;

        uint baseSpawnId = factory.GlobalSpawnCounter;
        uint spawnIdStride = hasSourceRenderer ? 2u : 1u;
        factory.GlobalSpawnCounter += (uint)amount * spawnIdStride;

        var laserSprite = laser.Sprite;
        var bodyRenderer = new Renderer
        {
            Texture = laserSprite.Texture,
            SourceRect = laserSprite.SourceRect,
            Anchor = new Vector2(laserSprite.SourceRect.Width * 0.5f, laserSprite.SourceRect.Height * 0.5f),
            Scale = new Vector2(laser.Length / laserSprite.SourceRect.Width,
                                laser.RenderHalfWidth * 2f / laserSprite.SourceRect.Height),
            Rotation = 0,
            Color = laser.Color,
            Layer = laser.Layer,
            BlendState = laser.BlendState
        };

        var laserCollider = baseCollider;
        laserCollider.ShapeType = ShapeType.ObbRect;
        laserCollider.ObbRect.HalfSize = new Vector2(laser.Length * 0.5f, laser.RenderHalfWidth * 0.5f);

        int sourceDepth = hasSourceHierarchy ? baseSourceHierarchy.Depth : -1;
        var bodyHierarchy = new Hierarchy(default, new Vector2(laser.Length * 0.5f, 0), Vector2.One, 0)
        {
            Depth = sourceDepth + 1
        };

        for (int i = 0; i < amount; i++)
        {
            int l = i / way;
            int w = i % way;

            float angle = baseVelAngle;
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

            float curVelMag = baseVelMag + l * layerVelocityDelta;
            float curAccMag = baseAccMag + l * layerAccelerationDelta;
            float curAccAngle = baseAccAngle + (angle - baseVelAngle);

            var velDir = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            var accDir = new Vector2(MathF.Cos(curAccAngle), MathF.Sin(curAccAngle));

            var sourceTransform = baseSourceTransform;
            if (hasSourceHierarchy)
            {
                var sourceHierarchy = baseSourceHierarchy;
                sourceHierarchy.LocalPosition += velDir * distanceToCenter;
                sourceHierarchies[i] = sourceHierarchy;
            }
            else
            {
                sourceTransform.Position += velDir * distanceToCenter;
            }

            sourceTransforms[i] = sourceTransform;
            sourceMovements[i] = new Movement(velDir * curVelMag,
                                              accDir * curAccMag,
                                              baseSourceMovement.SyncTransformRotation);
            sourceLifetimes[i] = lifetime;

            uint bodySpawnId = baseSpawnId + (uint)i * spawnIdStride;
            bodyRenderers[i] = bodyRenderer;
            bodyRenderers[i].SpawnId = bodySpawnId;

            if (hasSourceRenderer)
            {
                var sprite = sourceRenderer.Sprite!;
                sourceRenderers[i] = new Renderer
                {
                    Texture = sprite.Texture,
                    SourceRect = sprite.SourceRect,
                    Anchor = sprite.Anchor,
                    Scale = sourceRenderer.Scale,
                    Rotation = 0,
                    Color = laser.Color,
                    SpawnId = bodySpawnId + 1,  // why the fuck isn't this unique
                    Layer = laser.Layer,
                    BlendState = laser.BlendState
                };
            }

            if (hasRotCtr) rotCtrs[i] = new() { Instructions = sharedRotInstr!, Index = -1 };

            bodyTransforms[i] = CalculateChildTransform(sourceTransform, bodyHierarchy);
            bodyLifetimes[i] = lifetime;
            bodyHierarchies[i] = bodyHierarchy;
            if (hasCollider) bodyColliders[i] = laserCollider;
        }

        factory.World.ReserveEntityBulk(sourceEntities.AsSpan(), sourceTypes, out Archetype sourceArchetype, out Slot sourceStart, out Slot sourceEnd);
        sourceArchetype.SetRangeWithSpanBulk(sourceStart, sourceEnd, sourceTransforms.AsSpan(), sourceMovements.AsSpan(), sourceLifetimes.AsSpan());
        if (hasSourceRenderer) sourceArchetype.SetRangeWithSpanBulk(sourceStart, sourceEnd, sourceRenderers.AsSpan());
        if (hasRotCtr) sourceArchetype.SetRangeWithSpanBulk(sourceStart, sourceEnd, rotCtrs.AsSpan());
        if (hasSourceHierarchy) sourceArchetype.SetRangeWithSpanBulk(sourceStart, sourceEnd, sourceHierarchies.AsSpan());

        for (int i = 0; i < amount; i++)
            bodyHierarchies[i].Parent = sourceEntities[i];

        factory.World.ReserveEntityBulk(bodyEntities.AsSpan(), bodyTypes, out Archetype bodyArchetype, out Slot bodyStart, out Slot bodyEnd);
        bodyArchetype.SetRangeWithSpanBulk(bodyStart, bodyEnd, bodyTransforms.AsSpan(), bodyLifetimes.AsSpan(),
            bodyRenderers.AsSpan(), bodyHierarchies.AsSpan());
        if (hasCollider) bodyArchetype.SetRangeWithSpanBulk(bodyStart, bodyEnd, bodyColliders.AsSpan());

        if (!outputEntities.IsEmpty)
        {
            if (outputEntities.Length >= amount * 2)
            {
                int outIndex = 0;
                for (int i = 0; i < amount; i++)
                {
                    outputEntities[outIndex++] = sourceEntities[i];
                    outputEntities[outIndex++] = bodyEntities[i];
                }
            }
            else
            {
                sourceEntities.AsSpan()[..Math.Min(amount, outputEntities.Length)].CopyTo(outputEntities);
            }
        }
    }

    private static Transform CalculateChildTransform(Transform parentTransform, Hierarchy local)
    {
        float cos = MathF.Cos(parentTransform.Rotation);
        float sin = MathF.Sin(parentTransform.Rotation);

        float localX = local.LocalPosition.X * parentTransform.Scale.X;
        float localY = local.LocalPosition.Y * parentTransform.Scale.Y;

        return new Transform(
            new Vector2(
                parentTransform.Position.X + (localX * cos - localY * sin),
                parentTransform.Position.Y + (localX * sin + localY * cos)),
            parentTransform.Scale * local.LocalScale,
            local.PreserveTransformRotation ? parentTransform.Rotation : parentTransform.Rotation + local.LocalRotation);
    }

    private static int GetChildDepth(World world, Entity parent)
    {
        if (world.TryGetComponent<Hierarchy>(parent, out var hierarchy))
            return hierarchy.Depth + 1;
        return 0;
    }

    private readonly void DisposeInstructions()
    {
        rotationInstructions.Dispose();
    }
}
