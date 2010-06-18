/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Web.Routing;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using System.Collections.Generic;

  public class AdminRouteRegistrar : BaseRouteRegistrar
  {
    public AdminRouteRegistrar(IAppServiceRepository svcRepo, List<SiteRoute> routes, int defaultMerit)
      : base(svcRepo, routes, defaultMerit) { }

    public override void RegisterRoutes()
    {
      MapRoute("AdminDashboard", "Admin", new { controller = "Admin", action = "Dashboard" });
      MapRoute("AdminDefault", "Admin/{action}", new { controller = "Admin", action = "Dashboard" });
      MapRoute("AdminEditEntry", "Admin/EditEntry", new { controller = "Admin", action = "EditEntry" });
      MapRoute("AdminEditMedia", "Admin/EditMedia", new { controller = "Admin", action = "EditMedia" });
      MapRoute("AdminDeleteUser", "Admin/EditMedia", new { controller = "Admin", action = "DeleteUser" });
    }
  }
}
