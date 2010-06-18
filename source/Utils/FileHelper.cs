namespace AtomSite.Utils
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.IO;
  using System.Security.Cryptography;

  public static class FileHelper
  {
    static readonly char[] TrackbackChars = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

    public static void WriteStream(Stream stream, string path)
    {
      EnsurePathExists(path);
      byte[] buffer = new byte[10000];
      int bytesRead, totalBytesRead = 0;
      using (FileStream fs = System.IO.File.Create(path))
      {
        do
        {
          bytesRead = stream.Read(buffer, 0, buffer.Length);
          fs.Write(buffer, 0, bytesRead);
          totalBytesRead += bytesRead;
        } while (bytesRead > 0);
      }
    }

    public static string ComputeMD5Sum(string path)
    {
      return BitConverter.ToString(new
          MD5CryptoServiceProvider().ComputeHash(System.IO.File.ReadAllBytes(path)))
          .Replace("-", "").ToLower();
    }

    public static void EnsurePathExists(string path)
    {
      string dir = Path.GetDirectoryName(path);
      if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
    }

    public static void RemoveEmptyPaths(string dir, bool includeParent)
    {
      if (System.IO.Directory.Exists(dir) && System.IO.Directory.GetFiles(dir).Length == 0
        && Directory.GetDirectories(dir).Length == 0)
        System.IO.Directory.Delete(dir, true);

      if (includeParent)
      {
        dir = System.IO.Directory.GetParent(dir).FullName;
        if (System.IO.Directory.Exists(dir) && System.IO.Directory.GetFiles(dir).Length == 0
          && Directory.GetDirectories(dir).Length == 0)
          System.IO.Directory.Delete(dir, true);
      }
    }

    /// <summary>
    /// Combines the trackbacks in path.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns></returns>
    public static string CombineTrackbacksInPath(string uri)
    {
      int trackbackIndex = GetTrackbackIndex(uri);
      int slashIndex;
      while (trackbackIndex != -1)
      {
        // There is a trackback starting at the index, so from
        // there we find the nearest slash, and remove that piece
        slashIndex = uri.LastIndexOfAny(TrackbackChars, trackbackIndex - 2);
        if (slashIndex == -1)
        {
          uri = uri.Remove(0, trackbackIndex + 2);
        }
        else
        {
          uri = uri.Remove(slashIndex, trackbackIndex - slashIndex + 2);
        }

        trackbackIndex = GetTrackbackIndex(uri);
      }
      return uri;
    }

    /// <summary>
    /// Gets the index of the trackback.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns></returns>
    static int GetTrackbackIndex(string uri)
    {
      int index = uri.IndexOf("../");

      if (index == -1)
      {
        index = uri.IndexOf(@"..\");
      }

      return index;
    }
  }
}
