namespace ParaTH;

public abstract class Asset : IEquatable<Asset>
{
    private readonly uint id = 0;

    protected Asset(uint id)
    {
        this.id = id;
    }

    public bool Equals(Asset? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return id == other.id;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Asset);
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    public static bool operator ==(Asset? left, Asset? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Asset? left, Asset? right)
    {
        return !(left == right);
    }
}
