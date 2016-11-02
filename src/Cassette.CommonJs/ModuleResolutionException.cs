using System;
using System.IO;

namespace Cassette.CommonJs
{
  [Serializable]
  public class ModuleResolutionException : FileNotFoundException
  {
    public ModuleResolutionException(string message, string moduleName) : base(message, moduleName)
    {
    }
  }
}