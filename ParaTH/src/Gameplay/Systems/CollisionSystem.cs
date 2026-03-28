namespace ParaTH;

public sealed class CollisionSystem(World world)
{
    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Collider>();

    public void Update()
    {
        var q = world.GetOrCreateQuery(descriptor);
    }
}

