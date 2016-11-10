using Cassette.BundleProcessing;
using Cassette.Scripts;
using Cassette.Utilities;
using Microsoft.Ajax.Utilities;
using System;
using System.Linq;

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

        bundle.Assets[i] = referenceParser.Asset;
      }
    }

    private bool ShouldParse(IAsset asset)
    {
      return asset.Path.EndsWith(".js", StringComparison.OrdinalIgnoreCase);
    }

    internal class RequireReferenceParser : TreeVisitor
    {
      private readonly ScriptBundle _bundle;
      private readonly CommonJsSettings _settings;
      private readonly IExternalModuleResolver _moduleResolver;
      private readonly IAsset _asset;
      private IModuleAsset _moduleAsset = null;

      public RequireReferenceParser(IAsset asset, ScriptBundle bundle, CommonJsSettings settings, IExternalModuleResolver moduleResolver)
      {
        _asset = asset;
        _bundle = bundle;
        _settings = settings;
        _moduleResolver = moduleResolver;
        _moduleAsset = _asset as IModuleAsset;
      }

      public IAsset Asset
      {
        get
        {
          return _moduleAsset ?? _asset;
        }
      }

      public override void Visit(BinaryOperator node)
      {
        // if the file has no dependencies it might end up here
        // all we care about is whether it has any exports
        if (_moduleAsset == null && node.IsAssign && this.IsExportMember(node.Operand1))
        {
          _moduleAsset = new ModuleAsset(_asset);
        }

        base.Visit(node);
      }

      public override void Visit(CallNode node)
      {
        if (this.IsRequireCall(node))
        {
          var constWrapper = node.Arguments[0] as ConstantWrapper;
          if (constWrapper != null && constWrapper.PrimitiveType == PrimitiveType.String)
          {
            _moduleAsset = _moduleAsset ?? new ModuleAsset(_asset);
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

              _moduleAsset.AddReference(path, 0);
            }
            else if (this.ShouldIncludeReference(path))
            {
              var mainFile = _moduleResolver.Resolve(path);
              _bundle.Assets.Add(new ExternalModuleAsset(path, mainFile, _bundle));
            }
          }

          return;
        }

        base.Visit(node);
      }
      
      private bool ShouldIncludeReference(string path)
      {
        if (_settings.Globals.ContainsKey(path))
        {
          return false;
        }

        // make sure it is not already included
        return _bundle.Assets.OfType<ExternalModuleAsset>().Any(x => x.ModuleName == path) == false;
      }

      private bool IsRequireCall(CallNode node)
      {
        return node.Function.ToCode() == "require" && node.Arguments.Count > 0;
      }

      private bool IsExportMember(AstNode node)
      {
        return node.ToCode() == "module.exports" || node.LeftHandSide.ToCode() == "exports";
      }
    }
  }
}
