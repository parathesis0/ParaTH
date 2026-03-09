namespace ParaTH;

// todo: variadic source gen wip, only GroupTypeInfo field is needed for variadic versions
public static class Component<T>
{
    public static readonly ComponentTypeInfo TypeInfo;
    public static readonly ComponentTypeInfo[] GroupTypeInfo;
    public static readonly ulong GroupMask;
    // todo: maybe cache mask and hash here?

    static Component()
    {
        TypeInfo = ComponentRegistry.Add<T>();
        GroupTypeInfo = [TypeInfo];
        foreach (var type in GroupTypeInfo)
            GroupMask |= type.Mask;
    }
}
