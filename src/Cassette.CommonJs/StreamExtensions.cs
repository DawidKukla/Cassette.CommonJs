using System;
using System.Collections.Generic;
using System.IO;

namespace Cassette.CommonJs
{
  internal static class StreamExtensions
  {
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
      @this.Write(String.Format(format, args));
    }
  }
}