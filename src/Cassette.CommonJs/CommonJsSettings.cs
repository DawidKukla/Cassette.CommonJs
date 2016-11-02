using System;
using System.Collections.Generic;

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

    /// <summary>
    /// Gets or sets the node modules path.
    /// </summary>
    /// <value>
    /// The node modules path.
    /// </value>
    public string NodeModulesPath { get; set; }

    /// <summary>
    /// Adds a global reference. Any require references for this module will resolve from the global object.
    /// </summary>
    /// <param name="name">The name of the reference on the global object.</param>
    /// <param name="alias">The alias that is used with the require call. Will default to name if not defined.</param>
    public void AddGlobalReference(string name, string alias = null)
    {
      _globals[alias ?? name] = name;
    }
  }
}
