using System.Buffers;
using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed class UnsafePooledList<T> : IDisposable
{
    private T[] items;
    private int count;

    public int Count => count;
    public int Capacity => items.Length;

    public UnsafePooledList(int capacity = 4)
    {
        items = capacity <= 0 ? [] : ArrayPool<T>.Shared.Rent(capacity);
        count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        T[] items = this.items;
        int count = this.count;

        if ((uint)count < (uint)items.Length)
        {
            items.UnsafeAt(count) = item;
            this.count = count + 1;
        }
        else
        {
            AddWithResize(item);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddWithResize(T item)
    {
        int newCapacity = items.Length == 0 ? 4 : items.Length * 2;
        Resize(newCapacity);

        items.UnsafeAt(count++) = item;
    }


    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Resize(int newCapacity)
    {
        T[] newItems = ArrayPool<T>.Shared.Rent(newCapacity);

        if (count > 0)
            new ReadOnlySpan<T>(items, 0, count).CopyTo(newItems);

        if (items.Length > 0)
        {
            ArrayPool<T>.Shared.Return(items,
                RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }

        items = newItems;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan()
    {
        return new Span<T>(items, 0, count);
    }

    public T[] ToArray()
    {
        if (count == 0)
            return [];
        return AsSpan().ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear(bool clearValue)
    {
        if (clearValue && count > 0)
            Array.Clear(items, 0, count);

        count = 0;
    }

    public void Dispose()
    {
        T[] items = this.items;
        if (items.Length > 0)
        {
            this.items = [];
            count = 0;

            ArrayPool<T>.Shared.Return(items,
                RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref items.UnsafeAt(index);
    }
}
