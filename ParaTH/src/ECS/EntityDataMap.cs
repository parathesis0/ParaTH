using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed class EntityDataMap
{
    public readonly PagedArray<EntityData> EntityDatas;

    public EntityDataMap(int baseChunkByteSize, int capacity)
    {
        EntityDatas = new PagedArray<EntityData>(
            baseChunkByteSize / Unsafe.SizeOf<EntityData>(),
            capacity
        );
    }

    public void Add(int entityId, Archetype archetype, Slot slot, int version)
        => EntityDatas.Add(entityId, new EntityData(archetype, slot, version));

    public Archetype GetArchetype(int entityId)
        => EntityDatas[entityId].Archetype;

    public Slot GetSlot(int entityId)
        => EntityDatas[entityId].Slot;

    public int GetVersion(int entityId)
        => EntityDatas[entityId].Version;

    public ref EntityData GetEntityData(int entityId)
        => ref EntityDatas[entityId];

    public ref EntityData TryGetEntityData(int entityId)
        => ref EntityDatas.TryGetRef(entityId);

    public void Remove(int entityId)
        => EntityDatas.Remove(entityId);

    public bool Has(int entityId)
        => EntityDatas.ContainsKey(entityId);

    public void Move(int id, Slot slot)
        => EntityDatas[id].Slot = slot;

    public void Move(int id, Archetype archetype, Slot slot)
    {
        ref var data = ref EntityDatas[id];
        data.Archetype = archetype;
        data.Slot = slot;
    }

    public void MoveBulk(Archetype srcArchetype, Slot srcLastSlot, Archetype dstArchetype, Slot dstFirstFreeSlot)
    {
        for (int i = 0; i <= srcLastSlot.ChunkIndex; i++)
        {
            ref var chunk = ref srcArchetype.Chunks[i];

            for (int j = 0; j < chunk.EntityCount; j++)
            {
                var entity = chunk.Entities.UnsafeAt(j);

                Move(entity.Id, dstArchetype, dstFirstFreeSlot);
                dstFirstFreeSlot = Slot.GetNextFor(dstFirstFreeSlot, dstArchetype.EntitiesPerChunk);
            }
        }
    }

    public void EnsureCapacity(int capacity)
        => EntityDatas.EnsureCapacity(capacity);

    public void TrimExcess()
        => EntityDatas.TrimExcess();

    public void Clear()
        => EntityDatas.Clear();
}
