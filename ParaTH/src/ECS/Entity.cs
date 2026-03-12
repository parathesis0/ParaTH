using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ParaTH;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public readonly struct Entity : IEquatable<Entity>
{
    private const int VersionBits  = 12;
    private const int IdBits       = 32 - VersionBits;
    private const uint VersionMask = (1u << VersionBits) - 1;
    private const uint IdMask      = (1u << IdBits) - 1;

    public readonly uint PackedValue;

    public Entity(int id, int version)
    {
        PackedValue = (((uint)id & IdMask) << VersionBits) | ((uint)version & VersionMask); ;
    }

    private Entity(uint packedValue)
    {
        PackedValue = packedValue;
    }

    public int Id
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int)(PackedValue >> VersionBits);
    }

    public int Version
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int)(PackedValue & VersionMask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Entity BumpVersion()
    {
        return new Entity((PackedValue & ~VersionMask) | ((PackedValue + 1) & VersionMask));
    }
    public bool Equals(Entity other) => this.PackedValue == other.PackedValue;

    public override bool Equals(object? obj) => obj is Entity other && Equals(other);

    public static bool operator ==(Entity left, Entity right) => left.Equals(right);

    public static bool operator !=(Entity left, Entity right) => !(left == right);

    public override int GetHashCode() => (int)PackedValue;
}
