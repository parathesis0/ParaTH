namespace ParaTH;

public interface IAssetLoader
{
    public Type AssetType { get; }

    public void ParseAndLoad(string fullPath, AssetPool pool);
}

public interface IAssetLoader<T> : IAssetLoader where T : Asset
{
    Type IAssetLoader.AssetType => typeof(T);
}
