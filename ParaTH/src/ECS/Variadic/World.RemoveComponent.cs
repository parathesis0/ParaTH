using System.Diagnostics;

namespace ParaTH;

// add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to manually merge/remove
public sealed partial class World
{
    public void RemoveComponent<T0, T1>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
    public void RemoveComponent<T0, T1, T2>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1, T2>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1, T2>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1, T2>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
    public void RemoveComponent<T0, T1, T2, T3>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1, T2, T3>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1, T2, T3>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1, T2, T3>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
    public void RemoveComponent<T0, T1, T2, T3, T4>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1, T2, T3, T4>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1, T2, T3, T4>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1, T2, T3, T4>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
    public void RemoveComponent<T0, T1, T2, T3, T4, T5>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1, T2, T3, T4, T5>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1, T2, T3, T4, T5>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1, T2, T3, T4, T5>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1, T2, T3, T4, T5, T6>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1, T2, T3, T4, T5, T6>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1, T2, T3, T4, T5, T6>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1, T2, T3, T4, T5, T6, T7>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1, T2, T3, T4, T5, T6, T7>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1, T2, T3, T4, T5, T6, T7, T8>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));
        Debug.Assert(HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(entity));

        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        var mask = oldArchetype.Mask & ~Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.GroupMask;
        if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
        {
            var oldTypes = oldArchetype.ComponentTypes.AsSpan();
            var removeTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.GroupTypeInfo.AsSpan();
            Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length - removeTypes.Length];

            RemoveTypes(oldTypes, removeTypes, newTypes);
            newArchetype = GetOrCreateArchetype(newTypes);
        }

        Move(ref entityData, oldArchetype, newArchetype);
    }
}
