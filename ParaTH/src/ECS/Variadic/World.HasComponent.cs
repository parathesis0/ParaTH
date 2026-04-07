using System.Diagnostics;

namespace ParaTH;

public sealed partial class World
{
    public bool HasComponent<T0, T1>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1>();
    }
    public bool HasComponent<T0, T1, T2>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1, T2>();
    }
    public bool HasComponent<T0, T1, T2, T3>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1, T2, T3>();
    }
    public bool HasComponent<T0, T1, T2, T3, T4>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1, T2, T3, T4>();
    }
    public bool HasComponent<T0, T1, T2, T3, T4, T5>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1, T2, T3, T4, T5>();
    }
    public bool HasComponent<T0, T1, T2, T3, T4, T5, T6>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1, T2, T3, T4, T5, T6>();
    }
    public bool HasComponent<T0, T1, T2, T3, T4, T5, T6, T7>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1, T2, T3, T4, T5, T6, T7>();
    }
    public bool HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1, T2, T3, T4, T5, T6, T7, T8>();
    }
    public bool HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>();
    }
    public bool HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
    }
    public bool HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();
    }
    public bool HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>();
    }
    public bool HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>();
    }
    public bool HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>();
    }
    public bool HasComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Entity entity)
    {
        Debug.Assert(IsAlive(entity));

        var archetype = entityDatas.GetArchetype(entity.Id);
        return archetype.Has<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>();
    }
}
