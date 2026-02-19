using System.Diagnostics;

namespace ParaTH;

public sealed class AssetManager(string assetRoot)
{
    private readonly string assetRoot = Path.GetFullPath(assetRoot);

    private readonly AssetPool pool = new();
    private readonly Dictionary<Type, IAssetLoader> loaderRegistry = [];

    public string FullPath(string path) => Path.Combine(assetRoot, path);

    public void RegisterLoader(IAssetLoader loader)
    {
        if (!loaderRegistry.TryAdd(loader.AssetType, loader))
            throw new InvalidOperationException($"Loader '{loader}' already exists");
    }

    public T Load<T>(string path, string assetName) where T : Asset
    {
        if (TryGet(assetName, out T? cached))
            return cached!;

        if(!loaderRegistry.TryGetValue(typeof(T), out var loader))
            throw new KeyNotFoundException($"Loader for '{typeof(T)}' not registered");

        var fullPath = FullPath(path);
        var asset = loader.ParseAndLoad(fullPath, assetName, pool);

        return (T)asset;
    }

    public T Get<T>(string name) where T : Asset => pool.Get<T>(name);

    public bool TryGet<T>(string name, out T? asset) where T : Asset => pool.TryGet(name, out asset);
}
