/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Xml.Linq;
  using AtomSite.Domain;
  using TidyNet;

  /// <summary>
  /// Cleans content by treating it all as xhtml.
  /// </summary>
  public class XhtmlCleanContentService : ICleanContentService
  {
    protected static readonly Dictionary<string, List<string>> ValidHtmlTags = new Dictionary<string, List<string>> {
	    { "p", new List<string>() },
	    { "br", new List<string>() }, 
	    { "strong", new List<string>() }, 
	    { "b", new List<string>() }, 
	    { "em", new List<string>() }, 
	    { "i", new List<string>() }, 
	    { "ol", new List<string>() }, 
	    { "ul", new List<string>() }, 
	    { "li", new List<string>() }, 
	    { "a", new List<string> { "href", "title", "rel" } }, 
	    { "img", new List<string> { "src", "height", "width", "alt" } },
	    { "q", new List<string> { "cite" } }, 
	    { "cite", new List<string>() }, 
	    { "abbr", new List<string>() }, 
	    { "acronym", new List<string>() }, 
	    { "del", new List<string>() }, 
	    { "ins", new List<string>() }
    };
    
    protected ILogService Logger;

    public XhtmlCleanContentService(ILogService logger)
    {
      this.Logger = logger;
    }

    public void CleanContentTrusted(AtomContent content)
    {
      Logger.Info("Cleaning content to be valid xhtml.");

      string text = content.Text;
      if (content.Type == "html")
      {
        text = "<div xmlns=\"" + Atom.XhtmlNs.NamespaceName + "\">" + text + "</div>";
      }

      Tidy tidy = new Tidy();
      /* Set the options you want */
      tidy.Options.DocType = DocType.Strict;
      //tidy.Options.DropFontTags = true; 
      tidy.Options.LogicalEmphasis = true;
      tidy.Options.Xhtml = true;
      tidy.Options.XmlOut = true;
      tidy.Options.MakeClean = true;
      tidy.Options.TidyMark = false;
      tidy.Options.QuoteNbsp = false;
      tidy.Options.NumEntities = true;
      tidy.Options.CharEncoding = CharEncoding.UTF8;
      tidy.Options.FixBackslash = true;
      tidy.Options.FixComments = true;

      TidyMessageCollection tmc = new TidyMessageCollection();
      using (MemoryStream input = new MemoryStream())
      using (MemoryStream output = new MemoryStream())
      {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        input.Write(bytes, 0, bytes.Length);
        input.Position = 0;
        tidy.Parse(input, output, tmc);
        text = Encoding.UTF8.GetString(output.ToArray());
        if (string.IsNullOrEmpty(text)) throw new FailedToCleanContentException(
          string.Format("{0} HTML Tidy Error(s)" + Environment.NewLine, tmc.Errors)
          + string.Join(Environment.NewLine,
          tmc.Cast<TidyMessage>()
          .Where(m => m.Level == MessageLevel.Error)
          .Select(m => m.ToString()).ToArray()));
      }

      //remove html/body that gets added by tidy
      //int start = text.IndexOf("<div xmlns");
      //text = text.Substring(start, text.IndexOf("</body>") - start);

      XElement div = XElement.Parse(text).Element(Atom.XhtmlNs + "body").Element(Atom.XhtmlNs + "div");

      //remove decendent xmlns that gets added by tidy
      div.Descendants().Where(d => d.Attribute("xmlns") != null && 
        d.Attribute("xmlns").Value == Atom.XhtmlNs.NamespaceName)
        .Select(d => d.Attribute("xmlns")).Remove();

      //text = text.Replace(" xmlns=\"http://www.w3.org/1999/xhtml\"", string.Empty);
      //text = "<div xmlns=\"http://www.w3.org/1999/xhtml\">" + text.Substring("<div>".Length);

      //set as xhtml
      content.Type = "xhtml";
      content.Text = div.ToString(SaveOptions.None);
    }

    //protected void Validate(AtomContent content)
    //{
    //  XhtmlValidator validator = new XhtmlValidator(content.Text);
    //  IEnumerable<ValidationRecord> errors = validator.Validate()
    //    .Where(r => r.Severity == System.Xml.Schema.XmlSeverityType.Error);
    //  if (errors.Count() > 0)
    //  {
    //    throw new Exception("Content is not valid xhtml 1.0 strict:" + Environment.NewLine
    //      + string.Join(Environment.NewLine, errors.Select(e => e.ToString()).ToArray()));
    //  }
    //}

    public void CleanContentFully(AtomContent content)
    {
      CleanContentTrusted(content);
      XElement root = content.Xml.Element(Atom.XhtmlNs + "div");

      //remove invalid html elements and attributes
      RemoveInvalidNodes(root);

      //remove invalid href, src values
      foreach (XElement anchor in root.Descendants().Where(d => d.Name.LocalName == "a"))
      {
        XAttribute href = anchor.Attribute("href");
        if (href != null && href.Value.StartsWith("http://") || href.Value.StartsWith("https://")
           || href.Value.StartsWith("ftp://"))
        {
          Logger.Info("Found valid link: " + anchor.Value);
        }
        else href.Value = string.Empty;
      }
      foreach (XElement img in root.Descendants().Where(d => d.Name.LocalName == "img"))
      {
        XAttribute src = img.Attribute("src");
        if (src != null && src.Value.StartsWith("http://") || src.Value.StartsWith("https://"))
        {
          Logger.Info("Found valid img: " + img.Value);
        }
        else src.Value = string.Empty;
      }
    }

    protected void RemoveInvalidNodes(XElement root)
    {
      foreach (XElement element in root.Elements())
      {
        if (ValidHtmlTags.Keys.Contains(element.Name.LocalName))
        {
          Logger.Info("Valid element found: " + element.Name);
          foreach (XAttribute attribute in element.Attributes())
          {
            if (ValidHtmlTags[element.Name.LocalName].Contains(attribute.Name.LocalName))
            {
              Logger.Info("Valid attribute found: " + attribute.Name);
            }
            else attribute.Remove();
          }
          RemoveInvalidNodes(element);
        }
        else element.Remove();
      }
    }
  }
}
