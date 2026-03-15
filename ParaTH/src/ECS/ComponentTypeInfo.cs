namespace ParaTH;

public readonly struct ComponentTypeInfo(int id, int byteSize) : IEquatable<ComponentTypeInfo>
{
    public readonly int Id = id;
    public readonly int ByteSize = byteSize;

    public ComponentMask Mask => ComponentMask.FromBit(Id);

    public Type Type => ComponentRegistry.IdToType[Id];

    public bool Equals(ComponentTypeInfo other) => this.Id == other.Id;

    public override bool Equals(object? obj) => obj is ComponentTypeInfo other && Equals(other);

    public static bool operator ==(ComponentTypeInfo left, ComponentTypeInfo right) => left.Equals(right);

    public static bool operator !=(ComponentTypeInfo left, ComponentTypeInfo right) => !(left == right);

    public override int GetHashCode() => Id;
}
