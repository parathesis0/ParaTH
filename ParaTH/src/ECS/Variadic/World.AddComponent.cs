using System.Diagnostics;

namespace ParaTH;

// add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to manually merge/remove
public sealed partial class World
{
    public void AddComponent<T0, T1>(Entity entity, in T0 c0, in T1 c1)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1>(entity, c0, c1);
    }
    public void AddComponent<T0, T1, T2>(Entity entity, in T0 c0, in T1 c1, in T2 c2)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1, T2>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1, T2>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1, T2>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2>(entity, c0, c1, c2);
    }
    public void AddComponent<T0, T1, T2, T3>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1, T2, T3>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1, T2, T3>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1, T2, T3>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3>(entity, c0, c1, c2, c3);
    }
    public void AddComponent<T0, T1, T2, T3, T4>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1, T2, T3, T4>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1, T2, T3, T4>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1, T2, T3, T4>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4>(entity, c0, c1, c2, c3, c4);
    }
    public void AddComponent<T0, T1, T2, T3, T4, T5>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1, T2, T3, T4, T5>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1, T2, T3, T4, T5>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1, T2, T3, T4, T5>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5>(entity, c0, c1, c2, c3, c4, c5);
    }
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1, T2, T3, T4, T5, T6>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1, T2, T3, T4, T5, T6>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6>(entity, c0, c1, c2, c3, c4, c5, c6);
    }
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1, T2, T3, T4, T5, T6, T7>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1, T2, T3, T4, T5, T6, T7>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7>(entity, c0, c1, c2, c3, c4, c5, c6, c7);
    }
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8);
    }
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9);
    }
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);
    }
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11);
    }
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12);
    }
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13);
    }
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14);
    }
    public void AddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14, in T15 c15)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(!HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var mask = oldArchetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var addtypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addtypes.Length];

            MergeTypes(oldTypes, addtypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15);
    }
}
