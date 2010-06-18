namespace AtomSite.Utils
{
  using System;
  using System.Text;
  using System.Security.Cryptography;

  public static class StringHelper
  {

    /// <summary>
    /// Shorten input text to a desired length, preserving words and appending the
    /// specified trailer.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="length"></param>
    /// <param name="trailer"></param>
    /// <returns>A string with the trailer attached if the string greater than length</returns>
    public static string AbbreviateText(string input, int length, string trailer)
    {
      if (string.IsNullOrEmpty(input))
        throw new ArgumentNullException("input");

      if (null == trailer)
        throw new ArgumentNullException("trailer");


      StringBuilder output = new StringBuilder((length + 20)); //Add room for a word not breaking and the trailer			

      string[] words = input.Split(new char[] { ' ' });
      int i = 0;
      while (((output.Length + words[i].Length + trailer.Length) < (length - trailer.Length))
                && (i < words.GetUpperBound(0)))
      {
        output.Append(words[i]);
        output.Append(" ");
        i++;
      }

      if (i < words.GetUpperBound(0)) //We exited the loop before reaching the end of the array - which would normally be the case.
      {
        output.Remove(output.Length - 1, 1); //Remove the ending space before attaching the trailer.
        output.Append(trailer);
      }
      else
      {
        output.Append(words[i]);
      }

      return output.ToString();
    }
  }
}
