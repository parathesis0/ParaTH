namespace ParaTH;

public abstract record Asset
{
    private readonly uint id = 0;

    protected Asset(uint id)
    {
        this.id = id;
    }

    public virtual bool Equals(Asset? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return id == other.id;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}
