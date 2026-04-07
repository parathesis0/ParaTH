namespace ParaTH;

public sealed partial class World
{
    public Entity CreateEntity<T0, T1>(in T0 c0, in T1 c1)
    {
        var types = Component<T0, T1>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1>(entity, out var slot, c0, c1);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
    public Entity CreateEntity<T0, T1, T2>(in T0 c0, in T1 c1, in T2 c2)
    {
        var types = Component<T0, T1, T2>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1, T2>(entity, out var slot, c0, c1, c2);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
    public Entity CreateEntity<T0, T1, T2, T3>(in T0 c0, in T1 c1, in T2 c2, in T3 c3)
    {
        var types = Component<T0, T1, T2, T3>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1, T2, T3>(entity, out var slot, c0, c1, c2, c3);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
    public Entity CreateEntity<T0, T1, T2, T3, T4>(in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4)
    {
        var types = Component<T0, T1, T2, T3, T4>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1, T2, T3, T4>(entity, out var slot, c0, c1, c2, c3, c4);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
    public Entity CreateEntity<T0, T1, T2, T3, T4, T5>(in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5)
    {
        var types = Component<T0, T1, T2, T3, T4, T5>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1, T2, T3, T4, T5>(entity, out var slot, c0, c1, c2, c3, c4, c5);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
    public Entity CreateEntity<T0, T1, T2, T3, T4, T5, T6>(in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6)
    {
        var types = Component<T0, T1, T2, T3, T4, T5, T6>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1, T2, T3, T4, T5, T6>(entity, out var slot, c0, c1, c2, c3, c4, c5, c6);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
    public Entity CreateEntity<T0, T1, T2, T3, T4, T5, T6, T7>(in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7)
    {
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1, T2, T3, T4, T5, T6, T7>(entity, out var slot, c0, c1, c2, c3, c4, c5, c6, c7);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
    public Entity CreateEntity<T0, T1, T2, T3, T4, T5, T6, T7, T8>(in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8)
    {
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8>(entity, out var slot, c0, c1, c2, c3, c4, c5, c6, c7, c8);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
    public Entity CreateEntity<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9)
    {
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(entity, out var slot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
    public Entity CreateEntity<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10)
    {
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(entity, out var slot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
    public Entity CreateEntity<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11)
    {
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(entity, out var slot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
    public Entity CreateEntity<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12)
    {
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(entity, out var slot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
    public Entity CreateEntity<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13)
    {
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(entity, out var slot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
    public Entity CreateEntity<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14)
    {
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(entity, out var slot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
    public Entity CreateEntity<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14, in T15 c15)
    {
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.GroupTypeInfo;

        var entity = RecycleOrCreateEntity();

        var archetype = GetOrCreateArchetype(types);
        var allocatedEntities = archetype.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(entity, out var slot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15);

        entityCapacity += allocatedEntities;
        entityDatas.EnsureCapacity(entityCapacity);

        entityDatas.Add(entity.Id, archetype, slot, entity.Version);

        return entity;
    }
}
