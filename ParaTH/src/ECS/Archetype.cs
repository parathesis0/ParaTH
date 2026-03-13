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
    private int BaseChunkEntityCount { get; }

    private int ChunkByteSize { get; }
    public int EntitiesPerChunk { get; }
    private int CurrentChunkIndex { get; set; }
    private ref Chunk CurrentChunk => ref chunks[CurrentChunkIndex];
    private Slot CurrentSlot => new(CurrentChunk.EntityCount - 1, CurrentChunkIndex);
    public int EntityCount { get; private set; }
    public ComponentTypeInfo[] ComponentTypes => componentTypes;
    public ChunkList Chunks => chunks;
    public int EntityCapacity => chunks.Capacity * EntitiesPerChunk;

    public Archetype(ComponentTypeInfo[] componentTypes, int baseChunkByteSize, int baseChunkEntityCount)
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
            ref var type = ref componentTypes.UnsafeAt(i);
            var id = type.Id;
            componentIdToArrayIndex[id] = i;
        }
        this.componentTypes = componentTypes;

        var typesByteSize = 0;
        foreach (var type in componentTypes)
            typesByteSize += type.ByteSize;
        ChunkByteSize = GetChunkByteSize(typesByteSize, baseChunkByteSize, baseChunkEntityCount);
        EntitiesPerChunk = GetEntitesPerChunk(ChunkByteSize, typesByteSize);

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

    public void GetNextSlots(Span<Slot> slotBuffer, int amount)
    {
        var bufferIndex = 0;
        for (int chunkIndex = CurrentChunkIndex; chunkIndex < chunks.Count; chunkIndex++)
        {
            ref var chunk = ref chunks[chunkIndex];
            var chunkCount = chunk.EntityCount;
            var fillLimit = Math.Min(chunk.Capacity - chunkCount, amount);

            for (int index = chunkCount; index < chunkCount + fillLimit; index++)
                slotBuffer.UnsafeAt(bufferIndex++) = new Slot(index, chunkIndex);

            amount -= fillLimit;
            if (amount <= 0)
                break;
        }
    }

    public void EnsureEntityCapacity(int newEntityCapacity)
    {
        if (newEntityCapacity <= EntityCapacity)
            return;

        var oldChunkCapacity = Chunks.Capacity;
        // newEntityCapacity / EntitiesPerChunk rounded up
        var newChunkCapacity = (newEntityCapacity + EntitiesPerChunk - 1) / EntitiesPerChunk;
        Chunks.EnsureCapacity(newChunkCapacity);

        var entitiesPerChunk = EntitiesPerChunk;
        var componentIdToArrayIndex = this.componentIdToArrayIndex;
        var componentTypes = this.componentTypes;

        for (int i = oldChunkCapacity; i < newChunkCapacity; i++)
            Chunks.Add(new Chunk(entitiesPerChunk, componentIdToArrayIndex, componentTypes));
    }

    // returns the count of allocated entites
    public int Add<T0>(Entity entity, out Slot slot, in T0 component)
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

    // ensure capacity before this
    public void AddBulk<T0>(Span<Entity> entities, Span<T0> components)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            // copy entities
            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            // copy components
            chunk.GetComponentSpanFull<T0>(out var span);
            var srcC = components.Slice(created, fillAmount);
            var dstC = span.Slice(chunkEntityCount, fillAmount);
            srcC.CopyTo(dstC);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }

    // returns the count of allocated spots
    // blatant violation of the dry principle
    // should only be called by World.Move, i fucked up the architecture
    // and am now suffering the consequences of my stupidity
    public int Reserve(out Slot slot)
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
    public int Remove(Slot slot)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        ref var lastChunk = ref CurrentChunk;

        var movedEntityId = chunk.Remove(slot.Index, ref lastChunk);

        EntityCount--;

        if (lastChunk.EntityCount == 0 && CurrentChunkIndex > 0)
            CurrentChunkIndex--;

        return movedEntityId;
    }

    public void Set<T0>(Slot slot, in T0 component)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0>(slot.Index, component);
    }

    public ref T0 Get<T0>(Slot slot)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        return ref chunk.Get<T0>(slot.Index);
    }

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

        if ((uint)id >= (uint)mapping.Length)
        {
            arrayIndex = InvalidIndex;
            return false;
        }

        arrayIndex = mapping.UnsafeAt(id);
        return arrayIndex != InvalidIndex;
    }

    private static unsafe int GetChunkByteSize(int typesByteSize, int baseChunkByteSize, int baseChunkEntityCount)
    {
        var entityByteSize = baseChunkEntityCount * (sizeof(Entity) + typesByteSize);
        // (entityByteSize / baseChunkByteSize rounded up) * baseChunkByteSize
        return (entityByteSize + baseChunkByteSize - 1) / baseChunkByteSize * baseChunkByteSize;
    }

    private static unsafe int GetEntitesPerChunk(int chunkByteSize, int typesByteSize)
    {
        return chunkByteSize / (sizeof(Entity) + typesByteSize);
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

    public static void CopyEntityAndMatchingComponents(Archetype src, Slot srcSlot, Archetype dst, Slot dstSlot)
    {
        ref var srcChunk = ref src.GetChunk(srcSlot.ChunkIndex);
        ref var dstChunk = ref dst.GetChunk(dstSlot.ChunkIndex);

        var srcTypes = src.ComponentTypes;

        Chunk.CopyEntityAndMatchingComponents(
            ref srcChunk, srcSlot.Index,
            ref dstChunk, dstSlot.Index,
            srcTypes, 1);
    }
}
