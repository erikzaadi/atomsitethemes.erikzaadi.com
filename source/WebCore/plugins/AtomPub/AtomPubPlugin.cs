/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Web.Mvc;
  using System.Web.Routing;
  using StructureMap;
  using StructureMap.Attributes;
  using AtomSite.Domain;
  using System.Collections.Generic;

  public class AtomPubPlugin : BasePlugin
  {
    public AtomPubPlugin(ILogService logger)
      : base(logger)
    {
      DefaultMerit = (int)MeritLevel.High;
    }

    public override void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets)
    {
      container.Configure(x =>
      {
        x.For<IAtomPubService>().Singleton().Add<AtomPubService>();
        
        //this is default controller for all, no name given
        x.For<IController>().Add<AtomPubController>();
      });

      RegisterController<AtomPubController>(container);      
      RegisterRoutes<AtomPubRouteRegistrar>(container, routes);

      //register main site master page
      RegisterPage(container, new Page("Site") 
      { 
        Areas = new[] { "head", "nav", "content", "sidetop", "sidemid", "sidebot", "foot", "tail" },
        SupportedScopes = SupportedScopes.EntireSite | SupportedScopes.Workspace | SupportedScopes.Collection | SupportedScopes.Entry
      });

      //register other pages
      RegisterPage(container, new Page("AtomPubIndex", "Site") 
      { 
        SupportedScopes = SupportedScopes.EntireSite | SupportedScopes.Collection | SupportedScopes.Workspace 
      });
      
      RegisterPage(container, new Page("AtomPubResource", "Site") 
      { 
        SupportedScopes = SupportedScopes.Entry 
      });
    }
  }
}
