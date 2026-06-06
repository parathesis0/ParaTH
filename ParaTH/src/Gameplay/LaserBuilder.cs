using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ParaTH;

internal enum LaserSpawningType : byte
{
    None,
    SpreadTotal,
    SpreadDelta
}

[SkipLocalsInit]
public ref struct LaserBuilder(BulletFactory bulletFactory)
{
    private const byte DefaultLayer = 100;
    private const StgBlendState DefaultBlendState = StgBlendState.Additive;

    private readonly BulletFactory factory = bulletFactory;

    private Transform sourceTransform = new(Vector2.Zero, Vector2.One, 0);
    private Lifetime lifetime = new(-1);
    private Collider collider;

    private Renderer sourceRenderer;
    private Renderer beamRenderer;
    private Renderer endRenderer;

    private float length;
    private float halfWidth;
    private bool hasLaser;

    private ushort currentFrame;
    private readonly UnsafePooledList<RotationInstruction> rotationInstructions = new(4);

    private int way = 1;
    private float totalSpread;
    private float spreadDelta;
    private LaserSpawningType spawningType = LaserSpawningType.None;

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

    #region Main
    [UnscopedRef]
    public ref LaserBuilder MakeLaser(Vector2 pos, float length, float rotation, float halfWidth, string spriteName)
    {
        sourceTransform.Position = pos;
        sourceTransform.Rotation = rotation;

        this.length = length;
        this.halfWidth = halfWidth;
        hasLaser = true;
        SetLaserBeam(spriteName);
        return ref this;
    }

    [UnscopedRef]
    public ref LaserBuilder MakeLaser(Vector2 start, Vector2 end, float halfWidth, string spriteName)
    {
        var delta = end - start;
        return ref MakeLaser(start, delta.Length(), MathF.Atan2(delta.Y, delta.X), halfWidth, spriteName);
    }

    [UnscopedRef]
    public ref LaserBuilder SetLaserSource(string? spriteName, Vector2 scale, Color? color = null,
                                           byte layer = DefaultLayer,
                                           StgBlendState blendState = DefaultBlendState)
    {
        sourceRenderer = CreateRenderer(factory.AssetManager, spriteName, scale, color ?? Color.White, layer, blendState);
        return ref this;
    }

    [UnscopedRef]
    public ref LaserBuilder SetLaserBeam(string spriteName, Color? color = null,
                                         byte layer = DefaultLayer,
                                         StgBlendState blendState = DefaultBlendState)
    {
        var sprite = factory.AssetManager.Get<SpriteAsset>(spriteName);
        beamRenderer = new Renderer
        {
            Texture = sprite.Texture,
            SourceRect = sprite.SourceRect,
            Anchor = new Vector2(sprite.SourceRect.Width * 0.5f, sprite.SourceRect.Height * 0.5f),
            Scale = new Vector2(length / sprite.SourceRect.Width, halfWidth * 2f / sprite.SourceRect.Height),
            Color = color ?? Color.White,
            Layer = layer,
            BlendState = blendState,
            IsVisible = true
        };
        return ref this;
    }

    [UnscopedRef]
    public ref LaserBuilder SetLaserEnd(string? spriteName, Vector2 scale, Color? color = null,
                                        byte layer = DefaultLayer,
                                        StgBlendState blendState = DefaultBlendState)
    {
        endRenderer = CreateRenderer(factory.AssetManager, spriteName, scale, color ?? Color.White, layer, blendState);
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

    [UnscopedRef]
    public ref LaserBuilder SetMaxAliveFrames(ushort frames)
    {
        lifetime.MaxAliveFrames = frames;
        return ref this;
    }
    #endregion

    #region Spawning
    [UnscopedRef]
    public ref LaserBuilder SetSpawningNone()
    {
        spawningType = LaserSpawningType.None;
        way = 1;
        return ref this;
    }

    [UnscopedRef]
    public ref LaserBuilder SetSpawningSpreadByTotal(int way, float totalSpread)
    {
        spawningType = LaserSpawningType.SpreadTotal;
        this.way = way;
        this.totalSpread = totalSpread;
        spreadDelta = 0;
        return ref this;
    }

    [UnscopedRef]
    public ref LaserBuilder SetSpawningSpreadByDelta(int way, float spreadDelta)
    {
        spawningType = LaserSpawningType.SpreadDelta;
        this.way = way;
        totalSpread = 0;
        this.spreadDelta = spreadDelta;
        return ref this;
    }
    #endregion

    public readonly void Build(scoped Span<Entity> outputEntities = default)
    {
        if (!hasLaser || way <= 0)
        {
            DisposeInstructions();
            return;
        }

        BuildCore(factory, sourceTransform, lifetime, collider,
            sourceRenderer, beamRenderer, endRenderer, length, halfWidth,
            rotationInstructions, spawningType, way, totalSpread, spreadDelta,
            outputEntities);

        DisposeInstructions();
    }

    private static void BuildCore(
        BulletFactory factory,
        Transform baseSourceTransform,
        Lifetime lifetime,
        Collider baseCollider,
        Renderer sourceRenderer,
        Renderer beamRenderer,
        Renderer endRenderer,
        float length,
        float halfWidth,
        UnsafePooledList<RotationInstruction> rotationInstructions,
        LaserSpawningType spawningType,
        int amount,
        float totalSpread,
        float spreadDelta,
        scoped Span<Entity> outputEntities)
    {
        bool hasRotCtr = rotationInstructions.Count > 0;
        bool hasCollider = baseCollider.IsActive;

        int sourceTypeCount = 4 + Unsafe.As<bool, byte>(ref hasRotCtr);
        Span<ComponentTypeInfo> sourceTypes = stackalloc ComponentTypeInfo[sourceTypeCount];
        int idx = 0;
        sourceTypes.UnsafeAt(idx++) = Component<Transform>.TypeInfo;
        sourceTypes.UnsafeAt(idx++) = Component<Hierarchy>.TypeInfo;
        sourceTypes.UnsafeAt(idx++) = Component<Lifetime>.TypeInfo;
        sourceTypes.UnsafeAt(idx++) = Component<Renderer>.TypeInfo;
        if (hasRotCtr) sourceTypes.UnsafeAt(idx++) = Component<RotationController>.TypeInfo;

        int beamTypeCount = 4 + Unsafe.As<bool, byte>(ref hasCollider);
        Span<ComponentTypeInfo> beamTypes = stackalloc ComponentTypeInfo[beamTypeCount];
        idx = 0;
        beamTypes.UnsafeAt(idx++) = Component<Transform>.TypeInfo;
        beamTypes.UnsafeAt(idx++) = Component<Hierarchy>.TypeInfo;
        beamTypes.UnsafeAt(idx++) = Component<Lifetime>.TypeInfo;
        beamTypes.UnsafeAt(idx++) = Component<Renderer>.TypeInfo;
        if (hasCollider) beamTypes.UnsafeAt(idx++) = Component<Collider>.TypeInfo;

        Span<ComponentTypeInfo> endTypes = stackalloc ComponentTypeInfo[4];
        endTypes.UnsafeAt(0) = Component<Transform>.TypeInfo;
        endTypes.UnsafeAt(1) = Component<Hierarchy>.TypeInfo;
        endTypes.UnsafeAt(2) = Component<Lifetime>.TypeInfo;
        endTypes.UnsafeAt(3) = Component<Renderer>.TypeInfo;

        using var sourceEntities = ScopedPooledArray<Entity>.Rent(amount);
        using var beamEntities = ScopedPooledArray<Entity>.Rent(amount);
        using var endEntities = ScopedPooledArray<Entity>.Rent(amount);

        using var sourceTransforms = ScopedPooledArray<Transform>.Rent(amount);
        using var sourceHierarchies = ScopedPooledArray<Hierarchy>.Rent(amount);
        using var sourceLifetimes = ScopedPooledArray<Lifetime>.Rent(amount);
        using var sourceRenderers = ScopedPooledArray<Renderer>.Rent(amount);
        using var beamTransforms = ScopedPooledArray<Transform>.Rent(amount);
        using var beamHierarchies = ScopedPooledArray<Hierarchy>.Rent(amount);
        using var beamLifetimes = ScopedPooledArray<Lifetime>.Rent(amount);
        using var beamRenderers = ScopedPooledArray<Renderer>.Rent(amount);
        using var endTransforms = ScopedPooledArray<Transform>.Rent(amount);
        using var endHierarchies = ScopedPooledArray<Hierarchy>.Rent(amount);
        using var endLifetimes = ScopedPooledArray<Lifetime>.Rent(amount);
        using var endRenderers = ScopedPooledArray<Renderer>.Rent(amount);

        using var rotCtrs = hasRotCtr ? ScopedPooledArray<RotationController>.Rent(amount) : default;
        using var colliders = hasCollider ? ScopedPooledArray<Collider>.Rent(amount) : default;

        var sharedRotInstr = hasRotCtr ? rotationInstructions.ToArray() : null;
        uint spawnId = SpawnId.NextBlock((uint)amount * 3u);

        var baseBeamHierarchy = new Hierarchy(default, new Vector2(length * 0.5f, 0), Vector2.One, 0)
        {
            Depth = 0
        };
        var baseEndHierarchy = new Hierarchy(default, new Vector2(length, 0), Vector2.One, 0)
        {
            Depth = 0
        };

        var laserCollider = baseCollider;
        laserCollider.ShapeType = ShapeType.ObbRect;
        laserCollider.ObbRect.HalfSize = new Vector2(length * 0.5f, halfWidth);

        for (int i = 0; i < amount; i++)
        {
            float rotation = ResolveSpawnRotation(baseSourceTransform.Rotation, i, amount,
                spawningType, totalSpread, spreadDelta);

            var sourceTransform = baseSourceTransform;
            sourceTransform.Rotation = rotation;

            sourceTransforms[i] = sourceTransform;
            sourceHierarchies[i] = new Hierarchy(default, Vector2.Zero, Vector2.One, 0);
            sourceLifetimes[i] = lifetime;
            sourceRenderers[i] = sourceRenderer;
            sourceRenderers[i].SpawnId = spawnId++;

            var beamHierarchy = baseBeamHierarchy;
            var endHierarchy = baseEndHierarchy;

            beamTransforms[i] = CalculateChildTransform(sourceTransform, beamHierarchy);
            beamHierarchies[i] = beamHierarchy;
            beamLifetimes[i] = lifetime;
            beamRenderers[i] = beamRenderer;
            beamRenderers[i].SpawnId = spawnId++;

            endTransforms[i] = CalculateChildTransform(sourceTransform, endHierarchy);
            endHierarchies[i] = endHierarchy;
            endLifetimes[i] = lifetime;
            endRenderers[i] = endRenderer;
            endRenderers[i].SpawnId = spawnId++;

            if (hasRotCtr) rotCtrs[i] = new() { Instructions = sharedRotInstr!, Index = -1 };
            if (hasCollider) colliders[i] = laserCollider;
        }

        factory.World.ReserveEntityBulk(sourceEntities.AsSpan(), sourceTypes,
            out Archetype sourceArchetype, out Slot sourceStart, out Slot sourceEnd);
        factory.World.ReserveEntityBulk(beamEntities.AsSpan(), beamTypes,
            out Archetype beamArchetype, out Slot beamStart, out Slot beamEnd);
        factory.World.ReserveEntityBulk(endEntities.AsSpan(), endTypes,
            out Archetype endArchetype, out Slot endStart, out Slot endEnd);

        for (int i = 0; i < amount; i++)
        {
            sourceHierarchies[i].FirstChild = beamEntities[i];
            sourceHierarchies[i].ChildCount = 2;

            beamHierarchies[i].Parent = sourceEntities[i];
            beamHierarchies[i].NextSibling = endEntities[i];

            endHierarchies[i].Parent = sourceEntities[i];
            endHierarchies[i].PrevSibling = beamEntities[i];
        }

        sourceArchetype.SetRangeWithSpanBulk(sourceStart, sourceEnd,
            sourceTransforms.AsSpan(), sourceHierarchies.AsSpan(), sourceLifetimes.AsSpan(), sourceRenderers.AsSpan());
        if (hasRotCtr) sourceArchetype.SetRangeWithSpanBulk(sourceStart, sourceEnd, rotCtrs.AsSpan());

        beamArchetype.SetRangeWithSpanBulk(beamStart, beamEnd,
            beamTransforms.AsSpan(), beamHierarchies.AsSpan(), beamLifetimes.AsSpan(), beamRenderers.AsSpan());
        if (hasCollider) beamArchetype.SetRangeWithSpanBulk(beamStart, beamEnd, colliders.AsSpan());

        endArchetype.SetRangeWithSpanBulk(endStart, endEnd,
            endTransforms.AsSpan(), endHierarchies.AsSpan(), endLifetimes.AsSpan(), endRenderers.AsSpan());

        if (!outputEntities.IsEmpty)
            sourceEntities.AsSpan()[..Math.Min(amount, outputEntities.Length)].CopyTo(outputEntities);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float ResolveSpawnRotation(float baseRotation, int index, int amount,
        LaserSpawningType spawningType, float totalSpread, float spreadDelta)
    {
        if (amount <= 1 || spawningType == LaserSpawningType.None)
            return baseRotation;

        if (spawningType == LaserSpawningType.SpreadDelta)
            totalSpread = spreadDelta * (amount - 1);

        return baseRotation - totalSpread * 0.5f + totalSpread / (amount - 1) * index;
    }

    private static Renderer CreateRenderer(
        AssetManager assetManager,
        string? spriteName,
        Vector2 scale,
        Color color,
        byte layer,
        StgBlendState blendState)
    {
        if (spriteName is null)
        {
            return new Renderer
            {
                Scale = scale,
                Color = color,
                Layer = layer,
                BlendState = blendState,
                IsVisible = false
            };
        }

        var sprite = assetManager.Get<SpriteAsset>(spriteName);
        return new Renderer
        {
            Texture = sprite.Texture,
            SourceRect = sprite.SourceRect,
            Anchor = sprite.Anchor,
            Scale = scale,
            Color = color,
            Layer = layer,
            BlendState = blendState,
            IsVisible = true
        };
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

    private readonly void DisposeInstructions()
    {
        rotationInstructions.Dispose();
    }
}
