using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed record AnimationAsset(Texture2D Texture, AnimationAsset.PlayType Type, AnimationAsset.Frame[] Frames) : Asset
{
    public enum PlayType { Hold, Loop, PingPong }
    public readonly record struct Frame(Rectangle SourceRect, Vector2 Anchor, int FrameDuration);
}
