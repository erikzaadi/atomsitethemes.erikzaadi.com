/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Web.Routing;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using System.Collections.Generic;

  public class TrackbackRouteRegistrar : BaseRouteRegistrar
  {
    public TrackbackRouteRegistrar(IAppServiceRepository svcRepo, List<SiteRoute> routes, int defaultMerit)
      : base(svcRepo, routes, defaultMerit) { }

    public override void RegisterRoutes()
    {
      MapDatedRoutes("Trackback", @"/{path}.atom/trackback",
        new { controller = "Trackback", action = "Trackback" });

      MapDatedRoutes("Pingback", @"/{path}.atom/pingback",
        new { controller = "Trackback", action = "Pingback" });
    }

    protected override bool CollectionFilter(AppCollection coll)
    {
      return new BlogAppCollection(coll).TrackbacksOn;
    }
  }
}
