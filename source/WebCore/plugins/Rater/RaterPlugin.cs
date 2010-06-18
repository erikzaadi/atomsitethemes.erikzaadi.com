/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.Rater
{
  using System.Web.Mvc;
  using System.Web.Routing;
  using AtomSite.WebCore;
  using StructureMap;
  using System.Collections.Generic;
  using AtomSite.Domain;

  public class RaterPlugin : BasePlugin
  {
    public RaterPlugin(ILogService logger) : base(logger) { }

    public override void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets)
    {
      container.Configure(c => c.For<IRaterService>().Add<RaterService>());

      RegisterController<RaterController>(container);
      RegisterWidget<RaterWidget>(container);
      RegisterRoutes<RaterRouteRegistrar>(container, routes);
    }

    public override PluginState Setup(IContainer container, string appPath)
    {
        SetupIncludeInPageArea(container, "BlogEntry", "sidetop", "RaterWidget");
        return base.Setup(container, appPath);
    }

    public override PluginState Uninstall(IContainer container, string appPath)
    {
        UninstallInclude(container, (i) => i.Name == "RaterWidget");
        return base.Uninstall(container, appPath);
    }
  }
}
