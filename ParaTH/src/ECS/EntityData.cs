using System;
using System.Collections.Generic;

namespace ParaTH;

public record struct EntityData
{
    public Archetype archetype;
    public Slot slot;
    public int Version;

    public EntityData(Archetype archetype, Slot slot, int version)
    {
        this.archetype = archetype;
        this.slot = slot;
        this.Version = version;
    }
}
