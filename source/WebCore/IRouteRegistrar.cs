/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Linq;
  using System.Web.Mvc;
  using System.Web.Routing;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using System.Collections.Generic;

  public interface IRouteRegistrar
  {
    void RegisterRoutes();
  }
  
  [Flags]
  public enum MapRouteModes
  {
    None = 0,
    NeedsBase = 1,
    Secure = 2,
    Dated = 4,
    NonDated = 8
  }

  public abstract class BaseRouteRegistrar : IRouteRegistrar
  {
    protected AppService AppService { get; private set; }
    protected List<SiteRoute> Routes { get; private set; }
    public string RouteServiceType { get; set; }
    protected string BasePath { get; private set; }
    protected int DefaultMerit { get; private set; }

    public BaseRouteRegistrar(IAppServiceRepository svcRepo, List<SiteRoute> routes, int defaultMerit)
    {
      this.AppService = svcRepo.GetService();
      this.BasePath = GetBasePath();
      this.Routes = routes;
      this.RouteServiceType = typeof(RouteService).AssemblyQualifiedName;
      this.DefaultMerit = defaultMerit;
    }

    protected virtual string GetBasePath()
    {
      string basePath = string.Empty;
      if (AppService.ServiceType == ServiceType.MultiFolder)
        basePath += "{workspace}/";
      basePath += "{collection}";
      return basePath;
    }

    protected virtual SiteRoute[] MapDatedRoutes(string name, string urlWithoutBase, object defaults)
    {
      return new[] { MapRoute(name, urlWithoutBase, defaults, null, MapRouteModes.NeedsBase | MapRouteModes.NonDated, DefaultMerit)
      , MapRoute(name + "Dated", "/{year}/{month}/{day}" + urlWithoutBase, defaults, null, MapRouteModes.NeedsBase | MapRouteModes.Dated, DefaultMerit) };
    }

    protected virtual SiteRoute[] MapDatedRoutes(string name, string urlWithoutBase, object defaults, object constraints)
    {
      return new[] { MapRoute(name, urlWithoutBase, defaults, constraints, MapRouteModes.NeedsBase | MapRouteModes.NonDated, DefaultMerit)
      , MapRoute(name + "Dated", "/{year}/{month}/{day}" + urlWithoutBase, defaults, constraints, MapRouteModes.NeedsBase | MapRouteModes.Dated, DefaultMerit) };
    }

    protected virtual SiteRoute[] MapDatedRoutes(string name, string urlWithoutBase, object defaults, object constraints, MapRouteModes modes)
    {
      return new[] { MapRoute(name, urlWithoutBase, defaults, constraints, modes | MapRouteModes.NeedsBase | MapRouteModes.NonDated, DefaultMerit)
      , MapRoute(name + "Dated", "/{year}/{month}/{day}" + urlWithoutBase, defaults, constraints, modes | MapRouteModes.NeedsBase | MapRouteModes.Dated, DefaultMerit)};
    }

    protected virtual SiteRoute MapRoute(string routeName, string url)
    {
      return MapRoute(routeName, url, null, null, MapRouteModes.None, DefaultMerit);
    }

    protected virtual SiteRoute MapRoute(string routeName, string url, MapRouteModes modes)
    {
      return MapRoute(routeName, url, null, null, modes, DefaultMerit);
    }

    protected virtual SiteRoute MapRoute(string routeName, string url, object defaults)
    {
      return MapRoute(routeName, url, defaults, null, MapRouteModes.None, DefaultMerit);
    }

    protected virtual SiteRoute MapRoute(string routeName, string url, object defaults, int merit)
    {
      return MapRoute(routeName, url, defaults, null, MapRouteModes.None, merit);
    }

    protected virtual SiteRoute MapRoute(string routeName, string url, object defaults, MapRouteModes modes)
    {
      return MapRoute(routeName, url, defaults, null, modes, DefaultMerit);
    }

    protected virtual SiteRoute MapRoute(string routeName, string url, object defaults, object constraints)
    {
      return MapRoute(routeName, url, defaults, constraints, MapRouteModes.None, DefaultMerit);
    }

    protected virtual SiteRoute MapRoute(string routeName, string url, object defaults, object constraints, MapRouteModes modes, int merit)
    {
      var dataTokens = new RouteValueDictionary();
      var defaultsDict = defaults == null ? new RouteValueDictionary() : new RouteValueDictionary(defaults);
      var constraintsDict = defaults == null ? new RouteValueDictionary() : new RouteValueDictionary(constraints);

      if ((modes & MapRouteModes.Secure) == MapRouteModes.Secure)
        dataTokens.Add("Secure", true);

      if ((modes & MapRouteModes.NeedsBase) == MapRouteModes.NeedsBase)
        url = BasePath + url;

      //allow collection plugin routes to be constrained to just the collections they are activated for
      if (url.Contains("{collection}") && !constraintsDict.ContainsKey("collectionIds"))
      {
        constraintsDict.Add("collectionIds",
          (modes & MapRouteModes.Dated) == MapRouteModes.Dated ? GetDatedCollectionConstraint() :
          (modes & MapRouteModes.NonDated) == MapRouteModes.NonDated ? GetNonDatedCollectionConstraint() :
          GetCollectionConstraint()
          );
      }

      //add restricted path keywords to constraint 
      //HACK: this is currently a hack needed due to inability to
      //insert routes
      //TODO: we can now insert routes, go back and fix this hack
      if (url.Contains("{path}") && !constraintsDict.ContainsKey("path"))
      {
        constraintsDict.Add("path", new PathRouteConstraint()); 
      }

      SetDataTokens(dataTokens);
      System.Web.Routing.Route r = new System.Web.Routing.Route(url, defaultsDict, constraintsDict, dataTokens, new MvcRouteHandler());
      var siteRoute = new SiteRoute() { Merit = merit, Name = routeName, Route = r };
      Routes.AddRoute(siteRoute, merit);
      return siteRoute;
    }

    protected virtual void SetDataTokens(RouteValueDictionary dataTokens)
    {
      dataTokens.Add("RouteServiceType", RouteServiceType);
    }

    protected virtual bool CollectionFilter(AppCollection coll)
    {
      return true;
    }

    protected virtual object GetDatedCollectionConstraint()
    {
      return new CollectionRouteConstraint(
        AppService.Workspaces.SelectMany(w => w.Collections)
        .Where(CollectionFilter).Where(c => c.Dated).Select(c => c.Id));
    }

    protected virtual object GetNonDatedCollectionConstraint()
    {
      return new CollectionRouteConstraint(
          AppService.Workspaces.SelectMany(w => w.Collections)
          .Where(CollectionFilter).Where(c => !c.Dated).Select(c => c.Id));
    }

    protected virtual object GetCollectionConstraint()
    {
      return new CollectionRouteConstraint(
          AppService.Workspaces.SelectMany(w => w.Collections)
          .Where(CollectionFilter).Select(c => c.Id));
    }

    public abstract void RegisterRoutes();
  }

  public struct SiteRoute
  {
    public int Merit { get; set; }
    public string Name { get; set; }
    public RouteBase Route { get; set; }
    public override string ToString()
    {
      return Merit.ToString().PadLeft(10).PadRight(1) + Name.PadRight(30) + Route.ToString();
    }
  }

  public static class SiteRouteCollectionExtensions
  {
    public static void AddRoute(this IList<SiteRoute> routes, SiteRoute route, int merit)
    {
      var osr = routes.Where(r => r.Merit == merit).LastOrDefault();

      int insertAt = routes.Count;
      for (int i = 0; i < routes.Count; i++)
      {
        if (routes[i].Merit < route.Merit)
        {
          insertAt = i;
          break;
        }
      }
      routes.Insert(insertAt, route);
    }

    public static void AddRoute(this IList<SiteRoute> routes, SiteRoute route)
    {
      AddRoute(routes, route, (int)MeritLevel.Default);
    }

    public static void AddRoute(this BasePlugin plugin, IList<SiteRoute> routes, SiteRoute route)
    {
      routes.AddRoute(route, plugin.DefaultMerit);
    }
  }
}
