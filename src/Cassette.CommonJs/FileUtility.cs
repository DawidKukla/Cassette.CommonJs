using System;

namespace Cassette.CommonJs
{
  internal static class FileUtility
  {
    private static string NormalizeServerPath(string path)
    {
      if (path.StartsWith("~/", StringComparison.Ordinal) == false)
      {
        throw new ArgumentException("Expected a rooted path", "path");
      }

      return path.Length == 2 ? string.Empty : path.Substring(2);
    }

    internal static string ServerPathToCommonJsPath(string fromPath, string toPath)
    {
      // hack to allow us to leverage Uri to get a relative path
      fromPath = "c:/" + FileUtility.NormalizeServerPath(fromPath);
      toPath = "c:/" + FileUtility.NormalizeServerPath(toPath);

      var fromUri = new Uri(fromPath, UriKind.Absolute);
      var toUri = new Uri(toPath, UriKind.Absolute);

      var relativeUri = fromUri.MakeRelativeUri(toUri);
      var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

      if (relativePath.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
      {
        relativePath = relativePath.Substring(0, relativePath.Length - 3);
      }

      relativePath.Replace('\\', '/');
      if (FileUtility.IsRelativePath(relativePath) == false)
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
