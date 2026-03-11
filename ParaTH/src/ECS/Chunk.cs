using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        EntityCount = 0;
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

    public int EntityCount { get; set; }
    public int Capacity { get; }
    public readonly bool IsFull => EntityCount >= Capacity;

    // todo: variadic source gen wip
    // returns the index of the added entity
    public int Add<T0>(Entity entity, in T0 component)
    {
        var index = EntityCount;
        Entities.UnsafeAt(index) = entity;
        GetComponentArray<T0>().UnsafeAt(index) = component;
        EntityCount++;
        return index;
    }

    // returns the index of the reserved spot
    // blatant violation of the dry principle
    // should only be called by Archetype.Reserve, fuck
    public int Reserve()
    {
        var index = EntityCount;
        EntityCount++;
        return index;
    }

    // swap and pop with the last chunk's last entity
    // returns the moved entity's id
    public readonly ushort Remove(int index, ref Chunk lastChunk)
    {
        int lastIndex = lastChunk.EntityCount - 1;
        var lastEntity = lastChunk.Entities.UnsafeAt(lastIndex);
        Entities.UnsafeAt(index) = lastEntity;

        for (int i = 0; i < Components.Length; i++)
        {
            var srcArr = lastChunk.Components.UnsafeAt(i);
            var dstArr = Components.UnsafeAt(i);
            Array.Copy(srcArr, lastIndex, dstArr, index, 1);
        }

        lastChunk.EntityCount--;
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
        return MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), EntityCount);
    }

    public void Clear()
    {
        EntityCount = 0;
    }

    public bool TryGetComponentArrayIndex(int ComponentId, out int arrayIndex)
    {
        var mapping = ComponentIdToArrayIndex;

        if ((uint)ComponentId >= (uint)mapping.Length)
        {
            arrayIndex = Archetype.InvalidIndex;
            return false;
        }

        arrayIndex = mapping.UnsafeAt(ComponentId);
        return arrayIndex != Archetype.InvalidIndex;
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

    public static void CopyEntityComponents(
        ref Chunk src, int srcIndex,
        ref Chunk dst, int dstIndex,
        ComponentTypeInfo[] srcTypes, int length)
    {
        var srcArrays = src.Components;
        var dstArrays = dst.Components;

        // copy entity
        dst.Entities.UnsafeAt(dstIndex) = src.Entities.UnsafeAt(srcIndex);

        // copy components, the component removing of World.Remove is done here
        for (int i = 0; i < srcTypes.Length; i++)
        {
            var srcArray = srcArrays[i];
            var typeId = srcTypes[i].Id;

            // if source type not in dst type it will be removed
            if (!dst.TryGetComponentArrayIndex(typeId, out int arrayId))
                continue;

            var dstArray = dstArrays[arrayId];

            Array.Copy(srcArray, srcIndex, dstArray, dstIndex, length);
        }
    }
}
