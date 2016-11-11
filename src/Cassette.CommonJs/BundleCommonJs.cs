using Cassette.BundleProcessing;
using Cassette.Scripts;
using System.Linq;

namespace Cassette.CommonJs
{
  public class BundleCommonJs : IBundleProcessor<ScriptBundle>
  {
    private readonly ICommonJsWriter _writer;
    private readonly CassetteSettings _cassetteSettings;
    private readonly ConcatenateAssets _concatenateAssets;

    public BundleCommonJs(ICommonJsWriter writer, CassetteSettings cassetteSettings, ConcatenateAssets concatenateAssets)
    {
      _writer = writer;
      _cassetteSettings = cassetteSettings;
      _concatenateAssets = concatenateAssets;
    }

    public void Process(ScriptBundle bundle)
    {
      if (bundle.Assets.OfType<IModuleAsset>().Any())
      {
        var combined = new CommonJsAsset(bundle.Path, bundle.Assets.ToArray(), _writer);
        bundle.Assets.Clear();
        bundle.Assets.Add(combined);
      }
      else if (_cassetteSettings.IsDebuggingEnabled == false)
      {
        _concatenateAssets.Process(bundle);
      }
    }
  }
}
