using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed partial class World
{
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1>(entityBuffer, c0, c1);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1, T2>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1, Span<T2> c2)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1, T2>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1, T2>(entityBuffer, c0, c1, c2);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1, T2, T3>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1, T2, T3>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1, T2, T3>(entityBuffer, c0, c1, c2, c3);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1, T2, T3, T4>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1, T2, T3, T4>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1, T2, T3, T4>(entityBuffer, c0, c1, c2, c3, c4);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1, T2, T3, T4, T5>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1, T2, T3, T4, T5>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1, T2, T3, T4, T5>(entityBuffer, c0, c1, c2, c3, c4, c5);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1, T2, T3, T4, T5, T6>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1, T2, T3, T4, T5, T6>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1, T2, T3, T4, T5, T6>(entityBuffer, c0, c1, c2, c3, c4, c5, c6);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1, T2, T3, T4, T5, T6, T7>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1, T2, T3, T4, T5, T6, T7>(entityBuffer, c0, c1, c2, c3, c4, c5, c6, c7);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8>(entityBuffer, c0, c1, c2, c3, c4, c5, c6, c7, c8);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8, Span<T9> c9)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(entityBuffer, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8, Span<T9> c9, Span<T10> c10)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(entityBuffer, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8, Span<T9> c9, Span<T10> c10, Span<T11> c11)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(entityBuffer, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8, Span<T9> c9, Span<T10> c10, Span<T11> c11, Span<T12> c12)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(entityBuffer, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8, Span<T9> c9, Span<T10> c10, Span<T11> c11, Span<T12> c12, Span<T13> c13)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(entityBuffer, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8, Span<T9> c9, Span<T10> c10, Span<T11> c11, Span<T12> c12, Span<T13> c13, Span<T14> c14)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(entityBuffer, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
    [SkipLocalsInit]
    public void CreateEntityBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Span<Entity> entityBuffer, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8, Span<T9> c9, Span<T10> c10, Span<T11> c11, Span<T12> c12, Span<T13> c13, Span<T14> c14, Span<T15> c15)
    {
        var amount = entityBuffer.Length;
        var types = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.GroupTypeInfo;

        var archetype = GetOrCreateArchetypeWithCapacity(types, amount);

        using var entityDataBuffer = ScopedPooledArray<EntityData>.Rent(amount);
        var entityDataBufferSpan = entityDataBuffer.AsSpan();

        RecycleOrCreateEntityBulk(archetype, entityBuffer, entityDataBufferSpan);
        archetype.AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(entityBuffer, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15);

        AddEntityDataBulk(entityBuffer, entityDataBufferSpan);
    }
}
