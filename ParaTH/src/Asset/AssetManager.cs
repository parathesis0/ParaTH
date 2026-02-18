namespace ParaTH;

public sealed class AssetManager
{
    private readonly string assetRoot;

    private readonly AssetPool pool = new();
    private readonly Dictionary<Type, IAssetLoader> loaderRegistry = [];

    private string FullPath(string relativePath) => Path.Combine(assetRoot, relativePath);

    public AssetManager(string assetRoot)
    {
        this.assetRoot = Path.GetFullPath(assetRoot);
    }

    public void RegisterLoader(IAssetLoader loader)
    {
        if (!loaderRegistry.TryAdd(loader.AssetType, loader))
            throw new InvalidOperationException($"Loader '{loader}' already exists");
    }

    public void Load<T>(string path) where T : Asset
    {
        if(!loaderRegistry.TryGetValue(typeof(T), out var loader))
            throw new KeyNotFoundException($"Loader for '{typeof(T)}' not registered");

        var fullPath = FullPath(path);

        loader.ParseAndLoad(fullPath, pool);
    }

    public T Get<T>(string name) where T : Asset => pool.Get<T>(name);

    public bool TryGet<T>(string name, out T? asset) where T : Asset => pool.TryGet(name, out asset);
}
