using System.Runtime.CompilerServices;

namespace ParaTH;

public static class ComponentRegistry
{
    private static int nextId;
    private static readonly Dictionary<Type, ComponentTypeInfo> typeToComponentTypeInfo = new(64);
    private static readonly Type[] idToType = new Type[64];

    public static int Size => nextId;

    public static IReadOnlyDictionary<Type, ComponentTypeInfo> TypeTocomponentInfo => typeToComponentTypeInfo;

    public static ReadOnlySpan<Type> IdToType => idToType.AsSpan(0, Size);

    public static ComponentTypeInfo Add<T>()
    {
        return Add(typeof(T), Unsafe.SizeOf<T>());
    }

    private static ComponentTypeInfo Add(Type type, int typeByteSize)
    {
        if (typeToComponentTypeInfo.TryGetValue(type, out var info))
            return info;

        if (Size == 64)
            throw new InvalidOperationException("Component types exceeded the upper limit of 64.");

        var typeInfo = new ComponentTypeInfo(nextId, typeByteSize);
        typeToComponentTypeInfo.Add(type, typeInfo);
        idToType[nextId] = type;
        nextId++;

        return typeInfo;
    }
}
