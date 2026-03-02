namespace ParaTH;

public static class Component<T>
{
    public static readonly ComponentTypeInfo TypeInfo;

    static Component()
    {
        TypeInfo = ComponentRegistry.Add<T>();
    }
}
