using FontStashSharp;

namespace ParaTH;

public sealed record FontAsset(FontSystem FontSystem) : Asset
{
    public DynamicSpriteFont GetFont(float size) => FontSystem.GetFont(size);
}
