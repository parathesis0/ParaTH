namespace ParaTH;

public readonly struct Entity(ushort id, ushort version) : IEquatable<Entity>
{
    public readonly ushort Id = id;
    public readonly ushort Version = version;

    public bool Equals(Entity other) => ((Id ^ other.Id) | (Version ^ other.Version)) == 0;

    public override bool Equals(object? obj) => obj is Entity other && Equals(other);

    public static bool operator ==(Entity left, Entity right) => left.Equals(right);

    public static bool operator !=(Entity left, Entity right) => !(left == right);
    
    public override int GetHashCode() => (Id << 16) | Version;
}
