using System;
using System.Collections.Generic;

namespace ParaTH;

public sealed partial class World : IDisposable
{
    private EntityDataMap entityDatas;
    private ArchetypeList archetypes;
    private Dictionary<ulong, Archetype> groupMaskToArchetype;

    private readonly int baseChunkByteSize;
    private readonly int baseChunkEntityCount;

    public World(
        int baseChunkByteSize,
        int baseChunkEntityCount,
        int initialArchetypeCapacity,
        int initialEntityCapacity)
    {
        entityDatas = new EntityDataMap(baseChunkByteSize, initialEntityCapacity);
        archetypes = new(initialArchetypeCapacity);
        groupMaskToArchetype = new(initialArchetypeCapacity);

        this.baseChunkByteSize = baseChunkByteSize;
        this.baseChunkEntityCount = baseChunkEntityCount;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
