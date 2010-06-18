/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.Rater
{
  using System.Web.Routing;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using AtomSite.WebCore;
  using System.Collections.Generic;

  public class RaterRouteRegistrar : BaseRouteRegistrar
  {
    public RaterRouteRegistrar(IAppServiceRepository svcRepo, List<SiteRoute> routes, int defaultMerit)
      : base(svcRepo, routes, defaultMerit) { }

    public override void RegisterRoutes()
    {
      MapDatedRoutes("RaterRateEntry", @"/{path}.atom/rate",
        new { controller = "Rater", action = "RateEntry" });
    }

    protected override bool CollectionFilter(AppCollection coll)
    {
      return new RaterAppCollection(coll).RatingsOn;
    }
  }
}
