using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace ParaTH;

public ref partial struct BulletBuilder
{
    [SkipLocalsInit]
    public void Build(Span<Entity> outputEntities = default)
    {
        int amount = way * layer;
        if (amount <= 0)
            return;

        // mandatory components
        ScopedPooledArray<Entity> pooledEntities;
        Span<Entity> entities = (pooledEntities = ScopedPooledArray<Entity>.Rent(amount)).AsSpan();

        ScopedPooledArray<Transform> pooledTransforms;
        Span<Transform> transforms = (pooledTransforms = ScopedPooledArray<Transform>.Rent(amount)).AsSpan();

        ScopedPooledArray<Movement> pooledMovements;
        Span<Movement> movements = (pooledMovements = ScopedPooledArray<Movement>.Rent(amount)).AsSpan();

        ScopedPooledArray<Lifetime> pooledLifetimes;
        Span<Lifetime> lifetimes = (pooledLifetimes = ScopedPooledArray<Lifetime>.Rent(amount)).AsSpan();

        // one of renderer
        ScopedPooledArray<RenderState> pooledRenderStates = default;
        Span<RenderState> renderStates = default;
        if (activeRenderer != RendererType.None)
            renderStates = (pooledRenderStates = ScopedPooledArray<RenderState>.Rent(amount)).AsSpan();

        ScopedPooledArray<SpriteRenderer> pooledSpriteRenderers = default;
        Span<SpriteRenderer> spriteRenderers = default;
        if (activeRenderer == RendererType.SpriteRenderer)
            spriteRenderers = (pooledSpriteRenderers = ScopedPooledArray<SpriteRenderer>.Rent(amount)).AsSpan();

        ScopedPooledArray<AnimationRenderer> pooledAnimationRenderers = default;
        Span<AnimationRenderer> animationRenderers = default;
        if (activeRenderer == RendererType.AnimationRenderer)
            animationRenderers = (pooledAnimationRenderers = ScopedPooledArray<AnimationRenderer>.Rent(amount)).AsSpan();

        // mask
        byte mask = 0;
        if (positionInstructions.Count > 0) mask |= 1;
        if (velocityInstructions.Count > 0) mask |= 2;
        if (accelerationInstructions.Count > 0) mask |= 4;
        if (curveInstructions.Count > 0) mask |= 8;
        if (spawnAnimation.Duration > 0) mask |= 16;

        ScopedPooledArray<PositionController> pooledPcs = default;
        Span<PositionController> pcs = default;
        if ((mask & 1) != 0)
            pcs = (pooledPcs = ScopedPooledArray<PositionController>.Rent(amount)).AsSpan();

        ScopedPooledArray<VelocityController> pooledVcs = default;
        Span<VelocityController> vcs = default;
        if ((mask & 2) != 0)
            vcs = (pooledVcs = ScopedPooledArray<VelocityController>.Rent(amount)).AsSpan();

        ScopedPooledArray<AccelerationController> pooledAcs = default;
        Span<AccelerationController> acs = default;
        if ((mask & 4) != 0)
            acs = (pooledAcs = ScopedPooledArray<AccelerationController>.Rent(amount)).AsSpan();

        ScopedPooledArray<CurveController> pooledCcs = default;
        Span<CurveController> ccs = default;
        if ((mask & 8) != 0)
            ccs = (pooledCcs = ScopedPooledArray<CurveController>.Rent(amount)).AsSpan();

        ScopedPooledArray<SpawnAnimation> pooledSas = default;
        Span<SpawnAnimation> sas = default;
        if ((mask & 16) != 0)
            sas = (pooledSas = ScopedPooledArray<SpawnAnimation>.Rent(amount)).AsSpan();

        try
        {
            float baseVcMag = movement.Velocity.Length();
            float baseVcAngle = baseVcMag > 0 ? MathF.Atan2(movement.Velocity.Y, movement.Velocity.X) : 0;
            float baseAccMag = movement.Acceleration.Length();
            float baseAccAngle = baseAccMag > 0 ? MathF.Atan2(movement.Acceleration.Y, movement.Acceleration.X) : baseVcAngle;

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

                Vector2 dir = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                Vector2 accDir = new Vector2(MathF.Cos(currentAccAngle), MathF.Sin(currentAccAngle));

                transforms[i] = new Transform(transform.Position + dir * distanceToCenter, transform.Scale, transform.Rotation);
                movements[i] = new Movement { Velocity = dir * currentMag, Acceleration = accDir * currentAccMag, SyncRenderStateRotation = movement.SyncRenderStateRotation };
                lifetimes[i] = lifetime;

                if (activeRenderer != RendererType.None) renderStates[i] = renderState;
                if (activeRenderer == RendererType.SpriteRenderer) spriteRenderers[i] = spriteRenderer;
                if (activeRenderer == RendererType.AnimationRenderer) animationRenderers[i] = animationRenderer;

                if ((mask & 1) != 0) pcs[i] = new PositionController { Instructions = [.. positionInstructions], Index = -1 };
                if ((mask & 2) != 0) vcs[i] = new VelocityController { Instructions = [.. velocityInstructions], Index = -1 };
                if ((mask & 4) != 0) acs[i] = new AccelerationController { Instructions = [.. accelerationInstructions], Index = -1 };
                if ((mask & 8) != 0) ccs[i] = new CurveController { Instructions = [.. curveInstructions], Index = -1 };
                if ((mask & 16) != 0) sas[i] = spawnAnimation;
            }

            switch (activeRenderer)
            {
                case RendererType.None:
                    switch (mask)
                    {
                        case 0: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes); break;
                        case 1: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs); break;
                        case 2: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, vcs); break;
                        case 3: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, vcs); break;
                        case 4: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, acs); break;
                        case 5: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, acs); break;
                        case 6: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, vcs, acs); break;
                        case 7: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, vcs, acs); break;
                        case 8: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, ccs); break;
                        case 9: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, ccs); break;
                        case 10: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, vcs, ccs); break;
                        case 11: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, vcs, ccs); break;
                        case 12: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, acs, ccs); break;
                        case 13: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, acs, ccs); break;
                        case 14: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, vcs, acs, ccs); break;
                        case 15: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, vcs, acs, ccs); break;
                        case 16: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, sas); break;
                        case 17: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, sas); break;
                        case 18: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, vcs, sas); break;
                        case 19: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, vcs, sas); break;
                        case 20: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, acs, sas); break;
                        case 21: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, acs, sas); break;
                        case 22: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, vcs, acs, sas); break;
                        case 23: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, vcs, acs, sas); break;
                        case 24: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, ccs, sas); break;
                        case 25: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, ccs, sas); break;
                        case 26: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, vcs, ccs, sas); break;
                        case 27: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, vcs, ccs, sas); break;
                        case 28: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, acs, ccs, sas); break;
                        case 29: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, acs, ccs, sas); break;
                        case 30: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, vcs, acs, ccs, sas); break;
                        case 31: manager.World.CreateEntityBulk(entities, transforms, movements, lifetimes, pcs, vcs, acs, ccs, sas); break;
                    }
                    break;
                case RendererType.SpriteRenderer:
                    switch (mask)
                    {
                        case 0: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes); break;
                        case 1: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs); break;
                        case 2: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, vcs); break;
                        case 3: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, vcs); break;
                        case 4: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, acs); break;
                        case 5: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, acs); break;
                        case 6: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, vcs, acs); break;
                        case 7: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, vcs, acs); break;
                        case 8: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, ccs); break;
                        case 9: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, ccs); break;
                        case 10: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, vcs, ccs); break;
                        case 11: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, vcs, ccs); break;
                        case 12: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, acs, ccs); break;
                        case 13: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, acs, ccs); break;
                        case 14: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, vcs, acs, ccs); break;
                        case 15: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, vcs, acs, ccs); break;
                        case 16: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, sas); break;
                        case 17: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, sas); break;
                        case 18: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, vcs, sas); break;
                        case 19: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, vcs, sas); break;
                        case 20: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, acs, sas); break;
                        case 21: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, acs, sas); break;
                        case 22: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, vcs, acs, sas); break;
                        case 23: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, vcs, acs, sas); break;
                        case 24: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, ccs, sas); break;
                        case 25: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, ccs, sas); break;
                        case 26: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, vcs, ccs, sas); break;
                        case 27: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, vcs, ccs, sas); break;
                        case 28: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, acs, ccs, sas); break;
                        case 29: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, acs, ccs, sas); break;
                        case 30: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, vcs, acs, ccs, sas); break;
                        case 31: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, spriteRenderers, lifetimes, pcs, vcs, acs, ccs, sas); break;
                    }
                    break;
                case RendererType.AnimationRenderer:
                    switch (mask)
                    {
                        case 0: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes); break;
                        case 1: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs); break;
                        case 2: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, vcs); break;
                        case 3: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, vcs); break;
                        case 4: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, acs); break;
                        case 5: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, acs); break;
                        case 6: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, vcs, acs); break;
                        case 7: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, vcs, acs); break;
                        case 8: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, ccs); break;
                        case 9: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, ccs); break;
                        case 10: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, vcs, ccs); break;
                        case 11: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, vcs, ccs); break;
                        case 12: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, acs, ccs); break;
                        case 13: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, acs, ccs); break;
                        case 14: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, vcs, acs, ccs); break;
                        case 15: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, vcs, acs, ccs); break;
                        case 16: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, sas); break;
                        case 17: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, sas); break;
                        case 18: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, vcs, sas); break;
                        case 19: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, vcs, sas); break;
                        case 20: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, acs, sas); break;
                        case 21: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, acs, sas); break;
                        case 22: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, vcs, acs, sas); break;
                        case 23: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, vcs, acs, sas); break;
                        case 24: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, ccs, sas); break;
                        case 25: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, ccs, sas); break;
                        case 26: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, vcs, ccs, sas); break;
                        case 27: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, vcs, ccs, sas); break;
                        case 28: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, acs, ccs, sas); break;
                        case 29: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, acs, ccs, sas); break;
                        case 30: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, vcs, acs, ccs, sas); break;
                        case 31: manager.World.CreateEntityBulk(entities, transforms, movements, renderStates, animationRenderers, lifetimes, pcs, vcs, acs, ccs, sas); break;
                    }
                    break;
            }
        }
        finally
        {
            pooledEntities.Dispose();
            pooledTransforms.Dispose();
            pooledMovements.Dispose();
            pooledLifetimes.Dispose();
            pooledRenderStates.Dispose();
            pooledSpriteRenderers.Dispose();
            pooledAnimationRenderers.Dispose();
            pooledPcs.Dispose();
            pooledVcs.Dispose();
            pooledAcs.Dispose();
            pooledCcs.Dispose();
            pooledSas.Dispose();
        }

        if (!outputEntities.IsEmpty)
            entities.CopyTo(outputEntities);
    }
}
