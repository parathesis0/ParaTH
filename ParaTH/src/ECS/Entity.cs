using System.Runtime.CompilerServices;

namespace ParaTH;

public readonly struct Entity : IEquatable<Entity>
{
    private const int VersionBits = 12;
    private const int IdBits = 32 - VersionBits;
    private const uint VersionMask = (1u << VersionBits) - 1;
    private const uint IdMask = (1u << IdBits) - 1;
    private const uint VersionIncrement = 1u << IdBits;

    public readonly uint PackedValue;

    public Entity(int id, int version)
    {
        PackedValue = ((uint)id & IdMask) | (((uint)version & VersionMask) << IdBits);
    }

    private Entity(uint packedValue)
    {
        PackedValue = packedValue;
    }

    public int Id
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int)(PackedValue & IdMask);
    }

    public int Version
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int)(PackedValue >> IdBits);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Entity BumpVersion()
    {
        unchecked
        {
            return new Entity(PackedValue + VersionIncrement);
        }
    }

    public bool Equals(Entity other) => this.PackedValue == other.PackedValue;

    public override bool Equals(object? obj) => obj is Entity other && Equals(other);

    public static bool operator ==(Entity left, Entity right) => left.Equals(right);

    public static bool operator !=(Entity left, Entity right) => !(left == right);

    public override int GetHashCode() => (int)PackedValue;
}
