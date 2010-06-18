/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Web.Mvc;
  using System.Web.Routing;
  using StructureMap;
  using System.Collections.Generic;
  using AtomSite.Domain;

  public class WizardPlugin : BasePlugin
  {
    public static readonly string WizardAssetGroup = "wizard";

    public WizardPlugin(ILogService logger)
      : base(logger)
    {
      //we must have the highest merit to force display above all else
      DefaultMerit = (int)MeritLevel.Max;
      DefaultAssetGroup = WizardAssetGroup;
    }

    public override void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets)
    {
      RegisterController<WizardController>(container);

      //RegisterGlobalAsset(globalAssets, "wizard.css");
      RegisterPage(container, new Page("Wizard") //master page
      {
        Assets = new[] { new Asset("wizard.css", WizardAssetGroup) },
        Areas = new[] { "content", "tail" },
        SupportedScopes = SupportedScopes.EntireSite
      });
      RegisterPage(container, new Page("WizardBasicSettings", "Wizard"));
      RegisterPage(container, new Page("WizardComplete", "Wizard"));
      RegisterPage(container, new Page("WizardSetupChoice", "Wizard")
      {
        Areas = new[] { "setupOptions" },
        SupportedScopes = SupportedScopes.EntireSite
      });
      RegisterPage(container, new Page("WizardTestInstall", "Wizard"));
      RegisterPage(container, new Page("WizardThemeChoice", "Wizard"));
      
      this.AddRoute(routes, new SiteRoute() { Name = "Wizard", Route = new Route("Wizard/{action}", 
        new RouteValueDictionary(new { controller = "Wizard" }), new MvcRouteHandler()), Merit = DefaultMerit });

      this.AddRoute(routes, new SiteRoute()
      {
        Name = "WizardCatchAll",
        Route = new Route("{*all}", new RouteValueDictionary(new
          {
            controller = "Wizard",
            action = "CatchAll"
          }), new MvcRouteHandler()),
        Merit = (int)MeritLevel.High + 10000
      });
    }
  }
}
