namespace ParaTH;

public static class Component<T> where T : struct
{
    public static readonly ComponentTypeInfo TypeInfo;

    static Component()
    {
        TypeInfo = ComponentRegistry.Add<T>();
    }
}
