using System;
using Microsoft.Xna.Framework.Media;

namespace ParaTH;

public sealed class SongAssetLoader(AssetManager assetManager) : IAssetLoader<SongAsset>
{
    private readonly HashSet<string> parsedFiles = [];

    public Asset ParseAndLoad(string fullPath, string assetName, AssetPool pool)
    {
        if (parsedFiles.Contains(fullPath))
        {
            throw new KeyNotFoundException(
                $"Song '{assetName}' not found in already-parsed file '{fullPath}'");
        }
        parsedFiles.Add(fullPath);

        var lines = File.ReadAllLines(fullPath);

        if (lines.Length == 0 || lines[0].Trim() != "@songs")
            throw new FormatException($"Invalid song asset file: {fullPath}");

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            // name  path  startMs  endMs
            var p = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
            if (p.Length != 4)
            {
                throw new FormatException(
                    $"Invalid song definition at line {i + 1} in '{fullPath}': " +
                    $"expected 4 fields, got {p.Length}");
            }

            var songName = p[0];
            var songPath = p[1];
            var startMs  = uint.Parse(p[2]);
            var endMs    = uint.Parse(p[3]);

            var fullSongPath = assetManager.FullPath(songPath);
            var song = Song.FromUri(songName, new Uri(fullSongPath));
            var songAsset = new SongAsset(song, new LoopRange(startMs, endMs));
            pool.Add(songName, songAsset);
        }

        return pool.Get<SongAsset>(assetName);
    }
}
