using Cassette.BundleProcessing;
using Cassette.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassette.CommonJs
{
  public class BundleCommonJs : IBundleProcessor<ScriptBundle>
  {
    public void Process(ScriptBundle bundle)
    {
      var combined = new CommonJsAsset(bundle.Path, bundle.Assets);
      bundle.Assets.Clear();
      bundle.Assets.Add(combined);
    }
  }
}
