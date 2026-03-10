using System;
using System.Collections.Generic;

namespace ParaTH;

public sealed partial class World
{
    public Entity CreateEntity<T0, T1>(in T0 c0, in T1 c1)
    {
        var types = Component<T0, T1>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1>(entity, out var slot, c0, c1);

        capacity += allocatedEntities;
        entityDatas.EnsureCapacity(capacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
}
