using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed partial class World
{
#pragma warning disable RCS1242 // Do not pass non-read-only struct by read-only reference
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1>(in QueryDescriptor descriptor, in T0 c0, in T1 c1)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1>(out Span<T0> s0, out Span<T1> s1);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                }
            }
        }
    }
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1, T2>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1, T2>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                    ref var sc2 = ref s2.UnsafeAt(i);
                    sc2 = c2;
                }
            }
        }
    }
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1, T2, T3>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1, T2, T3>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                    ref var sc2 = ref s2.UnsafeAt(i);
                    sc2 = c2;
                    ref var sc3 = ref s3.UnsafeAt(i);
                    sc3 = c3;
                }
            }
        }
    }
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1, T2, T3, T4>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1, T2, T3, T4>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                    ref var sc2 = ref s2.UnsafeAt(i);
                    sc2 = c2;
                    ref var sc3 = ref s3.UnsafeAt(i);
                    sc3 = c3;
                    ref var sc4 = ref s4.UnsafeAt(i);
                    sc4 = c4;
                }
            }
        }
    }
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1, T2, T3, T4, T5>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1, T2, T3, T4, T5>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                    ref var sc2 = ref s2.UnsafeAt(i);
                    sc2 = c2;
                    ref var sc3 = ref s3.UnsafeAt(i);
                    sc3 = c3;
                    ref var sc4 = ref s4.UnsafeAt(i);
                    sc4 = c4;
                    ref var sc5 = ref s5.UnsafeAt(i);
                    sc5 = c5;
                }
            }
        }
    }
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1, T2, T3, T4, T5, T6>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1, T2, T3, T4, T5, T6>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                    ref var sc2 = ref s2.UnsafeAt(i);
                    sc2 = c2;
                    ref var sc3 = ref s3.UnsafeAt(i);
                    sc3 = c3;
                    ref var sc4 = ref s4.UnsafeAt(i);
                    sc4 = c4;
                    ref var sc5 = ref s5.UnsafeAt(i);
                    sc5 = c5;
                    ref var sc6 = ref s6.UnsafeAt(i);
                    sc6 = c6;
                }
            }
        }
    }
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                    ref var sc2 = ref s2.UnsafeAt(i);
                    sc2 = c2;
                    ref var sc3 = ref s3.UnsafeAt(i);
                    sc3 = c3;
                    ref var sc4 = ref s4.UnsafeAt(i);
                    sc4 = c4;
                    ref var sc5 = ref s5.UnsafeAt(i);
                    sc5 = c5;
                    ref var sc6 = ref s6.UnsafeAt(i);
                    sc6 = c6;
                    ref var sc7 = ref s7.UnsafeAt(i);
                    sc7 = c7;
                }
            }
        }
    }
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                    ref var sc2 = ref s2.UnsafeAt(i);
                    sc2 = c2;
                    ref var sc3 = ref s3.UnsafeAt(i);
                    sc3 = c3;
                    ref var sc4 = ref s4.UnsafeAt(i);
                    sc4 = c4;
                    ref var sc5 = ref s5.UnsafeAt(i);
                    sc5 = c5;
                    ref var sc6 = ref s6.UnsafeAt(i);
                    sc6 = c6;
                    ref var sc7 = ref s7.UnsafeAt(i);
                    sc7 = c7;
                    ref var sc8 = ref s8.UnsafeAt(i);
                    sc8 = c8;
                }
            }
        }
    }
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8, out Span<T9> s9);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                    ref var sc2 = ref s2.UnsafeAt(i);
                    sc2 = c2;
                    ref var sc3 = ref s3.UnsafeAt(i);
                    sc3 = c3;
                    ref var sc4 = ref s4.UnsafeAt(i);
                    sc4 = c4;
                    ref var sc5 = ref s5.UnsafeAt(i);
                    sc5 = c5;
                    ref var sc6 = ref s6.UnsafeAt(i);
                    sc6 = c6;
                    ref var sc7 = ref s7.UnsafeAt(i);
                    sc7 = c7;
                    ref var sc8 = ref s8.UnsafeAt(i);
                    sc8 = c8;
                    ref var sc9 = ref s9.UnsafeAt(i);
                    sc9 = c9;
                }
            }
        }
    }
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8, out Span<T9> s9, out Span<T10> s10);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                    ref var sc2 = ref s2.UnsafeAt(i);
                    sc2 = c2;
                    ref var sc3 = ref s3.UnsafeAt(i);
                    sc3 = c3;
                    ref var sc4 = ref s4.UnsafeAt(i);
                    sc4 = c4;
                    ref var sc5 = ref s5.UnsafeAt(i);
                    sc5 = c5;
                    ref var sc6 = ref s6.UnsafeAt(i);
                    sc6 = c6;
                    ref var sc7 = ref s7.UnsafeAt(i);
                    sc7 = c7;
                    ref var sc8 = ref s8.UnsafeAt(i);
                    sc8 = c8;
                    ref var sc9 = ref s9.UnsafeAt(i);
                    sc9 = c9;
                    ref var sc10 = ref s10.UnsafeAt(i);
                    sc10 = c10;
                }
            }
        }
    }
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8, out Span<T9> s9, out Span<T10> s10, out Span<T11> s11);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                    ref var sc2 = ref s2.UnsafeAt(i);
                    sc2 = c2;
                    ref var sc3 = ref s3.UnsafeAt(i);
                    sc3 = c3;
                    ref var sc4 = ref s4.UnsafeAt(i);
                    sc4 = c4;
                    ref var sc5 = ref s5.UnsafeAt(i);
                    sc5 = c5;
                    ref var sc6 = ref s6.UnsafeAt(i);
                    sc6 = c6;
                    ref var sc7 = ref s7.UnsafeAt(i);
                    sc7 = c7;
                    ref var sc8 = ref s8.UnsafeAt(i);
                    sc8 = c8;
                    ref var sc9 = ref s9.UnsafeAt(i);
                    sc9 = c9;
                    ref var sc10 = ref s10.UnsafeAt(i);
                    sc10 = c10;
                    ref var sc11 = ref s11.UnsafeAt(i);
                    sc11 = c11;
                }
            }
        }
    }
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8, out Span<T9> s9, out Span<T10> s10, out Span<T11> s11, out Span<T12> s12);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                    ref var sc2 = ref s2.UnsafeAt(i);
                    sc2 = c2;
                    ref var sc3 = ref s3.UnsafeAt(i);
                    sc3 = c3;
                    ref var sc4 = ref s4.UnsafeAt(i);
                    sc4 = c4;
                    ref var sc5 = ref s5.UnsafeAt(i);
                    sc5 = c5;
                    ref var sc6 = ref s6.UnsafeAt(i);
                    sc6 = c6;
                    ref var sc7 = ref s7.UnsafeAt(i);
                    sc7 = c7;
                    ref var sc8 = ref s8.UnsafeAt(i);
                    sc8 = c8;
                    ref var sc9 = ref s9.UnsafeAt(i);
                    sc9 = c9;
                    ref var sc10 = ref s10.UnsafeAt(i);
                    sc10 = c10;
                    ref var sc11 = ref s11.UnsafeAt(i);
                    sc11 = c11;
                    ref var sc12 = ref s12.UnsafeAt(i);
                    sc12 = c12;
                }
            }
        }
    }
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8, out Span<T9> s9, out Span<T10> s10, out Span<T11> s11, out Span<T12> s12, out Span<T13> s13);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                    ref var sc2 = ref s2.UnsafeAt(i);
                    sc2 = c2;
                    ref var sc3 = ref s3.UnsafeAt(i);
                    sc3 = c3;
                    ref var sc4 = ref s4.UnsafeAt(i);
                    sc4 = c4;
                    ref var sc5 = ref s5.UnsafeAt(i);
                    sc5 = c5;
                    ref var sc6 = ref s6.UnsafeAt(i);
                    sc6 = c6;
                    ref var sc7 = ref s7.UnsafeAt(i);
                    sc7 = c7;
                    ref var sc8 = ref s8.UnsafeAt(i);
                    sc8 = c8;
                    ref var sc9 = ref s9.UnsafeAt(i);
                    sc9 = c9;
                    ref var sc10 = ref s10.UnsafeAt(i);
                    sc10 = c10;
                    ref var sc11 = ref s11.UnsafeAt(i);
                    sc11 = c11;
                    ref var sc12 = ref s12.UnsafeAt(i);
                    sc12 = c12;
                    ref var sc13 = ref s13.UnsafeAt(i);
                    sc13 = c13;
                }
            }
        }
    }
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8, out Span<T9> s9, out Span<T10> s10, out Span<T11> s11, out Span<T12> s12, out Span<T13> s13, out Span<T14> s14);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                    ref var sc2 = ref s2.UnsafeAt(i);
                    sc2 = c2;
                    ref var sc3 = ref s3.UnsafeAt(i);
                    sc3 = c3;
                    ref var sc4 = ref s4.UnsafeAt(i);
                    sc4 = c4;
                    ref var sc5 = ref s5.UnsafeAt(i);
                    sc5 = c5;
                    ref var sc6 = ref s6.UnsafeAt(i);
                    sc6 = c6;
                    ref var sc7 = ref s7.UnsafeAt(i);
                    sc7 = c7;
                    ref var sc8 = ref s8.UnsafeAt(i);
                    sc8 = c8;
                    ref var sc9 = ref s9.UnsafeAt(i);
                    sc9 = c9;
                    ref var sc10 = ref s10.UnsafeAt(i);
                    sc10 = c10;
                    ref var sc11 = ref s11.UnsafeAt(i);
                    sc11 = c11;
                    ref var sc12 = ref s12.UnsafeAt(i);
                    sc12 = c12;
                    ref var sc13 = ref s13.UnsafeAt(i);
                    sc13 = c13;
                    ref var sc14 = ref s14.UnsafeAt(i);
                    sc14 = c14;
                }
            }
        }
    }
    [SkipLocalsInit]
    public void QuerySetComponentValue<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14, in T15 c15)
    {
        var query = GetOrCreateQuery(in descriptor);

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.Chunks.AsSpan())
            {
                chunk.GetComponentSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(out Span<T0> s0, out Span<T1> s1, out Span<T2> s2, out Span<T3> s3, out Span<T4> s4, out Span<T5> s5, out Span<T6> s6, out Span<T7> s7, out Span<T8> s8, out Span<T9> s9, out Span<T10> s10, out Span<T11> s11, out Span<T12> s12, out Span<T13> s13, out Span<T14> s14, out Span<T15> s15);
                var len = s0.Length; // all span length are the same
                for (int i = 0; i < len; i++)
                {
                    ref var sc0 = ref s0.UnsafeAt(i);
                    sc0 = c0;
                    ref var sc1 = ref s1.UnsafeAt(i);
                    sc1 = c1;
                    ref var sc2 = ref s2.UnsafeAt(i);
                    sc2 = c2;
                    ref var sc3 = ref s3.UnsafeAt(i);
                    sc3 = c3;
                    ref var sc4 = ref s4.UnsafeAt(i);
                    sc4 = c4;
                    ref var sc5 = ref s5.UnsafeAt(i);
                    sc5 = c5;
                    ref var sc6 = ref s6.UnsafeAt(i);
                    sc6 = c6;
                    ref var sc7 = ref s7.UnsafeAt(i);
                    sc7 = c7;
                    ref var sc8 = ref s8.UnsafeAt(i);
                    sc8 = c8;
                    ref var sc9 = ref s9.UnsafeAt(i);
                    sc9 = c9;
                    ref var sc10 = ref s10.UnsafeAt(i);
                    sc10 = c10;
                    ref var sc11 = ref s11.UnsafeAt(i);
                    sc11 = c11;
                    ref var sc12 = ref s12.UnsafeAt(i);
                    sc12 = c12;
                    ref var sc13 = ref s13.UnsafeAt(i);
                    sc13 = c13;
                    ref var sc14 = ref s14.UnsafeAt(i);
                    sc14 = c14;
                    ref var sc15 = ref s15.UnsafeAt(i);
                    sc15 = c15;
                }
            }
        }
    }
#pragma warning restore RCS1242 // Do not pass non-read-only struct by read-only reference
}
