/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using System.Xml.Linq;
  using AtomSite.Domain;

  public class MenuModel : BaseModel
  {
    public MenuModel()
    {
      MenuItems = new List<MenuItem>();
    }

    public IList<MenuItem> MenuItems { get; set; }
  }

  public class MenuItem
  {
    public string Href { get; set; }
    public string Text { get; set; }
    public string Title { get; set; }
    public bool Selected { get; set; }
    public bool ExactSelect { get; set; }
  }

  public class MenuConfigModel : BaseModel
  {
    public string IncludePath { get; set; }
    public string Menu { get; set; }
  }
}
