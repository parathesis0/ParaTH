using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ParaTH;

public sealed class SoundAssetLoader : IAssetLoader<SoundAsset>
{
    public Asset ParseAndLoad(string fullPath, string assetName, AssetPool pool)
    {
        using var stream = File.OpenRead(fullPath);
        var sound = SoundEffect.FromStream(stream);
        var soundAsset = new SoundAsset(sound);
        pool.Add(assetName, soundAsset);
        return soundAsset;
    }
}
