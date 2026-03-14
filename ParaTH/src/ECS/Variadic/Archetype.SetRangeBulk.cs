namespace ParaTH;

public sealed partial class Archetype
{
    public void SetRangeBulk<T0, T1>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1>(ref chunk, 0, chunk.EntityCapacity, c0, c1);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1>(ref lastChunk, 0, endSlot.Index + 1, c0, c1);
    }
    public void SetRangeBulk<T0, T1, T2>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1, in T2 c2)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1, T2>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1, c2);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1, T2>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1, c2);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1, T2>(ref chunk, 0, chunk.EntityCapacity, c0, c1, c2);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1, T2>(ref lastChunk, 0, endSlot.Index + 1, c0, c1, c2);
    }
    public void SetRangeBulk<T0, T1, T2, T3>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1, in T2 c2, in T3 c3)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1, T2, T3>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1, c2, c3);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1, T2, T3>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1, c2, c3);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1, T2, T3>(ref chunk, 0, chunk.EntityCapacity, c0, c1, c2, c3);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1, T2, T3>(ref lastChunk, 0, endSlot.Index + 1, c0, c1, c2, c3);
    }
    public void SetRangeBulk<T0, T1, T2, T3, T4>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1, T2, T3, T4>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1, c2, c3, c4);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1, T2, T3, T4>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1, c2, c3, c4);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1, T2, T3, T4>(ref chunk, 0, chunk.EntityCapacity, c0, c1, c2, c3, c4);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1, T2, T3, T4>(ref lastChunk, 0, endSlot.Index + 1, c0, c1, c2, c3, c4);
    }
    public void SetRangeBulk<T0, T1, T2, T3, T4, T5>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1, T2, T3, T4, T5>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1, c2, c3, c4, c5);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1, T2, T3, T4, T5>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1, c2, c3, c4, c5);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1, T2, T3, T4, T5>(ref chunk, 0, chunk.EntityCapacity, c0, c1, c2, c3, c4, c5);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1, T2, T3, T4, T5>(ref lastChunk, 0, endSlot.Index + 1, c0, c1, c2, c3, c4, c5);
    }
    public void SetRangeBulk<T0, T1, T2, T3, T4, T5, T6>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1, c2, c3, c4, c5, c6);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1, c2, c3, c4, c5, c6);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6>(ref chunk, 0, chunk.EntityCapacity, c0, c1, c2, c3, c4, c5, c6);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6>(ref lastChunk, 0, endSlot.Index + 1, c0, c1, c2, c3, c4, c5, c6);
    }
    public void SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1, c2, c3, c4, c5, c6, c7);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1, c2, c3, c4, c5, c6, c7);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7>(ref chunk, 0, chunk.EntityCapacity, c0, c1, c2, c3, c4, c5, c6, c7);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7>(ref lastChunk, 0, endSlot.Index + 1, c0, c1, c2, c3, c4, c5, c6, c7);
    }
    public void SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1, c2, c3, c4, c5, c6, c7, c8);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1, c2, c3, c4, c5, c6, c7, c8);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref chunk, 0, chunk.EntityCapacity, c0, c1, c2, c3, c4, c5, c6, c7, c8);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref lastChunk, 0, endSlot.Index + 1, c0, c1, c2, c3, c4, c5, c6, c7, c8);
    }
    public void SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref chunk, 0, chunk.EntityCapacity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref lastChunk, 0, endSlot.Index + 1, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9);
    }
    public void SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref chunk, 0, chunk.EntityCapacity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref lastChunk, 0, endSlot.Index + 1, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);
    }
    public void SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref chunk, 0, chunk.EntityCapacity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref lastChunk, 0, endSlot.Index + 1, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11);
    }
    public void SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref chunk, 0, chunk.EntityCapacity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref lastChunk, 0, endSlot.Index + 1, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12);
    }
    public void SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref chunk, 0, chunk.EntityCapacity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref lastChunk, 0, endSlot.Index + 1, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13);
    }
    public void SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref chunk, 0, chunk.EntityCapacity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref lastChunk, 0, endSlot.Index + 1, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14);
    }
    public void SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Slot startSlot, Slot endSlot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14, in T15 c15)
    {
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15);
            return;
        }

        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref firstChunk, startSlot.Index, lenTilEnd, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15);

        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref chunk, 0, chunk.EntityCapacity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15);
        }

        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref lastChunk, 0, endSlot.Index + 1, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15);
    }
}
