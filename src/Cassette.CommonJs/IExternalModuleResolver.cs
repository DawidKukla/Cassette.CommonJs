using Cassette.IO;

namespace Cassette.CommonJs
{
  public interface IExternalModuleResolver
  {
    IFile Resolve(string moduleName);
  }
}