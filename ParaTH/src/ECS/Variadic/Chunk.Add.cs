using System;
using System.Collections.Generic;

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
}
