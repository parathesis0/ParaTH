using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ParaTH;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public readonly struct Entity(ushort id, ushort version) : IEquatable<Entity>
{
    public readonly ushort Id = id;
    public readonly ushort Version = version;

    public bool Equals(Entity other) => Unsafe.BitCast<Entity, uint>(this) == Unsafe.BitCast<Entity, uint>(other);

    public override bool Equals(object? obj) => obj is Entity other && Equals(other);

    public static bool operator ==(Entity left, Entity right) => left.Equals(right);

    public static bool operator !=(Entity left, Entity right) => !(left == right);

    public override int GetHashCode() => (Id << 16) | Version;
}
