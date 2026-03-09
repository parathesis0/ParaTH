using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParaTH;

public partial struct Chunk
{
    public readonly void Set<T0, T1>(int index, in T0 c0, in T1 c1)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
    }
}
