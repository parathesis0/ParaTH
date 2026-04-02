using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ParaTH;

// don't rent over 2^20(unity) or 2^30(>=.net9)
// else ArrayPool.Shared stops return powers of 2 and this breaks
public sealed class UnsafePooledQueue<T> : IDisposable
{
    private T[] items;
    private int mask;
    private int head;
    private int tail;
    private int size;

    public int Count => size;
    public int Capacity => items.Length;

    public UnsafePooledQueue(int capacity = 4)
    {
        capacity = (int)BitOperations.RoundUpToPowerOf2((uint)capacity);

        items = ArrayPool<T>.Shared.Rent(capacity);
        mask = items.Length - 1;
        head = 0;
        tail = 0;
        size = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item)
    {
        if (size == items.Length)
            Grow();

        items.UnsafeAt(tail) = item;
        tail = (tail + 1) & mask;
        size++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Dequeue()
    {
        int h = head;
        T removed = items.UnsafeAt(h);

        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            items.UnsafeAt(h) = default!;

        head = (h + 1) & mask;
        size--;
        return removed;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryDequeue(out T result)
    {
        if (size == 0)
        {
            result = default!;
            return false;
        }
        result = Dequeue();
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T PeekHead() => ref items.UnsafeAt(head);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T PeekTail() => ref items.UnsafeAt((tail - 1) & mask);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPeekHead(out T result)
    {
        if (size == 0) { result = default!; return false; }
        result = items.UnsafeAt(head);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPeekTail(out T result)
    {
        if (size == 0) { result = default!; return false; }
        result = items.UnsafeAt((tail - 1) & mask);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AsSpans(out Span<T> first, out Span<T> second)
    {
        if (size == 0)
        {
            first = [];
            second = [];
        }
        else if (head < tail)
        {
            first = new Span<T>(items, head, size);
            second = [];
        }
        else
        {
            first = new Span<T>(items, head, items.Length - head);
            second = new Span<T>(items, 0, tail);
        }
    }

    public T[] ToArray()
    {
        if (size == 0)
            return [];

        T[] arr = new T[size];
        if (head < tail)
        {
            Array.Copy(items, head, arr, 0, size);
        }
        else
        {
            int firstLen = items.Length - head;
            Array.Copy(items, head, arr, 0, firstLen);
            Array.Copy(items, 0, arr, firstLen, tail);
        }
        return arr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() && size > 0)
        {
            if (head < tail)
            {
                items.AsSpan(head, size).Clear();
            }
            else
            {
                items.AsSpan(head, items.Length - head).Clear();
                items.AsSpan(0, tail).Clear();
            }
        }

        head = tail = size = 0;
    }

    public void Dispose()
    {
        T[] arr = items;
        if (arr.Length > 0)
        {
            items = [];
            mask = 0;
            head = tail = size = 0;
            ArrayPool<T>.Shared.Return(arr,
                RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow()
    {
        int newCapacity = items.Length == 0 ? 4 : items.Length * 2;
        SetCapacity(newCapacity);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void SetCapacity(int capacity)
    {
        T[] newArray = ArrayPool<T>.Shared.Rent(capacity);

        if (size > 0)
        {
            if (head < tail)
            {
                Array.Copy(items, head, newArray, 0, size);
            }
            else
            {
                int firstLen = items.Length - head;
                Array.Copy(items, head, newArray, 0, firstLen);
                Array.Copy(items, 0, newArray, firstLen, tail);
            }
        }

        if (items.Length > 0)
        {
            ArrayPool<T>.Shared.Return(items,
                RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }

        items = newArray;
        mask = newArray.Length - 1;
        head = 0;
        tail = (size == newArray.Length) ? 0 : size;
    }
}
