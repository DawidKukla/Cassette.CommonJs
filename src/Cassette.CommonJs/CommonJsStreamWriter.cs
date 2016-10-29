using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cassette.CommonJs
{
  public class CommonJsStreamWriter
  {
    private readonly CommonJsSettings _settings;

    public CommonJsStreamWriter(CommonJsSettings settings)
    {
      _settings = settings;
    }

    public MemoryStream ToStream(IEnumerable<IAsset> assets)
    {
      var outputStream = new MemoryStream();
      var writer = new StreamWriter(outputStream);
      writer.Write(CommonJsConstants.Prelude);

      writer.Write("(this, {");
      _settings.Globals.WriteCollection(writer, (w, g) =>
      {
        writer.WriteFormat("'{0}': '{1}'", g.Key, g.Value);
      });

      writer.Write("}, [");
      assets.WriteCollection(writer, this.WriteAsset);
      writer.Write("]);");

      writer.Flush();
      outputStream.Position = 0;
      return outputStream;
    }

    private void WriteAsset(StreamWriter writer, IAsset asset)
    {
      writer.Write("{");
      writer.WriteLine();
      writer.Write("  path: ");

      writer.Write(HttpUtility.JavaScriptStringEncode(asset.Path, true));
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

      if (asset.References.Any())
      {
        writer.Write("  refs: {");
        writer.WriteLine();

        asset.References.WriteCollection(writer, (w, r) =>
        {
          var relativePath = CommonJsUtility.ServerPathToCommonJsPath(r.FromAssetPath, r.ToPath);

          writer.Write("    ");
          writer.Write(HttpUtility.JavaScriptStringEncode(relativePath, true));
          writer.Write(": ");
          writer.Write(HttpUtility.JavaScriptStringEncode(r.ToPath, true));
        });

        writer.Write("  }"); // end refs
      }
      else
      {
        writer.Write("  refs: {}");
      }

      writer.WriteLine();
      writer.Write("}");
    }
  }
}
