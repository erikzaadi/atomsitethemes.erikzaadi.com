/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Linq;
  using System.Web.Mvc;
  using System.Web.Security;
  using AtomSite.Domain;
  using StructureMap;
  public class ScopeAuthorize : ActionFilterAttribute
  {
    public bool EntireSite { get; set; }
    public bool AnyScope { get; set; }
    public string ReturnUrl { get; set; }
    public AuthRoles Roles { get; set; }
    public AuthAction Action { get; set; }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      if (ReturnUrl == null) ReturnUrl = filterContext.HttpContext.Request.Url.AbsolutePath;

      //don't return to a post action
      if (filterContext.HttpContext.Request.RequestType != "GET")
      {
        ReturnUrl = null;
      }

      User user = filterContext.HttpContext.User.Identity as User;

      //redirect if not authenticated
      if (user == null || !user.IsAuthenticated)
      {
        //send them off to the login page
        string qs = ReturnUrl != null ? string.Format("?ReturnUrl={0}", ReturnUrl) : string.Empty;
        string loginUrl = FormsAuthentication.LoginUrl + qs;
        filterContext.HttpContext.Response.Redirect(loginUrl, true);
      }

      IContainer container = (IContainer)filterContext.HttpContext.Application["container"];
      IAuthorizeService auth = container.GetInstance<IAuthorizeService>();
      var scopes = auth.GetScopes(user);

      //user must have access to entire site
      if (EntireSite && scopes.Where(s => s.IsEntireSite).Count() == 0)
        throw new UserNotAuthorizedException(user.Name, "entire site access");

      //user must have access to current scope unless any scope is allowed
      IRouteService routing = RouteService.GetCurrent(filterContext.RequestContext);
        Scope scope = routing.GetScope();
      if (!AnyScope)
      {
        if (!scope.InScope(scopes))
        {
          throw new UserNotAuthorizedException(user.Name, "scope access");
        }
      }

        //user must have at least one of the roles specified
      if (Roles > 0)
      {
          var r = auth.GetRoles(user, scope);
          if ((r & Roles) == 0)
              throw new UserNotAuthorizedException(user.Name, "not in role");
      }

        //check if user is authorized to perform action
      if (Action > 0)
      {
          var r = auth.GetRoles(user, scope);
          if (!auth.IsAuthorized(user, scope, Action))
              throw new UserNotAuthorizedException(user.Name, "action");
      }
      
    }
  }
}
