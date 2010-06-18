/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Web.Routing;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using System.Collections.Generic;

  public class AtomPubRouteRegistrar : BaseRouteRegistrar
  {
    public AtomPubRouteRegistrar(IAppServiceRepository svcRepo, List<SiteRoute> routes, int defaultMerit)
      : base(svcRepo, routes, defaultMerit) { }

    public override void RegisterRoutes()
    {
      MapRoute("AtomPubError", @"Error",
        new { controller = "AtomPub", action = "Error" }, (int)MeritLevel.High + 50);
      MapRoute("AtomPubNotFound", @"NotFound",
        new { controller = "AtomPub", action = "NotFound" }, (int)MeritLevel.High + 50);

      MapRoute("AtomPubService", @"service.atomsvc",
        new { controller = "AtomPub", action = "Service" }, MapRouteModes.Secure);

      MapRoute("AtomPubFeed", BasePath + @"/feed.{type}",
        new { controller = "AtomPub", action = "Feed", type = "atom" }, new { type = "(atom|rss)" });

      MapDatedRoutes("AtomPubEntry", @"/{path}.atom",
        new { controller = "AtomPub", action = "Entry" });

      MapDatedRoutes("AtomPubApproveEntry", @"/{path}.atom/approve",
        new { controller = "AtomPub", action = "ApproveEntry" });

      MapDatedRoutes("AtomPubApproveAll", @"/{path}.atom/approveAll",
        new { controller = "AtomPub", action = "ApproveAll" });

      MapDatedRoutes("AtomPubEntryEdit", @"/{path}.atom",
        new { controller = "AtomPub", action = "Entry" }, MapRouteModes.Secure);

      MapDatedRoutes("AtomPubMedia", @"/{path}.media",
        new { controller = "AtomPub", action = "Media" });

      MapDatedRoutes("AtomPubMediaEdit", @"/{path}.media",
        new { controller = "AtomPub", action = "Media" }, MapRouteModes.Secure);

      MapRoute("AtomPubCollection", BasePath + @".atom",
        new { controller = "AtomPub", action = "Collection" }, MapRouteModes.Secure);

      if (AppService.ServiceType == ServiceType.MultiFolder)
      {
        MapRoute("AtomPubServiceIndex", string.Empty,
          new { controller = "AtomPub", action = "ServiceIndex" });
        MapRoute("AtomPubWorkspaceIndex", "{workspace}",
          new { controller = "AtomPub", action = "WorkspaceIndex" });
        MapRoute("AtomPubCollectionIndex", BasePath);
        MapDatedRoutes("AtomPubResource", @"/{path}",
          new { controller = "AtomPub", action = "Resource" });
      }
      else //single workspace or subdomain workspaces
      {
        //no service index?
        MapRoute("AtomPubWorkspaceIndex", string.Empty,
          new { controller = "AtomPub", action = "WorkspaceIndex" });
        MapRoute("AtomPubCollectionIndex", BasePath,
          new { controller = "AtomPub", action = "CollectionIndex" });
        MapDatedRoutes("AtomPubResource", @"/{path}",
          new { controller = "AtomPub", action = "Resource" });
      }
    }
  }
}
