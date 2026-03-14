namespace ParaTH;

// used for creating single component array during runtime without reflection
public static class ComponentArrayRegistry
{
    private static readonly Func<int, Array>[] arrayFactories =
        new Func<int, Array>[ComponentRegistry.MaxComponents];

    public static void AddFactory<T>()
    {
        arrayFactories[Component<T>.TypeInfo.Id] = Create<T>;
    }

    public static Array CreateArray(ComponentTypeInfo type, int capacity)
    {
        return arrayFactories[type.Id].Invoke(capacity);
    }

    private static T[] Create<T>(int capacity)
    {
        return new T[capacity];
    }
}
