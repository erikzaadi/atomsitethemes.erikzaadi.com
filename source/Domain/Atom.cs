/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Text;
  using System.Xml.Linq;

  /// <summary>
  /// Class to support Atom 1.0
  /// </summary>
  public static class Atom
  {
    public static readonly string DefaultWorkspaceName = "www";

    /// <summary>
    /// Namespace for Atom 1.0
    /// </summary>
    public static readonly XNamespace AtomNs = "http://www.w3.org/2005/Atom";
    /// <summary>
    /// Namespace for Atom Threading Extension 1.0
    /// </summary>
    public static readonly XNamespace ThreadNs = "http://purl.org/syndication/thread/1.0";
    /// <summary>
    /// Namespace for Atom Publishing Protocol 1.0
    /// </summary>
    public static readonly XNamespace AppNs = "http://www.w3.org/2007/app";
    /// <summary>
    /// Namespace for AtomSite Service extensions
    /// </summary>
    public static readonly XNamespace SvcNs = "http://atomsite.net/info/Service";
    /// <summary>
    /// Namespace for AtomSite Plugin extensions
    /// </summary>
    public static readonly XNamespace PluginNs = "http://atomsite.net/info/Plugins";
    /// <summary>
    /// Namespace for AtomSite Theme extensions
    /// </summary>
    public static readonly XNamespace ThemeNs = "http://atomsite.net/info/Themes";
    public static readonly XNamespace XhtmlNs = "http://www.w3.org/1999/xhtml";
    public static readonly XNamespace OpenSearchNs = "http://a9.com/-/spec/opensearch/1.1/";
    public static readonly XNamespace SqlNs = "http://atomsite.net/info/SqlRepository";
    public static readonly XNamespace FileNs = "http://atomsite.net/info/FileRepository";

    public const string ContentType = "application/atom+xml";
    public const string ContentTypeEntry = "application/atom+xml;type=entry";
    public const string ContentTypeFeed = "application/atom+xml;type=feed";
    public const string ContentTypeService = "application/atomsvc+xml";
    public const string ContentTypeCategories = "application/atomcat+xml";

    /// <summary>
    /// Cleans invalid characters that may cause problems on the file system or web server.
    /// Upper cases all words.  Replaces some common notatations with the word form.
    /// </summary>
    /// <remarks>TODO: move this to better class</remarks>
    /// <param name="slug"></param>
    /// <returns></returns>
    public static string CleanSlug(this string slug)
    {
      //custom slug word rules, add more word rules here
      List<Func<string, string>> wordRules = new List<Func<string, string>>();
      //clean up C#, F#, etc
      wordRules.Add((word) => word.Length == 2 && word[1] == '#' ? word.Replace("#", "Sharp") : word);

      //clean up C++
      wordRules.Add((word) => word.Length == 3 && word.EndsWith("++") ? word.Replace("++", "PlusPlus") : word);

      //clean up .NET
      wordRules.Add((word) => word.ToUpperInvariant().EndsWith(".NET") ? word.Substring(0, word.Length - 4) + "dotNET" : word);

      //capitalize the first letter of each word
      wordRules.Add((word) => word[0].ToString().ToUpper() + (word.Length > 1 ? word.Substring(1) : string.Empty));


      if (!string.IsNullOrEmpty(slug))
      {
        //remove html when not plain text
        slug = slug.StripHtml().Trim();

        //run word rules
        string[] words = slug.Split(' ').Select(w => w.Trim()).Where(w => w.Length > 0).ToArray();
        for (int i = 0; i < words.Length; i++)
        {
          foreach (Func<string, string> wordRule in wordRules)
          {
            words[i] = wordRule(words[i]);
          }
        }

        //NOTE: if user prefer's dashes or underscores, change below code
        //join words back together using specified separator
        string separator = string.Empty;
        slug = string.Join(separator, words.Where(w => !string.IsNullOrEmpty(w)).ToArray());

        //add more slug rules here if desired

        //remove invalid file name chars
        slug = new string(slug.ToCharArray().Where(c => !System.IO.Path.GetInvalidFileNameChars().Contains(c)).ToArray());

        //remove url special chars ;/?:@&=+$,()|\^[]'<>#%"
        slug = new string(slug.ToCharArray().Where(c => !";/?:@&=+$,()|\\^[]'<>#%\"".Contains(c)).ToArray());

        //normalize and remove non-spacing marks
        //TODO: is this really necessary?
        slug = slug.Normalize(NormalizationForm.FormD);
        slug = new string(slug.ToCharArray().Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());

        //WCF doesn't support periods and it may throw off IIS6 or other extension mime type issues
        slug = slug.Replace(".", string.Empty);
      }

      //use a guid if nothing supplied
      if (string.IsNullOrEmpty(slug)) slug = Guid.NewGuid().ToString();

      //max length is 250 (sometimes a number is appended to end to resolve naming conflicts, database should support upto 256
      slug = slug.Substring(0, Math.Min(slug.Length, 250));

      return slug;
    }

    public static string StripHtml(this string text)
    {
      return System.Text.RegularExpressions.Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
    }

  }
}
