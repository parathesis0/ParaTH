using System;
using System.Collections.Generic;

namespace ParaTH;

public sealed partial class Archetype
{
    public bool Has<T0, T1>()
    {
        var archetypeMask = Mask;
        var groupMask = Component<T0, T1>.GroupMask;
        return (archetypeMask & groupMask) == groupMask;
    }

}
