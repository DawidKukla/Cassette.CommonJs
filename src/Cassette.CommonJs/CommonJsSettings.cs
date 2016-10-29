using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassette.CommonJs
{
  public class CommonJsSettings
  {
    private readonly IDictionary<string, string> _globals = new Dictionary<string, string>(StringComparer.Ordinal);

    public CommonJsSettings()
    {
      this.NodeModulesPath = "~/node_modules/";
    }

    internal IDictionary<string, string> Globals
    {
      get { return _globals; }
    }

    public string NodeModulesPath { get; set; }

    public void AddGlobalReference(string name, string alias = null)
    {
      _globals[alias ?? name] = name;
    }
  }

  internal class GlobalReference
  {
    public string Name { get; set; }

    public string Alias { get; set; }
  }
}
