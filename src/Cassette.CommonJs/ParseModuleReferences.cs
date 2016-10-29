using Cassette.BundleProcessing;
using Cassette.IO;
using Cassette.Scripts;
using Cassette.Utilities;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Cassette.CommonJs
{
  public class ParseModuleReferences : IBundleProcessor<ScriptBundle>
  {
    private readonly CommonJsSettings _settings;
    private readonly CassetteSettings _cassetteSettings;

    public ParseModuleReferences(CommonJsSettings settings, CassetteSettings cassetteSettings)
    {
      _settings = settings;
      _cassetteSettings = cassetteSettings;
    }

    public void Process(ScriptBundle bundle)
    {
      foreach (var asset in bundle.Assets.Where(this.ShouldParse))
      {
        var source = asset.OpenStream().ReadToEnd();
        var referenceParser = new RequireReferenceParser(asset, _settings, _cassetteSettings);
        var parser = new JSParser(source);
        var tree = parser.Parse(new CodeSettings());
        tree.Accept(referenceParser);
      }
    }

    private bool ShouldParse(IAsset asset)
    {
      return asset.Path.EndsWith(".js", StringComparison.OrdinalIgnoreCase);
    }

    internal class RequireReferenceParser : TreeVisitor
    {
      private readonly IAsset _asset;
      private readonly CommonJsSettings _settings;
      private readonly CassetteSettings _cassetteSettings;
      private IDirectory _nodeDirectory;

      public RequireReferenceParser(IAsset asset, CommonJsSettings settings, CassetteSettings cassetteSettings)
      {
        _asset = asset;
        _settings = settings;
        _cassetteSettings = cassetteSettings;
      }

      public override void Visit(CallNode node)
      {
        if (node.Function.ToCode() == "require" && node.Arguments.Count > 0)
        {
          var constWrapper = node.Arguments[0] as ConstantWrapper;
          if (constWrapper != null && constWrapper.PrimitiveType == PrimitiveType.String)
          {
            var path = (string)constWrapper.Value;
            if (CommonJsUtility.IsRelativePath(path))
            {
              if (path.StartsWith("./", StringComparison.Ordinal))
              {
                path = path.Length == 2 ? string.Empty : path.Substring(2);
              }

              if (path.EndsWith("/"))
              {
                path += "index.js";
              }

              if (path.EndsWith(".js", StringComparison.OrdinalIgnoreCase) == false)
              {
                path += ".js";
              }

              _asset.AddReference(path, 0);
            }
            else if (_settings.Globals.ContainsKey(path) == false)
            {
              _nodeDirectory = _nodeDirectory ?? _cassetteSettings.SourceDirectory.GetDirectory(_settings.NodeModulesPath);

              var moduleDirectory = _nodeDirectory.GetDirectory(path);
              var packageJson = moduleDirectory.GetFile("package.json");
              var jsonString = packageJson.OpenRead().ReadToEnd();
              var serializer = new JavaScriptSerializer();
              var json = serializer.Deserialize<Dictionary<string, object>>(jsonString);
              var mainFile = (string)json["main"];
              var mainPath = Path.Combine(moduleDirectory.FullPath, mainFile);

              _asset.AddReference(mainPath, 0);
            }
          }

          return;
        }

        base.Visit(node);
      }
    }
  }
}
