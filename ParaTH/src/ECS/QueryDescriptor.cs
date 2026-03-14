using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ParaTH;

[SkipLocalsInit]
public partial struct QueryDescriptor
{
    public static readonly QueryDescriptor MatchAll = new QueryDescriptor()
    {
        hashCode = -1,
        All = 0,
        Any = UInt64.MaxValue,
        None = 0,
        Exclusive = 0
    };

    private int hashCode;

    public ulong All;
    public ulong Any;
    public ulong None;
    public ulong Exclusive;

    public QueryDescriptor()
    {
        hashCode = -1;

        All = 0;
        Any = UInt64.MaxValue;
        None = 0;
        Exclusive = 0;
    }

    [UnscopedRef]
    public ref QueryDescriptor WithAll<T0>()
    {
        All = Component<T0>.GroupMask;
        hashCode = -1;
        return ref this;
    }

    [UnscopedRef]
    public ref QueryDescriptor WithAny<T0>()
    {
        Any = Component<T0>.GroupMask;
        hashCode = -1;
        return ref this;
    }

    [UnscopedRef]
    public ref QueryDescriptor WithNone<T0>()
    {
        None = Component<T0>.GroupMask;
        hashCode = -1;
        return ref this;
    }

    [UnscopedRef]
    public ref QueryDescriptor WithExclusive<T0>()
    {
        Exclusive = Component<T0>.GroupMask;
        hashCode = -1;
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
        return hash != -1 ? hash : -2;
    }

    public readonly bool Equals(QueryDescriptor other)
    {
        return this.All == other.All &&
            this.Any == other.Any &&
            this.None == other.None &&
            this.Exclusive == other.Exclusive;
    }

    public override bool Equals(object? obj) => obj is QueryDescriptor other && Equals(other);

    public static bool operator ==(QueryDescriptor left, QueryDescriptor right) => left.Equals(right);

    public static bool operator !=(QueryDescriptor left, QueryDescriptor right) => !(left == right);

}
