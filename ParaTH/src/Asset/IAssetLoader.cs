namespace ParaTH;

public interface IAssetLoader
{
    public Type AssetType { get; }

    public Asset ParseAndLoad(string fullPath, string assetName, AssetPool pool);
}

public interface IAssetLoader<T> : IAssetLoader where T : Asset
{
    Type IAssetLoader.AssetType => typeof(T);
}
