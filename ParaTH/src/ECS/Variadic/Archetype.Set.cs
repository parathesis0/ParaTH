using System;
using System.Collections.Generic;

namespace ParaTH;

public sealed partial class Archetype
{
    public void Set<T0, T1>(Slot slot, in T0 c0, in T1 c1)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1>(slot.Index, c0, c1);
    }
}
