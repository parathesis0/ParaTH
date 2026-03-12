using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ParaTH;

public interface IForeach { void Update(Entity entity); }

public delegate void ForEach(Entity entity);

public sealed partial class World : IDisposable
{
    private readonly EntityDataMap entityDatas;
    private readonly ArchetypeList archetypes;
    private readonly Dictionary<ulong, Archetype> groupMaskToArchetype;
    private readonly Dictionary<QueryDescriptor, Query> queryCache;
    private readonly Queue<Entity> recycledEntities;

    private readonly int baseChunkByteSize;
    private readonly int baseChunkEntityCount;

    private int entityCount;
    private int capacity;

    private bool isDisposed;

    public World(
        int baseChunkByteSize,
        int baseChunkEntityCount,
        int initialArchetypeCapacity,
        int initialEntityCapacity)
    {
        entityDatas = new EntityDataMap(baseChunkByteSize, initialEntityCapacity);
        archetypes = new ArchetypeList(initialArchetypeCapacity);
        groupMaskToArchetype = new Dictionary<ulong, Archetype>(initialArchetypeCapacity);
        queryCache = new Dictionary<QueryDescriptor, Query>(initialArchetypeCapacity);
        recycledEntities = new Queue<Entity>(initialEntityCapacity);

        this.baseChunkByteSize = baseChunkByteSize;
        this.baseChunkEntityCount = baseChunkEntityCount;
    }

    #region Entity Creation and Destroy
    [SkipLocalsInit]
    public Entity CreateEntity<T0>(in T0 component)
    {
        var types = Component<T0>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0>(entity, out var slot, in component);

        capacity += allocatedEntities;
        entityDatas.EnsureCapacity(capacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }

    [SkipLocalsInit]
    public void DestroyEntity(Entity entity)
    {
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
        var recycle = recycledEntities.TryDequeue(out Entity entity);
        var result = recycle ? entity : new Entity(entityCount, 1);
        entityCount++;
        return result;
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
        ulong mask = 0;
        foreach (var type in types)
            mask |= type.Mask;

        if (groupMaskToArchetype.TryGetValue(mask, out var archetype))
            return archetype;

        var newArchetype = new Archetype(types, baseChunkByteSize, baseChunkEntityCount);
        groupMaskToArchetype[mask] = newArchetype;
        archetypes.Add(newArchetype);

        // archetypes allocate one chunk upon creation
        capacity += newArchetype.EntitiesPerChunk;
        entityDatas.EnsureCapacity(capacity);

        return newArchetype;
    }
    #endregion

    #region Component Manipulation
    [SkipLocalsInit]
    public ref T0 GetComponent<T0>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var slot = entityData.Slot;
        var archetype = entityData.Archetype;
        return ref archetype.Get<T0>(slot);
    }

    [SkipLocalsInit]
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

        ref var chunk = ref archetype.GetChunk(slot.ChunkIndex);
        var arr = Unsafe.As<T[]>(chunk.Components.UnsafeAt(arrayIndex));
        component = arr.UnsafeAt(slot.Index);
        return true;
    }

    [SkipLocalsInit]
    public ref T TryGetComponentRef<T>(Entity entity)
    {
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
        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0>();
    }

    [SkipLocalsInit]
    public void SetComponentValue<T0>(Entity entity, in T0 component)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0>(slot, component);
    }

    [SkipLocalsInit]
    public void AddComponent<T0>(Entity entity, in T0 component)
    {
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
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var newArchetype = GetOrCreateArchetypeByRemoveEdge(Component<T0>.TypeInfo, oldArchetype);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }

    [SkipLocalsInit]
    private Archetype GetOrCreateArchetypeByAddEdge(in ComponentTypeInfo type, Archetype oldArchetype)
    {
        var index = type.Id;

        if (oldArchetype.TryGetAddEdge(index, out var archetype))
            return archetype;

        // todo: find a way to do this without allocating new memory?
        var types = Merge(oldArchetype.ComponentTypes, [type]);
        var newArchetype = GetOrCreateArchetype(types);
        oldArchetype.AddAddEdge(index, newArchetype);
        newArchetype.AddRemoveEdge(index, oldArchetype);

        return newArchetype;
    }

    [SkipLocalsInit]
    private Archetype GetOrCreateArchetypeByRemoveEdge(in ComponentTypeInfo type, Archetype oldArchetype)
    {
        var index = type.Id;

        if (oldArchetype.TryGetRemoveEdge(index, out var archetype))
            return archetype;

        // todo: find a way to do this without allocating new memory?
        var types = Remove(oldArchetype.ComponentTypes, [type]);
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

        Archetype.CopyEntityAndMatchingComponents(srcArchetype, oldSlot, dstArchetype, newSlot);
        var movedEntityId = srcArchetype.Remove(oldSlot);

        var entityDatas = this.entityDatas;
        entityDatas.Move(movedEntityId, oldSlot);

        srcEntityData.Archetype = dstArchetype;
        srcEntityData.Slot = newSlot;

        capacity += allocatedEntities;
        entityDatas.EnsureCapacity(capacity);
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
    public Query GetOrCreateQuery(in QueryDescriptor descriptor)
    {
        var queryCache = this.queryCache;
        if (queryCache.TryGetValue(descriptor, out var query))
            return query;

        var newQuery = new Query(archetypes, descriptor);
        queryCache[descriptor] = newQuery;
        return newQuery;

    }

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
        var index = 0;

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
                buffer.UnsafeAt(index++) = chunk;
        }
    }

    [SkipLocalsInit]
    public void GetEntities(in QueryDescriptor descriptor, Span<Entity> buffer)
    {
        var query = GetOrCreateQuery(in descriptor);
        var index = 0;

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                var entities = chunk.Entities;
                for (int i = 0; i < chunk.EntityCount; i++)
                    buffer.UnsafeAt(index++) = entities.UnsafeAt(i);
            }
        }
    }
#pragma warning restore RCS1242 // Do not pass non-read-only struct by read-only reference
    #endregion

    #region Dispose
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
        capacity = 0;

        entityDatas.Clear();
        archetypes.Clear();
        groupMaskToArchetype.Clear();
        recycledEntities.Clear();
    }
    #endregion
}
