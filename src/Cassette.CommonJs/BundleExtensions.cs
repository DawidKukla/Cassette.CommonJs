using Cassette.BundleProcessing;
using Cassette.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassette.CommonJs
{
  public static class BundleExtensions
  {
    public static void AlterPipelineForCommonJs(this ScriptBundle @this)
    {
      // keep minifier
      var minifier = @this.Pipeline.OfType<MinifyAssets>().FirstOrDefault();

      // note: we can't do this in a pipeline modifier because those are global
      // we can't create a new bundle type because cassette hasn't exposed everything
      // we'd need to create a new bundle type - so we are stuck with this hacky
      // extension method
      @this.Pipeline.Clear();
      @this.Pipeline.Add<AssignScriptRenderer>();
      @this.Pipeline.Add<ParseModuleReferences>();
      @this.Pipeline.Add<BundleCommonJs>();
      @this.Pipeline.Add<AssignHash>();

      if (minifier != null)
      {
        @this.Pipeline.Add(minifier);
      }
    }

    internal static void WriteCollection<T>(this IEnumerable<T> @this, StreamWriter writer, Action<StreamWriter, T> writerFunc)
    {
      bool isFirst = true;
      foreach (var item in @this)
      {
        if (isFirst == false)
        {
          writer.Write(",");
          writer.WriteLine();
        }

        writerFunc(writer, item);
        isFirst = false;
      }
    }

    internal static void WriteFormat(this StreamWriter @this, string format, params object[] args)
    {
      @this.Write(string.Format(format, args));
    }
  }
}
