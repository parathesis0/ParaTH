namespace ParaTH;

public partial struct Chunk
{
    public static void Fill<T0, T1>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
    }
    public static void Fill<T0, T1, T2>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1, in T2 c2)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
        chunk.GetFullComponentSpan<T2>(out var s2);
        s2.Slice(startIndex, length).Fill(c2);
    }
    public static void Fill<T0, T1, T2, T3>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1, in T2 c2, in T3 c3)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
        chunk.GetFullComponentSpan<T2>(out var s2);
        s2.Slice(startIndex, length).Fill(c2);
        chunk.GetFullComponentSpan<T3>(out var s3);
        s3.Slice(startIndex, length).Fill(c3);
    }
    public static void Fill<T0, T1, T2, T3, T4>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
        chunk.GetFullComponentSpan<T2>(out var s2);
        s2.Slice(startIndex, length).Fill(c2);
        chunk.GetFullComponentSpan<T3>(out var s3);
        s3.Slice(startIndex, length).Fill(c3);
        chunk.GetFullComponentSpan<T4>(out var s4);
        s4.Slice(startIndex, length).Fill(c4);
    }
    public static void Fill<T0, T1, T2, T3, T4, T5>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
        chunk.GetFullComponentSpan<T2>(out var s2);
        s2.Slice(startIndex, length).Fill(c2);
        chunk.GetFullComponentSpan<T3>(out var s3);
        s3.Slice(startIndex, length).Fill(c3);
        chunk.GetFullComponentSpan<T4>(out var s4);
        s4.Slice(startIndex, length).Fill(c4);
        chunk.GetFullComponentSpan<T5>(out var s5);
        s5.Slice(startIndex, length).Fill(c5);
    }
    public static void Fill<T0, T1, T2, T3, T4, T5, T6>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
        chunk.GetFullComponentSpan<T2>(out var s2);
        s2.Slice(startIndex, length).Fill(c2);
        chunk.GetFullComponentSpan<T3>(out var s3);
        s3.Slice(startIndex, length).Fill(c3);
        chunk.GetFullComponentSpan<T4>(out var s4);
        s4.Slice(startIndex, length).Fill(c4);
        chunk.GetFullComponentSpan<T5>(out var s5);
        s5.Slice(startIndex, length).Fill(c5);
        chunk.GetFullComponentSpan<T6>(out var s6);
        s6.Slice(startIndex, length).Fill(c6);
    }
    public static void Fill<T0, T1, T2, T3, T4, T5, T6, T7>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
        chunk.GetFullComponentSpan<T2>(out var s2);
        s2.Slice(startIndex, length).Fill(c2);
        chunk.GetFullComponentSpan<T3>(out var s3);
        s3.Slice(startIndex, length).Fill(c3);
        chunk.GetFullComponentSpan<T4>(out var s4);
        s4.Slice(startIndex, length).Fill(c4);
        chunk.GetFullComponentSpan<T5>(out var s5);
        s5.Slice(startIndex, length).Fill(c5);
        chunk.GetFullComponentSpan<T6>(out var s6);
        s6.Slice(startIndex, length).Fill(c6);
        chunk.GetFullComponentSpan<T7>(out var s7);
        s7.Slice(startIndex, length).Fill(c7);
    }
    public static void Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
        chunk.GetFullComponentSpan<T2>(out var s2);
        s2.Slice(startIndex, length).Fill(c2);
        chunk.GetFullComponentSpan<T3>(out var s3);
        s3.Slice(startIndex, length).Fill(c3);
        chunk.GetFullComponentSpan<T4>(out var s4);
        s4.Slice(startIndex, length).Fill(c4);
        chunk.GetFullComponentSpan<T5>(out var s5);
        s5.Slice(startIndex, length).Fill(c5);
        chunk.GetFullComponentSpan<T6>(out var s6);
        s6.Slice(startIndex, length).Fill(c6);
        chunk.GetFullComponentSpan<T7>(out var s7);
        s7.Slice(startIndex, length).Fill(c7);
        chunk.GetFullComponentSpan<T8>(out var s8);
        s8.Slice(startIndex, length).Fill(c8);
    }
    public static void Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
        chunk.GetFullComponentSpan<T2>(out var s2);
        s2.Slice(startIndex, length).Fill(c2);
        chunk.GetFullComponentSpan<T3>(out var s3);
        s3.Slice(startIndex, length).Fill(c3);
        chunk.GetFullComponentSpan<T4>(out var s4);
        s4.Slice(startIndex, length).Fill(c4);
        chunk.GetFullComponentSpan<T5>(out var s5);
        s5.Slice(startIndex, length).Fill(c5);
        chunk.GetFullComponentSpan<T6>(out var s6);
        s6.Slice(startIndex, length).Fill(c6);
        chunk.GetFullComponentSpan<T7>(out var s7);
        s7.Slice(startIndex, length).Fill(c7);
        chunk.GetFullComponentSpan<T8>(out var s8);
        s8.Slice(startIndex, length).Fill(c8);
        chunk.GetFullComponentSpan<T9>(out var s9);
        s9.Slice(startIndex, length).Fill(c9);
    }
    public static void Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
        chunk.GetFullComponentSpan<T2>(out var s2);
        s2.Slice(startIndex, length).Fill(c2);
        chunk.GetFullComponentSpan<T3>(out var s3);
        s3.Slice(startIndex, length).Fill(c3);
        chunk.GetFullComponentSpan<T4>(out var s4);
        s4.Slice(startIndex, length).Fill(c4);
        chunk.GetFullComponentSpan<T5>(out var s5);
        s5.Slice(startIndex, length).Fill(c5);
        chunk.GetFullComponentSpan<T6>(out var s6);
        s6.Slice(startIndex, length).Fill(c6);
        chunk.GetFullComponentSpan<T7>(out var s7);
        s7.Slice(startIndex, length).Fill(c7);
        chunk.GetFullComponentSpan<T8>(out var s8);
        s8.Slice(startIndex, length).Fill(c8);
        chunk.GetFullComponentSpan<T9>(out var s9);
        s9.Slice(startIndex, length).Fill(c9);
        chunk.GetFullComponentSpan<T10>(out var s10);
        s10.Slice(startIndex, length).Fill(c10);
    }
    public static void Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
        chunk.GetFullComponentSpan<T2>(out var s2);
        s2.Slice(startIndex, length).Fill(c2);
        chunk.GetFullComponentSpan<T3>(out var s3);
        s3.Slice(startIndex, length).Fill(c3);
        chunk.GetFullComponentSpan<T4>(out var s4);
        s4.Slice(startIndex, length).Fill(c4);
        chunk.GetFullComponentSpan<T5>(out var s5);
        s5.Slice(startIndex, length).Fill(c5);
        chunk.GetFullComponentSpan<T6>(out var s6);
        s6.Slice(startIndex, length).Fill(c6);
        chunk.GetFullComponentSpan<T7>(out var s7);
        s7.Slice(startIndex, length).Fill(c7);
        chunk.GetFullComponentSpan<T8>(out var s8);
        s8.Slice(startIndex, length).Fill(c8);
        chunk.GetFullComponentSpan<T9>(out var s9);
        s9.Slice(startIndex, length).Fill(c9);
        chunk.GetFullComponentSpan<T10>(out var s10);
        s10.Slice(startIndex, length).Fill(c10);
        chunk.GetFullComponentSpan<T11>(out var s11);
        s11.Slice(startIndex, length).Fill(c11);
    }
    public static void Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
        chunk.GetFullComponentSpan<T2>(out var s2);
        s2.Slice(startIndex, length).Fill(c2);
        chunk.GetFullComponentSpan<T3>(out var s3);
        s3.Slice(startIndex, length).Fill(c3);
        chunk.GetFullComponentSpan<T4>(out var s4);
        s4.Slice(startIndex, length).Fill(c4);
        chunk.GetFullComponentSpan<T5>(out var s5);
        s5.Slice(startIndex, length).Fill(c5);
        chunk.GetFullComponentSpan<T6>(out var s6);
        s6.Slice(startIndex, length).Fill(c6);
        chunk.GetFullComponentSpan<T7>(out var s7);
        s7.Slice(startIndex, length).Fill(c7);
        chunk.GetFullComponentSpan<T8>(out var s8);
        s8.Slice(startIndex, length).Fill(c8);
        chunk.GetFullComponentSpan<T9>(out var s9);
        s9.Slice(startIndex, length).Fill(c9);
        chunk.GetFullComponentSpan<T10>(out var s10);
        s10.Slice(startIndex, length).Fill(c10);
        chunk.GetFullComponentSpan<T11>(out var s11);
        s11.Slice(startIndex, length).Fill(c11);
        chunk.GetFullComponentSpan<T12>(out var s12);
        s12.Slice(startIndex, length).Fill(c12);
    }
    public static void Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
        chunk.GetFullComponentSpan<T2>(out var s2);
        s2.Slice(startIndex, length).Fill(c2);
        chunk.GetFullComponentSpan<T3>(out var s3);
        s3.Slice(startIndex, length).Fill(c3);
        chunk.GetFullComponentSpan<T4>(out var s4);
        s4.Slice(startIndex, length).Fill(c4);
        chunk.GetFullComponentSpan<T5>(out var s5);
        s5.Slice(startIndex, length).Fill(c5);
        chunk.GetFullComponentSpan<T6>(out var s6);
        s6.Slice(startIndex, length).Fill(c6);
        chunk.GetFullComponentSpan<T7>(out var s7);
        s7.Slice(startIndex, length).Fill(c7);
        chunk.GetFullComponentSpan<T8>(out var s8);
        s8.Slice(startIndex, length).Fill(c8);
        chunk.GetFullComponentSpan<T9>(out var s9);
        s9.Slice(startIndex, length).Fill(c9);
        chunk.GetFullComponentSpan<T10>(out var s10);
        s10.Slice(startIndex, length).Fill(c10);
        chunk.GetFullComponentSpan<T11>(out var s11);
        s11.Slice(startIndex, length).Fill(c11);
        chunk.GetFullComponentSpan<T12>(out var s12);
        s12.Slice(startIndex, length).Fill(c12);
        chunk.GetFullComponentSpan<T13>(out var s13);
        s13.Slice(startIndex, length).Fill(c13);
    }
    public static void Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
        chunk.GetFullComponentSpan<T2>(out var s2);
        s2.Slice(startIndex, length).Fill(c2);
        chunk.GetFullComponentSpan<T3>(out var s3);
        s3.Slice(startIndex, length).Fill(c3);
        chunk.GetFullComponentSpan<T4>(out var s4);
        s4.Slice(startIndex, length).Fill(c4);
        chunk.GetFullComponentSpan<T5>(out var s5);
        s5.Slice(startIndex, length).Fill(c5);
        chunk.GetFullComponentSpan<T6>(out var s6);
        s6.Slice(startIndex, length).Fill(c6);
        chunk.GetFullComponentSpan<T7>(out var s7);
        s7.Slice(startIndex, length).Fill(c7);
        chunk.GetFullComponentSpan<T8>(out var s8);
        s8.Slice(startIndex, length).Fill(c8);
        chunk.GetFullComponentSpan<T9>(out var s9);
        s9.Slice(startIndex, length).Fill(c9);
        chunk.GetFullComponentSpan<T10>(out var s10);
        s10.Slice(startIndex, length).Fill(c10);
        chunk.GetFullComponentSpan<T11>(out var s11);
        s11.Slice(startIndex, length).Fill(c11);
        chunk.GetFullComponentSpan<T12>(out var s12);
        s12.Slice(startIndex, length).Fill(c12);
        chunk.GetFullComponentSpan<T13>(out var s13);
        s13.Slice(startIndex, length).Fill(c13);
        chunk.GetFullComponentSpan<T14>(out var s14);
        s14.Slice(startIndex, length).Fill(c14);
    }
    public static void Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref Chunk chunk, int startIndex, int length, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14, in T15 c15)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        s0.Slice(startIndex, length).Fill(c0);
        chunk.GetFullComponentSpan<T1>(out var s1);
        s1.Slice(startIndex, length).Fill(c1);
        chunk.GetFullComponentSpan<T2>(out var s2);
        s2.Slice(startIndex, length).Fill(c2);
        chunk.GetFullComponentSpan<T3>(out var s3);
        s3.Slice(startIndex, length).Fill(c3);
        chunk.GetFullComponentSpan<T4>(out var s4);
        s4.Slice(startIndex, length).Fill(c4);
        chunk.GetFullComponentSpan<T5>(out var s5);
        s5.Slice(startIndex, length).Fill(c5);
        chunk.GetFullComponentSpan<T6>(out var s6);
        s6.Slice(startIndex, length).Fill(c6);
        chunk.GetFullComponentSpan<T7>(out var s7);
        s7.Slice(startIndex, length).Fill(c7);
        chunk.GetFullComponentSpan<T8>(out var s8);
        s8.Slice(startIndex, length).Fill(c8);
        chunk.GetFullComponentSpan<T9>(out var s9);
        s9.Slice(startIndex, length).Fill(c9);
        chunk.GetFullComponentSpan<T10>(out var s10);
        s10.Slice(startIndex, length).Fill(c10);
        chunk.GetFullComponentSpan<T11>(out var s11);
        s11.Slice(startIndex, length).Fill(c11);
        chunk.GetFullComponentSpan<T12>(out var s12);
        s12.Slice(startIndex, length).Fill(c12);
        chunk.GetFullComponentSpan<T13>(out var s13);
        s13.Slice(startIndex, length).Fill(c13);
        chunk.GetFullComponentSpan<T14>(out var s14);
        s14.Slice(startIndex, length).Fill(c14);
        chunk.GetFullComponentSpan<T15>(out var s15);
        s15.Slice(startIndex, length).Fill(c15);
    }
}
