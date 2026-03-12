using System.Runtime.InteropServices;

namespace ParaTH;

public partial struct Chunk
{
    public readonly void GetComponentSpan<T0, T1>(out Span<T0> s0, out Span<T1> s1)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
    }
    public readonly void GetComponentSpan<T0, T1, T2>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
        s2 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T2>(), count);
    }
    public readonly void GetComponentSpan<T0, T1, T2, T3>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
        s2 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T2>(), count);
        s3 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T3>(), count);
    }
    public readonly void GetComponentSpan<T0, T1, T2, T3, T4>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
        s2 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T2>(), count);
        s3 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T3>(), count);
        s4 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T4>(), count);
    }
    public readonly void GetComponentSpan<T0, T1, T2, T3, T4, T5>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
        s2 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T2>(), count);
        s3 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T3>(), count);
        s4 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T4>(), count);
        s5 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T5>(), count);
    }
    public readonly void GetComponentSpan<T0, T1, T2, T3, T4, T5, T6>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
        s2 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T2>(), count);
        s3 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T3>(), count);
        s4 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T4>(), count);
        s5 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T5>(), count);
        s6 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T6>(), count);
    }
    public readonly void GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
        s2 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T2>(), count);
        s3 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T3>(), count);
        s4 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T4>(), count);
        s5 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T5>(), count);
        s6 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T6>(), count);
        s7 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T7>(), count);
    }
    public readonly void GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
        s2 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T2>(), count);
        s3 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T3>(), count);
        s4 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T4>(), count);
        s5 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T5>(), count);
        s6 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T6>(), count);
        s7 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T7>(), count);
        s8 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T8>(), count);
    }
    public readonly void GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8, out Span<T9> s9)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
        s2 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T2>(), count);
        s3 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T3>(), count);
        s4 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T4>(), count);
        s5 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T5>(), count);
        s6 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T6>(), count);
        s7 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T7>(), count);
        s8 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T8>(), count);
        s9 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T9>(), count);
    }
    public readonly void GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8, out Span<T9> s9, out Span<T10> s10)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
        s2 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T2>(), count);
        s3 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T3>(), count);
        s4 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T4>(), count);
        s5 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T5>(), count);
        s6 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T6>(), count);
        s7 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T7>(), count);
        s8 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T8>(), count);
        s9 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T9>(), count);
        s10 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T10>(), count);
    }
    public readonly void GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8, out Span<T9> s9, out Span<T10> s10, out Span<T11> s11)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
        s2 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T2>(), count);
        s3 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T3>(), count);
        s4 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T4>(), count);
        s5 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T5>(), count);
        s6 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T6>(), count);
        s7 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T7>(), count);
        s8 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T8>(), count);
        s9 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T9>(), count);
        s10 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T10>(), count);
        s11 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T11>(), count);
    }
    public readonly void GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8, out Span<T9> s9, out Span<T10> s10, out Span<T11> s11, out Span<T12> s12)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
        s2 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T2>(), count);
        s3 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T3>(), count);
        s4 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T4>(), count);
        s5 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T5>(), count);
        s6 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T6>(), count);
        s7 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T7>(), count);
        s8 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T8>(), count);
        s9 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T9>(), count);
        s10 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T10>(), count);
        s11 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T11>(), count);
        s12 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T12>(), count);
    }
    public readonly void GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8, out Span<T9> s9, out Span<T10> s10, out Span<T11> s11, out Span<T12> s12, out Span<T13> s13)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
        s2 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T2>(), count);
        s3 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T3>(), count);
        s4 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T4>(), count);
        s5 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T5>(), count);
        s6 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T6>(), count);
        s7 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T7>(), count);
        s8 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T8>(), count);
        s9 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T9>(), count);
        s10 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T10>(), count);
        s11 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T11>(), count);
        s12 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T12>(), count);
        s13 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T13>(), count);
    }
    public readonly void GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8, out Span<T9> s9, out Span<T10> s10, out Span<T11> s11, out Span<T12> s12, out Span<T13> s13, out Span<T14> s14)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
        s2 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T2>(), count);
        s3 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T3>(), count);
        s4 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T4>(), count);
        s5 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T5>(), count);
        s6 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T6>(), count);
        s7 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T7>(), count);
        s8 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T8>(), count);
        s9 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T9>(), count);
        s10 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T10>(), count);
        s11 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T11>(), count);
        s12 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T12>(), count);
        s13 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T13>(), count);
        s14 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T14>(), count);
    }
    public readonly void GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8, out Span<T9> s9, out Span<T10> s10, out Span<T11> s11, out Span<T12> s12, out Span<T13> s13, out Span<T14> s14, out Span<T15> s15)
    {
        var count = EntityCount;
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), count);
        s1 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T1>(), count);
        s2 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T2>(), count);
        s3 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T3>(), count);
        s4 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T4>(), count);
        s5 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T5>(), count);
        s6 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T6>(), count);
        s7 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T7>(), count);
        s8 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T8>(), count);
        s9 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T9>(), count);
        s10 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T10>(), count);
        s11 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T11>(), count);
        s12 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T12>(), count);
        s13 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T13>(), count);
        s14 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T14>(), count);
        s15 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T15>(), count);
    }
}
