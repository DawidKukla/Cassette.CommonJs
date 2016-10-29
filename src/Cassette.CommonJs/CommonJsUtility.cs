using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassette.CommonJs
{
  public static class CommonJsUtility
  {
    private static void ValidateServerPath(string path)
    {
      if (path.StartsWith("~/", StringComparison.Ordinal) == false)
      {
        throw new ArgumentException("Expected a rooted path", "path");
      }
    }

    public static string ServerPathToCommonJsPath(string fromPath, string toPath)
    {
      CommonJsUtility.ValidateServerPath(fromPath);
      CommonJsUtility.ValidateServerPath(toPath);

      // hack to allow us to leverage Uri to get a relative path
      fromPath = "c:/" + (fromPath.Length == 2 ? string.Empty : fromPath.Substring(2));
      toPath = "c:/" + (toPath.Length == 2 ? string.Empty : toPath.Substring(2));

      var fromUri = new Uri(fromPath, UriKind.Absolute);
      var toUri = new Uri(toPath, UriKind.Absolute);

      if (fromUri.Scheme != toUri.Scheme)
      {
        return toUri.ToString();
      }

      var relativeUri = fromUri.MakeRelativeUri(toUri);
      var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

      if (relativePath.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
      {
        relativePath = relativePath.Substring(0, relativePath.Length - 3);
      }

      relativePath.Replace('\\', '/');
      if (CommonJsUtility.IsRelativePath(relativePath) == false)
      {
        // this would indicate the files are in the same directory
        return "./" + relativePath;
      }

      return relativePath;
    }

    internal static bool IsRelativePath(string path)
    {
      if (string.IsNullOrEmpty(path))
      {
        return false;
      }

      return path[0] == '.';
    }
  }
}
