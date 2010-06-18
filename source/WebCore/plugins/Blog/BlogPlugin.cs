/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Web.Mvc;
  using System.Web.Routing;
  using StructureMap;
  using StructureMap.Attributes;
  using System.Collections.Generic;
  using AtomSite.Domain;

  public class BlogPlugin : BasePlugin
  {
    public BlogPlugin(ILogService logger)
      : base(logger)
    {
      DefaultMerit = (int)MeritLevel.High + 20;
    }
    public override void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets)
    {
      container.Configure(c =>
      {
        c.For<IBlogService>().Singleton().Add<BlogService>();
        c.For<ITrackbackService>().Singleton().Add<TrackbackService>();
        c.For<ICleanContentService>().Add<XhtmlCleanContentService>();
      });

      RegisterController<BlogController>(container);
      RegisterController<TrackbackController>(container);

      RegisterRoutes<TrackbackRouteRegistrar>(container, routes);
      RegisterRoutes<BlogRouteRegistrar>(container, routes);

      RegisterPage(container, new Page("BlogEntry", "Site", new[] { new Asset("Blog.js") }) 
      {
        SupportedScopes = SupportedScopes.Entry 
      });

      RegisterPage(container, new Page("BlogHome", "Site")
      { 
        SupportedScopes = SupportedScopes.EntireSite 
      });
      
      RegisterPage(container, new Page("BlogListing", "Site") 
      { 
        SupportedScopes = SupportedScopes.EntireSite | SupportedScopes.Workspace | SupportedScopes.Collection 
      });

      RegisterWidget<BlogAddCommentWidget>(container);

      RegisterWidget(container, new CompositeWidget("BlogArchiveWidget", "Widget", "FullFeed")
      {
        Description = "This wigit shows a listing of months with a post count for each month.",
        SupportedScopes = SupportedScopes.All,
        OnGetConfigInclude = (s) =>
        {
          return new FeedConfigLinkInclude()
            {
              Controller = "Widget",
              Action = "FeedConfig",
              HasCount = false,
              HasTitle = false,
              IncludePath = s
            };
        },
        OnValidate = (i) => new IdInclude(i).Id != null,
        AreaHints = new[]{ "sidetop", "sidemid", "sidebot" }
      });

      RegisterWidget(container, new CompositeWidget("BlogCategoriesWidget", "Widget", "FullFeed")
      {
        Description = "This widget shows a listing of categories with a post count for each category.",
        SupportedScopes = SupportedScopes.All,
        OnGetConfigInclude = (s) =>
        {
          return new FeedConfigLinkInclude()
          {
            Controller = "Widget",
            Action = "FeedConfig",
            HasCount = false,
            HasTitle = false,
            IncludePath = s
          };
        },
        OnValidate = (i) => new IdInclude(i).Id != null,
        AreaHints = new[] { "sidetop", "sidemid", "sidebot" }
      });

      RegisterWidget(container, new CompositeWidget("BlogCategoryCloudWidget", "Widget", "FullFeed")
      {
        Description = "This widget shows a cloud of category links (larger font = more posts) for a collection.",
        SupportedScopes = SupportedScopes.All,
        OnGetConfigInclude = (s) =>
        {
          return new FeedConfigLinkInclude()
          {
            Controller = "Widget",
            Action = "FeedConfig",
            HasCount = false,
            HasTitle = false,
            IncludePath = s
          };
        },
        OnValidate = (i) => new IdInclude(i).Id != null,
        AreaHints = new[] { "sidetop", "sidemid", "sidebot" }
      });

      RegisterWidget<BlogCommentsWidget>(container);
      RegisterWidget<BlogEntrySimpleWidget>(container);

      RegisterWidget(container, new CompositeWidget("BlogRecentCommentsWidget", "Widget", "SizeAnnotations")
      {
        Description = "This widget shows a listing of the recent comments in a collection.",
        SupportedScopes = SupportedScopes.All,
        OnGetConfigInclude = (s) =>
        {
          return new FeedConfigLinkInclude()
            {
              Controller = "Widget",
              Action = "FeedConfig",
              HasCount = true,
              HasTitle = true,
              IncludePath = s
            };
        },
        OnValidate = (i) => new IdInclude(i).Id != null,
        AreaHints = new[] { "sidetop", "sidemid", "sidebot", "content" }
      });

      RegisterWidget(container, new CompositeWidget("BlogRecentWidget", "Widget", "SizeFeed")
      {
        Description = "This widget shows the recent posts to a collection.",
        SupportedScopes = SupportedScopes.All,
        OnGetConfigInclude = (s) =>
        {
          return new FeedConfigLinkInclude()
          {
            Controller = "Widget",
            Action = "FeedConfig",
            HasCount = true,
            HasTitle = true,
            IncludePath = s
          };
        },
        OnValidate = (i) => new IdInclude(i).Id != null,
        AreaHints = new[] { "sidetop", "sidemid", "sidebot" }
      });

      RegisterWidget<BlogSearchWidget>(container);

      RegisterWidget(container, new CompositeWidget("BlogSettingsWidget", "Blog", "BlogSettings")
      {
        Description = "This widget allows the user to change blog settings for each collection.",
        SupportedScopes = SupportedScopes.Collection,
        AreaHints = new[] { "settingsPane" }
      });
      RegisterWidget(container, new CompositeWidget("BlogSettingsEntireSiteWidget", "Blog", "BlogSettingsEntireSite")
      {
        Description = "This widget allows the user to change blog settings for each collection.",
        SupportedScopes = SupportedScopes.EntireSite,
        AreaHints = new[] { "settingsPane" }
      });

      RegisterWidget(container, new CompositeWidget("BlogSummaryWidget", "Widget", "SizeFeed")
      {
        Description = "This wiget shows the summary of multiple posts from a collection.",
        SupportedScopes = SupportedScopes.All,
        OnGetConfigInclude = (s) =>
        {
          return new FeedConfigLinkInclude()
          {
            Controller = "Widget",
            Action = "FeedConfig",
            HasCount = true,
            HasTitle = true,
            IncludePath = s
          };
        },
        OnValidate = (i) => new IdInclude(i).Id != null,
        AreaHints = new[] { "content" }
      });

      RegisterWidget(container, new CompositeWidget("BlogTrackbackWidget", "Trackback", "TrackbackWidget")
      {
        Description = "This widget adds trackback information to the html of a post.",
        SupportedScopes = SupportedScopes.Entry,
        AreaHints = new[] { "tail" }
      });
    }

    public override PluginState Setup(IContainer container, string appPath)
    {
      SetupBlogSettings(container);
      SetupBlogSettingsEntireSite(container);
      return base.Setup(container, appPath);
    }

    public override PluginState Upgrade(IContainer container, string appPath, System.Version previous, System.Version current)
    {
      if (previous == ServerApp.Version11) SetupBlogSettings(container);
      if (previous >= ServerApp.Version11 && previous < ServerApp.Version14) SetupBlogSettingsEntireSite(container);
      return base.Upgrade(container, appPath, previous, current);
    }

    void SetupBlogSettings(IContainer container)
    {
      SetupIncludeInPageArea(container, "AdminSettingsCollection", "settingsTabs",
        new LiteralInclude("<li><a href=\"#blogSettings\">Blog Settings</a></li>"));
      SetupIncludeInPageArea(container, "AdminSettingsCollection", "settingsPanes", "BlogSettingsWidget");
    }

    void SetupBlogSettingsEntireSite(IContainer container)
    {
      SetupIncludeInPageArea(container, "AdminSettingsEntireSite", "settingsTabs",
        new LiteralInclude("<li><a href=\"#blogSettings\">Blog Settings</a></li>"));
      SetupIncludeInPageArea(container, "AdminSettingsEntireSite", "settingsPanes", "BlogSettingsEntireSiteWidget");
    }
  }
}