using System.Runtime.CompilerServices;

namespace ParaTH;

public partial struct Chunk
{
    public readonly Components<T0, T1> Get<T0, T1>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);

        return new Components<T0, T1>(ref c0, ref c1);
    }
    public readonly Components<T0, T1, T2> Get<T0, T1, T2>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var c2 = ref Unsafe.Add(ref arr2, index);

        return new Components<T0, T1, T2>(ref c0, ref c1, ref c2);
    }
    public readonly Components<T0, T1, T2, T3> Get<T0, T1, T2, T3>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var c2 = ref Unsafe.Add(ref arr2, index);
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var c3 = ref Unsafe.Add(ref arr3, index);

        return new Components<T0, T1, T2, T3>(ref c0, ref c1, ref c2, ref c3);
    }
    public readonly Components<T0, T1, T2, T3, T4> Get<T0, T1, T2, T3, T4>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var c2 = ref Unsafe.Add(ref arr2, index);
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var c3 = ref Unsafe.Add(ref arr3, index);
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var c4 = ref Unsafe.Add(ref arr4, index);

        return new Components<T0, T1, T2, T3, T4>(ref c0, ref c1, ref c2, ref c3, ref c4);
    }
    public readonly Components<T0, T1, T2, T3, T4, T5> Get<T0, T1, T2, T3, T4, T5>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var c2 = ref Unsafe.Add(ref arr2, index);
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var c3 = ref Unsafe.Add(ref arr3, index);
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var c4 = ref Unsafe.Add(ref arr4, index);
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var c5 = ref Unsafe.Add(ref arr5, index);

        return new Components<T0, T1, T2, T3, T4, T5>(ref c0, ref c1, ref c2, ref c3, ref c4, ref c5);
    }
    public readonly Components<T0, T1, T2, T3, T4, T5, T6> Get<T0, T1, T2, T3, T4, T5, T6>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var c2 = ref Unsafe.Add(ref arr2, index);
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var c3 = ref Unsafe.Add(ref arr3, index);
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var c4 = ref Unsafe.Add(ref arr4, index);
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var c5 = ref Unsafe.Add(ref arr5, index);
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var c6 = ref Unsafe.Add(ref arr6, index);

        return new Components<T0, T1, T2, T3, T4, T5, T6>(ref c0, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6);
    }
    public readonly Components<T0, T1, T2, T3, T4, T5, T6, T7> Get<T0, T1, T2, T3, T4, T5, T6, T7>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var c2 = ref Unsafe.Add(ref arr2, index);
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var c3 = ref Unsafe.Add(ref arr3, index);
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var c4 = ref Unsafe.Add(ref arr4, index);
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var c5 = ref Unsafe.Add(ref arr5, index);
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var c6 = ref Unsafe.Add(ref arr6, index);
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var c7 = ref Unsafe.Add(ref arr7, index);

        return new Components<T0, T1, T2, T3, T4, T5, T6, T7>(ref c0, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7);
    }
    public readonly Components<T0, T1, T2, T3, T4, T5, T6, T7, T8> Get<T0, T1, T2, T3, T4, T5, T6, T7, T8>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var c2 = ref Unsafe.Add(ref arr2, index);
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var c3 = ref Unsafe.Add(ref arr3, index);
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var c4 = ref Unsafe.Add(ref arr4, index);
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var c5 = ref Unsafe.Add(ref arr5, index);
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var c6 = ref Unsafe.Add(ref arr6, index);
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var c7 = ref Unsafe.Add(ref arr7, index);
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var c8 = ref Unsafe.Add(ref arr8, index);

        return new Components<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref c0, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7, ref c8);
    }
    public readonly Components<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> Get<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var c2 = ref Unsafe.Add(ref arr2, index);
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var c3 = ref Unsafe.Add(ref arr3, index);
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var c4 = ref Unsafe.Add(ref arr4, index);
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var c5 = ref Unsafe.Add(ref arr5, index);
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var c6 = ref Unsafe.Add(ref arr6, index);
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var c7 = ref Unsafe.Add(ref arr7, index);
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var c8 = ref Unsafe.Add(ref arr8, index);
        ref var arr9 = ref GetComponentArrayReference<T9>();
        ref var c9 = ref Unsafe.Add(ref arr9, index);

        return new Components<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref c0, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7, ref c8, ref c9);
    }
    public readonly Components<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Get<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var c2 = ref Unsafe.Add(ref arr2, index);
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var c3 = ref Unsafe.Add(ref arr3, index);
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var c4 = ref Unsafe.Add(ref arr4, index);
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var c5 = ref Unsafe.Add(ref arr5, index);
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var c6 = ref Unsafe.Add(ref arr6, index);
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var c7 = ref Unsafe.Add(ref arr7, index);
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var c8 = ref Unsafe.Add(ref arr8, index);
        ref var arr9 = ref GetComponentArrayReference<T9>();
        ref var c9 = ref Unsafe.Add(ref arr9, index);
        ref var arr10 = ref GetComponentArrayReference<T10>();
        ref var c10 = ref Unsafe.Add(ref arr10, index);

        return new Components<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref c0, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7, ref c8, ref c9, ref c10);
    }
    public readonly Components<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Get<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var c2 = ref Unsafe.Add(ref arr2, index);
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var c3 = ref Unsafe.Add(ref arr3, index);
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var c4 = ref Unsafe.Add(ref arr4, index);
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var c5 = ref Unsafe.Add(ref arr5, index);
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var c6 = ref Unsafe.Add(ref arr6, index);
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var c7 = ref Unsafe.Add(ref arr7, index);
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var c8 = ref Unsafe.Add(ref arr8, index);
        ref var arr9 = ref GetComponentArrayReference<T9>();
        ref var c9 = ref Unsafe.Add(ref arr9, index);
        ref var arr10 = ref GetComponentArrayReference<T10>();
        ref var c10 = ref Unsafe.Add(ref arr10, index);
        ref var arr11 = ref GetComponentArrayReference<T11>();
        ref var c11 = ref Unsafe.Add(ref arr11, index);

        return new Components<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref c0, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7, ref c8, ref c9, ref c10, ref c11);
    }
    public readonly Components<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Get<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var c2 = ref Unsafe.Add(ref arr2, index);
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var c3 = ref Unsafe.Add(ref arr3, index);
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var c4 = ref Unsafe.Add(ref arr4, index);
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var c5 = ref Unsafe.Add(ref arr5, index);
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var c6 = ref Unsafe.Add(ref arr6, index);
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var c7 = ref Unsafe.Add(ref arr7, index);
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var c8 = ref Unsafe.Add(ref arr8, index);
        ref var arr9 = ref GetComponentArrayReference<T9>();
        ref var c9 = ref Unsafe.Add(ref arr9, index);
        ref var arr10 = ref GetComponentArrayReference<T10>();
        ref var c10 = ref Unsafe.Add(ref arr10, index);
        ref var arr11 = ref GetComponentArrayReference<T11>();
        ref var c11 = ref Unsafe.Add(ref arr11, index);
        ref var arr12 = ref GetComponentArrayReference<T12>();
        ref var c12 = ref Unsafe.Add(ref arr12, index);

        return new Components<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref c0, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7, ref c8, ref c9, ref c10, ref c11, ref c12);
    }
    public readonly Components<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Get<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var c2 = ref Unsafe.Add(ref arr2, index);
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var c3 = ref Unsafe.Add(ref arr3, index);
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var c4 = ref Unsafe.Add(ref arr4, index);
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var c5 = ref Unsafe.Add(ref arr5, index);
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var c6 = ref Unsafe.Add(ref arr6, index);
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var c7 = ref Unsafe.Add(ref arr7, index);
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var c8 = ref Unsafe.Add(ref arr8, index);
        ref var arr9 = ref GetComponentArrayReference<T9>();
        ref var c9 = ref Unsafe.Add(ref arr9, index);
        ref var arr10 = ref GetComponentArrayReference<T10>();
        ref var c10 = ref Unsafe.Add(ref arr10, index);
        ref var arr11 = ref GetComponentArrayReference<T11>();
        ref var c11 = ref Unsafe.Add(ref arr11, index);
        ref var arr12 = ref GetComponentArrayReference<T12>();
        ref var c12 = ref Unsafe.Add(ref arr12, index);
        ref var arr13 = ref GetComponentArrayReference<T13>();
        ref var c13 = ref Unsafe.Add(ref arr13, index);

        return new Components<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref c0, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7, ref c8, ref c9, ref c10, ref c11, ref c12, ref c13);
    }
    public readonly Components<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Get<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var c2 = ref Unsafe.Add(ref arr2, index);
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var c3 = ref Unsafe.Add(ref arr3, index);
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var c4 = ref Unsafe.Add(ref arr4, index);
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var c5 = ref Unsafe.Add(ref arr5, index);
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var c6 = ref Unsafe.Add(ref arr6, index);
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var c7 = ref Unsafe.Add(ref arr7, index);
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var c8 = ref Unsafe.Add(ref arr8, index);
        ref var arr9 = ref GetComponentArrayReference<T9>();
        ref var c9 = ref Unsafe.Add(ref arr9, index);
        ref var arr10 = ref GetComponentArrayReference<T10>();
        ref var c10 = ref Unsafe.Add(ref arr10, index);
        ref var arr11 = ref GetComponentArrayReference<T11>();
        ref var c11 = ref Unsafe.Add(ref arr11, index);
        ref var arr12 = ref GetComponentArrayReference<T12>();
        ref var c12 = ref Unsafe.Add(ref arr12, index);
        ref var arr13 = ref GetComponentArrayReference<T13>();
        ref var c13 = ref Unsafe.Add(ref arr13, index);
        ref var arr14 = ref GetComponentArrayReference<T14>();
        ref var c14 = ref Unsafe.Add(ref arr14, index);

        return new Components<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref c0, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7, ref c8, ref c9, ref c10, ref c11, ref c12, ref c13, ref c14);
    }
    public readonly Components<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Get<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(int index)
    {

        ref var arr0 = ref GetComponentArrayReference<T0>();
        ref var c0 = ref Unsafe.Add(ref arr0, index);
        ref var arr1 = ref GetComponentArrayReference<T1>();
        ref var c1 = ref Unsafe.Add(ref arr1, index);
        ref var arr2 = ref GetComponentArrayReference<T2>();
        ref var c2 = ref Unsafe.Add(ref arr2, index);
        ref var arr3 = ref GetComponentArrayReference<T3>();
        ref var c3 = ref Unsafe.Add(ref arr3, index);
        ref var arr4 = ref GetComponentArrayReference<T4>();
        ref var c4 = ref Unsafe.Add(ref arr4, index);
        ref var arr5 = ref GetComponentArrayReference<T5>();
        ref var c5 = ref Unsafe.Add(ref arr5, index);
        ref var arr6 = ref GetComponentArrayReference<T6>();
        ref var c6 = ref Unsafe.Add(ref arr6, index);
        ref var arr7 = ref GetComponentArrayReference<T7>();
        ref var c7 = ref Unsafe.Add(ref arr7, index);
        ref var arr8 = ref GetComponentArrayReference<T8>();
        ref var c8 = ref Unsafe.Add(ref arr8, index);
        ref var arr9 = ref GetComponentArrayReference<T9>();
        ref var c9 = ref Unsafe.Add(ref arr9, index);
        ref var arr10 = ref GetComponentArrayReference<T10>();
        ref var c10 = ref Unsafe.Add(ref arr10, index);
        ref var arr11 = ref GetComponentArrayReference<T11>();
        ref var c11 = ref Unsafe.Add(ref arr11, index);
        ref var arr12 = ref GetComponentArrayReference<T12>();
        ref var c12 = ref Unsafe.Add(ref arr12, index);
        ref var arr13 = ref GetComponentArrayReference<T13>();
        ref var c13 = ref Unsafe.Add(ref arr13, index);
        ref var arr14 = ref GetComponentArrayReference<T14>();
        ref var c14 = ref Unsafe.Add(ref arr14, index);
        ref var arr15 = ref GetComponentArrayReference<T15>();
        ref var c15 = ref Unsafe.Add(ref arr15, index);

        return new Components<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref c0, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7, ref c8, ref c9, ref c10, ref c11, ref c12, ref c13, ref c14, ref c15);
    }
}
