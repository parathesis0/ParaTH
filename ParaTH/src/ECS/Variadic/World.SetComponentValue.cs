using System;
using System.Collections.Generic;

namespace ParaTH;

public sealed partial class World
{
    public void SetComponentValue<T0, T1>(Entity entity, in T0 c0, in T1 c1)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1>(slot, in c0, in c1);
    }
}
