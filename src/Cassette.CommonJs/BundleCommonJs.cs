using Cassette.BundleProcessing;
using Cassette.Scripts;

namespace Cassette.CommonJs
{
  public class BundleCommonJs : IBundleProcessor<ScriptBundle>
  {
    private readonly CommonJsWriter _writer;

    public BundleCommonJs(CommonJsWriter writer)
    {
      _writer = writer;
    }

    public void Process(ScriptBundle bundle)
    {
      var combined = new CommonJsAsset(bundle.Path, bundle.Assets, _writer);
      bundle.Assets.Clear();
      bundle.Assets.Add(combined);
    }
  }
}
