namespace ParaTH;

public sealed partial class Archetype
{
    public void Set<T0, T1>(Slot slot, in T0 c0, in T1 c1)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1>(slot.Index, c0, c1);
    }
    public void Set<T0, T1, T2>(Slot slot, in T0 c0, in T1 c1, in T2 c2)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1, T2>(slot.Index, c0, c1, c2);
    }
    public void Set<T0, T1, T2, T3>(Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1, T2, T3>(slot.Index, c0, c1, c2, c3);
    }
    public void Set<T0, T1, T2, T3, T4>(Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1, T2, T3, T4>(slot.Index, c0, c1, c2, c3, c4);
    }
    public void Set<T0, T1, T2, T3, T4, T5>(Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1, T2, T3, T4, T5>(slot.Index, c0, c1, c2, c3, c4, c5);
    }
    public void Set<T0, T1, T2, T3, T4, T5, T6>(Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1, T2, T3, T4, T5, T6>(slot.Index, c0, c1, c2, c3, c4, c5, c6);
    }
    public void Set<T0, T1, T2, T3, T4, T5, T6, T7>(Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1, T2, T3, T4, T5, T6, T7>(slot.Index, c0, c1, c2, c3, c4, c5, c6, c7);
    }
    public void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8>(slot.Index, c0, c1, c2, c3, c4, c5, c6, c7, c8);
    }
    public void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(slot.Index, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9);
    }
    public void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(slot.Index, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);
    }
    public void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(slot.Index, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11);
    }
    public void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(slot.Index, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12);
    }
    public void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(slot.Index, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13);
    }
    public void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(slot.Index, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14);
    }
    public void Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Slot slot, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14, in T15 c15)
    {
        ref var chunk = ref chunks[slot.ChunkIndex];
        chunk.Set<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(slot.Index, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15);
    }
}
