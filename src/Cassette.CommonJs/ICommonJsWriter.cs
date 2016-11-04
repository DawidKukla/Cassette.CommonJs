using System;
using System.Collections.Generic;
using System.IO;

namespace Cassette.CommonJs
{
  public interface ICommonJsWriter
  {
    void WriteToStream(Stream stream, IEnumerable<IAsset> assets);
  }
}
