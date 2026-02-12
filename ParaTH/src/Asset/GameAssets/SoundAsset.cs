using Microsoft.Xna.Framework.Audio;

namespace ParaTH;

public sealed record SoundAsset : Asset
{
    public readonly SoundEffect SoundEffect;

    public SoundAsset(uint id, SoundEffect soundEffect) : base(id)
    {
        SoundEffect = soundEffect;
    }
}
