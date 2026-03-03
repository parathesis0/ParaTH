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
            Components.UnsafeAt(i) = Array.CreateInstance(type, capacity);
        }
    }

    public int Count { get; set; }
    public int Capacity { get; }
    public readonly bool IsFull => Count >= Capacity;

    // variadic source gen wip
    // returns the index of the added entity
    public int Add<T>(Entity entity, in T component)
    {
        var index = Count;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T>().UnsafeAt(index) = component;
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

    // variadic source gen wip
    public readonly void Set<T>(int index, in T component)
    {
        ref var arr = ref GetComponentArrayReference<T>();
        Unsafe.Add(ref arr, index) = component; 
    }

    // variadic source gen wip
    public readonly ref T Get<T>(int index)
    {
        ref var arr = ref GetComponentArrayReference<T>();
        return ref Unsafe.Add(ref arr, index);
    }

    // variadic source gen wip
    public readonly Span<T> GetAsSpan<T>()
    {
        return MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T>(), Count);
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
