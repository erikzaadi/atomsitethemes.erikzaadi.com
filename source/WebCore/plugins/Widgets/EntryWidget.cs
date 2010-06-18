/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Domain;

  public class EntryWidget : CompositeWidget
  {
    public EntryWidget() :
      base("EntryWidget", "Widget", "Entry")
    {
      Description = "This widget shows the summary content of a single entry.";
      SupportedScopes = SupportedScopes.All;
      OnGetConfigInclude = (s) => new ConfigLinkInclude("WidgetConfigLinkWidget", s, "Widget", "EntryConfig");
    }
  }
}
