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
        EntityCapacity = capacity;
        EntityCount = 0;
        ComponentIdToArrayIndex = componentIdToArrayIndex;

        Entities = new Entity[capacity];
        Components = new Array[types.Length];

        for (int i = 0; i < Components.Length; i++)
        {
            var type = types.UnsafeAt(i);
            Components.UnsafeAt(i) = ComponentArrayRegistry.CreateArray(type, capacity);
        }
    }

    public int EntityCount { get; set; }
    public int EntityCapacity { get; }
    public readonly bool IsFull => EntityCount >= EntityCapacity;

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
    public readonly int Remove(int index, ref Chunk lastChunk)
    {
        int lastIndex = lastChunk.EntityCount - 1;
        var lastEntity = lastChunk.Entities.UnsafeAt(lastIndex);
        Entities.UnsafeAt(index) = lastEntity;

        for (int i = 0; i < Components.Length; i++)
        {
            var srcArr = lastChunk.Components.UnsafeAt(i);
            var dstArr = Components.UnsafeAt(i);
            Array.Copy(srcArr, lastIndex, dstArr, index, 1);
            Array.Clear(lastChunk.Components.UnsafeAt(i), lastIndex, 1);
        }

        lastChunk.EntityCount--;
        return lastEntity.Id;
    }

    public readonly void Set<T0>(int index, in T0 component)
    {
        ref var arr = ref GetComponentArrayReference<T0>();
        Unsafe.Add(ref arr, index) = component;
    }

    public readonly ref T0 Get<T0>(int index)
    {
        ref var arr = ref GetComponentArrayReference<T0>();
        return ref Unsafe.Add(ref arr, index);
    }

    public readonly void GetFilledComponentSpan<T0>(out Span<T0> s0)
    {
        s0 = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T0>(), EntityCount);
    }

    public void Clear()
    {
        EntityCount = 0;
    }

    public readonly bool TryGetComponentArrayIndex(int ComponentId, out int arrayIndex)
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

    // not providing a variadic version because its only used internally
    public readonly void GetFullComponentSpan<T>(out Span<T> span)
    {
        span = MemoryMarshal.CreateSpan(ref GetComponentArrayReference<T>(), EntityCapacity);
    }

    public static void CopyEntityAndMatchingComponents(
        ref Chunk src, int srcIndex,
        ref Chunk dst, int dstIndex,
        ComponentTypeInfo[] srcTypes, int length)
    {
        var srcArrays = src.Components;
        var dstArrays = dst.Components;

        // copy entity
        Array.Copy(src.Entities, srcIndex, dst.Entities, dstIndex, length);

        // copy components, the component removing of World.RemoveComponent is done here
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

    public static void Fill<T0>(ref Chunk chunk, int startIndex, int length, in T0 component)
    {
        chunk.GetFullComponentSpan<T0>(out var span);
        span.Slice(startIndex, length).Fill(component);
    }
}
