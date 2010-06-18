/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;
  using AtomSite.Repository;
  using System.Web.Routing;
  using AtomSite.Domain;

  public class BlogRouteRegistrar : BaseRouteRegistrar
  {
    public BlogRouteRegistrar(IAppServiceRepository svcRepo, List<SiteRoute> routes, int defaultMerit)
      : base(svcRepo, routes, defaultMerit) { }

    public override void RegisterRoutes()
    {
      var svc = new BlogAppService(AppService);

      MapRoute("BlogWriterManifest", BasePath + @"/wlwmanifest.xml",
        new { controller = "Blog", action = "WriterManifest" });

      MapRoute("BlogSitemap", BasePath + @"/sitemap.xml",
        new { controller = "Blog", action = "Sitemap" });

      MapRoute("BlogPerson", BasePath + @"/person/{person}" + svc.BlogPageExt,
        new { controller = "Blog", action = "Person" });

      MapRoute("BlogAuthor", BasePath + @"/author/{author}" + svc.BlogPageExt,
        new { controller = "Blog", action = "Author" });

      MapRoute("BlogContributor", BasePath + @"/contributor/{contributor}" + svc.BlogPageExt,
        new { controller = "Blog", action = "Contributor" });

      MapRoute("BlogDateYear", BasePath + @"/{year}" + svc.BlogPageExt,
        new { controller = "Blog", action = "Date" }, new { year = @"(20|19)\d{2}" });

      MapRoute("BlogDateMonth", BasePath + @"/{year}/{month}" + svc.BlogPageExt,
        new { controller = "Blog", action = "Date" }, new
        {
          year = @"(20|19)\d{2}",
          month = @"[0]\d|[1][0-2]"
        });

      MapRoute("BlogDateDay", BasePath + @"/{year}/{month}/{day}" + svc.BlogPageExt,
        new { controller = "Blog", action = "Date" }, new
        {
          year = @"(20|19)\d{2}",
          month = @"[0]\d|[1][0-2]",
          day = @"[0-2]\d|[3][0-1]"
        });

      MapRoute("BlogCategory", BasePath + @"/category/{term}" + svc.BlogPageExt,
        new { controller = "Blog", action = "Category" });

      if (AppService.ServiceType == ServiceType.MultiFolder)
      {
        MapRoute("BlogSearchWorkspace", "{workspace}/Search" + svc.BlogPageExt,
          new { controller = "Blog", action = "Search" });
        MapRoute("BlogSearchService", "Search" + svc.BlogPageExt,
          new { controller = "Blog", action = "Search" });
      }
      else
      {
        MapRoute("BlogSearchWorkspace", "Search" + svc.BlogPageExt,
          new { controller = "Blog", action = "Search" });
      }

      MapRoute("BlogSearch", BasePath + @"/Search" + svc.BlogPageExt,
        new { controller = "Blog", action = "Search" });

      MapRoute("BlogListing", BasePath + @"" + svc.BlogPageExt,
        new { controller = "Blog", action = "Listing" });

      MapDatedRoutes("BlogEntry", @"/{path}" + svc.BlogPageExt,
        new { controller = "Blog", action = "Entry" });

      MapDatedRoutes("BlogPostComment", @"/{path}.atom",
        new { controller = "Blog", action = "PostComment" },
        new { httpMethod = new HttpMethodConstraint("POST") }, MapRouteModes.Secure);

      MapDatedRoutes("BlogEntryDefault", @"/{path}",
        new { controller = "Blog", action = "EntryDefault" }, new { path = @"^((?!\.).)*$" });

      MapRoute("BlogDefault", BasePath,
        new { controller = "Blog", action = "Default" });

      if (AppService.ServiceType == ServiceType.MultiFolder)
      {
        MapRoute("BlogHome", "{workspace}",
          new { controller = "Blog", action = "Home" });
      }
      else
      {
        MapRoute("BlogHome", string.Empty,
          new { controller = "Blog", action = "Home" });
      }
    }

    protected override bool CollectionFilter(AppCollection coll)
    {
      return new BlogAppCollection(coll).BloggingOn;
    }
  }
}
