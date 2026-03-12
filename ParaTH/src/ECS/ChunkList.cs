using System.Buffers;
using System.Runtime.CompilerServices;

namespace ParaTH;

[SkipLocalsInit]
public sealed class ChunkList
{
    private Chunk[] items;
    public int Count { get; set; }
    public int Capacity { get; private set; }

    public ChunkList(int capacity)
    {
        Count = 0;
        Capacity = capacity;
        items = ArrayPool<Chunk>.Shared.Rent(capacity);
    }

#pragma warning disable RCS1242 // Do not pass non-read-only struct by read-only reference
    public void Add(in Chunk chunk)
#pragma warning restore RCS1242 // Do not pass non-read-only struct by read-only reference
    {
        items.UnsafeAt(Count++) = chunk;
    }

    public void EnsureCapacity(int newCapacity)
    {
        if (newCapacity <= Capacity)
            return;

        var newArr = ArrayPool<Chunk>.Shared.Rent(newCapacity);
        items.AsSpan(0, Count).CopyTo(newArr);
        ArrayPool<Chunk>.Shared.Return(items, true);

        items = newArr;
        Capacity = newCapacity;
    }

    public void TrimExcess()
    {
        int min = Count == 0 ? 1 : Count;

        var newArr = ArrayPool<Chunk>.Shared.Rent(min);
        items.AsSpan(0, Count).CopyTo(newArr);
        ArrayPool<Chunk>.Shared.Return(items, true);

        items = newArr;
        Capacity = min;
    }

    public void Clear()
    {
        for (int i = 0; i < Count; i++)
        {
            ref var chunk = ref this[i];
            chunk.Clear();
        }
    }

    public Span<Chunk> AsSpan()
    {
        return items.AsSpan(0, Count);
    }

    public ref Chunk this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref items.UnsafeAt(index);
    }
}
