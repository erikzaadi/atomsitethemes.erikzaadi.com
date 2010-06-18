/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Web.Mvc;
  using System.Web.Routing;
  using StructureMap;
  using System.Collections.Generic;
  using AtomSite.Domain;

  public class MenuPlugin : BasePlugin
  {
    public MenuPlugin(ILogService logger) : base(logger) { }

    public override void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets)
    {
      RegisterController<MenuController>(container);
      RegisterWidget<MenuCustomWidget>(container);

      RegisterWidget(container, new CompositeWidget("MenuWidget", "Menu", "MenuWidget")
      {
          SupportedScopes = SupportedScopes.All,
          Description = "This widget will automatically create links to your *visible* collections.",
          AreaHints = new[] { "nav" }
      });
    }
  }
}
