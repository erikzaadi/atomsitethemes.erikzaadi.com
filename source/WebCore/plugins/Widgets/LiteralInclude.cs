/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Linq;
  using AtomSite.Domain;
  using System.Web;

  public class LiteralInclude : Include
  {
    public LiteralInclude()
      : base()
    {
      this.Name = "LiteralWidget";
    }

    public LiteralInclude(string html)
      : this()
    {
      this.Html = html;
    }

    public LiteralInclude(Include include) : base(include) { }

    public string Html
    {
      get
      {
        if (this.Xml.Nodes().Count() == 0) return null;
        return HttpUtility.HtmlDecode(string.Concat(this.Xml.Nodes().Select(e => e.ToString()).ToArray()));
      }
      set
      {
        //it automatically encodes
        this.Xml.SetValue(value);
      }
    }
  }
}
