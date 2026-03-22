using Microsoft.Xna.Framework;

namespace ParaTH;

public sealed class BulletManager(World world, AssetManager asset)
{
    public World World { get; } = world;

    public AssetManager AssetManager { get; } = asset;

    public BulletBuilder SpawnBullet()
    {
        return new BulletBuilder(this);
    }
}
