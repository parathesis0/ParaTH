namespace ParaTH;

public sealed partial class Archetype
{
    private const int BaseChunkByteSize = 1024 * 16;
    private const int BaseChunkEntityCount = 100;
    private const int InitChunksCapacity = 8;

    private readonly ComponentTypeInfo[] componentTypes;
    private readonly int[] componentIdToArrayIndex;

    private readonly ChunkList chunks;

    public ulong Mask { get; }
    private int ChunkSize { get; }
    private int EntitiesPerChunk { get; }
    private int CurrentChunkIndex { get; set; }
    private ref Chunk CurrentChunk => ref chunks[CurrentChunkIndex];
    private Slot CurrentSlot => new(CurrentChunkIndex, CurrentChunk.Count - 1);
    private int EntityCount { get; set; }

    public Archetype(ComponentTypeInfo[] componentTypes)
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

        chunks = new ChunkList(InitChunksCapacity);
        AddChunk();
    }

    public ref Chunk AddChunk()
    {
        var chunk = new Chunk(EntitiesPerChunk, componentIdToArrayIndex, componentTypes);
        var index = chunks.Count;
        chunks.Add(chunk);
        CurrentChunkIndex = chunks.Count - 1;
        return ref chunks[index];
    }

    // variadic source gen wip
    public void Add<T>(Entity entity, in T component)
    {
        EntityCount++;

        // current chunk has space
        ref var chunk = ref CurrentChunk;

        if (!chunk.IsFull)
        {
            chunk.Add<T>(entity, component);
            return;
        }

        // current chunk full, allocate the next chunk and use that
        AddChunk();
        CurrentChunkIndex++;
        CurrentChunk.Add<T>(entity, component);
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

    // variadic source gen wip
    public void Set<T>(Slot slot, in T component)
    {
        chunks[slot.ChunkIndex].Set<T>(slot.Index, component);
    }

    // variadic source gen wip
    public bool Has<T>()
    {
        var mask = Mask;
        return (Component<T>.TypeInfo.Mask & mask) != 0;
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public void TrimExcess()
    {
        throw new NotImplementedException();
    }

    private static unsafe int GetChunkSize(int typesByteSize)
    {
        var entityByteSize = BaseChunkEntityCount * (sizeof(Entity) + typesByteSize);
        return (entityByteSize + BaseChunkByteSize - 1) / BaseChunkByteSize * BaseChunkByteSize;
    }

    private static unsafe int GetEntitesPerChunk(int chunkByteSize, int typesByteSize)
    {
        return chunkByteSize / (sizeof(Entity) + typesByteSize);
    }
}
