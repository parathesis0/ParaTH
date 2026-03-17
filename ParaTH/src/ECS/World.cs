using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ParaTH;

public interface IForeach { void Update(Entity entity); }

public delegate void ForEach(Entity entity);

public sealed partial class World : IDisposable
{
    private readonly EntityDataMap entityDatas;
    private readonly ArchetypeList archetypes;
    private readonly Dictionary<ComponentMask, Archetype> groupMaskToArchetype;
    private readonly Dictionary<QueryDescriptor, Query> queryCache;
    private readonly Queue<Entity> recycledEntities;

    private readonly int baseChunkByteSize;
    private readonly int baseChunkEntityCount;

    private int entityCount;
    private int entityCapacity;

    private bool isDisposed;

    public World(
        int baseChunkByteSize,
        int baseChunkEntityCount,
        int initialArchetypeCapacity,
        int initialEntityCapacity)
    {
        entityDatas = new EntityDataMap(baseChunkByteSize, initialEntityCapacity);
        archetypes = new ArchetypeList(initialArchetypeCapacity);
        groupMaskToArchetype = new Dictionary<ComponentMask, Archetype>(initialArchetypeCapacity);
        queryCache = new Dictionary<QueryDescriptor, Query>(initialArchetypeCapacity);
        recycledEntities = new Queue<Entity>(initialEntityCapacity);

        this.baseChunkByteSize = baseChunkByteSize;
        this.baseChunkEntityCount = baseChunkEntityCount;
    }

    #region Entity Management
    [SkipLocalsInit]
    public Entity CreateEntity<T0>(in T0 component)
    {
        var types = Component<T0>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0>(entity, out var slot, in component);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }

    [SkipLocalsInit]
    public void CreateEntityBulk<T0>(Span<Entity> entityBuffer, Span<T0> components)
    {
        Debug.Assert(entityBuffer.Length == components.Length);

        var amount = entityBuffer.Length;
        var types = Component<T0>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0>(entityBuffer, components);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }

    [SkipLocalsInit]
    public void DestroyEntity(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var movedEntityId = entityData.Archetype.Remove(entityData.Slot);
        entityDatas.Move(movedEntityId, entityData.Slot);

        DestroyAndRecycleEntity(entity);
    }

    [SkipLocalsInit]
    public bool IsAlive(Entity entity)
    {
        ref var entityData = ref entityDatas.TryGetEntityData(entity.Id);

        return !Unsafe.IsNullRef(ref entityData) &&
            entityData.Version == entity.Version;
    }

    [SkipLocalsInit]
    private Entity RecycleOrCreateEntity()
    {
        var shouldRecycle = recycledEntities.TryDequeue(out Entity entity);
        var result = shouldRecycle ? entity : new Entity(entityCount, 1);
        entityCount++;
        return result;
    }

    [SkipLocalsInit]
    private void RecycleOrCreateEntityBulk(
        Archetype archetype, Span<Entity> entityBuffer, Span<EntityData> entityDataBuffer)
    {
        int amount = entityBuffer.Length;

        using var slotBuffer = ScopedPooledArray<Slot>.Rent(amount);
        var slots = slotBuffer.AsSpan();

        archetype.GetNextSlots(slots, amount);
        for (int i = 0; i < amount; i++)
        {
            var entity = RecycleOrCreateEntity();
            entityBuffer.UnsafeAt(i) = entity;
            entityDataBuffer.UnsafeAt(i) = new EntityData(archetype, slots.UnsafeAt(i), entity.Version);
        }
    }

    [SkipLocalsInit]
    private void DestroyAndRecycleEntity(Entity entity)
    {
        entityDatas.Remove(entity.Id);
        var recycledEntity = entity.BumpVersion();
        recycledEntities.Enqueue(recycledEntity);
        entityCount--;
    }

    [SkipLocalsInit]
    private Archetype GetOrCreateArchetype(ComponentTypeInfo[] types)
    {
        var mask = ComponentMask.Zero;
        foreach (var type in types)
            mask |= type.Mask;

        if (groupMaskToArchetype.TryGetValue(mask, out var archetype))
            return archetype;

        var newArchetype = new Archetype(types, baseChunkByteSize, baseChunkEntityCount);
        groupMaskToArchetype[mask] = newArchetype;
        archetypes.Add(newArchetype);

        // archetypes allocate one chunk upon creation
        entityCapacity += newArchetype.EntitiesPerChunk;
        entityDatas.EnsureCapacity(entityCapacity);

        return newArchetype;
    }

    [SkipLocalsInit]
    private Archetype GetOrCreateArchetypeWithCapacity(ComponentTypeInfo[] types, int amount)
    {
        var archetype = GetOrCreateArchetype(types);
        entityCapacity -= archetype.EntityCapacity;

        archetype.EnsureEntityCapacity(archetype.EntityCount + amount);

        var requiredCapacity = entityCapacity + archetype.EntityCapacity;
        entityDatas.EnsureCapacity(requiredCapacity);
        entityCapacity = requiredCapacity;

        return archetype;
    }

    [SkipLocalsInit]
    private void AddEntityDataBulk(Span<Entity> entities, Span<EntityData> entityData)
    {
        int amount = entities.Length;
        var entityDataArrInternal = entityDatas.EntityDatas;
        for (int i = 0; i < amount; i++)
        {
            var entity = entities.UnsafeAt(i);
            ref var data = ref entityData.UnsafeAt(i);
            entityDataArrInternal.Add(entity.Id, data);
        }
    }
    #endregion

    #region Component Manipulation
    [SkipLocalsInit]
    public ref T0 GetComponent<T0>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var slot = entityData.Slot;
        var archetype = entityData.Archetype;
        return ref archetype.Get<T0>(slot);
    }

    [SkipLocalsInit]
    public bool TryGetComponent<T>(Entity entity, out T component)
    {
        Debug.Assert(IsAlive(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        if (!archetype.TryGetComponentArrayIndex<T>(out var arrayIndex))
        {
            component = default!;
            return false;
        }

        ref var chunk = ref archetype.GetChunk(slot.ChunkIndex);
        var arr = Unsafe.As<T[]>(chunk.Components.UnsafeAt(arrayIndex));
        component = arr.UnsafeAt(slot.Index);
        return true;
    }

    [SkipLocalsInit]
    public ref T TryGetComponentRef<T>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        if (!archetype.TryGetComponentArrayIndex<T>(out var arrayIndex))
            return ref Unsafe.NullRef<T>();

        ref var chunk = ref archetype.GetChunk(slot.ChunkIndex);
        var arr = Unsafe.As<T[]>(chunk.Components.UnsafeAt(arrayIndex));
        return ref arr.UnsafeAt(slot.Index);
    }

    [SkipLocalsInit]
    public bool HasComponent<T0>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0>();
    }

    [SkipLocalsInit]
    public void SetComponentValue<T0>(Entity entity, in T0 component)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0>(slot, component);
    }

    [SkipLocalsInit]
    public void AddComponent<T0>(Entity entity, in T0 component)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var newArchetype = GetOrCreateArchetypeByAddEdge(Component<T0>.TypeInfo, oldArchetype);

        // moving to a archetype with more component types reserves space for the new component
        Move(ref entityData, oldArchetype, newArchetype);

        // new component is empty, give it value
        SetComponentValue<T0>(entity, component);
    }

    [SkipLocalsInit]
    public void RemoveComponent<T0>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var newArchetype = GetOrCreateArchetypeByRemoveEdge(Component<T0>.TypeInfo, oldArchetype);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }

    [SkipLocalsInit]
    private Archetype GetOrCreateArchetypeByAddEdge(ComponentTypeInfo newType, Archetype oldArchetype)
    {
        var index = newType.Id;

        if (oldArchetype.TryGetAddEdge(index, out var archetype))
            return archetype;

        var types = Merge(oldArchetype.ComponentTypes, [newType]);
        var newArchetype = GetOrCreateArchetype(types);
        oldArchetype.AddAddEdge(index, newArchetype);
        newArchetype.AddRemoveEdge(index, oldArchetype);

        return newArchetype;
    }

    [SkipLocalsInit]
    private Archetype GetOrCreateArchetypeByRemoveEdge(ComponentTypeInfo removedType, Archetype oldArchetype)
    {
        var index = removedType.Id;

        if (oldArchetype.TryGetRemoveEdge(index, out var archetype))
            return archetype;

        var types = Remove(oldArchetype.ComponentTypes, [removedType]);
        var newArchetype = GetOrCreateArchetype(types);
        oldArchetype.AddRemoveEdge(index, newArchetype);
        newArchetype.AddAddEdge(index, oldArchetype);

        return newArchetype;
    }

    [SkipLocalsInit]
    private void Move(ref EntityData srcEntityData, Archetype srcArchetype, Archetype dstArchetype)
    {
        Debug.Assert(srcArchetype != dstArchetype,
            "Source archetype cannot be the same as destination archetype.");

        var oldSlot = srcEntityData.Slot;
        var allocatedEntities = dstArchetype.Reserve(out var newSlot);

        Archetype.CopyEntityAndMatchingComponentsTo(srcArchetype, oldSlot, dstArchetype, newSlot);
        var movedEntityId = srcArchetype.Remove(oldSlot);

        var entityDatas = this.entityDatas;
        entityDatas.Move(movedEntityId, oldSlot);

        srcEntityData.Archetype = dstArchetype;
        srcEntityData.Slot = newSlot;

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);
    }

    // todo: idk make theses util?
    [SkipLocalsInit]
    private static T[] Merge<T>(T[] a, T[] b)
    {
        if (a.Length == 0) return b;
        if (b.Length == 0) return a;

        var result = GC.AllocateUninitializedArray<T>(a.Length + b.Length);
        a.AsSpan().CopyTo(result);
        b.AsSpan().CopyTo(result.AsSpan(a.Length));
        return result;
    }

    [SkipLocalsInit]
    private static T[] Remove<T>(T[] a, T[] b) where T : IEquatable<T>
    {
        var result = GC.AllocateUninitializedArray<T>(a.Length - b.Length);
        int ri = 0;
        ReadOnlySpan<T> bSpan = b;

        for (int i = 0; i < a.Length; i++)
        {
            if (!bSpan.Contains(a.UnsafeAt(i)))
                result.UnsafeAt(ri++) = a.UnsafeAt(i);
        }
        return result;
    }
    #endregion

    #region Query
#pragma warning disable RCS1242 // Do not pass non-read-only struct by read-only reference
    [SkipLocalsInit]
    // for optimal performance, use this and iterate archetypes & chunks manually
    public Query GetOrCreateQuery(in QueryDescriptor descriptor)
    {
        var queryCache = this.queryCache;
        if (queryCache.TryGetValue(descriptor, out var query))
            return query;

        var newQuery = new Query(archetypes, descriptor);
        queryCache[descriptor] = newQuery;
        return newQuery;

    }

    // slow, only use for prototyping
    [SkipLocalsInit]
    public void Query(in QueryDescriptor descriptor, ForEach forEntity)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                var entities = chunk.Entities;
                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    var entity = entities.UnsafeAt(i);
                    forEntity(entity);
                }
            }
        }
    }

    // fast but could still be faster
    [SkipLocalsInit]
    public void Query<T>(in QueryDescriptor descriptor, ref T iForEach) where T : struct, IForeach
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                var entities = chunk.Entities;
                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    var entity = entities.UnsafeAt(i);
                    iForEach.Update(entity);
                }
            }
        }
    }

    [SkipLocalsInit]
    public int CountArchetypes(in QueryDescriptor descriptor)
    {
        var query = GetOrCreateQuery(in descriptor);
        return query.GetMatchingArchetypesSpan().Length;
    }

    [SkipLocalsInit]
    public int CountChunks(in QueryDescriptor descriptor)
    {
        var count = 0;
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
            count += archetype.Chunks.Count;

        return count;
    }

    [SkipLocalsInit]
    public int CountEntities(in QueryDescriptor descriptor)
    {
        var count = 0;
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
            count += archetype.EntityCount;

        return count;
    }

    // BYOM get methods
    [SkipLocalsInit]
    public void GetArchetypes(in QueryDescriptor descriptor, Span<Archetype> buffer)
    {
        var query = GetOrCreateQuery(in descriptor);
        var matchingArchetypes = query.GetMatchingArchetypesSpan();
        matchingArchetypes.CopyTo(buffer);
    }

    [SkipLocalsInit]
    public void GetChunks(in QueryDescriptor descriptor, Span<Chunk> buffer)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
            archetype.Chunks.AsSpan().CopyTo(buffer);
    }

    [SkipLocalsInit]
    public void GetEntities(in QueryDescriptor descriptor, Span<Entity> buffer)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
                chunk.Entities.AsSpan(0, chunk.EntityCount).CopyTo(buffer);
        }
    }
#pragma warning restore RCS1242 // Do not pass non-read-only struct by read-only reference
    #endregion

    #region Batch Query Operations
#pragma warning disable RCS1242 // Do not pass non-read-only struct by read-only reference
    [SkipLocalsInit]
    public void QueryDestroyEntity(in QueryDescriptor descriptor)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                var entities = chunk.Entities;
                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    var entity = entities.UnsafeAt(i);
                    DestroyAndRecycleEntity(entity);
                }
                chunk.Clear();
            }
            archetype.Clear();
        }
    }

    [SkipLocalsInit]
    public void QuerySetComponentValue<T0>(in QueryDescriptor descriptor, in T0 component)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(query.Matches(Component<T0>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetFilledComponentSpan<T0>(out var span);
                span.Fill(component);
            }
        }
    }

    [SkipLocalsInit]
    public void QueryAddComponent<T0>(in QueryDescriptor descriptor, in T0 component)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var newArchetype = GetOrCreateArchetypeByAddEdge(Component<T0>.TypeInfo, archetype);

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0>(setRangeStartSlot, setRangeEndSlot, component);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }

    [SkipLocalsInit]
    public void QueryRemoveComponent<T0>(in QueryDescriptor descriptor)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(query.Matches(Component<T0>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var newArchetype = GetOrCreateArchetypeByRemoveEdge(Component<T0>.TypeInfo, archetype);

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
#pragma warning restore RCS1242 // Do not pass non-read-only struct by read-only reference
    #endregion

    #region Capacity Management
    public void DestroyArchetype(Archetype archetype)
    {
        var mask = archetype.Mask;

        archetypes.Remove(archetype);
        groupMaskToArchetype.Remove(mask);

        foreach (var otherArchetype in archetypes.AsSpan())
            otherArchetype.RemoveAllEdgesByValue(archetype);

        archetype.Dispose(); // dispose?
        entityCapacity -= archetype.EntityCapacity;
    }

    // don't destroy any archetype for simplicity's sake
    public void TrimExcess()
    {
        // we do not trim entityDatas since it's sparse
        // and trimming it desyncs its capacity and entityCapacity
        for (int i = archetypes.Count - 1; i >= 0; i--)
        {
            var archetype = archetypes[i];

            if (archetype.EntityCount == 0)
            {
                DestroyArchetype(archetype);
                continue;
            }

            var original = archetype.EntityCapacity;
            archetype.TrimExcess();
            var trimmed = archetype.EntityCapacity;

            var delta = trimmed - original;
            entityCapacity += delta;
        }

        // trim recycledEntities
        var queue = recycledEntities;
        for (int i = 0; i < queue.Count; i++)
        {
            var entity = queue.Dequeue();
            if (entity.Id <= entityCapacity)
                queue.Enqueue(entity);
        }
    }

    public void Clear()
    {
        entityCount = 0;
        entityCapacity = 0;

        entityDatas.Clear();
        archetypes.Clear();
        groupMaskToArchetype.Clear();
        queryCache.Clear();
        recycledEntities.Clear();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (isDisposed)
            return;

        isDisposed = true;

        entityCount = 0;
        entityCapacity = 0;

        entityDatas.Clear();
        archetypes.Dispose();
        groupMaskToArchetype.Clear();
        queryCache.Clear();
        recycledEntities.Clear();
    }
    #endregion
}
