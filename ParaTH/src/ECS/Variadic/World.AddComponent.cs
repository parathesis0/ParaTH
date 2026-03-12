using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed partial class World
{
    [SkipLocalsInit]
    public void AddComponent<T0, T1>(Entity entity, in T0 c0, in T1 c1)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1>(entity, c0, c1);
    }
    [SkipLocalsInit]
    public void AddComponent<T0, T1, T2>(Entity entity, in T0 c0, in T1 c1, in T2 c2)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1, T2>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2>(entity, c0, c1, c2);
    }
    [SkipLocalsInit]
    public void AddComponent<T0, T1, T2, T3>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3>(entity, c0, c1, c2, c3);
    }
    [SkipLocalsInit]
    public void AddComponent<T0, T1, T2, T3, T4>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4>(entity, c0, c1, c2, c3, c4);
    }
    [SkipLocalsInit]
    public void AddComponent<T0, T1, T2, T3, T4, T5>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5>(entity, c0, c1, c2, c3, c4, c5);
    }
    [SkipLocalsInit]
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6>(entity, c0, c1, c2, c3, c4, c5, c6);
    }
    [SkipLocalsInit]
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7>(entity, c0, c1, c2, c3, c4, c5, c6, c7);
    }
    [SkipLocalsInit]
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8);
    }
    [SkipLocalsInit]
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9);
    }
    [SkipLocalsInit]
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);
    }
    [SkipLocalsInit]
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11);
    }
    [SkipLocalsInit]
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12);
    }
    [SkipLocalsInit]
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13);
    }
    [SkipLocalsInit]
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14);
    }
    [SkipLocalsInit]
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14, in T15 c15)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15);
    }
}
