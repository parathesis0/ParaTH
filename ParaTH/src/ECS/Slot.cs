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
}
