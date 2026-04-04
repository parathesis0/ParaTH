using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ParaTH;

[SkipLocalsInit]
public partial struct QueryDescriptor
{
    public static readonly QueryDescriptor MatchAll = new QueryDescriptor();

    private int hashCode;

    public ComponentMask All;
    public ComponentMask Any;
    public ComponentMask None;
    public ComponentMask Exclusive;

    public QueryDescriptor()
    {
        hashCode = 0;
    }

    [UnscopedRef]
    public ref QueryDescriptor WithAll<T0>()
    {
        All |= Component<T0>.GroupMask;
        hashCode = 0;
        return ref this;
    }

    [UnscopedRef]
    public ref QueryDescriptor WithAny<T0>()
    {
        Any |= Component<T0>.GroupMask;
        hashCode = 0;
        return ref this;
    }

    [UnscopedRef]
    public ref QueryDescriptor WithNone<T0>()
    {
        None |= Component<T0>.GroupMask;
        hashCode = 0;
        return ref this;
    }

    [UnscopedRef]
    public ref QueryDescriptor WithExclusive<T0>()
    {
        Exclusive |= Component<T0>.GroupMask;
        hashCode = 0;
        return ref this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        if (hashCode != 0)
            return hashCode;

        var hash = HashCode.Combine(
            All.GetHashCode(), Any.GetHashCode(),
            None.GetHashCode(), Exclusive.GetHashCode());
        hashCode = hash;
        return hash != 0 ? hash : -1;
    }

    public readonly bool Equals(QueryDescriptor other)
    {
        return this.All == other.All &&
            this.Any == other.Any &&
            this.None == other.None &&
            this.Exclusive == other.Exclusive;
    }

    public override readonly bool Equals(object? obj) => obj is QueryDescriptor other && Equals(other);

    public static bool operator ==(QueryDescriptor left, QueryDescriptor right) => left.Equals(right);

    public static bool operator !=(QueryDescriptor left, QueryDescriptor right) => !(left == right);

}
