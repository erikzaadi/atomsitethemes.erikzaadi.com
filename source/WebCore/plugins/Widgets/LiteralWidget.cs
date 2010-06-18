/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Domain;

  public class LiteralWidget : BaseWidget
  {
    public LiteralWidget()
      : base("LiteralWidget")
    {
      SupportedScopes = SupportedScopes.All;
      Description = "This widget will include static HTML into your page or widget.";
      OnGetConfigInclude = (s) => new ConfigLinkInclude(s, "Widget", "LiteralConfig");
      AreaHints = new[] { "head", "nav", "content", "sidetop", "sidemid", "sidebot", "foot", "tail" };
    }

    public override void Render(System.Web.Mvc.ViewContext ctx, AtomSite.Domain.Include include)
    {
      var i = new LiteralInclude(include);
      ctx.HttpContext.Response.Write(i.Html);
    }
  }
}
