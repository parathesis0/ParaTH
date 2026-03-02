namespace ParaTH;

public readonly record struct ComponentTypeInfo
{
    public readonly int Id;
    public readonly int ByteSize;

    public ComponentTypeInfo(int id, int byteSize)
    {
        Id = id;
        ByteSize = byteSize;
    }

    public ulong Mask => 1UL << Id;

    public Type Type => ComponentRegistry.IdToType[Id];
}
