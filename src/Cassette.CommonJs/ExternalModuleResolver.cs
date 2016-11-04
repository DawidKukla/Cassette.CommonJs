using System;
using System.Collections.Generic;
using Cassette.IO;
using Cassette.Utilities;
using Newtonsoft.Json;

namespace Cassette.CommonJs
{
  public class ExternalModuleResolver : IExternalModuleResolver
  {
    private readonly CassetteSettings _cassetteSettings;
    private readonly CommonJsSettings _moduleSettings;
    private IDirectory _nodeDirectory;

    public ExternalModuleResolver(CassetteSettings cassetteSettings, CommonJsSettings moduleSettings)
    {
      _cassetteSettings = cassetteSettings;
      _moduleSettings = moduleSettings;
    }

    public IFile Resolve(string moduleName)
    {
      _nodeDirectory = _nodeDirectory ?? _cassetteSettings.SourceDirectory.GetDirectory(_moduleSettings.NodeModulesPath);
      if (_nodeDirectory.Exists == false)
      {
        throw new ModuleResolutionException("Unable to find the node_modules directory.", moduleName);
      }

      var moduleDirectory = _nodeDirectory.GetDirectory(moduleName);
      if (moduleDirectory.Exists == false)
      {
        throw new ModuleResolutionException("Unable to find module in node_modules directory.", moduleName);
      }

      var packageJson = moduleDirectory.GetFile("package.json");
      if (packageJson.Exists == false)
      {
        throw new ModuleResolutionException("Unable to find package.json file for module.", moduleName);
      }

      var jsonString = packageJson.OpenRead().ReadToEnd();
      var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

      if (json.ContainsKey("main") == false || string.IsNullOrEmpty((string)json["main"]))
      {
        throw new ModuleResolutionException("No main entry found in package.json.", moduleName);
      }

      var mainPath = (string)json["main"];
      if (mainPath.StartsWith("./", StringComparison.Ordinal))
      {
        mainPath = mainPath.Substring(2);
      }

      return moduleDirectory.GetFile(mainPath);
    }
  }
}