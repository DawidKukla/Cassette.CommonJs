using Cassette.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cassette.CommonJs
{
  public class CommonJsAsset : AssetBase, IDisposable
  {
    private readonly string _path;
    private readonly IEnumerable<IAsset> _children;
    private readonly MemoryStream _stream;
    private readonly byte[] _hash;
    
    public CommonJsAsset(string path, IEnumerable<IAsset> children, CommonJsStreamWriter writer)
    {
      _path = path;
      _children = children.ToArray();
      
      _stream = writer.ToStream(_children);
      _hash = _stream.ComputeSHA1Hash();
    }

    //private MemoryStream CombineAssets(IEnumerable<IAsset> assets)
    //{
    //  var outputStream = new MemoryStream();
    //  var writer = new StreamWriter(outputStream);
    //  writer.Write(CommonJsConstants.Prelude);

    //  writer.Write("(this, {");
    //  _settings.Globals.WriteCollection(writer, (w, g) =>
    //  {
    //    writer.WriteFormat("'{0}': '{1}'", g.Key, g.Value);
    //  });

    //  writer.Write("}, [");
    //  assets.WriteCollection(writer, this.WriteAsset);
    //  writer.Write("]);");

    //  writer.Flush();
    //  outputStream.Position = 0;
    //  return outputStream;
    //}

    //private void WriteAsset(StreamWriter writer, IAsset asset)
    //{
    //  writer.Write("{");
    //  writer.WriteLine();
    //  writer.Write("  path: ");

    //  writer.Write(HttpUtility.JavaScriptStringEncode(asset.Path, true));
    //  writer.Write(",");
    //  writer.WriteLine();

    //  writer.Write("  body: function (require, module, exports) {{ // start {0}", asset.Path);
    //  writer.WriteLine();

    //  using (var reader = new StreamReader(asset.OpenStream()))
    //  {
    //    string line;
    //    while ((line = reader.ReadLine()) != null)
    //    {
    //      writer.Write(line);
    //      writer.WriteLine();
    //    }
    //  }

    //  writer.WriteLine();
    //  writer.WriteFormat("  }}, // end {0}", asset.Path);  // end body
    //  writer.WriteLine();

    //  if (asset.References.Any())
    //  {
    //    writer.Write("  refs: {");
    //    writer.WriteLine();

    //    asset.References.WriteCollection(writer, (w, r) =>
    //    {
    //      var relativePath = CommonJsUtility.ServerPathToCommonJsPath(r.FromAssetPath, r.ToPath);

    //      writer.Write("    ");
    //      writer.Write(HttpUtility.JavaScriptStringEncode(relativePath, true));
    //      writer.Write(": ");
    //      writer.Write(HttpUtility.JavaScriptStringEncode(r.ToPath, true));
    //    });

    //    writer.Write("  }"); // end refs
    //  }
    //  else
    //  {
    //    writer.Write("  refs: {}");
    //  }

    //  writer.WriteLine();
    //  writer.Write("}");
    //}

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
