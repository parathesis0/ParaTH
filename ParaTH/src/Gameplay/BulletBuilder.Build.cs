using System.Runtime.CompilerServices;

namespace ParaTH;

public ref partial struct BulletBuilder
{
    // we have to create entities with all components at once to avoid the structual changes of adding components during creation
    [SkipLocalsInit]
    public Entity Build()
    {
        byte mask = 0;
        if (positionInstructions.Count > 0)     mask |= 1;
        if (velocityInstructions.Count > 0)     mask |= 2;
        if (accelerationInstructions.Count > 0) mask |= 4;
        if (curveInstructions.Count > 0)        mask |= 8;
        if (spawnAnimation.Duration > 0)        mask |= 16;

        PositionController pc = default;
        VelocityController vc = default;
        AccelerationController ac = default;
        CurveController cc = default;

        if ((mask & 1) != 0) pc = new PositionController { Instructions = [.. positionInstructions], Index = -1 };
        if ((mask & 2) != 0) vc = new VelocityController { Instructions = [.. velocityInstructions], Index = -1 };
        if ((mask & 4) != 0) ac = new AccelerationController { Instructions = [.. accelerationInstructions], Index = -1 };
        if ((mask & 8) != 0) cc = new CurveController { Instructions = [.. curveInstructions], Index = -1 };

        return activeRenderer switch
        {
            RendererType.None => mask switch
            {
                0  => manager.World.CreateEntity(transform, movement, lifetime),
                1  => manager.World.CreateEntity(transform, movement, lifetime, pc),
                2  => manager.World.CreateEntity(transform, movement, lifetime, vc),
                3  => manager.World.CreateEntity(transform, movement, lifetime, pc, vc),
                4  => manager.World.CreateEntity(transform, movement, lifetime, ac),
                5  => manager.World.CreateEntity(transform, movement, lifetime, pc, ac),
                6  => manager.World.CreateEntity(transform, movement, lifetime, vc, ac),
                7  => manager.World.CreateEntity(transform, movement, lifetime, pc, vc, ac),
                8  => manager.World.CreateEntity(transform, movement, lifetime, cc),
                9  => manager.World.CreateEntity(transform, movement, lifetime, pc, cc),
                10 => manager.World.CreateEntity(transform, movement, lifetime, vc, cc),
                11 => manager.World.CreateEntity(transform, movement, lifetime, pc, vc, cc),
                12 => manager.World.CreateEntity(transform, movement, lifetime, ac, cc),
                13 => manager.World.CreateEntity(transform, movement, lifetime, pc, ac, cc),
                14 => manager.World.CreateEntity(transform, movement, lifetime, vc, ac, cc),
                15 => manager.World.CreateEntity(transform, movement, lifetime, pc, vc, ac, cc),
                16 => manager.World.CreateEntity(transform, movement, lifetime, spawnAnimation),
                17 => manager.World.CreateEntity(transform, movement, lifetime, pc, spawnAnimation),
                18 => manager.World.CreateEntity(transform, movement, lifetime, vc, spawnAnimation),
                19 => manager.World.CreateEntity(transform, movement, lifetime, pc, vc, spawnAnimation),
                20 => manager.World.CreateEntity(transform, movement, lifetime, ac, spawnAnimation),
                21 => manager.World.CreateEntity(transform, movement, lifetime, pc, ac, spawnAnimation),
                22 => manager.World.CreateEntity(transform, movement, lifetime, vc, ac, spawnAnimation),
                23 => manager.World.CreateEntity(transform, movement, lifetime, pc, vc, ac, spawnAnimation),
                24 => manager.World.CreateEntity(transform, movement, lifetime, cc, spawnAnimation),
                25 => manager.World.CreateEntity(transform, movement, lifetime, pc, cc, spawnAnimation),
                26 => manager.World.CreateEntity(transform, movement, lifetime, vc, cc, spawnAnimation),
                27 => manager.World.CreateEntity(transform, movement, lifetime, pc, vc, cc, spawnAnimation),
                28 => manager.World.CreateEntity(transform, movement, lifetime, ac, cc, spawnAnimation),
                29 => manager.World.CreateEntity(transform, movement, lifetime, pc, ac, cc, spawnAnimation),
                30 => manager.World.CreateEntity(transform, movement, lifetime, vc, ac, cc, spawnAnimation),
                31 => manager.World.CreateEntity(transform, movement, lifetime, pc, vc, ac, cc, spawnAnimation),
                _  => default // unreachable
            },
            RendererType.SpriteRenderer => mask switch
            {
                0  => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime),
                1  => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc),
                2  => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, vc),
                3  => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, vc),
                4  => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, ac),
                5  => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, ac),
                6  => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, vc, ac),
                7  => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, vc, ac),
                8  => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, cc),
                9  => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, cc),
                10 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, vc, cc),
                11 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, vc, cc),
                12 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, ac, cc),
                13 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, ac, cc),
                14 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, vc, ac, cc),
                15 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, vc, ac, cc),
                16 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, spawnAnimation),
                17 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, spawnAnimation),
                18 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, vc, spawnAnimation),
                19 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, vc, spawnAnimation),
                20 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, ac, spawnAnimation),
                21 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, ac, spawnAnimation),
                22 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, vc, ac, spawnAnimation),
                23 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, vc, ac, spawnAnimation),
                24 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, cc, spawnAnimation),
                25 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, cc, spawnAnimation),
                26 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, vc, cc, spawnAnimation),
                27 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, vc, cc, spawnAnimation),
                28 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, ac, cc, spawnAnimation),
                29 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, ac, cc, spawnAnimation),
                30 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, vc, ac, cc, spawnAnimation),
                31 => manager.World.CreateEntity(transform, movement, renderState, spriteRenderer, lifetime, pc, vc, ac, cc, spawnAnimation),
                _  => default // unreachable
            },
            RendererType.AnimationRenderer => mask switch
            {
                0  => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime),
                1  => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc),
                2  => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, vc),
                3  => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, vc),
                4  => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, ac),
                5  => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, ac),
                6  => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, vc, ac),
                7  => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, vc, ac),
                8  => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, cc),
                9  => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, cc),
                10 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, vc, cc),
                11 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, vc, cc),
                12 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, ac, cc),
                13 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, ac, cc),
                14 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, vc, ac, cc),
                15 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, vc, ac, cc),
                16 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, spawnAnimation),
                17 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, spawnAnimation),
                18 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, vc, spawnAnimation),
                19 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, vc, spawnAnimation),
                20 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, ac, spawnAnimation),
                21 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, ac, spawnAnimation),
                22 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, vc, ac, spawnAnimation),
                23 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, vc, ac, spawnAnimation),
                24 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, cc, spawnAnimation),
                25 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, cc, spawnAnimation),
                26 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, vc, cc, spawnAnimation),
                27 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, vc, cc, spawnAnimation),
                28 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, ac, cc, spawnAnimation),
                29 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, ac, cc, spawnAnimation),
                30 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, vc, ac, cc, spawnAnimation),
                31 => manager.World.CreateEntity(transform, movement, renderState, animationRenderer, lifetime, pc, vc, ac, cc, spawnAnimation),
                _  => default // unreachable
            },
            _ => default // unreachable
        };
    }
}
