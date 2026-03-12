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
    public readonly void Set<T0, T1, T2>(int index, in T0 c0, in T1 c1, in T2 c2)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var arr2 = ref GetComponentArrayReference<T2>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
        Unsafe.Add(ref arr2, index) = c2;
    }
    public readonly void Set<T0, T1, T2, T3>(int index, in T0 c0, in T1 c1, in T2 c2, in T3 c3)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var arr3 = ref GetComponentArrayReference<T3>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
        Unsafe.Add(ref arr2, index) = c2;
        Unsafe.Add(ref arr3, index) = c3;
    }
    public readonly void Set<T0, T1, T2, T3, T4>(int index, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var arr4 = ref GetComponentArrayReference<T4>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
        Unsafe.Add(ref arr2, index) = c2;
        Unsafe.Add(ref arr3, index) = c3;
        Unsafe.Add(ref arr4, index) = c4;
    }
    public readonly void Set<T0, T1, T2, T3, T4, T5>(int index, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var arr5 = ref GetComponentArrayReference<T5>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
        Unsafe.Add(ref arr2, index) = c2;
        Unsafe.Add(ref arr3, index) = c3;
        Unsafe.Add(ref arr4, index) = c4;
        Unsafe.Add(ref arr5, index) = c5;
    }
    public readonly void Set<T0, T1, T2, T3, T4, T5, T6>(int index, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var arr6 = ref GetComponentArrayReference<T6>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
        Unsafe.Add(ref arr2, index) = c2;
        Unsafe.Add(ref arr3, index) = c3;
        Unsafe.Add(ref arr4, index) = c4;
        Unsafe.Add(ref arr5, index) = c5;
        Unsafe.Add(ref arr6, index) = c6;
    }
    public readonly void Set<T0, T1, T2, T3, T4, T5, T6, T7>(int index, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var arr7 = ref GetComponentArrayReference<T7>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
        Unsafe.Add(ref arr2, index) = c2;
        Unsafe.Add(ref arr3, index) = c3;
        Unsafe.Add(ref arr4, index) = c4;
        Unsafe.Add(ref arr5, index) = c5;
        Unsafe.Add(ref arr6, index) = c6;
        Unsafe.Add(ref arr7, index) = c7;
    }
    public readonly void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8>(int index, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var arr8 = ref GetComponentArrayReference<T8>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
        Unsafe.Add(ref arr2, index) = c2;
        Unsafe.Add(ref arr3, index) = c3;
        Unsafe.Add(ref arr4, index) = c4;
        Unsafe.Add(ref arr5, index) = c5;
        Unsafe.Add(ref arr6, index) = c6;
        Unsafe.Add(ref arr7, index) = c7;
        Unsafe.Add(ref arr8, index) = c8;
    }
    public readonly void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(int index, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var arr9 = ref GetComponentArrayReference<T9>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
        Unsafe.Add(ref arr2, index) = c2;
        Unsafe.Add(ref arr3, index) = c3;
        Unsafe.Add(ref arr4, index) = c4;
        Unsafe.Add(ref arr5, index) = c5;
        Unsafe.Add(ref arr6, index) = c6;
        Unsafe.Add(ref arr7, index) = c7;
        Unsafe.Add(ref arr8, index) = c8;
        Unsafe.Add(ref arr9, index) = c9;
    }
    public readonly void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(int index, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var arr9 = ref GetComponentArrayReference<T9>();
        ref var arr10 = ref GetComponentArrayReference<T10>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
        Unsafe.Add(ref arr2, index) = c2;
        Unsafe.Add(ref arr3, index) = c3;
        Unsafe.Add(ref arr4, index) = c4;
        Unsafe.Add(ref arr5, index) = c5;
        Unsafe.Add(ref arr6, index) = c6;
        Unsafe.Add(ref arr7, index) = c7;
        Unsafe.Add(ref arr8, index) = c8;
        Unsafe.Add(ref arr9, index) = c9;
        Unsafe.Add(ref arr10, index) = c10;
    }
    public readonly void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(int index, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var arr9 = ref GetComponentArrayReference<T9>();
        ref var arr10 = ref GetComponentArrayReference<T10>();
        ref var arr11 = ref GetComponentArrayReference<T11>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
        Unsafe.Add(ref arr2, index) = c2;
        Unsafe.Add(ref arr3, index) = c3;
        Unsafe.Add(ref arr4, index) = c4;
        Unsafe.Add(ref arr5, index) = c5;
        Unsafe.Add(ref arr6, index) = c6;
        Unsafe.Add(ref arr7, index) = c7;
        Unsafe.Add(ref arr8, index) = c8;
        Unsafe.Add(ref arr9, index) = c9;
        Unsafe.Add(ref arr10, index) = c10;
        Unsafe.Add(ref arr11, index) = c11;
    }
    public readonly void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(int index, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var arr9 = ref GetComponentArrayReference<T9>();
        ref var arr10 = ref GetComponentArrayReference<T10>();
        ref var arr11 = ref GetComponentArrayReference<T11>();
        ref var arr12 = ref GetComponentArrayReference<T12>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
        Unsafe.Add(ref arr2, index) = c2;
        Unsafe.Add(ref arr3, index) = c3;
        Unsafe.Add(ref arr4, index) = c4;
        Unsafe.Add(ref arr5, index) = c5;
        Unsafe.Add(ref arr6, index) = c6;
        Unsafe.Add(ref arr7, index) = c7;
        Unsafe.Add(ref arr8, index) = c8;
        Unsafe.Add(ref arr9, index) = c9;
        Unsafe.Add(ref arr10, index) = c10;
        Unsafe.Add(ref arr11, index) = c11;
        Unsafe.Add(ref arr12, index) = c12;
    }
    public readonly void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(int index, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var arr9 = ref GetComponentArrayReference<T9>();
        ref var arr10 = ref GetComponentArrayReference<T10>();
        ref var arr11 = ref GetComponentArrayReference<T11>();
        ref var arr12 = ref GetComponentArrayReference<T12>();
        ref var arr13 = ref GetComponentArrayReference<T13>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
        Unsafe.Add(ref arr2, index) = c2;
        Unsafe.Add(ref arr3, index) = c3;
        Unsafe.Add(ref arr4, index) = c4;
        Unsafe.Add(ref arr5, index) = c5;
        Unsafe.Add(ref arr6, index) = c6;
        Unsafe.Add(ref arr7, index) = c7;
        Unsafe.Add(ref arr8, index) = c8;
        Unsafe.Add(ref arr9, index) = c9;
        Unsafe.Add(ref arr10, index) = c10;
        Unsafe.Add(ref arr11, index) = c11;
        Unsafe.Add(ref arr12, index) = c12;
        Unsafe.Add(ref arr13, index) = c13;
    }
    public readonly void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(int index, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var arr9 = ref GetComponentArrayReference<T9>();
        ref var arr10 = ref GetComponentArrayReference<T10>();
        ref var arr11 = ref GetComponentArrayReference<T11>();
        ref var arr12 = ref GetComponentArrayReference<T12>();
        ref var arr13 = ref GetComponentArrayReference<T13>();
        ref var arr14 = ref GetComponentArrayReference<T14>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
        Unsafe.Add(ref arr2, index) = c2;
        Unsafe.Add(ref arr3, index) = c3;
        Unsafe.Add(ref arr4, index) = c4;
        Unsafe.Add(ref arr5, index) = c5;
        Unsafe.Add(ref arr6, index) = c6;
        Unsafe.Add(ref arr7, index) = c7;
        Unsafe.Add(ref arr8, index) = c8;
        Unsafe.Add(ref arr9, index) = c9;
        Unsafe.Add(ref arr10, index) = c10;
        Unsafe.Add(ref arr11, index) = c11;
        Unsafe.Add(ref arr12, index) = c12;
        Unsafe.Add(ref arr13, index) = c13;
        Unsafe.Add(ref arr14, index) = c14;
    }
    public readonly void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(int index, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14, in T15 c15)
    {
        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var arr9 = ref GetComponentArrayReference<T9>();
        ref var arr10 = ref GetComponentArrayReference<T10>();
        ref var arr11 = ref GetComponentArrayReference<T11>();
        ref var arr12 = ref GetComponentArrayReference<T12>();
        ref var arr13 = ref GetComponentArrayReference<T13>();
        ref var arr14 = ref GetComponentArrayReference<T14>();
        ref var arr15 = ref GetComponentArrayReference<T15>();

        Unsafe.Add(ref arr0, index) = c0;
        Unsafe.Add(ref arr1, index) = c1;
        Unsafe.Add(ref arr2, index) = c2;
        Unsafe.Add(ref arr3, index) = c3;
        Unsafe.Add(ref arr4, index) = c4;
        Unsafe.Add(ref arr5, index) = c5;
        Unsafe.Add(ref arr6, index) = c6;
        Unsafe.Add(ref arr7, index) = c7;
        Unsafe.Add(ref arr8, index) = c8;
        Unsafe.Add(ref arr9, index) = c9;
        Unsafe.Add(ref arr10, index) = c10;
        Unsafe.Add(ref arr11, index) = c11;
        Unsafe.Add(ref arr12, index) = c12;
        Unsafe.Add(ref arr13, index) = c13;
        Unsafe.Add(ref arr14, index) = c14;
        Unsafe.Add(ref arr15, index) = c15;
    }
}
