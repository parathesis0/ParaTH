namespace ParaTH;

public static class Component<T>
{
    public static readonly ComponentTypeInfo TypeInfo;
    public static readonly ComponentTypeInfo[] GroupTypeInfo;
    public static readonly ulong GroupMask;

    static Component()
    {
        TypeInfo = ComponentRegistry.Add<T>();
        ComponentArrayRegistry.AddFactory<T>();
        GroupTypeInfo = [TypeInfo];
        foreach (var type in GroupTypeInfo)
            GroupMask |= type.Mask;
    }
}
