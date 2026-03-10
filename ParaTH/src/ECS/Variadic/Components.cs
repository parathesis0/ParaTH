using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParaTH;

[SkipLocalsInit]
public ref struct Components<T0, T1>
{
    public ref T0 c0;
    public ref T1 c1;

    [SkipLocalsInit]
    public Components(ref T0 c0, ref T1 c1)
    {
        this.c0 = ref c0;
        this.c1 = ref c1;
    }

    [SkipLocalsInit]
    public readonly void Deconstruct(out T0 c0, out T1 c1)
    {
        c0 = this.c0;
        c1 = this.c1;
    }
}
