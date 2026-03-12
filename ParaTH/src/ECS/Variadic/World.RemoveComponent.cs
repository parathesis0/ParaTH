using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed partial class World
{
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1, T2>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1, T2>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1, T2, T3>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1, T2, T3, T4>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1, T2, T3, T4, T5>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
    [SkipLocalsInit]
    public void RemoveComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
}
