using Cassette.BundleProcessing;
using Cassette.Scripts;

namespace Cassette.CommonJs
{
  public class CommonJsPipelineModifier : IBundlePipelineModifier<ScriptBundle>
  {
    public IBundlePipeline<ScriptBundle> Modify(IBundlePipeline<ScriptBundle> pipeline)
    {
      var index = pipeline.IndexOf<SortAssetsByDependency>();
      pipeline.Insert<ParseModuleReferences>(++index);
      pipeline.Insert<BundleCommonJs>(++index);

      index = pipeline.IndexOf<ConcatenateAssets>();
      if (index > -1)
      {
        pipeline.RemoveAt(index);
      }

      return pipeline;
    }
  }
}
