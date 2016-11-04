using Cassette.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cassette.CommonJs
{
  internal class CommonJsAsset : AssetBase, IDisposable
  {
    private readonly string _path;
    private readonly IEnumerable<IAsset> _children;
    private readonly MemoryStream _stream;
    private readonly byte[] _hash;
    
    internal CommonJsAsset(string path, IEnumerable<IAsset> children, ICommonJsWriter writer)
    {
      _path = path;
      _children = children.ToArray();
      
      _stream = new MemoryStream();
      writer.WriteToStream(_stream, _children);

      _hash = _stream.ComputeSHA1Hash();
    }

    public override void Accept(IBundleVisitor visitor)
    {
      visitor.Visit(this);
      foreach (var child in _children)
      {
        visitor.Visit(child);
      }
    }

    protected override Stream OpenStreamCore()
    {
      return new MemoryStream(_stream.ToArray());
    }

    public override void AddReference(string path, int lineNumber)
    {
      throw new NotSupportedException();
    }

    public override void AddRawFileReference(string relativeFilename)
    {
      throw new NotSupportedException();
    }

    public override Type AssetCacheValidatorType
    {
      get { return _children.First().AssetCacheValidatorType; }
    }

    public override IEnumerable<AssetReference> References
    {
      get { return _children.SelectMany(c => c.References); }
    }

    public override string Path
    {
      get { return _path; }
    }

    public override byte[] Hash
    {
      get { return _hash; }
    }

    public void Dispose()
    {
      _stream.Dispose();
    }
  }
}
