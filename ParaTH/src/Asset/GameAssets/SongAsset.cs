using Microsoft.Xna.Framework.Media;

namespace ParaTH;

public readonly record struct LoopRange(uint StartMs, uint EndMs)
{
    public TimeSpan Start => TimeSpan.FromMilliseconds(StartMs);
    public TimeSpan End   => TimeSpan.FromMilliseconds(EndMs);
}

public sealed record SongAsset : Asset
{
    public readonly Song Song;
    public readonly LoopRange LoopRange;

    public SongAsset(uint id, Song song, LoopRange loopRange) : base(id)
    {
        Song = song;
        LoopRange = loopRange;
    }
}
