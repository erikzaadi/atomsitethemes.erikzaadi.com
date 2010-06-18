/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Web.Mvc;
  using System.Web.Routing;
  using AtomSite.Domain;
  using StructureMap;
  using StructureMap.Attributes;
  using System.Collections.Generic;

  public class AnnotatePlugin : BasePlugin
  {
    public AnnotatePlugin(ILogService logger)
      : base(logger)
    {
      DefaultMerit = (int)MeritLevel.High - 10;
    }

    public override void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets)
    {
      container.Configure(x => x.For<IAnnotateService>().Singleton().Add<AnnotateService>());

      RegisterController<AnnotateController>(container);
      RegisterWidget<AnnotateListWidget>(container);
      RegisterRoutes<AnnotateRouteRegistrar>(container, routes);
    }
    public override PluginState Setup(IContainer container, string appPath)
    {
      base.SetupIncludeInPageArea(container, "AtomPubResource", "content", "AnnotateListWidget");

      return base.Setup(container, appPath);
    }
  }
}
