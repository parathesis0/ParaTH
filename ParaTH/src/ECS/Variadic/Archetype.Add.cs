using System;
using System.Collections.Generic;

namespace ParaTH;

public sealed partial class Archetype
{
    public int Add<T0, T1>(Entity entity, out Slot slot, in T0 c0, in T1 c1)
    {
        EntityCount++;

        // store stack variables for faster repeated access
        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        // current chunk has space
        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1>(entity, c0, c1);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        // current chunk full, use the next allocated chunk
        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1>(entity, c0, c1);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        // no more free allocated chunks, create new chunk
        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0>(entity, c0);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
}
