/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
using AtomSite.Domain;
namespace AtomSite.WebCore
{
  public class BlogSearchWidget : CompositeWidget
  {
    public BlogSearchWidget()
      : base("BlogSearchWidget", "Blog", "SearchWidget")
    {
      Description = "This widget adds a search box that searches visible entries in the selected scope.";
      AddAsset("jquery.watermark.js");
      TailScript += "$(document).ready(function() { $('#search input[name=term]').val('Search...').watermark({ watermarkText: 'Search...', watermarkCssClass: 'watermark' }); });";
      SupportedScopes = SupportedScopes.All;
      OnGetConfigInclude = (s) => new ConfigLinkInclude("WidgetConfigLinkWidget", s, "Widget", "ScopeConfig");
      AreaHints = new[] { "nav" };
      // OnValidate = (i) => new ScopeInclude(i).ScopeName != null;
    }
  }
}
