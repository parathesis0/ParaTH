using System.Buffers;
using System.Runtime.CompilerServices;

namespace ParaTH;

[SkipLocalsInit]
public sealed class ChunkList
{
    private Chunk[] Items;
    public int Count { get; private set; }
    public int Capacity { get; private set; }

    public ChunkList(int capacity)
    {
        Count = 0;
        Capacity = capacity;
        Items = ArrayPool<Chunk>.Shared.Rent(capacity);
    }

    public void Add(Chunk chunk)
    {
        if (Count == Capacity)
            EnsureCapacity();
        Items.UnsafeAt(Count++) = chunk;
    }

    private void EnsureCapacity()
    {
        int newCapacity = Capacity * 2;

        var newArr = ArrayPool<Chunk>.Shared.Rent(newCapacity);
        Items.AsSpan(0, Count).CopyTo(newArr);
        ArrayPool<Chunk>.Shared.Return(Items, true);

        Items = newArr;
        Capacity = newCapacity;
    }

    public void TrimExcess()
    {
        int min = Count == 0 ? 1 : Count;

        var newArr = ArrayPool<Chunk>.Shared.Rent(min);
        Items.AsSpan(0, Count).CopyTo(newArr);
        ArrayPool<Chunk>.Shared.Return(Items, true);

        Items = newArr;
        Capacity = min;
    }

    public ref Chunk this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Items.UnsafeAt(index);
    }
}
