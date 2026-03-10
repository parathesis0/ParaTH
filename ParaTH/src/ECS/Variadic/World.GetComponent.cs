using System;
using System.Collections.Generic;

namespace ParaTH;

public sealed partial class World
{
    public Components<T0, T1> GetComponent<T0, T1>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var slot = entityData.Slot;
        var archetype = entityData.Archetype;
        return archetype.Get<T0, T1>(slot);
    }
}
