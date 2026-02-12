using Microsoft.Xna.Framework;

namespace ParaTH;

public sealed record AnimationAsset : Asset
{
    public enum PlayType { Hold, Loop, PingPong }

    public readonly record struct Frame(
        Rectangle SourceRect, Vector2 Anchor, uint FrameDuration);

    public TextureAsset Texture;
    public PlayType Type;
    public IReadOnlyList<Frame> Frames;

    public AnimationAsset(uint id, TextureAsset texture,
        PlayType type, IReadOnlyList<Frame> frames) : base(id)
    {
        Texture = texture;
        Type = type;
        Frames = frames;
    }
}
