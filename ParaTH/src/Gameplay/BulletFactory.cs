namespace ParaTH;

public sealed class BulletFactory(World world, AssetManager asset)
{
    public World World { get; } = world;

    public AssetManager AssetManager { get; } = asset;

    public uint GlobalSpawnCounter = 0;

    public BulletBuilder Create()
    {
        return new BulletBuilder(this);
    }
}
