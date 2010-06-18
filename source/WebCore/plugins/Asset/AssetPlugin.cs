/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using System.Web.Mvc;
  using System.Web.Routing;
  using System.Linq;
  using AtomSite.Repository;
  using AtomSite.Domain;

  public class AssetPlugin : BasePlugin
  {
    public AssetPlugin(ILogService logger)
      : base(logger)
    {
      DefaultMerit = (int)MeritLevel.Max;
    }

    public override AtomSite.Domain.PluginState Upgrade(StructureMap.IContainer container, string appPath, System.Version previous, System.Version current)
    {
      //fix non named pages to specify group
      if (previous >= ServerApp.Version12 && previous < ServerApp.Version14)
      {
        LogService.Info("Updating non named page includes in configuration to the default site master page, and getting rid of group specifier if there is one");
        var svcRepo = container.GetInstance<IAppServiceRepository>();
        var appSvc = svcRepo.GetService();
        bool changes = false;
        var pages = appSvc.Pages.Where(p => string.IsNullOrEmpty(p.Name));
        foreach (var page in pages)
        {
          page.Name = "Site";
          page.SetProperty<string>("group", null);
          changes = true;
        }
        if (changes)
        {
          LogService.Info("Saving changes to service configuration");
          svcRepo.UpdateService(appSvc);
        }
      }
      return base.Upgrade(container, appPath, previous, current);
    }

    public override void Register(StructureMap.IContainer container, List<SiteRoute> routes, System.Web.Mvc.ViewEngineCollection viewEngines, System.Web.Mvc.ModelBinderDictionary modelBinders, ICollection<AtomSite.Domain.Asset> assets)
    {
      RegisterController<AssetController>(container);

      //this.AddRoute(routes, new SiteRoute() { Name = "IgnoreImages", Route = new Route("img/{file}.{ext}", new StopRoutingHandler()) });
      //this.AddRoute(routes, new SiteRoute() { Name = "IgnoreImages2", Route = new Route("img/{theme}/{file}.{ext}", new StopRoutingHandler()) });

      this.AddRoute(routes, new SiteRoute()
      {
        Merit = DefaultMerit,
        Name = "AssetGroupJs",
        Route = new System.Web.Routing.Route("js/combined/{group}.js",
          new RouteValueDictionary(new { controller = "Asset", action = "GroupJs" }), new MvcRouteHandler())
      });
      //this.AddRoute(routes, new SiteRoute() { Name = "IgnoreJavascript", Route = new Route("js/{*pathInfo}", new StopRoutingHandler()) });

      this.AddRoute(routes, new SiteRoute()
      {
        Merit = DefaultMerit,
        Name = "AssetGroupCss",
        Route = new System.Web.Routing.Route("css/combined/{group}.css",
          new RouteValueDictionary(new { controller = "Asset", action = "GroupCss" }), new MvcRouteHandler())
      });
      //this.AddRoute(routes, new SiteRoute() { Name = "IgnoreStyles", Route = new Route("css/{*pathInfo}", new StopRoutingHandler()) });
    }
  }
}
