using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed class TextureAtlas(Texture2D fullTexture)
{
    private readonly Dictionary<string, TextureRegion> regions = [];

    public Texture2D Texture { get; } = fullTexture;

    public void CreateRegion(string name, TextureRegion region)
    {
        regions.Add(name, region);
    }

    public void CreateRegion(string name, int x, int y,
                             int width, int height,
                             int offsetX, int offsetY)
    {
        regions.Add(name, new TextureRegion(x, y, width, height, new Vector2(offsetX, offsetY)));
    }

    public TextureRegion GetRegion(string name)
    {
        if (regions.TryGetValue(name, out var region))
            return region;

        throw new KeyNotFoundException($"Region '{name}' not found in atlas.");
    }
}

public readonly record struct TextureRegion
{
    public readonly Rectangle Bounds;
    public readonly Vector2 Offset;

    public TextureRegion(int x, int y, int w, int h, Vector2 offset)
    {
        Bounds = new Rectangle(x, y, w, h);
        Offset = offset;
    }

    public TextureRegion(Rectangle bounds, Vector2 offset)
    {
        Bounds = bounds;
        Offset = offset;
    }
}
