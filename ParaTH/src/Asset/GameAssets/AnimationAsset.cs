using Microsoft.Xna.Framework;

namespace ParaTH;

public sealed record AnimationAsset(TextureAsset Texture, AnimationAsset.PlayType Type, IReadOnlyList<AnimationAsset.Frame> Frames) : Asset
{
    public enum PlayType { Hold, Loop, PingPong }
    public readonly record struct Frame(Rectangle SourceRect, Vector2 Anchor, uint FrameDuration);
}
