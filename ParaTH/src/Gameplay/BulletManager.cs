namespace ParaTH;

public sealed class BulletManager(World world, AssetManager assetManager)
{
    public World World { get; } = world;
    public AssetManager Asset { get; } = assetManager;

    public BulletBuilder SpawnBullet()
    {
        return new BulletBuilder(this);
    }
}
