using Cassette.BundleProcessing;
using Cassette.Scripts;
using Cassette.Utilities;
using Microsoft.Ajax.Utilities;
using System;

namespace Cassette.CommonJs
{
  public class ParseModuleReferences : IBundleProcessor<ScriptBundle>
  {
    private readonly CommonJsSettings _settings;
    private readonly IExternalModuleResolver _moduleResolver;

    public ParseModuleReferences(CommonJsSettings settings, IExternalModuleResolver moduleResolver)
    {
      _settings = settings;
      _moduleResolver = moduleResolver;
    }

    public void Process(ScriptBundle bundle)
    {
      for (var i = 0; i < bundle.Assets.Count; i++)
      {
        var asset = bundle.Assets[i];
        if (this.ShouldParse(asset) == false)
        {
          continue;
        }

        var source = asset.OpenStream().ReadToEnd();
        var referenceParser = new RequireReferenceParser(asset, bundle, _settings, _moduleResolver);
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
      private readonly ScriptBundle _bundle;
      private readonly CommonJsSettings _settings;
      private readonly IExternalModuleResolver _moduleResolver;

      public RequireReferenceParser(IAsset asset, ScriptBundle bundle, CommonJsSettings settings, IExternalModuleResolver moduleResolver)
      {
        _asset = asset;
        _bundle = bundle;
        _settings = settings;
        _moduleResolver = moduleResolver;
      }

      public override void Visit(CallNode node)
      {
        if (node.Function.ToCode() == "require" && node.Arguments.Count > 0)
        {
          var constWrapper = node.Arguments[0] as ConstantWrapper;
          if (constWrapper != null && constWrapper.PrimitiveType == PrimitiveType.String)
          {
            var path = (string)constWrapper.Value;
            if (FileUtility.IsRelativePath(path))
            {
              if (path.StartsWith("./", StringComparison.Ordinal))
              {
                path = path.Length == 2 ? string.Empty : path.Substring(2);
              }

              if (path.EndsWith("/", StringComparison.Ordinal))
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
              var mainFile = _moduleResolver.Resolve(path);
              _bundle.Assets.Add(new ExternalModuleAsset(path, mainFile, _bundle));
            }
          }

          return;
        }

        base.Visit(node);
      }
    }
  }
}
