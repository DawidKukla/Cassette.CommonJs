using Cassette.IO;

namespace Cassette.CommonJs
{
  public class ExternalModuleAsset : FileAsset
  {
    public ExternalModuleAsset(string moduleName, IFile sourceFile, Bundle parentBundle) : base(sourceFile, parentBundle)
    {
      this.ModuleName = moduleName;
    }

    public string ModuleName { get; set; }
  }
}