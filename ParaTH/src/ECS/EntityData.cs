namespace ParaTH;

// todo: def wasted space here
public record struct EntityData
{
    public Archetype Archetype;
    public Slot Slot;
    public int Version;

    public EntityData(Archetype archetype, Slot slot, int version)
    {
        Archetype = archetype;
        Slot = slot;
        Version = version;
    }
}
