namespace ParaTH;

public partial struct Chunk
{
    public static void FillWithSpan<T0, T1>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
    }
    public static void FillWithSpan<T0, T1, T2>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1, Span<T2> cs2)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T2>(out var s2);
        cs2.CopyTo(s2.Slice(startIndex, length));
    }
    public static void FillWithSpan<T0, T1, T2, T3>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1, Span<T2> cs2, Span<T3> cs3)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T2>(out var s2);
        cs2.CopyTo(s2.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T3>(out var s3);
        cs3.CopyTo(s3.Slice(startIndex, length));
    }
    public static void FillWithSpan<T0, T1, T2, T3, T4>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1, Span<T2> cs2, Span<T3> cs3, Span<T4> cs4)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T2>(out var s2);
        cs2.CopyTo(s2.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T3>(out var s3);
        cs3.CopyTo(s3.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T4>(out var s4);
        cs4.CopyTo(s4.Slice(startIndex, length));
    }
    public static void FillWithSpan<T0, T1, T2, T3, T4, T5>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1, Span<T2> cs2, Span<T3> cs3, Span<T4> cs4, Span<T5> cs5)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T2>(out var s2);
        cs2.CopyTo(s2.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T3>(out var s3);
        cs3.CopyTo(s3.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T4>(out var s4);
        cs4.CopyTo(s4.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T5>(out var s5);
        cs5.CopyTo(s5.Slice(startIndex, length));
    }
    public static void FillWithSpan<T0, T1, T2, T3, T4, T5, T6>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1, Span<T2> cs2, Span<T3> cs3, Span<T4> cs4, Span<T5> cs5, Span<T6> cs6)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T2>(out var s2);
        cs2.CopyTo(s2.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T3>(out var s3);
        cs3.CopyTo(s3.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T4>(out var s4);
        cs4.CopyTo(s4.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T5>(out var s5);
        cs5.CopyTo(s5.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T6>(out var s6);
        cs6.CopyTo(s6.Slice(startIndex, length));
    }
    public static void FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1, Span<T2> cs2, Span<T3> cs3, Span<T4> cs4, Span<T5> cs5, Span<T6> cs6, Span<T7> cs7)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T2>(out var s2);
        cs2.CopyTo(s2.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T3>(out var s3);
        cs3.CopyTo(s3.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T4>(out var s4);
        cs4.CopyTo(s4.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T5>(out var s5);
        cs5.CopyTo(s5.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T6>(out var s6);
        cs6.CopyTo(s6.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T7>(out var s7);
        cs7.CopyTo(s7.Slice(startIndex, length));
    }
    public static void FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1, Span<T2> cs2, Span<T3> cs3, Span<T4> cs4, Span<T5> cs5, Span<T6> cs6, Span<T7> cs7, Span<T8> cs8)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T2>(out var s2);
        cs2.CopyTo(s2.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T3>(out var s3);
        cs3.CopyTo(s3.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T4>(out var s4);
        cs4.CopyTo(s4.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T5>(out var s5);
        cs5.CopyTo(s5.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T6>(out var s6);
        cs6.CopyTo(s6.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T7>(out var s7);
        cs7.CopyTo(s7.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T8>(out var s8);
        cs8.CopyTo(s8.Slice(startIndex, length));
    }
    public static void FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1, Span<T2> cs2, Span<T3> cs3, Span<T4> cs4, Span<T5> cs5, Span<T6> cs6, Span<T7> cs7, Span<T8> cs8, Span<T9> cs9)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T2>(out var s2);
        cs2.CopyTo(s2.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T3>(out var s3);
        cs3.CopyTo(s3.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T4>(out var s4);
        cs4.CopyTo(s4.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T5>(out var s5);
        cs5.CopyTo(s5.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T6>(out var s6);
        cs6.CopyTo(s6.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T7>(out var s7);
        cs7.CopyTo(s7.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T8>(out var s8);
        cs8.CopyTo(s8.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T9>(out var s9);
        cs9.CopyTo(s9.Slice(startIndex, length));
    }
    public static void FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1, Span<T2> cs2, Span<T3> cs3, Span<T4> cs4, Span<T5> cs5, Span<T6> cs6, Span<T7> cs7, Span<T8> cs8, Span<T9> cs9, Span<T10> cs10)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T2>(out var s2);
        cs2.CopyTo(s2.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T3>(out var s3);
        cs3.CopyTo(s3.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T4>(out var s4);
        cs4.CopyTo(s4.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T5>(out var s5);
        cs5.CopyTo(s5.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T6>(out var s6);
        cs6.CopyTo(s6.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T7>(out var s7);
        cs7.CopyTo(s7.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T8>(out var s8);
        cs8.CopyTo(s8.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T9>(out var s9);
        cs9.CopyTo(s9.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T10>(out var s10);
        cs10.CopyTo(s10.Slice(startIndex, length));
    }
    public static void FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1, Span<T2> cs2, Span<T3> cs3, Span<T4> cs4, Span<T5> cs5, Span<T6> cs6, Span<T7> cs7, Span<T8> cs8, Span<T9> cs9, Span<T10> cs10, Span<T11> cs11)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T2>(out var s2);
        cs2.CopyTo(s2.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T3>(out var s3);
        cs3.CopyTo(s3.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T4>(out var s4);
        cs4.CopyTo(s4.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T5>(out var s5);
        cs5.CopyTo(s5.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T6>(out var s6);
        cs6.CopyTo(s6.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T7>(out var s7);
        cs7.CopyTo(s7.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T8>(out var s8);
        cs8.CopyTo(s8.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T9>(out var s9);
        cs9.CopyTo(s9.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T10>(out var s10);
        cs10.CopyTo(s10.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T11>(out var s11);
        cs11.CopyTo(s11.Slice(startIndex, length));
    }
    public static void FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1, Span<T2> cs2, Span<T3> cs3, Span<T4> cs4, Span<T5> cs5, Span<T6> cs6, Span<T7> cs7, Span<T8> cs8, Span<T9> cs9, Span<T10> cs10, Span<T11> cs11, Span<T12> cs12)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T2>(out var s2);
        cs2.CopyTo(s2.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T3>(out var s3);
        cs3.CopyTo(s3.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T4>(out var s4);
        cs4.CopyTo(s4.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T5>(out var s5);
        cs5.CopyTo(s5.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T6>(out var s6);
        cs6.CopyTo(s6.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T7>(out var s7);
        cs7.CopyTo(s7.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T8>(out var s8);
        cs8.CopyTo(s8.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T9>(out var s9);
        cs9.CopyTo(s9.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T10>(out var s10);
        cs10.CopyTo(s10.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T11>(out var s11);
        cs11.CopyTo(s11.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T12>(out var s12);
        cs12.CopyTo(s12.Slice(startIndex, length));
    }
    public static void FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1, Span<T2> cs2, Span<T3> cs3, Span<T4> cs4, Span<T5> cs5, Span<T6> cs6, Span<T7> cs7, Span<T8> cs8, Span<T9> cs9, Span<T10> cs10, Span<T11> cs11, Span<T12> cs12, Span<T13> cs13)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T2>(out var s2);
        cs2.CopyTo(s2.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T3>(out var s3);
        cs3.CopyTo(s3.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T4>(out var s4);
        cs4.CopyTo(s4.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T5>(out var s5);
        cs5.CopyTo(s5.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T6>(out var s6);
        cs6.CopyTo(s6.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T7>(out var s7);
        cs7.CopyTo(s7.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T8>(out var s8);
        cs8.CopyTo(s8.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T9>(out var s9);
        cs9.CopyTo(s9.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T10>(out var s10);
        cs10.CopyTo(s10.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T11>(out var s11);
        cs11.CopyTo(s11.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T12>(out var s12);
        cs12.CopyTo(s12.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T13>(out var s13);
        cs13.CopyTo(s13.Slice(startIndex, length));
    }
    public static void FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1, Span<T2> cs2, Span<T3> cs3, Span<T4> cs4, Span<T5> cs5, Span<T6> cs6, Span<T7> cs7, Span<T8> cs8, Span<T9> cs9, Span<T10> cs10, Span<T11> cs11, Span<T12> cs12, Span<T13> cs13, Span<T14> cs14)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T2>(out var s2);
        cs2.CopyTo(s2.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T3>(out var s3);
        cs3.CopyTo(s3.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T4>(out var s4);
        cs4.CopyTo(s4.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T5>(out var s5);
        cs5.CopyTo(s5.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T6>(out var s6);
        cs6.CopyTo(s6.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T7>(out var s7);
        cs7.CopyTo(s7.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T8>(out var s8);
        cs8.CopyTo(s8.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T9>(out var s9);
        cs9.CopyTo(s9.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T10>(out var s10);
        cs10.CopyTo(s10.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T11>(out var s11);
        cs11.CopyTo(s11.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T12>(out var s12);
        cs12.CopyTo(s12.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T13>(out var s13);
        cs13.CopyTo(s13.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T14>(out var s14);
        cs14.CopyTo(s14.Slice(startIndex, length));
    }
    public static void FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref Chunk chunk, int startIndex, int length, Span<T0> cs0, Span<T1> cs1, Span<T2> cs2, Span<T3> cs3, Span<T4> cs4, Span<T5> cs5, Span<T6> cs6, Span<T7> cs7, Span<T8> cs8, Span<T9> cs9, Span<T10> cs10, Span<T11> cs11, Span<T12> cs12, Span<T13> cs13, Span<T14> cs14, Span<T15> cs15)
    {
        chunk.GetFullComponentSpan<T0>(out var s0);
        cs0.CopyTo(s0.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T1>(out var s1);
        cs1.CopyTo(s1.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T2>(out var s2);
        cs2.CopyTo(s2.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T3>(out var s3);
        cs3.CopyTo(s3.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T4>(out var s4);
        cs4.CopyTo(s4.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T5>(out var s5);
        cs5.CopyTo(s5.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T6>(out var s6);
        cs6.CopyTo(s6.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T7>(out var s7);
        cs7.CopyTo(s7.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T8>(out var s8);
        cs8.CopyTo(s8.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T9>(out var s9);
        cs9.CopyTo(s9.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T10>(out var s10);
        cs10.CopyTo(s10.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T11>(out var s11);
        cs11.CopyTo(s11.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T12>(out var s12);
        cs12.CopyTo(s12.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T13>(out var s13);
        cs13.CopyTo(s13.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T14>(out var s14);
        cs14.CopyTo(s14.Slice(startIndex, length));
        chunk.GetFullComponentSpan<T15>(out var s15);
        cs15.CopyTo(s15.Slice(startIndex, length));
    }
}
