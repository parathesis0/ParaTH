using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ParaTH;

[SkipLocalsInit]
public partial struct Chunk
{
    public readonly int[] ComponentIdToArrayIndex;
    public readonly Entity[] Entities;
    public readonly Array[] Components;

    public Chunk(int capacity, int[] componentIdToArrayIndex, Span<ComponentTypeInfo> types)
    {
        Capacity = capacity;
        Count = 0;
        ComponentIdToArrayIndex = componentIdToArrayIndex;

        Entities = new Entity[capacity];
        Components = new Array[types.Length];

        for (int i = 0; i < Components.Length; i++)
        {
            var type = types.UnsafeAt(i).Type;
            // todo: reflection is slow, array registry maybe
            Components.UnsafeAt(i) = Array.CreateInstance(type, capacity);
        }
    }

    public int Count { get; set; }
    public int Capacity { get; }
    public readonly bool IsFull => Count >= Capacity;

    // todo: variadic source gen wip
    // returns the index of the added entity
    public int Add<T0>(Entity entity, in T0 component)
    {
        var index = Count;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = component;
        Count++;
        return index;
    }

    // swap and pop with the last chunk's last entity, returns the moved entity's id
    public readonly ushort Remove(int index, ref Chunk lastChunk)
    {
        int lastIndex = lastChunk.Count - 1;
        var lastEntity = lastChunk.Entities.UnsafeAt(lastIndex);
        Entities.UnsafeAt(index) = lastEntity;

        for (int i = 0; i < Components.Length; i++)
        {
            var srcArr = lastChunk.Components.UnsafeAt(i);
            var dstArr = Components.UnsafeAt(i);
            Array.Copy(srcArr, lastIndex, dstArr, index, 1);
        }

        lastChunk.Count--;
        return lastEntity.Id;
    }

    // todo: variadic source gen wip
    public readonly void Set<T0>(int index, in T0 component)
    {
        ref var arr = ref GetComponentArrayReference<T0>();
        Unsafe.Add(ref arr, index) = component;
    }

    // todo: variadic source gen wip
    public readonly ref T0 Get<T0>(int index)
    {
        ref var arr = ref GetComponentArrayReference<T0>();
        return ref Unsafe.Add(ref arr, index);
    }

    // todo: variadic source gen wip
    public readonly Span<T0> GetAsSpan<T0>()
    {
        return MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), Count);
    }

    public void Clear()
    {
        Count = 0;
    }

    private readonly ref T GetComponentArrayReference<T>()
    {
        return ref MemoryMarshal.GetArrayDataReference(GetComponentArray<T>());
    }

    private readonly T[] GetComponentArray<T>()
    {
        int typeId = Component<T>.TypeInfo.Id;
        int arrayId = ComponentIdToArrayIndex.UnsafeAt(typeId);
        return Unsafe.As<T[]>(Components.UnsafeAt(arrayId));
    }
}
