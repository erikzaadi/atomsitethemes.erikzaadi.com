/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using StructureMap;

  public class WidgetPlugin : BasePlugin
  {
    public WidgetPlugin(ILogService logger)
      : base(logger)
    {
    }

    public override void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets)
    {
      RegisterController<WidgetController>(container);
      RegisterWidget<LiteralWidget>(container);
      RegisterWidget(container, new ConfigLinkWidget());
      RegisterWidget(container, new ConfigLinkWidget("WidgetConfigLinkWidget", new[] { new Asset("WidgetConfig.css", AdminPlugin.AdminAssetGroup)}));
      RegisterWidget(container, new FeedConfigLinkWidget()
      {
        Assets = new[] 
        { 
          new Asset("WidgetConfig.css", AdminPlugin.AdminAssetGroup)
        }
      });
    }
  }
}
