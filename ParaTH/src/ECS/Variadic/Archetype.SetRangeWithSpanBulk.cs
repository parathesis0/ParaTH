namespace ParaTH;

public sealed partial class Archetype
{
    public void SetRangeWithSpanBulk<T0, T1>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen));
    }

    public void SetRangeWithSpanBulk<T0, T1, T2>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1, Span<T2> s2)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1, T2>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len), s2.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1, T2>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd), s2.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1, T2>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap), s2.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1, T2>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen), s2.Slice(offset, lastLen));
    }

    public void SetRangeWithSpanBulk<T0, T1, T2, T3>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1, Span<T2> s2, Span<T3> s3)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1, T2, T3>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len), s2.Slice(offset, len), s3.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1, T2, T3>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd), s2.Slice(offset, lenTilEnd), s3.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1, T2, T3>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap), s2.Slice(offset, chunkCap), s3.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1, T2, T3>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen), s2.Slice(offset, lastLen), s3.Slice(offset, lastLen));
    }

    public void SetRangeWithSpanBulk<T0, T1, T2, T3, T4>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1, Span<T2> s2, Span<T3> s3, Span<T4> s4)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len), s2.Slice(offset, len), s3.Slice(offset, len), s4.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd), s2.Slice(offset, lenTilEnd), s3.Slice(offset, lenTilEnd), s4.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap), s2.Slice(offset, chunkCap), s3.Slice(offset, chunkCap), s4.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen), s2.Slice(offset, lastLen), s3.Slice(offset, lastLen), s4.Slice(offset, lastLen));
    }

    public void SetRangeWithSpanBulk<T0, T1, T2, T3, T4, T5>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1, Span<T2> s2, Span<T3> s3, Span<T4> s4, Span<T5> s5)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len), s2.Slice(offset, len), s3.Slice(offset, len), s4.Slice(offset, len), s5.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd), s2.Slice(offset, lenTilEnd), s3.Slice(offset, lenTilEnd), s4.Slice(offset, lenTilEnd), s5.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap), s2.Slice(offset, chunkCap), s3.Slice(offset, chunkCap), s4.Slice(offset, chunkCap), s5.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen), s2.Slice(offset, lastLen), s3.Slice(offset, lastLen), s4.Slice(offset, lastLen), s5.Slice(offset, lastLen));
    }

    public void SetRangeWithSpanBulk<T0, T1, T2, T3, T4, T5, T6>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1, Span<T2> s2, Span<T3> s3, Span<T4> s4, Span<T5> s5, Span<T6> s6)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len), s2.Slice(offset, len), s3.Slice(offset, len), s4.Slice(offset, len), s5.Slice(offset, len), s6.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd), s2.Slice(offset, lenTilEnd), s3.Slice(offset, lenTilEnd), s4.Slice(offset, lenTilEnd), s5.Slice(offset, lenTilEnd), s6.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap), s2.Slice(offset, chunkCap), s3.Slice(offset, chunkCap), s4.Slice(offset, chunkCap), s5.Slice(offset, chunkCap), s6.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen), s2.Slice(offset, lastLen), s3.Slice(offset, lastLen), s4.Slice(offset, lastLen), s5.Slice(offset, lastLen), s6.Slice(offset, lastLen));
    }

    public void SetRangeWithSpanBulk<T0, T1, T2, T3, T4, T5, T6, T7>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1, Span<T2> s2, Span<T3> s3, Span<T4> s4, Span<T5> s5, Span<T6> s6, Span<T7> s7)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len), s2.Slice(offset, len), s3.Slice(offset, len), s4.Slice(offset, len), s5.Slice(offset, len), s6.Slice(offset, len), s7.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd), s2.Slice(offset, lenTilEnd), s3.Slice(offset, lenTilEnd), s4.Slice(offset, lenTilEnd), s5.Slice(offset, lenTilEnd), s6.Slice(offset, lenTilEnd), s7.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap), s2.Slice(offset, chunkCap), s3.Slice(offset, chunkCap), s4.Slice(offset, chunkCap), s5.Slice(offset, chunkCap), s6.Slice(offset, chunkCap), s7.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen), s2.Slice(offset, lastLen), s3.Slice(offset, lastLen), s4.Slice(offset, lastLen), s5.Slice(offset, lastLen), s6.Slice(offset, lastLen), s7.Slice(offset, lastLen));
    }

    public void SetRangeWithSpanBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1, Span<T2> s2, Span<T3> s3, Span<T4> s4, Span<T5> s5, Span<T6> s6, Span<T7> s7, Span<T8> s8)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len), s2.Slice(offset, len), s3.Slice(offset, len), s4.Slice(offset, len), s5.Slice(offset, len), s6.Slice(offset, len), s7.Slice(offset, len), s8.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd), s2.Slice(offset, lenTilEnd), s3.Slice(offset, lenTilEnd), s4.Slice(offset, lenTilEnd), s5.Slice(offset, lenTilEnd), s6.Slice(offset, lenTilEnd), s7.Slice(offset, lenTilEnd), s8.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap), s2.Slice(offset, chunkCap), s3.Slice(offset, chunkCap), s4.Slice(offset, chunkCap), s5.Slice(offset, chunkCap), s6.Slice(offset, chunkCap), s7.Slice(offset, chunkCap), s8.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen), s2.Slice(offset, lastLen), s3.Slice(offset, lastLen), s4.Slice(offset, lastLen), s5.Slice(offset, lastLen), s6.Slice(offset, lastLen), s7.Slice(offset, lastLen), s8.Slice(offset, lastLen));
    }

    public void SetRangeWithSpanBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1, Span<T2> s2, Span<T3> s3, Span<T4> s4, Span<T5> s5, Span<T6> s6, Span<T7> s7, Span<T8> s8, Span<T9> s9)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len), s2.Slice(offset, len), s3.Slice(offset, len), s4.Slice(offset, len), s5.Slice(offset, len), s6.Slice(offset, len), s7.Slice(offset, len), s8.Slice(offset, len), s9.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd), s2.Slice(offset, lenTilEnd), s3.Slice(offset, lenTilEnd), s4.Slice(offset, lenTilEnd), s5.Slice(offset, lenTilEnd), s6.Slice(offset, lenTilEnd), s7.Slice(offset, lenTilEnd), s8.Slice(offset, lenTilEnd), s9.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap), s2.Slice(offset, chunkCap), s3.Slice(offset, chunkCap), s4.Slice(offset, chunkCap), s5.Slice(offset, chunkCap), s6.Slice(offset, chunkCap), s7.Slice(offset, chunkCap), s8.Slice(offset, chunkCap), s9.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen), s2.Slice(offset, lastLen), s3.Slice(offset, lastLen), s4.Slice(offset, lastLen), s5.Slice(offset, lastLen), s6.Slice(offset, lastLen), s7.Slice(offset, lastLen), s8.Slice(offset, lastLen), s9.Slice(offset, lastLen));
    }

    public void SetRangeWithSpanBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1, Span<T2> s2, Span<T3> s3, Span<T4> s4, Span<T5> s5, Span<T6> s6, Span<T7> s7, Span<T8> s8, Span<T9> s9, Span<T10> s10)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len), s2.Slice(offset, len), s3.Slice(offset, len), s4.Slice(offset, len), s5.Slice(offset, len), s6.Slice(offset, len), s7.Slice(offset, len), s8.Slice(offset, len), s9.Slice(offset, len), s10.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd), s2.Slice(offset, lenTilEnd), s3.Slice(offset, lenTilEnd), s4.Slice(offset, lenTilEnd), s5.Slice(offset, lenTilEnd), s6.Slice(offset, lenTilEnd), s7.Slice(offset, lenTilEnd), s8.Slice(offset, lenTilEnd), s9.Slice(offset, lenTilEnd), s10.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap), s2.Slice(offset, chunkCap), s3.Slice(offset, chunkCap), s4.Slice(offset, chunkCap), s5.Slice(offset, chunkCap), s6.Slice(offset, chunkCap), s7.Slice(offset, chunkCap), s8.Slice(offset, chunkCap), s9.Slice(offset, chunkCap), s10.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen), s2.Slice(offset, lastLen), s3.Slice(offset, lastLen), s4.Slice(offset, lastLen), s5.Slice(offset, lastLen), s6.Slice(offset, lastLen), s7.Slice(offset, lastLen), s8.Slice(offset, lastLen), s9.Slice(offset, lastLen), s10.Slice(offset, lastLen));
    }

    public void SetRangeWithSpanBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1, Span<T2> s2, Span<T3> s3, Span<T4> s4, Span<T5> s5, Span<T6> s6, Span<T7> s7, Span<T8> s8, Span<T9> s9, Span<T10> s10, Span<T11> s11)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len), s2.Slice(offset, len), s3.Slice(offset, len), s4.Slice(offset, len), s5.Slice(offset, len), s6.Slice(offset, len), s7.Slice(offset, len), s8.Slice(offset, len), s9.Slice(offset, len), s10.Slice(offset, len), s11.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd), s2.Slice(offset, lenTilEnd), s3.Slice(offset, lenTilEnd), s4.Slice(offset, lenTilEnd), s5.Slice(offset, lenTilEnd), s6.Slice(offset, lenTilEnd), s7.Slice(offset, lenTilEnd), s8.Slice(offset, lenTilEnd), s9.Slice(offset, lenTilEnd), s10.Slice(offset, lenTilEnd), s11.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap), s2.Slice(offset, chunkCap), s3.Slice(offset, chunkCap), s4.Slice(offset, chunkCap), s5.Slice(offset, chunkCap), s6.Slice(offset, chunkCap), s7.Slice(offset, chunkCap), s8.Slice(offset, chunkCap), s9.Slice(offset, chunkCap), s10.Slice(offset, chunkCap), s11.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen), s2.Slice(offset, lastLen), s3.Slice(offset, lastLen), s4.Slice(offset, lastLen), s5.Slice(offset, lastLen), s6.Slice(offset, lastLen), s7.Slice(offset, lastLen), s8.Slice(offset, lastLen), s9.Slice(offset, lastLen), s10.Slice(offset, lastLen), s11.Slice(offset, lastLen));
    }

    public void SetRangeWithSpanBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1, Span<T2> s2, Span<T3> s3, Span<T4> s4, Span<T5> s5, Span<T6> s6, Span<T7> s7, Span<T8> s8, Span<T9> s9, Span<T10> s10, Span<T11> s11, Span<T12> s12)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len), s2.Slice(offset, len), s3.Slice(offset, len), s4.Slice(offset, len), s5.Slice(offset, len), s6.Slice(offset, len), s7.Slice(offset, len), s8.Slice(offset, len), s9.Slice(offset, len), s10.Slice(offset, len), s11.Slice(offset, len), s12.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd), s2.Slice(offset, lenTilEnd), s3.Slice(offset, lenTilEnd), s4.Slice(offset, lenTilEnd), s5.Slice(offset, lenTilEnd), s6.Slice(offset, lenTilEnd), s7.Slice(offset, lenTilEnd), s8.Slice(offset, lenTilEnd), s9.Slice(offset, lenTilEnd), s10.Slice(offset, lenTilEnd), s11.Slice(offset, lenTilEnd), s12.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap), s2.Slice(offset, chunkCap), s3.Slice(offset, chunkCap), s4.Slice(offset, chunkCap), s5.Slice(offset, chunkCap), s6.Slice(offset, chunkCap), s7.Slice(offset, chunkCap), s8.Slice(offset, chunkCap), s9.Slice(offset, chunkCap), s10.Slice(offset, chunkCap), s11.Slice(offset, chunkCap), s12.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen), s2.Slice(offset, lastLen), s3.Slice(offset, lastLen), s4.Slice(offset, lastLen), s5.Slice(offset, lastLen), s6.Slice(offset, lastLen), s7.Slice(offset, lastLen), s8.Slice(offset, lastLen), s9.Slice(offset, lastLen), s10.Slice(offset, lastLen), s11.Slice(offset, lastLen), s12.Slice(offset, lastLen));
    }

    public void SetRangeWithSpanBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1, Span<T2> s2, Span<T3> s3, Span<T4> s4, Span<T5> s5, Span<T6> s6, Span<T7> s7, Span<T8> s8, Span<T9> s9, Span<T10> s10, Span<T11> s11, Span<T12> s12, Span<T13> s13)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len), s2.Slice(offset, len), s3.Slice(offset, len), s4.Slice(offset, len), s5.Slice(offset, len), s6.Slice(offset, len), s7.Slice(offset, len), s8.Slice(offset, len), s9.Slice(offset, len), s10.Slice(offset, len), s11.Slice(offset, len), s12.Slice(offset, len), s13.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd), s2.Slice(offset, lenTilEnd), s3.Slice(offset, lenTilEnd), s4.Slice(offset, lenTilEnd), s5.Slice(offset, lenTilEnd), s6.Slice(offset, lenTilEnd), s7.Slice(offset, lenTilEnd), s8.Slice(offset, lenTilEnd), s9.Slice(offset, lenTilEnd), s10.Slice(offset, lenTilEnd), s11.Slice(offset, lenTilEnd), s12.Slice(offset, lenTilEnd), s13.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap), s2.Slice(offset, chunkCap), s3.Slice(offset, chunkCap), s4.Slice(offset, chunkCap), s5.Slice(offset, chunkCap), s6.Slice(offset, chunkCap), s7.Slice(offset, chunkCap), s8.Slice(offset, chunkCap), s9.Slice(offset, chunkCap), s10.Slice(offset, chunkCap), s11.Slice(offset, chunkCap), s12.Slice(offset, chunkCap), s13.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen), s2.Slice(offset, lastLen), s3.Slice(offset, lastLen), s4.Slice(offset, lastLen), s5.Slice(offset, lastLen), s6.Slice(offset, lastLen), s7.Slice(offset, lastLen), s8.Slice(offset, lastLen), s9.Slice(offset, lastLen), s10.Slice(offset, lastLen), s11.Slice(offset, lastLen), s12.Slice(offset, lastLen), s13.Slice(offset, lastLen));
    }

    public void SetRangeWithSpanBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1, Span<T2> s2, Span<T3> s3, Span<T4> s4, Span<T5> s5, Span<T6> s6, Span<T7> s7, Span<T8> s8, Span<T9> s9, Span<T10> s10, Span<T11> s11, Span<T12> s12, Span<T13> s13, Span<T14> s14)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len), s2.Slice(offset, len), s3.Slice(offset, len), s4.Slice(offset, len), s5.Slice(offset, len), s6.Slice(offset, len), s7.Slice(offset, len), s8.Slice(offset, len), s9.Slice(offset, len), s10.Slice(offset, len), s11.Slice(offset, len), s12.Slice(offset, len), s13.Slice(offset, len), s14.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd), s2.Slice(offset, lenTilEnd), s3.Slice(offset, lenTilEnd), s4.Slice(offset, lenTilEnd), s5.Slice(offset, lenTilEnd), s6.Slice(offset, lenTilEnd), s7.Slice(offset, lenTilEnd), s8.Slice(offset, lenTilEnd), s9.Slice(offset, lenTilEnd), s10.Slice(offset, lenTilEnd), s11.Slice(offset, lenTilEnd), s12.Slice(offset, lenTilEnd), s13.Slice(offset, lenTilEnd), s14.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap), s2.Slice(offset, chunkCap), s3.Slice(offset, chunkCap), s4.Slice(offset, chunkCap), s5.Slice(offset, chunkCap), s6.Slice(offset, chunkCap), s7.Slice(offset, chunkCap), s8.Slice(offset, chunkCap), s9.Slice(offset, chunkCap), s10.Slice(offset, chunkCap), s11.Slice(offset, chunkCap), s12.Slice(offset, chunkCap), s13.Slice(offset, chunkCap), s14.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen), s2.Slice(offset, lastLen), s3.Slice(offset, lastLen), s4.Slice(offset, lastLen), s5.Slice(offset, lastLen), s6.Slice(offset, lastLen), s7.Slice(offset, lastLen), s8.Slice(offset, lastLen), s9.Slice(offset, lastLen), s10.Slice(offset, lastLen), s11.Slice(offset, lastLen), s12.Slice(offset, lastLen), s13.Slice(offset, lastLen), s14.Slice(offset, lastLen));
    }

    public void SetRangeWithSpanBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Slot startSlot, Slot endSlot, Span<T0> s0, Span<T1> s1, Span<T2> s2, Span<T3> s3, Span<T4> s4, Span<T5> s5, Span<T6> s6, Span<T7> s7, Span<T8> s8, Span<T9> s9, Span<T10> s10, Span<T11> s11, Span<T12> s12, Span<T13> s13, Span<T14> s14, Span<T15> s15)
    {
        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, s0.Slice(offset, len), s1.Slice(offset, len), s2.Slice(offset, len), s3.Slice(offset, len), s4.Slice(offset, len), s5.Slice(offset, len), s6.Slice(offset, len), s7.Slice(offset, len), s8.Slice(offset, len), s9.Slice(offset, len), s10.Slice(offset, len), s11.Slice(offset, len), s12.Slice(offset, len), s13.Slice(offset, len), s14.Slice(offset, len), s15.Slice(offset, len));
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref firstChunk, startSlot.Index, lenTilEnd, s0.Slice(offset, lenTilEnd), s1.Slice(offset, lenTilEnd), s2.Slice(offset, lenTilEnd), s3.Slice(offset, lenTilEnd), s4.Slice(offset, lenTilEnd), s5.Slice(offset, lenTilEnd), s6.Slice(offset, lenTilEnd), s7.Slice(offset, lenTilEnd), s8.Slice(offset, lenTilEnd), s9.Slice(offset, lenTilEnd), s10.Slice(offset, lenTilEnd), s11.Slice(offset, lenTilEnd), s12.Slice(offset, lenTilEnd), s13.Slice(offset, lenTilEnd), s14.Slice(offset, lenTilEnd), s15.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref chunk, 0, chunkCap, s0.Slice(offset, chunkCap), s1.Slice(offset, chunkCap), s2.Slice(offset, chunkCap), s3.Slice(offset, chunkCap), s4.Slice(offset, chunkCap), s5.Slice(offset, chunkCap), s6.Slice(offset, chunkCap), s7.Slice(offset, chunkCap), s8.Slice(offset, chunkCap), s9.Slice(offset, chunkCap), s10.Slice(offset, chunkCap), s11.Slice(offset, chunkCap), s12.Slice(offset, chunkCap), s13.Slice(offset, chunkCap), s14.Slice(offset, chunkCap), s15.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref lastChunk, 0, lastLen, s0.Slice(offset, lastLen), s1.Slice(offset, lastLen), s2.Slice(offset, lastLen), s3.Slice(offset, lastLen), s4.Slice(offset, lastLen), s5.Slice(offset, lastLen), s6.Slice(offset, lastLen), s7.Slice(offset, lastLen), s8.Slice(offset, lastLen), s9.Slice(offset, lastLen), s10.Slice(offset, lastLen), s11.Slice(offset, lastLen), s12.Slice(offset, lastLen), s13.Slice(offset, lastLen), s14.Slice(offset, lastLen), s15.Slice(offset, lastLen));
    }

}
