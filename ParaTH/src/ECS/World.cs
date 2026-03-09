using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.AccessControl;

namespace ParaTH;

public sealed partial class World : IDisposable
{
    private EntityDataMap entityDatas;
    private ArchetypeList archetypes;
    private Dictionary<ulong, Archetype> groupMaskToArchetype;
    private Queue<Entity> recycledEntities;

    private readonly int baseChunkByteSize;
    private readonly ushort baseChunkEntityCount;

    private ushort entityCount;
    private ushort capacity;

    public World(
        int baseChunkByteSize,
        ushort baseChunkEntityCount,
        int initialArchetypeCapacity,
        int initialEntityCapacity)
    {
        entityDatas = new EntityDataMap(baseChunkByteSize, initialEntityCapacity);
        archetypes = new ArchetypeList(initialArchetypeCapacity);
        groupMaskToArchetype = new Dictionary<ulong, Archetype>(initialArchetypeCapacity);

        recycledEntities = new Queue<Entity>();

        this.baseChunkByteSize = baseChunkByteSize;
        this.baseChunkEntityCount = baseChunkEntityCount;
    }

    // todo: variadic source gen wip
    public Entity Create<T0>(in T0 component)
    {
        var types = Component<T0>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0>(entity, out var slot, component);

        capacity += allocatedEntities;
        entityDatas.EnsureCapacity(capacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }

    public void Destroy(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var movedEntityId = entityData.Archetype.Remove(entityData.Slot);
        entityDatas.Move(movedEntityId, entityData.Slot);

        DestroyAndRecycleEntity(entity);
    }

    private Entity RecycleOrCreateEntity()
    {
        var recycle = recycledEntities.TryDequeue(out Entity entity);
        return recycle ? entity : new Entity(entityCount++, 1);
    }

    private void DestroyAndRecycleEntity(Entity entity)
    {
        entityDatas.Remove(entity.Id);
        var recycledEntity = new Entity(entity.Id, (ushort)unchecked(entity.Version + 1));
        recycledEntities.Enqueue(recycledEntity);
        entityCount--;
    }

    private Archetype GetOrCreateArchetype(ComponentTypeInfo[] types)
    {
        ulong mask = 0;
        foreach (var type in types)
            mask |= type.Mask;

        if (groupMaskToArchetype.TryGetValue(mask, out var archetype))
            return archetype;

        var newArchetype = new Archetype(types, baseChunkByteSize, baseChunkEntityCount);
        groupMaskToArchetype[mask] = newArchetype;

        // archetypes allocate one chunk upon creation
        capacity += (ushort)newArchetype.EntitiesPerChunk;
        entityDatas.EnsureCapacity(capacity);

        return newArchetype;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
