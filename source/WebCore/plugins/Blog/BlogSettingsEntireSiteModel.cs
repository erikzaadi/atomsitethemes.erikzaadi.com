/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Domain;
  using System.Web.Mvc;
using System.Collections.Generic;

  [Bind(Include = "BlogPageExt")]
  public class BlogSettingsEntireSiteModel : AdminModel
  {
    public static readonly Dictionary<string, string> Extensions;

    static BlogSettingsEntireSiteModel()
    {
      Extensions = new Dictionary<string, string>();
      Extensions.Add(string.Empty, "(none)");
      Extensions.Add(".xhtml", ".xhtml (default)");
      Extensions.Add(".html", ".html");
      Extensions.Add(".htm", ".htm");
      Extensions.Add(".aspx", ".aspx");
      Extensions.Add(".mvc", ".mvc");
      Extensions.Add(".asp", ".asp");
    }

    public string BlogPageExt { get; set; }
    public IEnumerable<SelectListItem> ExtSelections { get; set; }
  }

}
