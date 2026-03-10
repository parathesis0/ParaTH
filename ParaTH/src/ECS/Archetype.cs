using System.ComponentModel;

namespace ParaTH;

public sealed partial class Archetype
{
    public const int InvalidIndex = -1;
    public const int PageSize = 16;

    private readonly ComponentTypeInfo[] componentTypes;
    private readonly int[] componentIdToArrayIndex;

    private readonly ChunkList chunks;

    private readonly SparsePagedArray<Archetype> addEdges;
    private readonly SparsePagedArray<Archetype> removeEdges;

    public ulong Mask { get; }
    private int BaseChunkByteSize { get; }
    private ushort BaseChunkEntityCount { get; }

    private int ChunkSize { get; }
    public ushort EntitiesPerChunk { get; }
    private int CurrentChunkIndex { get; set; }
    private ref Chunk CurrentChunk => ref chunks[CurrentChunkIndex];
    private Slot CurrentSlot => new(CurrentChunk.Count - 1, CurrentChunkIndex);
    private int EntityCount { get; set; }
    public ComponentTypeInfo[] ComponentTypes => componentTypes;

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

        Array.Fill(componentIdToArrayIndex, InvalidIndex);
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
        ChunkSize = GetChunkSize(typesByteSize, baseChunkByteSize, baseChunkEntityCount);
        EntitiesPerChunk = GetEntitesPerChunk(ChunkSize, typesByteSize);

        chunks = new ChunkList(1);
        AddChunk();

        addEdges = new SparsePagedArray<Archetype>(PageSize);
        removeEdges = new SparsePagedArray<Archetype>(PageSize);
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

    public ref Chunk GetChunk(int index)
    {
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

    // returns the count of allocated spots
    // blatant violation of the dry principle
    // should only be called by World.Move, i fucked up the architecture
    // and am now suffering the consequences of my stupidity
    public ushort Reserve(out Slot slot)
    {
        EntityCount++;

        // store stack variables for faster repeated access
        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        // current chunk has space
        if (!currentChunk.IsFull)
        {
            index = currentChunk.Reserve();
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        // current chunk full, use the next allocated chunk
        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Reserve();
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        // no more free allocated chunks, create new chunk
        ref var newChunk = ref AddChunk();
        index = newChunk.Reserve();
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
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0>(slot.Index, component);
    }

    // todo: variadic source gen wip
    public ref T0 Get<T0>(Slot slot)
    {
        ref var chunk = ref chunks[slot.Index];
        return ref chunk.Get<T0>(slot.Index);
    }

    // todo: variadic source gen wip
    public bool Has<T0>()
    {
        var archetypeMask = Mask;
        var groupMask = Component<T0>.GroupMask;
        return (archetypeMask & groupMask) == groupMask;
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

    public bool TryGetComponentArrayIndex<T>(out int arrayIndex)
    {
        var id = Component<T>.TypeInfo.Id;

        var mapping = componentIdToArrayIndex;

        if ((uint)id > (uint)mapping.Length)
        {
            arrayIndex = InvalidIndex;
            return false;
        }

        arrayIndex = mapping.UnsafeAt(id);
        return id != InvalidIndex;
    }

    private static unsafe int GetChunkSize(int typesByteSize, int baseChunkByteSize, int baseChunkEntityCount)
    {
        var entityByteSize = baseChunkEntityCount * (sizeof(Entity) + typesByteSize);
        return (entityByteSize + baseChunkByteSize - 1) / baseChunkByteSize * baseChunkByteSize;
    }

    private static unsafe ushort GetEntitesPerChunk(int chunkByteSize, int typesByteSize)
    {
        return (ushort)(chunkByteSize / (sizeof(Entity) + typesByteSize));
    }

    public void AddAddEdge(int id, Archetype archetype)
    {
        addEdges.EnsureCapacity(id); // todo: why not id + 1?
        addEdges.Add(id, archetype);
    }

    public bool HasAddEdge(int id)
    {
        return addEdges.ContainsKey(id);
    }

    public Archetype GetAddEdge(int id)
    {
        return addEdges[id];
    }

    public bool TryGetAddEdge(int id, out Archetype archetype)
    {
        return addEdges.TryGetValue(id, out archetype);
    }

    public void RemoveAddEdge(int id)
    {
        addEdges.Remove(id);
    }

    public void AddRemoveEdge(int id, Archetype archetype)
    {
        removeEdges.EnsureCapacity(id); // todo: why not id + 1?
        removeEdges.Add(id, archetype);
    }

    public bool HasRemoveEdge(int id)
    {
        return removeEdges.ContainsKey(id);
    }

    public Archetype GetRemoveEdge(int id)
    {
        return removeEdges[id];
    }

    public bool TryGetRemoveEdge(int id, out Archetype archetype)
    {
        return removeEdges.TryGetValue(id, out archetype);
    }

    public void RemoveRemoveEdge(int id)
    {
        removeEdges.Remove(id);
    }

    public static void CopyEntityComponents(Archetype src, Slot srcSlot, Archetype dst, Slot dstSlot)
    {
        ref var srcChunk = ref src.GetChunk(srcSlot.ChunkIndex);
        ref var dstChunk = ref dst.GetChunk(dstSlot.ChunkIndex);

        var srcTypes = src.ComponentTypes;

        Chunk.CopyEntityComponents(
            ref srcChunk, srcSlot.Index,
            ref dstChunk, dstSlot.Index,
            srcTypes, 1);
    }
}
