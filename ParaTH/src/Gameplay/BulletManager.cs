using Microsoft.Xna.Framework;

namespace ParaTH;

public sealed class BulletManager(World world)
{
    public World World { get; } = world;

    public BulletBuilder SpawnBullet(Vector2 position)
    {
        return new BulletBuilder(this, position);
    }
}
