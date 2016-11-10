using System.Collections.Generic;
using System.IO;
using Cassette.Utilities;

namespace Cassette.CommonJs
{
  public class CommonJsWriter : ICommonJsWriter
  {
    private readonly CommonJsSettings _settings;

    public CommonJsWriter(CommonJsSettings settings)
    {
      _settings = settings;
    }

    public void WriteToStream(Stream stream, IEnumerable<IAsset> assets)
    {
      var requireShim = this.GetType().Assembly.GetManifestResourceStream("Cassette.CommonJs.Resources.require-shim.js").ReadToEnd();

      var writer = new StreamWriter(stream);
      writer.Write("var require = ");
      writer.Write(requireShim);

      writer.Write("(this, {");
      _settings.Globals.WriteCollection(writer, (w, g) => writer.WriteFormat("\"{0}\": \"{1}\"", g.Key, g.Value));

      writer.Write("}, [");
      assets.WriteCollection(writer, this.WriteAsset);
      writer.Write("]);");

      writer.Flush();
      stream.Position = 0;
    }

    private void WriteAsset(StreamWriter writer, IAsset asset)
    {
      writer.Write("{");
      writer.WriteLine();
      writer.Write("  path: ");

      string path = asset.Path;
      var externalAsset = asset as ExternalModuleAsset;
      if (externalAsset != null)
      {
        path = externalAsset.ModuleName;
      }

      writer.JavaScriptStringEncodeQuoted(path);
      writer.Write(",");
      writer.WriteLine();

      writer.Write("  body: function (require, module, exports) {{ // start {0}", asset.Path);
      writer.WriteLine();

      using (var reader = new StreamReader(asset.OpenStream()))
      {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
          writer.Write(line);
          writer.WriteLine();
        }
      }

      writer.WriteLine();
      writer.WriteFormat("  }}, // end {0}", asset.Path);  // end body
      writer.WriteLine();

      writer.Write("  refs: {");
      writer.WriteLine();

      asset.References.WriteCollection(writer, (w, r) =>
      {
        var relativePath = FileUtility.ServerPathToCommonJsPath(r.FromAssetPath, r.ToPath);

        writer.Write("    ");
        writer.JavaScriptStringEncodeQuoted(relativePath);
        writer.Write(": ");
        writer.JavaScriptStringEncodeQuoted(r.ToPath);
      });

      writer.Write("  }"); // end refs
      
      writer.WriteLine();
      writer.Write("}");
    }
  }
}
