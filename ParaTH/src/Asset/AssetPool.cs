namespace ParaTH;

public sealed class AssetPool
{
    private readonly Dictionary<string, Asset> assets = [];

    public void Add(string name, Asset asset)
    {
        if (!assets.TryAdd(name, asset))
            throw new InvalidOperationException($"Asset {name} already exists");
    }

    public bool Remove(string name)
    {
        return assets.Remove(name);
    }

    public void Clear()
    {
         assets.Clear();
    }
}
