namespace ParaTH;

public partial struct Chunk
{
    public int Add<T0, T1>(Entity entity, in T0 c0, in T1 c1)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        EntityCount++;
        return index;
    }
    public int Add<T0, T1, T2>(Entity entity, in T0 c0, in T1 c1, in T2 c2)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        GetComponentArray<T2>().UnsafeAt(index) = c2;
        EntityCount++;
        return index;
    }
    public int Add<T0, T1, T2, T3>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        GetComponentArray<T2>().UnsafeAt(index) = c2;
        GetComponentArray<T3>().UnsafeAt(index) = c3;
        EntityCount++;
        return index;
    }
    public int Add<T0, T1, T2, T3, T4>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        GetComponentArray<T2>().UnsafeAt(index) = c2;
        GetComponentArray<T3>().UnsafeAt(index) = c3;
        GetComponentArray<T4>().UnsafeAt(index) = c4;
        EntityCount++;
        return index;
    }
    public int Add<T0, T1, T2, T3, T4, T5>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        GetComponentArray<T2>().UnsafeAt(index) = c2;
        GetComponentArray<T3>().UnsafeAt(index) = c3;
        GetComponentArray<T4>().UnsafeAt(index) = c4;
        GetComponentArray<T5>().UnsafeAt(index) = c5;
        EntityCount++;
        return index;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        GetComponentArray<T2>().UnsafeAt(index) = c2;
        GetComponentArray<T3>().UnsafeAt(index) = c3;
        GetComponentArray<T4>().UnsafeAt(index) = c4;
        GetComponentArray<T5>().UnsafeAt(index) = c5;
        GetComponentArray<T6>().UnsafeAt(index) = c6;
        EntityCount++;
        return index;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        GetComponentArray<T2>().UnsafeAt(index) = c2;
        GetComponentArray<T3>().UnsafeAt(index) = c3;
        GetComponentArray<T4>().UnsafeAt(index) = c4;
        GetComponentArray<T5>().UnsafeAt(index) = c5;
        GetComponentArray<T6>().UnsafeAt(index) = c6;
        GetComponentArray<T7>().UnsafeAt(index) = c7;
        EntityCount++;
        return index;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        GetComponentArray<T2>().UnsafeAt(index) = c2;
        GetComponentArray<T3>().UnsafeAt(index) = c3;
        GetComponentArray<T4>().UnsafeAt(index) = c4;
        GetComponentArray<T5>().UnsafeAt(index) = c5;
        GetComponentArray<T6>().UnsafeAt(index) = c6;
        GetComponentArray<T7>().UnsafeAt(index) = c7;
        GetComponentArray<T8>().UnsafeAt(index) = c8;
        EntityCount++;
        return index;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        GetComponentArray<T2>().UnsafeAt(index) = c2;
        GetComponentArray<T3>().UnsafeAt(index) = c3;
        GetComponentArray<T4>().UnsafeAt(index) = c4;
        GetComponentArray<T5>().UnsafeAt(index) = c5;
        GetComponentArray<T6>().UnsafeAt(index) = c6;
        GetComponentArray<T7>().UnsafeAt(index) = c7;
        GetComponentArray<T8>().UnsafeAt(index) = c8;
        GetComponentArray<T9>().UnsafeAt(index) = c9;
        EntityCount++;
        return index;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        GetComponentArray<T2>().UnsafeAt(index) = c2;
        GetComponentArray<T3>().UnsafeAt(index) = c3;
        GetComponentArray<T4>().UnsafeAt(index) = c4;
        GetComponentArray<T5>().UnsafeAt(index) = c5;
        GetComponentArray<T6>().UnsafeAt(index) = c6;
        GetComponentArray<T7>().UnsafeAt(index) = c7;
        GetComponentArray<T8>().UnsafeAt(index) = c8;
        GetComponentArray<T9>().UnsafeAt(index) = c9;
        GetComponentArray<T10>().UnsafeAt(index) = c10;
        EntityCount++;
        return index;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        GetComponentArray<T2>().UnsafeAt(index) = c2;
        GetComponentArray<T3>().UnsafeAt(index) = c3;
        GetComponentArray<T4>().UnsafeAt(index) = c4;
        GetComponentArray<T5>().UnsafeAt(index) = c5;
        GetComponentArray<T6>().UnsafeAt(index) = c6;
        GetComponentArray<T7>().UnsafeAt(index) = c7;
        GetComponentArray<T8>().UnsafeAt(index) = c8;
        GetComponentArray<T9>().UnsafeAt(index) = c9;
        GetComponentArray<T10>().UnsafeAt(index) = c10;
        GetComponentArray<T11>().UnsafeAt(index) = c11;
        EntityCount++;
        return index;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        GetComponentArray<T2>().UnsafeAt(index) = c2;
        GetComponentArray<T3>().UnsafeAt(index) = c3;
        GetComponentArray<T4>().UnsafeAt(index) = c4;
        GetComponentArray<T5>().UnsafeAt(index) = c5;
        GetComponentArray<T6>().UnsafeAt(index) = c6;
        GetComponentArray<T7>().UnsafeAt(index) = c7;
        GetComponentArray<T8>().UnsafeAt(index) = c8;
        GetComponentArray<T9>().UnsafeAt(index) = c9;
        GetComponentArray<T10>().UnsafeAt(index) = c10;
        GetComponentArray<T11>().UnsafeAt(index) = c11;
        GetComponentArray<T12>().UnsafeAt(index) = c12;
        EntityCount++;
        return index;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        GetComponentArray<T2>().UnsafeAt(index) = c2;
        GetComponentArray<T3>().UnsafeAt(index) = c3;
        GetComponentArray<T4>().UnsafeAt(index) = c4;
        GetComponentArray<T5>().UnsafeAt(index) = c5;
        GetComponentArray<T6>().UnsafeAt(index) = c6;
        GetComponentArray<T7>().UnsafeAt(index) = c7;
        GetComponentArray<T8>().UnsafeAt(index) = c8;
        GetComponentArray<T9>().UnsafeAt(index) = c9;
        GetComponentArray<T10>().UnsafeAt(index) = c10;
        GetComponentArray<T11>().UnsafeAt(index) = c11;
        GetComponentArray<T12>().UnsafeAt(index) = c12;
        GetComponentArray<T13>().UnsafeAt(index) = c13;
        EntityCount++;
        return index;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        GetComponentArray<T2>().UnsafeAt(index) = c2;
        GetComponentArray<T3>().UnsafeAt(index) = c3;
        GetComponentArray<T4>().UnsafeAt(index) = c4;
        GetComponentArray<T5>().UnsafeAt(index) = c5;
        GetComponentArray<T6>().UnsafeAt(index) = c6;
        GetComponentArray<T7>().UnsafeAt(index) = c7;
        GetComponentArray<T8>().UnsafeAt(index) = c8;
        GetComponentArray<T9>().UnsafeAt(index) = c9;
        GetComponentArray<T10>().UnsafeAt(index) = c10;
        GetComponentArray<T11>().UnsafeAt(index) = c11;
        GetComponentArray<T12>().UnsafeAt(index) = c12;
        GetComponentArray<T13>().UnsafeAt(index) = c13;
        GetComponentArray<T14>().UnsafeAt(index) = c14;
        EntityCount++;
        return index;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Entity entity, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14, in T15 c15)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = c0;
        GetComponentArray<T1>().UnsafeAt(index) = c1;
        GetComponentArray<T2>().UnsafeAt(index) = c2;
        GetComponentArray<T3>().UnsafeAt(index) = c3;
        GetComponentArray<T4>().UnsafeAt(index) = c4;
        GetComponentArray<T5>().UnsafeAt(index) = c5;
        GetComponentArray<T6>().UnsafeAt(index) = c6;
        GetComponentArray<T7>().UnsafeAt(index) = c7;
        GetComponentArray<T8>().UnsafeAt(index) = c8;
        GetComponentArray<T9>().UnsafeAt(index) = c9;
        GetComponentArray<T10>().UnsafeAt(index) = c10;
        GetComponentArray<T11>().UnsafeAt(index) = c11;
        GetComponentArray<T12>().UnsafeAt(index) = c12;
        GetComponentArray<T13>().UnsafeAt(index) = c13;
        GetComponentArray<T14>().UnsafeAt(index) = c14;
        GetComponentArray<T15>().UnsafeAt(index) = c15;
        EntityCount++;
        return index;
    }
}
