namespace ParaTH;

public sealed partial class Archetype
{
    public int Add<T0, T1>(Entity entity, out Slot slot, in T0 c0, in T1 c1)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1>(entity, c0, c1);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1>(entity, c0, c1);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1>(entity, c0, c1);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
    public int Add<T0, T1, T2>(Entity entity, out Slot slot, in T0 c0, in T1 c1, in T2 c2)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1, T2>(entity, c0, c1, c2);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1, T2>(entity, c0, c1, c2);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1, T2>(entity, c0, c1, c2);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
    public int Add<T0, T1, T2, T3>(Entity entity, out Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1, T2, T3>(entity, c0, c1, c2, c3);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1, T2, T3>(entity, c0, c1, c2, c3);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1, T2, T3>(entity, c0, c1, c2, c3);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
    public int Add<T0, T1, T2, T3, T4>(Entity entity, out Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1, T2, T3, T4>(entity, c0, c1, c2, c3, c4);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1, T2, T3, T4>(entity, c0, c1, c2, c3, c4);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1, T2, T3, T4>(entity, c0, c1, c2, c3, c4);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
    public int Add<T0, T1, T2, T3, T4, T5>(Entity entity, out Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5>(entity, c0, c1, c2, c3, c4, c5);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5>(entity, c0, c1, c2, c3, c4, c5);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1, T2, T3, T4, T5>(entity, c0, c1, c2, c3, c4, c5);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6>(Entity entity, out Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6>(entity, c0, c1, c2, c3, c4, c5, c6);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6>(entity, c0, c1, c2, c3, c4, c5, c6);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1, T2, T3, T4, T5, T6>(entity, c0, c1, c2, c3, c4, c5, c6);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7>(Entity entity, out Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7>(entity, c0, c1, c2, c3, c4, c5, c6, c7);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7>(entity, c0, c1, c2, c3, c4, c5, c6, c7);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7>(entity, c0, c1, c2, c3, c4, c5, c6, c7);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity, out Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Entity entity, out Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Entity entity, out Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Entity entity, out Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Entity entity, out Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Entity entity, out Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Entity entity, out Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
    public int Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Entity entity, out Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14, in T15 c15)
    {
        EntityCount++;

        var currentChunkIndex = CurrentChunkIndex;
        ref var currentChunk = ref CurrentChunk;

        int index;

        if (!currentChunk.IsFull)
        {
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15);
            slot = new Slot(index, currentChunkIndex);
            return 0;
        }

        currentChunkIndex++;
        var chunks = this.chunks;
        if (currentChunkIndex < chunks.Count)
        {
            currentChunk = ref chunks[currentChunkIndex];
            index = currentChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15);
            slot = new Slot(index, currentChunkIndex);

            CurrentChunkIndex = currentChunkIndex;
            return 0;
        }

        ref var newChunk = ref AddChunk();
        index = newChunk.Add<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(entity, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15);
        slot = new Slot(index, currentChunkIndex);

        CurrentChunkIndex = currentChunkIndex;
        return EntitiesPerChunk;
    }
}
