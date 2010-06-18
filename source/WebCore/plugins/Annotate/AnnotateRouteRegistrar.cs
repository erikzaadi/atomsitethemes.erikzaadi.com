/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Web.Routing;
  using AtomSite.Repository;
  using System.Collections.Generic;

  public class AnnotateRouteRegistrar : BaseRouteRegistrar
  {
    public AnnotateRouteRegistrar(IAppServiceRepository svcRepo, List<SiteRoute> routes, int defaultMerit)
      : base(svcRepo, routes, defaultMerit) { }

    public override void RegisterRoutes()
    {
      MapRoute("AnnotateAnnotationsFeed", BasePath + @"/annotations.{type}",
        new { controller = "Annotate", action = "AnnotationsFeed", type = "atom" }, new { type = "(atom|rss)" });
      MapDatedRoutes("AnnotateEntryAnnotationsFeed", @"/{path}/annotations.{type}",
        new { controller = "Annotate", action = "AnnotationsFeed", type = "atom" }, new { type = "(atom|rss)" });

      MapDatedRoutes("AnnotateEntry", @"/{path}.atom",
        new { controller = "Annotate", action = "Entry" }, 
        new { httpMethod = new HttpMethodConstraint("POST") }, MapRouteModes.Secure);
    }
  }
}
