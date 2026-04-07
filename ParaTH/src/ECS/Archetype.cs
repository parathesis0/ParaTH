using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ParaTH;

[SkipLocalsInit]
public sealed partial class Archetype : IDisposable
{
    public const int InvalidIndex = -1;
    public const int PageSize = 16;

    private readonly ComponentTypeInfo[] componentTypes;
    private readonly int[] componentIdToArrayIndex;

    private readonly ChunkList chunks;

    // overenginnering, use normal array
    //private readonly SparsePagedArray<Archetype> addEdges;
    //private readonly SparsePagedArray<Archetype> removeEdges;

    private readonly Archetype[] addEdges;
    private readonly Archetype[] removeEdges;

    public ComponentMask Mask { get; }
    private int BaseChunkByteSize { get; }
    private int BaseChunkEntityCount { get; }

    private int ChunkByteSize { get; }
    public int EntitiesPerChunk { get; }
    private int CurrentChunkIndex { get; set; }
    private ref Chunk CurrentChunk => ref chunks[CurrentChunkIndex];
    public Slot CurrentSlot => new(CurrentChunk.EntityCount - 1, CurrentChunkIndex);
    public ComponentTypeInfo[] ComponentTypes => componentTypes;
    public ChunkList Chunks => chunks;
    public int EntityCount { get; private set; }
    public int EntityCapacity => chunks.Capacity * EntitiesPerChunk;

    public Archetype(ComponentTypeInfo[] componentTypes, int baseChunkByteSize, int baseChunkEntityCount)
    {
        var mask = ComponentMask.Zero;
        var typesByteSize = 0;
        var max = 0;

        foreach (var type in componentTypes)
        {
            mask |= type.Mask;
            typesByteSize += type.ByteSize;

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

        ChunkByteSize = GetChunkByteSize(typesByteSize, baseChunkByteSize, baseChunkEntityCount);
        EntitiesPerChunk = GetEntitesPerChunk(ChunkByteSize, typesByteSize);

        chunks = new ChunkList(1);
        AddChunk();

        addEdges = new Archetype[ComponentRegistry.MaxComponents];
        removeEdges = new Archetype[ComponentRegistry.MaxComponents];

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
            var fillLimit = Math.Min(chunk.EntityCapacity - chunkCount, amount);

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

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            // copy entities
            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            // copy components
            chunk.GetFullComponentSpan<T0>(out var span);
            var srcC = components.Slice(created, fillAmount);
            var dstC = span.Slice(chunkEntityCount, fillAmount);
            srcC.CopyTo(dstC);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
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

    // literally just AddBulk without copying components
    // ensure capacity before this
    public void ReserveBulk(Span<Entity> entities)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            // copy entities
            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
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

    public void SetRangeBulk<T0>(Slot startSlot, Slot endSlot, in T0 component)
    {
        // edge case, only one chunk
        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            Chunk.Fill<T0>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, component);
            return;
        }

        // fill the first
        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.Fill<T0>(ref firstChunk, startSlot.Index, lenTilEnd, component);

        // fill the middle
        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            Chunk.Fill<T0>(ref chunk, 0, chunk.EntityCapacity, component);
        }

        // fill the last
        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        Chunk.Fill<T0>(ref lastChunk, 0, endSlot.Index + 1, component);
    }

    // SetRangeBulk but with span copyto
    public void SetRangeWithSpanBulk<T0>(Slot startSlot, Slot endSlot, Span<T0> components)
    {
        Debug.Assert(Has<T0>() && components.Length != 0);

        int offset = 0;

        if (startSlot.ChunkIndex == endSlot.ChunkIndex)
        {
            var len = endSlot.Index + 1 - startSlot.Index;
            var src = components.Slice(offset, len);
            Chunk.FillWithSpan<T0>(ref chunks[startSlot.ChunkIndex], startSlot.Index, len, src);
            return;
        }

        // handle the first
        ref var firstChunk = ref chunks[startSlot.ChunkIndex];
        var lenTilEnd = firstChunk.EntityCapacity - startSlot.Index;
        Chunk.FillWithSpan<T0>(ref firstChunk, startSlot.Index, lenTilEnd, components.Slice(offset, lenTilEnd));
        offset += lenTilEnd;

        // handle the middle
        for (int i = startSlot.ChunkIndex + 1; i <= endSlot.ChunkIndex - 1; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkCap = chunk.EntityCapacity;
            Chunk.FillWithSpan<T0>(ref chunk, 0, chunkCap, components.Slice(offset, chunkCap));
            offset += chunkCap;
        }

        // handle the last
        ref var lastChunk = ref chunks[endSlot.ChunkIndex];
        var lastLen = endSlot.Index + 1;
        Chunk.FillWithSpan<T0>(ref lastChunk, 0, lastLen, components.Slice(offset, lastLen));
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

    public Span<Chunk> GetChunksSpan()
    {
        return Chunks.AsSpan();
    }

    public void TrimExcess()
    {
        chunks.Count = CurrentChunkIndex + 1;
        chunks.TrimExcess();
    }

    // doesn't dispose any resources
    public void Clear()
    {
        chunks.Clear();
        CurrentChunkIndex = 0;
        EntityCount = 0;
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
        addEdges[id] =  archetype;
    }

    public bool HasAddEdge(int id)
    {
        return addEdges[id] != null;
    }

    public Archetype GetAddEdge(int id)
    {
        return addEdges[id];
    }

    public bool TryGetAddEdge(int id, out Archetype archetype)
    {
        archetype = addEdges[id];
        return archetype != null;
    }

    public void RemoveAddEdge(int id)
    {
        addEdges[id] = null!;
    }

    public void AddRemoveEdge(int id, Archetype archetype)
    {
        removeEdges[id] = archetype;
    }

    public bool HasRemoveEdge(int id)
    {
        return removeEdges[id] != null;
    }

    public Archetype GetRemoveEdge(int id)
    {
        return removeEdges[id];
    }

    public bool TryGetRemoveEdge(int id, out Archetype archetype)
    {
        archetype = removeEdges[id];
        return archetype != null;
    }

    public void RemoveRemoveEdge(int id)
    {
        removeEdges[id] = null!;
    }

    public void RemoveAllEdgesByValue(Archetype archetype)
    {
        for (int i = 0; i < ComponentRegistry.MaxComponents; i++)
        {
            if (addEdges[i] == archetype)
                RemoveAddEdge(i);

            if (removeEdges[i] == archetype)
                RemoveRemoveEdge(i);
        }
    }

    // reserve capacity before you call, dstSlot should be the reserved spot
    public static void CopyEntityAndMatchingComponentsTo(Archetype src, Slot srcSlot, Archetype dst, Slot dstSlot)
    {
        ref var srcChunk = ref src.GetChunk(srcSlot.ChunkIndex);
        ref var dstChunk = ref dst.GetChunk(dstSlot.ChunkIndex);

        var srcTypes = src.ComponentTypes;

        Chunk.CopyEntityAndMatchingComponents(
            ref srcChunk, srcSlot.Index,
            ref dstChunk, dstSlot.Index,
            srcTypes, 1);
    }

    // ensure capacity before you call
    public static void CopyEntityAndMatchingComponentsAppendBulk(Archetype src, Archetype dst)
    {
        var srcTypes = src.ComponentTypes;

        for (int i = 0; i < src.Chunks.Count; i++)
        {
            ref var srcChunk = ref src.Chunks[i];

            var copiedAmount = 0;
            var remainingAmount = srcChunk.EntityCount;

            for (int j = dst.CurrentChunkIndex; j < dst.Chunks.Capacity; j++)
            {
                ref var dstChunk = ref dst.Chunks[j];
                var fillAmount = Math.Min(remainingAmount, dstChunk.EntityCapacity - dstChunk.EntityCount);

                Chunk.CopyEntityAndMatchingComponents(
                    ref srcChunk, copiedAmount,
                    ref dstChunk, dstChunk.EntityCount,
                    srcTypes, fillAmount);

                copiedAmount += fillAmount;
                remainingAmount -= fillAmount;

                // fuck this, CopyEntityAndMatchingComponents
                // should modify dst.EntityCount and not this
                dstChunk.EntityCount += fillAmount;

                if (remainingAmount <= 0)
                {
                    dst.CurrentChunkIndex = j;
                    break;
                }
            }
        }

        dst.EntityCount += src.EntityCount;
    }

    public void Dispose()
    {
        chunks.Dispose();
        EntityCount = 0;
        CurrentChunkIndex = 0;
    }
}
