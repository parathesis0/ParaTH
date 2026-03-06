using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ParaTH;

[SkipLocalsInit]
public sealed class JaggedArray<T>
{
    [SkipLocalsInit]
    private record struct Bucket
    {
        public readonly T[] Array;
        public readonly ulong Occupied;

        public Bucket(int capacity, T filler)
        {
            Array = new T[capacity];
            System.Array.Fill(Array, filler);
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        public readonly bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Count <= 0;
        }

        public ref T this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Array.UnsafeAt(i);
        }

        public void Clear(T filler)
        {
            Count = 0;
            System.Array.Fill(Array, filler);
        }
    }

    private readonly int bucketSize;
    private readonly int bucketSizeMinusOne;
    private readonly int bucketSizeShift;
    private Bucket[] buckets;
    private readonly T filler;

    public int Capacity => bucketSize * buckets.Length;
    public int BucketCount => buckets.Length;

    public JaggedArray(int bucketSize, int capacity = 64, T filler = default!)
    {
        this.bucketSize = (int)BitOperations.RoundUpToPowerOf2((uint)bucketSize);
        bucketSizeMinusOne = this.bucketSize - 1;
        bucketSizeShift = BitOperations.Log2((uint)bucketSize);

        buckets = new Bucket[(capacity / bucketSize) + 1];

        this.filler = filler;

        for (int i = 0; i < buckets.Length; i++)
            buckets.UnsafeAt(i) = new Bucket(bucketSize, filler);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(int index, in T item)
    {
        IndexToSlot(index, out var bucketIndex, out var itemIndex);

        ref var bucket = ref buckets[bucketIndex];
        bucket[itemIndex] = item;

        if (!EqualityComparer<T>.Default.Equals(item, filler))
            bucket.Count++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureCapacity(int newCapacity)
    {
        if (newCapacity <= Capacity)
            return;

        var oldBucketCount = BucketCount;
        var newBucketCount = (newCapacity / bucketSize) + 1;

        Array.Resize(ref buckets, newBucketCount);

        for (int i = oldBucketCount; i < newBucketCount; i++)
            buckets.UnsafeAt(i) = new Bucket(bucketSize, filler);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimExcess()
    {
        var oldBucketCount = BucketCount;
        var count = 0;
        for (int i = oldBucketCount - 1; i >= 0; i--)
        {
            ref var bucket = ref buckets[i];
            if (!bucket.IsEmpty)
                break;

            count++;
        }

        var newBucketCount = oldBucketCount - count;
        Array.Resize(ref buckets, newBucketCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(int index, out T value)
    {
        if (index >= Capacity)
        {
            value = filler;
            return false;
        }

        ref var item = ref this[index];

        if (EqualityComparer<T>.Default.Equals(item, filler))
        {
            value = filler;
            return false;
        }

        value = item;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(int index)
    {
        if (index >= Capacity)
            return false;

        ref var item = ref this[index];

        return !EqualityComparer<T>.Default.Equals(item, filler);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove(int index)
    {
        IndexToSlot(index, out var bucketIndex, out var itemIndex);

        ref var bucket = ref buckets[bucketIndex];
        ref var item = ref this[itemIndex];
        item = filler;
        if (!EqualityComparer<T>.Default.Equals(item, filler))
            bucket.Count--;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        for (int i = 0; i < BucketCount; i++)
        {
            ref var bucket = ref buckets[i];
            if (bucket.IsEmpty)
                continue;

            bucket.Clear(filler);
        }
    }

    public ref T this[int i]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            IndexToSlot(i, out var bucketIndex, out var itemIndex);
            return ref buckets[bucketIndex][itemIndex];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void IndexToSlot(int index, out int bucketIndex, out int itemIndex)
    {
        Debug.Assert(index >= 0, "Id cannot be negative.");

        bucketIndex = index >> bucketSizeShift;
        itemIndex = index & bucketSizeMinusOne;
    }
}
