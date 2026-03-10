using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParaTH;

public partial struct Chunk
{
    public Components<T0, T1> Get<T0, T1>(int index)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();

        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var c1 = ref Unsafe.Add(ref arr1, index);

        return new Components<T0, T1>(ref c0, ref c1);
    }
}
