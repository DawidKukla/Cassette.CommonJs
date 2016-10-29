using Cassette.BundleProcessing;
using Cassette.Scripts;
using Cassette.Utilities;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassette.CommonJs
{
  public class ParseModuleReferences : IBundleProcessor<ScriptBundle>
  {
    public void Process(ScriptBundle bundle)
    {
      foreach (var asset in bundle.Assets.Where(this.ShouldParse))
      {
        var source = asset.OpenStream().ReadToEnd();
        var referenceParser = new RequireReferenceParser(asset);
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

      public RequireReferenceParser(IAsset asset)
      {
        _asset = asset;
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
          }

          return;
        }

        base.Visit(node);
      }
    }
  }
}
