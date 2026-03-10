using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed partial class World : IDisposable
{
    private readonly EntityDataMap entityDatas;
    private readonly ArchetypeList archetypes;
    private readonly Dictionary<ulong, Archetype> groupMaskToArchetype;
    private readonly Queue<Entity> recycledEntities;

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

    #region Entity Creation and Destroy
    // todo: variadic source gen wip
    public Entity CreateEntity<T0>(in T0 component)
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

    public void DestroyEntity(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var movedEntityId = entityData.Archetype.Remove(entityData.Slot);
        entityDatas.Move(movedEntityId, entityData.Slot);

        DestroyAndRecycleEntity(entity);
    }

    public bool IsAlive(Entity entity)
    {
        ref var entityData = ref entityDatas.TryGetEntityDataRef(entity.Id);

        return !Unsafe.IsNullRef(ref entityData) &&
            entityData.Version == entity.Version;
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
    #endregion

    #region Component Manipulation
    // todo: variadic source gen wip
    public ref T0 GetComponents<T0>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var slot = entityData.Slot;
        var archetype = entityData.Archetype;
        return ref archetype.Get<T0>(slot);
    }

    // todo: is this correct/optimal?
    public bool TryGetComponent<T>(Entity entity, out T component)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        if (!archetype.TryGetComponentArrayIndex<T>(out var arrayIndex))
        {
            component = default!;
            return false;
        }

        ref var chunk = ref archetype.GetChunk(arrayIndex);
        var arr = Unsafe.As<T[]>(chunk.Components.UnsafeAt(arrayIndex));
        component = arr[slot.Index];
        return true;
    }

    // todo: is this correct/optimal?
    public ref T TryGetComponentRef<T>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        if (!archetype.TryGetComponentArrayIndex<T>(out var arrayIndex))
            return ref Unsafe.NullRef<T>();

        ref var chunk = ref archetype.GetChunk(arrayIndex);
        var arr = Unsafe.As<T[]>(chunk.Components.UnsafeAt(arrayIndex));
        return ref arr[slot.Index];
    }

    #endregion
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
