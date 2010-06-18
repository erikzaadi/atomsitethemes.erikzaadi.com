/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Web;
  using System.Web.Routing;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using StructureMap;
  using System.Linq;

  public interface IRouteService
  {
    AppService GetService();
    AppWorkspace GetWorkspace();
    AppCollection GetCollection();
    Id GetEntryId();
    Scope GetScope();

    Uri RouteUrl(string routeName, Id id);
    Uri RouteUrl(string routeName, Id id, AbsoluteMode abMode);

    Uri RouteUrl(string routeName, Id id, RouteValueDictionary routeData);
    Uri RouteUrl(string routeName, Id id, RouteValueDictionary routeData, AbsoluteMode abMode);

    Uri RouteUrl(string routeName, Id id, object routeData);
    Uri RouteUrl(string routeName, Id id, object routeData, AbsoluteMode abMode);

    Uri RouteUrl(string routeName);
    Uri RouteUrl(string routeName, AbsoluteMode abMode);

    Uri RouteUrl(string routeName, RouteValueDictionary routeData);
    Uri RouteUrl(string routeName, RouteValueDictionary routeData, AbsoluteMode abMode);

    Uri RouteUrl(string routeName, object routeData);
    Uri RouteUrl(string routeName, object routeData, AbsoluteMode abMode);
  }

  public enum AbsoluteMode : byte
  {
    Default = 0,
    Force
  }
  
  /// <summary>
  /// This class provides base functionality for routing.
  /// </summary>
  public class RouteService : IRouteService
  {
    protected AppService AppService;
    protected readonly string BasePath;
    public RequestContext Context { get; protected set; }
    public RouteCollection Routes { get; protected set; }
    protected ILogService LogService { get; private set; }

    public RouteService(IAppServiceRepository svcRepo, RequestContext context, RouteCollection routes, ILogService logger)
    {
      AppService = svcRepo.GetService();
      Routes = routes;
      Context = context;
      LogService = logger;
    }

    public virtual Scope GetScope()
    {
      AppWorkspace workspace = GetWorkspace();
      AppCollection collection = GetCollection();
      string w = workspace != null ? workspace.Name ?? Atom.DefaultWorkspaceName : null;
      string c = collection != null ? collection.Id.Collection : null;
      return new Scope() { Workspace = w, Collection = c };
    }

    public virtual AppService GetService()
    {
      return AppService;
    }

    public virtual AppWorkspace GetWorkspace()
    {
      if (Context.RouteData.Values["workspace"] != null)
        return AppService.GetWorkspace((string)Context.RouteData.Values["workspace"]);
      else if (AppService.Workspaces.Count() > 0)
        return AppService.GetWorkspace(); //get default

      return null;
    }

    public virtual AppCollection GetCollection()
    {
      var ws = GetWorkspace();
      if (ws == null) return null;

      if (Context.RouteData.Values["collection"] != null)
        return ws.GetCollection((string)Context.RouteData.Values["collection"]);
      else if (ws.Collections.Count() > 0)
        return ws.GetCollection(); //get default

      return null;
    }

    /// <summary>
    /// Get the entry id based on the route data.
    /// </summary>
    public virtual Id GetEntryId()
    {
      var coll = GetCollection();
      if (coll == null) return null;

      //if viewing an entry, get the id
      if (Context.RouteData.Values["path"] != null)
      {
        //dated entry
        if (Context.RouteData.Values["year"] != null &&
          Context.RouteData.Values["month"] != null &&
          Context.RouteData.Values["day"] != null)
        {
          return coll.Id.AddPath(
            int.Parse((string)Context.RouteData.Values["year"]),
            int.Parse((string)Context.RouteData.Values["month"]),
            int.Parse((string)Context.RouteData.Values["day"]),
            (string)Context.RouteData.Values["path"]);
        }
        //non-dated
        else return coll.Id.AddPath((string)Context.RouteData.Values["path"]);
      }
      return null;
    }

    public virtual Uri RouteUrl(string routeName, Id id)
    {
      return RouteUrl(routeName, id, AbsoluteMode.Default);
    }
    public virtual Uri RouteUrl(string routeName, Id id, AbsoluteMode abMode)
    {
      return RouteUrl(routeName, id, null, abMode);
    }

    public virtual Uri RouteUrl(string routeName, Id id, RouteValueDictionary routeData)
    {
      return RouteUrl(routeName, id, routeData, AbsoluteMode.Default);
    }

    public virtual Uri RouteUrl(string routeName, Id id, object routeData)
    {
      var dic = routeData == null ? new RouteValueDictionary() :
        routeData is RouteValueDictionary ? (RouteValueDictionary)routeData :
        new RouteValueDictionary(routeData);
      return RouteUrl(routeName, id, dic, AbsoluteMode.Default);
    }

    public virtual Uri RouteUrl(string routeName, Id id, object routeData, AbsoluteMode abMode)
    {
      var dic = routeData == null ? new RouteValueDictionary() :
        routeData is RouteValueDictionary ? (RouteValueDictionary)routeData :
        new RouteValueDictionary(routeData);
      return RouteUrl(routeName, id, dic, abMode);
    }

    public virtual Uri RouteUrl(string routeName, Id id, RouteValueDictionary routeData, AbsoluteMode abMode)
    {
      if (routeData == null) routeData = new RouteValueDictionary();
      if (id != null)
      {
        if (AppService.ServiceType == ServiceType.MultiFolder)
        {
          routeData["workspace"] = id.Workspace;
        }

        routeData["collection"] = id.Collection;

        //dated resource
        if (id.Date.Length == 10 && AppService.GetCollection(id).Dated)
        {
          routeData["year"] = id.Date.Substring(0, 4);
          routeData["month"] = id.Date.Substring(5, 2);
          routeData["day"] = id.Date.Substring(8, 2);
          if (!routeName.EndsWith("Dated")) routeName = routeName + "Dated";
        }

        if (!string.IsNullOrEmpty(id.EntryPath))
          routeData["path"] = id.EntryPath;
      }

      VirtualPathData path = Routes.GetVirtualPath(Context, routeName, routeData);
      string url = path == null ? string.Empty : path.VirtualPath;
      return ToUri(routeName, url, abMode);
    }

    protected virtual Uri ToUri(string routeName, string url, AbsoluteMode abMode)
    {
      System.Web.Routing.Route r = Routes[routeName] as System.Web.Routing.Route;
      bool secure = false;
      if (r != null && r.DataTokens["Secure"] != null) secure = (bool)r.DataTokens["Secure"];
      return ToUri(Context, url, secure & AppService.Secure, abMode);
    }

    public virtual Uri ToUri(RequestContext context, string url, bool secure, AbsoluteMode abMode)
    {
      //make absolute if current scheme doesn't match needed scheme
      //which is typical when switching between http and https
      if (secure && !context.HttpContext.Request.IsSecureConnection)
      {
        return new Uri(ToAbsolute(context, url, true));
      }
      if (abMode == AbsoluteMode.Force) return new Uri(ToAbsolute(context, url, false));
      Uri uri = new Uri(url, UriKind.Relative);
      return uri;
    }

    public virtual string ToAbsolute(RequestContext context, string partialUrl, bool secure)
    {
      IContainer container = (IContainer)context.HttpContext.Application["Container"];
      ILogService logger = container.GetInstance<ILogService>();

      try
      {
        string url = partialUrl;
        if (url == null) url = string.Empty;
        Uri uri = this.AppService.Base; //context.HttpContext.Request.Url; //TODO: causes bug, investigate deeper

        UriBuilder uriBuilder = secure ? new UriBuilder(Uri.UriSchemeHttps, uri.Host, uri.Port) :
          new UriBuilder(uri.Scheme, uri.Host, uri.Port);

        if (url.Contains("?"))
        {
          string[] var = url.Split(new char[] { '?' }, 2);
          uriBuilder.Path = var[0];
          uriBuilder.Query = var[1];
        }
        else uriBuilder.Path = url;
        return uriBuilder.Uri.ToString();
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        return VirtualPathUtility.ToAbsolute(partialUrl);
      }
    }

    #region IRouteService Members


    public virtual Uri RouteUrl(string routeName)
    {
      return RouteUrl(routeName, null, null, AbsoluteMode.Default);
    }

    public virtual Uri RouteUrl(string routeName, AbsoluteMode abMode)
    {
      return RouteUrl(routeName, null, null, abMode);
    }

    public virtual Uri RouteUrl(string routeName, RouteValueDictionary routeData)
    {
      return RouteUrl(routeName, null, routeData, AbsoluteMode.Default);
    }

    public virtual Uri RouteUrl(string routeName, RouteValueDictionary routeData, AbsoluteMode abMode)
    {
      return RouteUrl(routeName, null, routeData, abMode);
    }

    public virtual Uri RouteUrl(string routeName, object routeData)
    {
      return RouteUrl(routeName, null, new RouteValueDictionary(routeData), AbsoluteMode.Default);
    }

    public virtual Uri RouteUrl(string routeName, object routeData, AbsoluteMode abMode)
    {
      return RouteUrl(routeName, null, new RouteValueDictionary(routeData), abMode);
    }

    #endregion


    public static IRouteService GetCurrent(RequestContext ctx)
    {
      Route r = ctx.RouteData.Route as Route;
      return GetForRoute(ctx, r);
    }

    public static IRouteService GetForRoute(RequestContext ctx, string routeName)
    {
      Route r = RouteTable.Routes[routeName] as Route;
      return GetForRoute(ctx, r);
    }

    static IRouteService GetForRoute(RequestContext ctx, Route r)
    {
      IRouteService routing = null;
      IContainer container = (IContainer)ctx.HttpContext.Application["Container"];

      if (r != null && r.DataTokens != null)
      {
        string routeServiceType = r.DataTokens["RouteServiceType"] as string;

        if (routeServiceType != null)
        {
          routing = (IRouteService)container
            .With<RequestContext>(ctx)
            .With<RouteCollection>(RouteTable.Routes)
            .GetInstance(Type.GetType(routeServiceType));
        }
      }

      //get default
      if (routing == null)
      {
        routing = container
          .With<RequestContext>(ctx)
          .With<RouteCollection>(RouteTable.Routes)
          .GetInstance<IRouteService>();
      }
      return routing;
    }
  }
}
