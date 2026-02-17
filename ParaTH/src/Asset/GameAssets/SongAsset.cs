using Microsoft.Xna.Framework.Media;

namespace ParaTH;

public readonly record struct LoopRange(uint StartMs, uint EndMs)
{
    public TimeSpan Start => TimeSpan.FromMilliseconds(StartMs);
    public TimeSpan End   => TimeSpan.FromMilliseconds(EndMs);
}

public sealed record SongAsset(Song Song, LoopRange LoopRange) : Asset;
