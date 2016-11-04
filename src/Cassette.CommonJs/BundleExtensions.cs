using Cassette.BundleProcessing;
using Cassette.Scripts;

namespace Cassette.CommonJs
{
  public static class BundleExtensions
  {
    /// <summary>
    /// Alters the pipeline to process the bundled files as CommonJS. This will replace the reference
    /// parsing to use "require" references instead of the XML comment references, and will always
    /// combine the bundle into a single file.
    /// </summary>
    /// <param name="this">The script bundle.</param>
    public static void AlterPipelineForCommonJs(this ScriptBundle @this)
    {
      // note: we can't do this in a pipeline modifier because those are global
      // we can't create a new bundle type because cassette hasn't exposed everything
      // we'd need to create a new bundle type - so we are stuck with this hacky
      // extension method
      var index = @this.Pipeline.IndexOf<ParseReferences<ScriptBundle>>();
      if (index >= 0)
      {
        @this.Pipeline.RemoveAt(index);
      }

      index = @this.Pipeline.IndexOf<SortAssetsByDependency>();
      if (index >= 0)
      {
        @this.Pipeline.RemoveAt(index);
      }
      else
      {
        index = 0;
      }

      @this.Pipeline.Insert<ParseModuleReferences>(index++);
      @this.Pipeline.Insert<BundleCommonJs>(index);
    }
  }
}
