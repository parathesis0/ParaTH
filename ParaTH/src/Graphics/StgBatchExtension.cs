using System.Text;
using FontStashSharp;
using FontStashSharp.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

internal sealed class StgBatchRenderer : IFontStashRenderer
{
    public static readonly StgBatchRenderer Instance = new();

    public StgBatch Batch { get; set; } = null!;
    public byte LayerDepth { get; set; }
    public StgBlendState BlendState { get; set; }

    public GraphicsDevice GraphicsDevice => Batch.GraphicsDevice;

    public void Draw(Texture2D texture, Vector2 pos, Rectangle? src, Color color,
        float rotation, Vector2 scale, float depth)
    {
        Batch.Draw(
            texture,
            pos,
            src,
            color,
            rotation,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            LayerDepth,
            BlendState
        );
    }
}

public static class StgBatchExtension
{
    public static float DrawString(this StgBatch batch, SpriteFontBase font, string text,
        Vector2 position, Color color, byte layerDepth, StgBlendState blendState,
        float rotation = 0, Vector2 origin = default, Vector2? scale = null,
        float characterSpacing = 0.0f, float lineSpacing = 0.0f,
        TextStyle textStyle = TextStyle.None,
        FontSystemEffect effect = FontSystemEffect.None, int effectAmount = 0)
    {
        var renderer = StgBatchRenderer.Instance;
        renderer.Batch = batch;
        renderer.LayerDepth = layerDepth;
        renderer.BlendState = blendState;

        return font.DrawText(renderer, text, position, color,
            rotation, origin, scale, 0f,
            characterSpacing, lineSpacing, textStyle, effect, effectAmount);
    }

    public static float DrawString(this StgBatch batch, SpriteFontBase font, string text,
        Vector2 position, Color[] colors, byte layerDepth, StgBlendState blendState,
        float rotation = 0, Vector2 origin = default, Vector2? scale = null,
        float characterSpacing = 0.0f, float lineSpacing = 0.0f,
        TextStyle textStyle = TextStyle.None,
        FontSystemEffect effect = FontSystemEffect.None, int effectAmount = 0)
    {
        var renderer = StgBatchRenderer.Instance;
        renderer.Batch = batch;
        renderer.LayerDepth = layerDepth;
        renderer.BlendState = blendState;

        return font.DrawText(renderer, text, position, colors,
            rotation, origin, scale, 0f,
            characterSpacing, lineSpacing, textStyle, effect, effectAmount);
    }

    public static float DrawString(this StgBatch batch, SpriteFontBase font, StringBuilder text,
        Vector2 position, Color color, byte layerDepth, StgBlendState blendState,
        float rotation = 0, Vector2 origin = default, Vector2? scale = null,
        float characterSpacing = 0.0f, float lineSpacing = 0.0f,
        TextStyle textStyle = TextStyle.None,
        FontSystemEffect effect = FontSystemEffect.None, int effectAmount = 0)
    {
        var renderer = StgBatchRenderer.Instance;
        renderer.Batch = batch;
        renderer.LayerDepth = layerDepth;
        renderer.BlendState = blendState;

        return font.DrawText(renderer, text, position, color,
            rotation, origin, scale, 0f,
            characterSpacing, lineSpacing, textStyle, effect, effectAmount);
    }
}
