using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ParaTH;

public partial struct QueryDescriptor
{
    [UnscopedRef]
    public ref QueryDescriptor WithNone<T0, T1>()
    {
        None = Component<T0, T1>.GroupMask;
        return ref this;
    }
}
