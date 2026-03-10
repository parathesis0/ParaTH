using System;
using System.Collections.Generic;

namespace ParaTH;

public sealed partial class World
{
    public bool HasComponent<T0, T1>(Entity entity)
    {
        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1>();
    }
}
