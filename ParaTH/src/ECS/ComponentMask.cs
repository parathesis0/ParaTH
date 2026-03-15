using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace ParaTH;

public readonly struct ComponentMask : IEquatable<ComponentMask>
{
    public static readonly ComponentMask Zero = default;
    public static readonly ComponentMask AllBits = new(Vector256.Create(ulong.MaxValue));

    public readonly Vector256<ulong> Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ComponentMask(Vector256<ulong> value) => Value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentMask FromBit(int id)
    {
        var bit = 1UL << (id & 63);
        return (id >> 6) switch
        {
            0 => new(Vector256.Create(bit, 0UL, 0UL, 0UL)),
            1 => new(Vector256.Create(0UL, bit, 0UL, 0UL)),
            2 => new(Vector256.Create(0UL, 0UL, bit, 0UL)),
            _ => new(Vector256.Create(0UL, 0UL, 0UL, bit)),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentMask operator |(ComponentMask a, ComponentMask b)
        => new(a.Value | b.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentMask operator &(ComponentMask a, ComponentMask b)
        => new(a.Value & b.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentMask operator ~(ComponentMask a)
        => new(~a.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ComponentMask a, ComponentMask b)
        => a.Value == b.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ComponentMask a, ComponentMask b)
        => a.Value != b.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ComponentMask other) => this == other;

    public override bool Equals(object? obj) => obj is ComponentMask other && Equals(other);

    public override int GetHashCode()
    {
        var v = Value;
        return HashCode.Combine(
            v.GetElement(0), v.GetElement(1),
            v.GetElement(2), v.GetElement(3));
    }
}
