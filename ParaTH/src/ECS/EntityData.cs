using System;
using System.Collections.Generic;

namespace ParaTH;

public record struct EntityData
{
    public Archetype Archetype;
    public Slot Slot;
    public int Version;

    public EntityData(Archetype archetype, Slot slot, int version)
    {
        Archetype = archetype;
        Slot = slot;
        Version = version;
    }
}
