using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ParaTH;

[SkipLocalsInit]
public sealed class SparsePagedArray<T>
{
    [SkipLocalsInit]
    private struct SparsePage
    {
        public T[] Items;
        public ulong[] Occupied;

        public SparsePage(int capacity)
        {
            Capacity = capacity;
            Items = [];
            Occupied = [];
        }

        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureCapacity()
        {
            if (Items.Length != 0)
                return;

            var capacity = Capacity;
            Items = new T[capacity];
            Occupied = new ulong[(capacity + 63) >> 6];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TrimExcess()
        {
            if (Count > 0)
                return;

            Items = [];
            Occupied = [];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsSet(int index)
        {
            return (Occupied.UnsafeAt(index >> 6) & (1UL << (index & 0x3F))) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int index)
        {
            Occupied.UnsafeAt(index >> 6) |= (1UL << (index & 0x3F));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unset(int index)
        {
            Occupied.UnsafeAt(index >> 6) &= ~(1UL << (index & 0x3F));
        }

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Items.UnsafeAt(index);
        }

        public readonly bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Count <= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                Array.Clear(Items);
            Array.Clear(Occupied);
            Count = 0;
        }
    }

    private readonly int pageSize;
    private readonly int pageSizeMinusOne;
    private readonly int pageSizeShift;
    private SparsePage[] pages;

    public int Capacity => pageSize * pages.Length;
    public int PageCount => pages.Length;

    public SparsePagedArray(int pageSize, int capacity = 64)
    {
        pageSize = (int)BitOperations.RoundUpToPowerOf2((uint)pageSize);
        this.pageSize = pageSize;
        pageSizeMinusOne = pageSize - 1;
        pageSizeShift = BitOperations.Log2((uint)pageSize);

        pages = new SparsePage[(capacity / pageSize) + 1];

        for (int i = 0; i < pages.Length; i++)
            pages.UnsafeAt(i) = new SparsePage(pageSize);
    }

    // ensure capacity before you add
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(int index, in T item)
    {
        IndexToSlot(index, out var pageIndex, out var itemIndex);
        ref var page = ref pages.UnsafeAt(pageIndex);

        page.EnsureCapacity();

        if (!page.IsSet(itemIndex))
            page.Count++;
        page.Set(itemIndex);
        page[itemIndex] = item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureCapacity(int newCapacity)
    {
        if (newCapacity <= Capacity)
            return;

        var oldpageCount = PageCount;
        var newpageCount = (newCapacity / pageSize) + 1;

        Array.Resize(ref pages, newpageCount);

        for (int i = oldpageCount; i < newpageCount; i++)
            pages.UnsafeAt(i) = new SparsePage(pageSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimExcess()
    {
        var oldpageCount = PageCount;
        var count = 0;
        for (int i = oldpageCount - 1; i >= 0; i--)
        {
            ref var page = ref pages.UnsafeAt(i);
            if (!page.IsEmpty)
                break;

            count++;
        }

        var newpageCount = Math.Max(1, oldpageCount - count);
        Array.Resize(ref pages, newpageCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(int index, out T value)
    {
        if ((uint)index >= (uint)Capacity)
        {
            value = default!;
            return false;
        }

        IndexToSlot(index, out var pageIndex, out var itemIndex);
        ref var page = ref pages.UnsafeAt(pageIndex);

        if (page.IsEmpty)
        {
            value = default!;
            return false;
        }

        if (!page.IsSet(itemIndex))
        {
            value = default!;
            return false;
        }

        value = page[itemIndex];
        return true;
    }

    // nullref if value doesn't exist
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T TryGetRef(int index)
    {
        if ((uint)index >= (uint)Capacity)
            return ref Unsafe.NullRef<T>();

        IndexToSlot(index, out var pageIndex, out var itemIndex);
        ref var page = ref pages.UnsafeAt(pageIndex);

        if (!page.IsSet(itemIndex))
            return ref Unsafe.NullRef<T>();

        return ref page[itemIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(int index)
    {
        if ((uint)index >= (uint)Capacity)
            return false;

        IndexToSlot(index, out var pageIndex, out var itemIndex);
        ref var page = ref pages.UnsafeAt(pageIndex);

        if (page.IsEmpty)
            return false;

        return page.IsSet(itemIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove(int index)
    {
        if ((uint)index >= (uint)Capacity)
            return;

        IndexToSlot(index, out var pageIndex, out var itemIndex);
        ref var page = ref pages.UnsafeAt(pageIndex);

        if (page.IsEmpty)
            return;

        if (page.IsSet(itemIndex))
        {
            page.Unset(itemIndex);
            page.Count--;
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                page[itemIndex] = default!;

            //page.TrimExcess();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        for (int i = 0; i < PageCount; i++)
        {
            ref var page = ref pages.UnsafeAt(i);
            if (page.IsEmpty)
                continue;

            page.Clear();
        }
    }

    // used only for accessing
    public ref T this[int i]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Debug.Assert(ContainsKey(i), $"Key '{i}' not found.");
            IndexToSlot(i, out var pageIndex, out var itemIndex);
            return ref pages.UnsafeAt(pageIndex)[itemIndex];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void IndexToSlot(int index, out int pageIndex, out int itemIndex)
    {
        Debug.Assert(index >= 0, "Index cannot be negative.");
        pageIndex = index >> pageSizeShift;
        Debug.Assert(pageIndex < pages.Length,
            "Index out of range. Did you Call EnsureCapacity before adding?");
        itemIndex = index & pageSizeMinusOne;
    }
}
