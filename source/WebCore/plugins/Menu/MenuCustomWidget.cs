/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Linq;
  using AtomSite.Domain;

  public class MenuCustomWidget : CompositeWidget
  {
    public MenuCustomWidget()
      : base("MenuCustomWidget", "Menu", "CustomWidget")
    {
      SupportedScopes = SupportedScopes.All;
      Description = "This widget allows you to create a custom menu with custom links.";
      OnGetConfigInclude = (s) => new ConfigLinkInclude(s, "Menu", "MenuConfig");
      OnValidate = (i) => new MenuCustomInclude(i).MenuItems.Count() > 0;
      AreaHints = new[] { "nav" };
    }
  }
}
