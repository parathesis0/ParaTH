using System;
using System.Collections.Generic;

namespace ParaTH;

public record struct Slot
{
    public int Index;
    public int ChunkIndex;

    public Slot(int index, int chunkIndex)
    {
        Index = index;
        ChunkIndex = chunkIndex;
    }

    public static Slot GetNextFor(Slot slot, int capacity)
    {
        slot.Index++;
        if (slot.Index >= capacity)
        {
            slot.Index = 0;
            slot.ChunkIndex++;
        }

        return slot;
    }
}
