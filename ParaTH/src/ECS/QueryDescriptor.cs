using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ParaTH;

public partial struct QueryDescriptor
{
    private int hashCode;

    public ulong All;
    public ulong Any;
    public ulong None;
    public ulong Exclusive;

    public QueryDescriptor()
    {
        hashCode = -1;
    }

    [UnscopedRef]
    public ref QueryDescriptor WithAll<T0>()
    {
        All = Component<T0>.GroupMask;
        return ref this;
    }

    [UnscopedRef]
    public ref QueryDescriptor WithAny<T0>()
    {
        Any = Component<T0>.GroupMask;
        return ref this;
    }

    [UnscopedRef]
    public ref QueryDescriptor WithNone<T0>()
    {
        None = Component<T0>.GroupMask;
        return ref this;
    }

    [UnscopedRef]
    public ref QueryDescriptor WithExclusive<T0>()
    {
        Exclusive = Component<T0>.GroupMask;
        return ref this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        if (hashCode != -1)
            return hashCode;

        // idk claude told me this is fast
        ulong h = (All       * 0x9E3779B97F4A7C15UL)
                ^ (Any       * 0x517CC1B727220A95UL)
                ^ (None      * 0x6C62272E07BB0142UL)
                ^ (Exclusive * 0x9E3779B185EBCA87UL);

        // avalanche finalizer
        h ^= h >> 32;
        var hash = (int)(h ^ (h >> 16));
        hashCode = hash;
        return hash;
    }
}
