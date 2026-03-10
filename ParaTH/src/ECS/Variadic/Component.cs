using System;
using System.Collections.Generic;
using System.Reflection;

namespace ParaTH;

public sealed class Component<T0, T1>
{
    public static readonly ComponentTypeInfo[] GroupTypeInfo;
    public static readonly ulong GroupMask;

    static Component()
    {
        GroupTypeInfo = [ Component<T0>.TypeInfo, Component<T1>.TypeInfo];
        foreach (var type in GroupTypeInfo)
            GroupMask |= type.Mask;
    }
}
