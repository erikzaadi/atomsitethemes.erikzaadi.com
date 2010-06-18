namespace AtomSite.Utils.Extensions
{
  using System.IO;
  using System;

  public static class Extensions
  {
    #region File

    public static void WriteToFile(this Stream stream, string path)
    {
      FileHelper.WriteStream(stream, path);
    }

    #endregion

    #region Security

    /// <summary>
    /// Hash utility - pass the hash algorithm name as a string i.e. SHA1, MD5 etc.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="algorithm"></param>
    /// <param name="upperCase"></param>
    /// <returns></returns>
    public static string HashIt(this string input, string algorithm, bool upperCase)
    {
      return SecurityHelper.HashIt(input, algorithm, upperCase);
    }

    /// <summary>
    /// Hash utility - pass the hash algorithm name as a string i.e. SHA1, MD5 etc.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="algorithm"></param>
    /// <returns>Hashed String</returns>
    public static string HashIt(this string input, string algorithm)
    {
      return SecurityHelper.HashIt(input, algorithm);
    }
    #endregion Security

    #region String

    /// <summary>
    /// Shorten input text to a desired length, preserving words and appending the
    /// specified trailer.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="length"></param>
    /// <param name="trailer"></param>
    /// <returns>A string with the trailer attached if the string greater than length</returns>
    public static string AbbreviateText(this string input, int length, string trailer)
    {
      return StringHelper.AbbreviateText(input, length, trailer);
    }

    #endregion String

    #region Web

    public static string GetSubDomain(this Uri uri)
    {
      return WebHelper.GetSubDomain(uri);
    }

    #endregion Web
  }
}
