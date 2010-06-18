namespace AtomSite.WebCore
{
  using System;
  using System.Web.Routing;
  using AtomSite.Domain;
  using AtomSite.Repository;

  public class AdminRouteService : RouteService
  {
    public AdminRouteService(IAppServiceRepository svcRepo, RequestContext context, RouteCollection routes, ILogService logger)
      : base(svcRepo, context, routes, logger)
    { }

    public override AppWorkspace GetWorkspace()
    {
      if (!string.IsNullOrEmpty(Context.HttpContext.Request.QueryString["id"]))
      {
        Id id = new Uri(Context.HttpContext.Request.QueryString["id"]);
        return AppService.GetWorkspace(id.Workspace);
      }
      else if (Context.HttpContext.Request.QueryString["workspace"] != null)
      {
        return AppService.GetWorkspace(Context.HttpContext.Request.QueryString["workspace"]);
      }
      else if (Context.HttpContext.Request.Form["workspace"] != null)
      {
        return AppService.GetWorkspace(Context.HttpContext.Request.Form["workspace"]);
      }
      return null;
    }

    public override AppCollection GetCollection()
    {
      if (!string.IsNullOrEmpty(Context.HttpContext.Request.QueryString["id"]))
      {
        Id id = new Uri(Context.HttpContext.Request.QueryString["id"]);
        return AppService.GetCollection(id);
        //TODO: support form post of id
      }
      var workspace = GetWorkspace();

      if (workspace != null)
      {
        if (!string.IsNullOrEmpty(Context.HttpContext.Request.QueryString["collection"]))
          return AppService.GetCollection(workspace.Name, Context.HttpContext.Request.QueryString["collection"]);
        else if (Context.HttpContext.Request.Form["collection"] != null)
          return AppService.GetCollection(workspace.Name, Context.HttpContext.Request.Form["collection"]);
      }
      return null;
    }

    public override Id GetEntryId()
    {
      if (!string.IsNullOrEmpty(Context.HttpContext.Request.QueryString["id"]))
      {
        Id id = new Uri(Context.HttpContext.Request.QueryString["id"]);
        if (!string.IsNullOrEmpty(id.EntryPath)) return id;
      }
      return null;
    }
  }
}
