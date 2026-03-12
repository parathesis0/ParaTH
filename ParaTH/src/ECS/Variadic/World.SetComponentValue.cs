namespace ParaTH;

public sealed partial class World
{
    public void SetComponentValue<T0, T1>(Entity entity, in T0 c0, in T1 c1)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1>(slot, c0, c1);
    }
    public void SetComponentValue<T0, T1, T2>(Entity entity, in T0 c0, in T1 c1, in T2 c2)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1, T2>(slot, c0, c1, c2);
    }
    public void SetComponentValue<T0, T1, T2, T3>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1, T2, T3>(slot, c0, c1, c2, c3);
    }
    public void SetComponentValue<T0, T1, T2, T3, T4>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1, T2, T3, T4>(slot, c0, c1, c2, c3, c4);
    }
    public void SetComponentValue<T0, T1, T2, T3, T4, T5>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1, T2, T3, T4, T5>(slot, c0, c1, c2, c3, c4, c5);
    }
    public void SetComponentValue<T0, T1, T2, T3, T4, T5, T6>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1, T2, T3, T4, T5, T6>(slot, c0, c1, c2, c3, c4, c5, c6);
    }
    public void SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1, T2, T3, T4, T5, T6, T7>(slot, c0, c1, c2, c3, c4, c5, c6, c7);
    }
    public void SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8>(slot, c0, c1, c2, c3, c4, c5, c6, c7, c8);
    }
    public void SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(slot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9);
    }
    public void SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(slot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);
    }
    public void SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(slot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11);
    }
    public void SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(slot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12);
    }
    public void SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(slot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13);
    }
    public void SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(slot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14);
    }
    public void SetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14, in T15 c15)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var archetype = entityData.Archetype;
        var slot = entityData.Slot;

        archetype.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(slot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15);
    }
}
