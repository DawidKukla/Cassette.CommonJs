using System;
using System.Collections.Generic;
using System.IO;

namespace Cassette.CommonJs
{
  internal class ModuleAsset : IModuleAsset
  {
    private readonly IAsset _asset;

    public ModuleAsset(IAsset underlyingAsset)
    {
      _asset = underlyingAsset;
    }

    public Type AssetCacheValidatorType
    {
      get { return _asset.AssetCacheValidatorType; }
    }

    public byte[] Hash
    {
      get { return _asset.Hash; }
    }

    public string Path
    {
      get { return _asset.Path; }
    }

    public IEnumerable<AssetReference> References
    {
      get { return _asset.References; }
    }

    public void Accept(IBundleVisitor visitor)
    {
      _asset.Accept(visitor);
    }

    public void AddAssetTransformer(IAssetTransformer transformer)
    {
      _asset.AddAssetTransformer(transformer);
    }

    public void AddRawFileReference(string relativeFilename)
    {
      _asset.AddRawFileReference(relativeFilename);
    }

    public void AddReference(string assetRelativePath, int lineNumber)
    {
      _asset.AddReference(assetRelativePath, lineNumber);
    }

    public Stream OpenStream()
    {
      return _asset.OpenStream();
    }
  }
}