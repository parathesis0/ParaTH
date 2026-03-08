using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed class EntityDataMap
{
    private readonly PagedArray<EntityData> entityDatas;

    public EntityDataMap(int baseChunkByteSize, int capacity)
    {
        entityDatas = new PagedArray<EntityData>(
            baseChunkByteSize / Unsafe.SizeOf<EntityData>(),
            capacity
        );
    }

    public void Add(int entityId, Archetype archetype, Slot slot, int version)
        => entityDatas.Add(entityId, new EntityData(archetype, slot, version));

    public Archetype GetArchetype(int entityId)
        =>  entityDatas[entityId].Archetype;

    public Slot GetSlot(int entityId)
        => entityDatas[entityId].Slot;

    public int GetVersion(int entityId)
        => entityDatas[entityId].Version;

    public ref EntityData GetEntityData(int entityId)
        => ref entityDatas[entityId];

    public void Remove(int entityId)
        => entityDatas.Remove(entityId);

    public bool Has(int entityId)
        => entityDatas.ContainsKey(entityId);

    public void EnsureCapacity(int capacity)
        => entityDatas.EnsureCapacity(capacity);

    public void TrimExcess()
        => entityDatas.TrimExcess();

    public void Clear()
        => entityDatas.Clear();
}
