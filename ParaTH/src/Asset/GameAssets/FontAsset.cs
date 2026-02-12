using FontStashSharp;

namespace ParaTH;

public sealed record FontAsset : Asset
{
    public SpriteFontBase FontBase;

    public FontAsset(uint id, SpriteFontBase fontBase) : base(id)
    {
        FontBase = fontBase;
    }
}
