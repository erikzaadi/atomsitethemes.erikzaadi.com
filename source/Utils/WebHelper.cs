namespace AtomSite.Utils
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Xml;
  using System.Xml.Linq;
  using Sgml;
  
  public static class WebHelper
  {
    public static string GetSubDomain(Uri uri)
    {
      if (uri.HostNameType == UriHostNameType.Dns)
      {
        if (uri.Host.Split('.').Length > 2)
        {
          int len = uri.Host.LastIndexOf(".", uri.Host.LastIndexOf(".") - 1);
          return uri.Host.Substring(0, len);
        }
        return string.Empty;
      }
      return null;
    }

    public static Uri GetGravatarHref(string email, int size)
    {
      if (email == null) email = "";
      return new Uri("http://gravatar.com/avatar/" + 
                //SecurityHelper.ToMd5Hash(email.ToLowerInvariant())
                SecurityHelper.HashIt(email.ToLowerInvariant(), "MD5", false)
                + ".jpg?s=" + size + "&amp;d=");
    }

    public static XElement ReadHtmlAsXhtml(string html)
    {
      //detect if xhtml by looking for namespace near start
      if (html.IndexOf("http://www.w3.org/1999/xhtml") < 200)
      {
        //must be xhtml, so just parse as xml
        return XElement.Parse(html);
      }
      else
      {
        //probably html, so parse as sgml
        SgmlReader sgml = new SgmlReader();
        sgml.DocType = "HTML";
        sgml.WhitespaceHandling = WhitespaceHandling.All;
        sgml.CaseFolding = Sgml.CaseFolding.ToLower;
        sgml.InputStream = new StringReader(html);
        return XElement.Load(sgml);
      }
    }

    public static string ExtractTitleForPage(string page)
    {
      try
      {
        //XElement x = ReadPageAsXhtml(page);
        //if (x != null)
        //{
        //    XElement title = x.Descendants().Where(t => t.Name.LocalName == "title")
        //        .FirstOrDefault();
        //    if (title != null) return title.Value;
        //}
        int start = page.IndexOf("<head");
        if (start > -1) start = page.IndexOf("<title", start);
        if (start > -1) start = page.IndexOf(">", start) + 1;
        if (start > -1)
        {
          int end = page.IndexOf("</title>", start);
          if (end > -1) return page.Substring(start, end - start);
        }

        //TODO: extract from H1, H2, H3?
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }
      return null;
    }

    public static string ExtractDescriptionForPage(string page)
    {
      try
      {
        //XElement x = ReadPageAsXhtml(page);
        //if (x != null)
        //{
        //    XElement desc = x.Descendants().Where(m => m.Name.LocalName == "meta" &&
        //        m.Attribute("name").Value == "description").FirstOrDefault();
        //    if (desc != null) return desc.Attribute("content").Value;
        //}
        int start = page.IndexOf("<head");
        if (start > -1) start = page.Replace('\'', '"').IndexOf("<meta name=\"description\"", start);
        if (start > -1) start = page.IndexOf("content=", start) + 8;
        if (start > -1)
        {
          int end = page.IndexOf(">", start);
          if (end > -1) end = page.Substring(start, end - start).LastIndexOf('"');
          if (end > -1) return page.Substring(start, end - start);
        }

        //TODO: extract from first paragraph?
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }
      return null;
    }

    public static IEnumerable<Uri> ExtractLinks(string html)
    {
      try
      {
        XElement x = ReadHtmlAsXhtml(html);
        //TODO: support base address for local links?
        return x.Descendants()
                    .Where(a => a.Name.LocalName == "a" && a.Attribute("href") != null 
                        && a.Attribute("href").Value.StartsWith("http"))
          .Select(a => new Uri(a.Attribute("href").Value));
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }
      return null;
    }
  }
}
