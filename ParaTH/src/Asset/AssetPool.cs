namespace ParaTH;

public sealed class AssetPool
{
    private readonly Dictionary<string, Asset> assets = [];

    public void Add(string name, Asset asset)
    {
        if (!assets.TryAdd(name, asset))
            throw new InvalidOperationException($"Asset '{name}' already exists");
    }

    public T Get<T>(string name) where T : Asset
    {
        if (!assets.TryGetValue(name, out var asset))
            throw new KeyNotFoundException($"Asset '{name}' not found");
        return asset as T ?? throw new InvalidCastException(
            $"Asset '{name}' is {asset.GetType().Name}, expected {typeof(T).Name}");
    }

    public bool TryGet<T>(string name, out T? asset) where T : Asset
    {
        if (assets.TryGetValue(name, out var raw) && raw is T typed)
        {
            asset = typed;
            return true;
        }
        asset = null;
        return false;
    }

    public bool Contains(string name) => assets.ContainsKey(name);

    public bool Remove(string name) => assets.Remove(name);

    public void Clear() => assets.Clear();
}
