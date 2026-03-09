namespace ParaTH;

public sealed partial class Archetype
{
    private readonly ComponentTypeInfo[] componentTypes;
    private readonly int[] componentIdToArrayIndex;

    private readonly ChunkList chunks;

    public ulong Mask { get; }
    private int BaseChunkByteSize { get; }
    private ushort BaseChunkEntityCount { get; }

    private int ChunkSize { get; }
    public ushort EntitiesPerChunk { get; }
    private int CurrentChunkIndex { get; set; }
    private ref Chunk CurrentChunk => ref chunks[CurrentChunkIndex];
    private Slot CurrentSlot => new(CurrentChunk.Count - 1, CurrentChunkIndex);
    private int EntityCount { get; set; }

    public Archetype(ComponentTypeInfo[] componentTypes, int baseChunkByteSize, ushort baseChunkEntityCount)
    {
        ulong mask = 0;
        int max = 0;

        foreach (var type in componentTypes)
        {
            mask |= type.Mask;
            var id = type.Id;
            if (id > max)
                max = id;
        }

        Mask = mask;
        componentIdToArrayIndex = new int[max + 1];
        BaseChunkByteSize = baseChunkByteSize;
        BaseChunkEntityCount = baseChunkEntityCount;

        Array.Fill(componentIdToArrayIndex, -1);
        for (int i = 0; i < componentTypes.Length; i++)
        {
            ref var type = ref componentTypes[i];
            var id = type.Id;
            componentIdToArrayIndex[id] = i;
        }
        this.componentTypes = componentTypes;

        var typesByteSize = 0;
        foreach (var type in componentTypes)
            typesByteSize += type.ByteSize;
        ChunkSize = GetChunkSize(typesByteSize);
        EntitiesPerChunk = GetEntitesPerChunk(ChunkSize, typesByteSize);

        chunks = new ChunkList(1);
        AddChunk();
    }

    public ref Chunk AddChunk()
    {
        var chunk = new Chunk(EntitiesPerChunk, componentIdToArrayIndex, componentTypes);
        var chunks = this.chunks;
        var index = chunks.Count;
        chunks.EnsureCapacity(index + 1);
        chunks.Add(chunk);
        return ref chunks[index];
    }

    // returns the count of allocated entites
    // todo: variadic source gen wip
    public ushort Add<T0>(Entity entity, out Slot slot, in T0 component)
    {
        EntityCount++;

        // store stack variables for faster repeated access
        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        // current chunk has space
        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0>(entity, component);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        // current chunk full, use the next allocated chunk
        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0>(entity, component);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        // no more free allocated chunks, create new chunk
        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0>(entity, component);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }

    // swap and pop with the last chunk's last entity
    // returns the moved entity's id
    public ushort Remove(Slot slot)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        ref var lastChunk = ref CurrentChunk;

        var movedEntityId = chunk.Remove(slot.Index, ref lastChunk);

        EntityCount--;

        if (lastChunk.Count == 0 && CurrentChunkIndex > 0)
            CurrentChunkIndex--;

        return movedEntityId;
    }

    // todo: variadic source gen wip
    public void Set<T0>(Slot slot, in T0 component)
    {
        chunks[slot.ChunkIndex].Set<T0>(slot.Index, component);
    }

    // todo: variadic source gen wip
    public bool Has<T0>()
    {
        var mask = Mask;
        return (Component<T0>.GroupMask & mask) != 0;
    }

    public void TrimExcess()
    {
        chunks.Count = CurrentChunkIndex + 1;
        chunks.TrimExcess();
    }

    // doesn't dispose any resources
    public void Clear()
    {
        CurrentChunkIndex = 0;
        EntityCount = 0;
        chunks.Clear();
    }

    private unsafe int GetChunkSize(int typesByteSize)
    {
        var entityByteSize = BaseChunkEntityCount * (sizeof(Entity) + typesByteSize);
        return (entityByteSize + BaseChunkByteSize - 1) / BaseChunkByteSize * BaseChunkByteSize;
    }

    private unsafe ushort GetEntitesPerChunk(int chunkByteSize, int typesByteSize)
    {
        return (ushort)(chunkByteSize / (sizeof(Entity) + typesByteSize));
    }
}
