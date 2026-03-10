using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed partial class Archetype
{
    public Components<T0, T1> Get<T0, T1>(Slot slot)
    {
        ref var chunk = ref chunks[slot.Index];
        return chunk.Get<T0, T1>(slot.Index);
    }
}
