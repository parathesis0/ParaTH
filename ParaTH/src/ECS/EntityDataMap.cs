using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed class EntityDataMap
{
    private readonly PagedArray<EntityData> EntityDatas;

    public EntityDataMap(int baseChunkSize, int capacity)
    {
        EntityDatas = new PagedArray<EntityData>(
            baseChunkSize / Unsafe.SizeOf<EntityData>(),
            capacity
        );
    }
}
