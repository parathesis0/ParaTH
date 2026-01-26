using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public enum StgBlendState : byte
{
    Alpha,
    Additive,
    Subtract,
    ReverseSubtract,
    Invert
}

public static class StgBlendStates
{
    public static BlendState Alpha => BlendState.AlphaBlend;

    public static BlendState Additive => BlendState.Additive;

    public static BlendState Subtract { get; } = new()
    {
        Name = "Stg.Subtract",

        ColorBlendFunction = BlendFunction.ReverseSubtract,
        ColorSourceBlend = Blend.SourceAlpha,
        ColorDestinationBlend = Blend.One,

        AlphaBlendFunction = BlendFunction.Add,
        AlphaSourceBlend = Blend.One,
        AlphaDestinationBlend = Blend.One
    };

    public static BlendState ReverseSubtract { get; } = new()
    {
        Name = "Stg.ReverseSubtract",

        ColorBlendFunction = BlendFunction.Subtract,
        ColorSourceBlend = Blend.SourceAlpha,
        ColorDestinationBlend = Blend.One,

        AlphaBlendFunction = BlendFunction.Add,
        AlphaSourceBlend = Blend.One,
        AlphaDestinationBlend = Blend.One
    };

    public static BlendState Invert { get; } = new()
    {
        Name = "Stg.Invert",

        ColorBlendFunction = BlendFunction.Add,
        ColorSourceBlend = Blend.InverseDestinationColor,
        ColorDestinationBlend = Blend.Zero,

        AlphaBlendFunction = BlendFunction.Add,
        AlphaSourceBlend = Blend.Zero,
        AlphaDestinationBlend = Blend.One
    };
}
