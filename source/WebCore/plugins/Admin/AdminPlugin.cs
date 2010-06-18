/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using StructureMap;
  using StructureMap.Attributes;
  using System.Linq;

  public class AdminPlugin : BasePlugin
  {
    public static readonly string AdminAssetGroup = "admin";

    public AdminPlugin(ILogService logger)
      : base(logger)
    {
      DefaultMerit = (int)MeritLevel.High + 30;
      DefaultAssetGroup = AdminAssetGroup;
    }

    public override void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets)
    {
      container.Configure(a =>
      {
        a.For<AdminService>().Singleton().Add<AdminService>();
        a.For<IRouteService>().HybridHttpOrThreadLocalScoped().Add<AdminRouteService>();

        //TODO: not sure why the above concrete type becomes the default so adding this line
        //a.For<IRouteService>().CacheBy(InstanceScope.HttpContext)
        //  .TheDefault.Is.OfConcreteType<RouteService>();
      });

      RegisterController<AdminController>(container);

      RegisterRoutes<AdminRouteRegistrar>(container, routes, typeof(AdminRouteService).AssemblyQualifiedName);

      //framework assets loaded in master page
      //RegisterFrameworkAsset(globalAssets, "jquery.js");
      //RegisterFrameworkAsset(globalAssets, "reset-fonts-grids-2.7.0.css");

      //master page
      RegisterPage(container, new Page("Admin")
      {
        Areas = new[] { "head", "content", "menu", "tail" },
        Assets = AsAdminAssets(new[] { "jquery.timeago-0.8.2.js", "jquery.single-ddm-1.2.js", "jquery.tools-1.1.2.js", "Admin.js", "Admin.css" }),
        SupportedScopes = SupportedScopes.EntireSite | SupportedScopes.Workspace | SupportedScopes.Collection | SupportedScopes.Entry
      });

      RegisterPage(container, new Page("AdminAnnotations", "Admin", AsAdminAssets(new[] { "AdminListings.css" }))
      {
        SupportedScopes = SupportedScopes.EntireSite | SupportedScopes.Workspace | SupportedScopes.Collection
      });

      RegisterPage(container, new Page("AdminDashboard", "Admin")
      {
        Areas = new[] { "dashboardLeft", "dashboardRight" },
        SupportedScopes = SupportedScopes.EntireSite | SupportedScopes.Workspace | SupportedScopes.Collection
      });

      RegisterPage(container, new Page("AdminEditEntry", "Admin", AsAdminAssets(new[] { "AdminEdit.css", "jquery.watermark.js", "AdminEdit.js" }))
      {
        Areas = new[] { "editEntryContent", "editEntryLeft", "editEntryRight" },
        SupportedScopes = SupportedScopes.Entry
      });

      RegisterPage(container, new Page("AdminEditMedia", "Admin", AsAdminAssets(new[] { "AdminEdit.css", "jquery.watermark.js", "jquery.multifile-1.46.js", "AdminEdit.js" }))
      {
        Areas = new[] { "editEntryLeft", "editEntryRight" },
        SupportedScopes = SupportedScopes.Entry
      });

      RegisterPage(container, new Page("AdminEntries", "Admin", AsAdminAssets(new[] { "AdminListings.css" }))
      {
        SupportedScopes = SupportedScopes.EntireSite | SupportedScopes.Workspace | SupportedScopes.Collection
      });

      RegisterPage(container, new Page("AdminSettingsCollection", "Admin", AsAdminAssets(new[] { "AdminSettings.css" }))
      {
        Areas = new[] { "settingsTabs", "settingsPanes", "settingsLeft", "settingsMiddle", "settingsRight" },
        SupportedScopes = SupportedScopes.Collection
      });

      RegisterPage(container, new Page("AdminSettingsEntireSite", "Admin", AsAdminAssets(new[] { "AdminSettings.css" }))
      {
        Areas = new[] { "settingsTabs", "settingsPanes", "settingsLeft", "settingsRight" },
        SupportedScopes = SupportedScopes.EntireSite
      });

      RegisterPage(container, new Page("AdminSettingsWorkspace", "Admin", AsAdminAssets(new[] { "AdminSettings.css" }))
      {
        Areas = new[] { "settingsTabs", "settingsPanes", "settingsLeft", "settingsRight" },
        SupportedScopes = SupportedScopes.Workspace
      });

      RegisterPage(container, new Page("AdminTheme", "Admin", AsAdminAssets(new[] { "AdminTheme.css", "jquery.rater-1.1.js", "AdminTheme.js", "RaterWidget.css" }))
      {
        SupportedScopes = SupportedScopes.EntireSite | SupportedScopes.Workspace | SupportedScopes.Collection
      });

      RegisterPage(container, new Page("AdminThemeUpload", "Admin", AsAdminAssets(new[] { "jquery.multifile-1.46.js" }))
      {
        SupportedScopes = SupportedScopes.EntireSite
      });

      RegisterPage(container, new Page("AdminWidgets", "Admin", AsAdminAssets(new[] { "AdminWidgets.css", "AdminWidgets.js" }))
      {
        SupportedScopes = SupportedScopes.EntireSite | SupportedScopes.Workspace | SupportedScopes.Collection
      });

      RegisterPage(container, new Page("AdminPlugins", "Admin", AsAdminAssets(new[] { "jquery.multifile-1.46.js", "AdminPlugins.js", "AdminPluginListing.css" }))
      {
        SupportedScopes = SupportedScopes.EntireSite
      });

      RegisterPage(container, new Page("AdminPlugin", "Admin"));

      RegisterPage(container, new Page("AdminProgress", "Admin"));

      RegisterPage(container, new Page("AdminRoleMatrix", "Admin"));

      RegisterPage(container, new Page("AdminTools", "Admin")
      {
        Areas = new[] { "toolsTabs", "toolsPanes", "toolsImport", "toolsExport" },
        SupportedScopes = SupportedScopes.EntireSite
      });

      RegisterPage(container, new Page("AdminUser", "Admin")
      {
        Areas = new[] { "userTabs", "userPanes" },
        SupportedScopes = SupportedScopes.EntireSite
      });

      RegisterPage(container, new Page("AdminUsers", "Admin", AsAdminAssets(new[] { "AdminUsers.js" }))
      {
        SupportedScopes = SupportedScopes.EntireSite
      });

      RegisterWidget(container, new CompositeWidget("AdminAcceptsWidget", "Admin", "Accepts")
      {
        Description = "This widget allows you to change what types of information a collection accepts.",
        Assets = AsAdminAssets(new[] { "AdminAcceptsWidget.css", "AdminAcceptsWidget.js" }),
        SupportedScopes = SupportedScopes.Collection,
        AreaHints = new[] { "settingsLeft" }
      });

      RegisterWidget(container, new ViewWidget("AdminAddCategoryWidget")
      {
        Description = "This widget allows you to add a catergory to an entry.",
        SupportedScopes = SupportedScopes.Entry
      });

      RegisterWidget(container, new CompositeWidget("AdminCategoriesWidget", "Admin", "Categories")
      {
        Description = "This widget shows the categories for a collection and allows you to remove a category.",
        Assets = AsAdminAssets(new[] { "AdminCategoriesWidget.css", "AdminCategoriesWidget.js" }),
        SupportedScopes = SupportedScopes.Collection,
        AreaHints = new[] { "settingsMiddle" }
      });

      RegisterWidget(container, new ViewWidget("AdminCKEditorWidget")
      {
        Description = "This widget allows you to edit an entry with a WYSIWYG editor.",
        Assets = new[] { new Asset("ckeditor/ckeditor.js", Asset.NoGroup, AssetScope.Widget) },
        SupportedScopes = SupportedScopes.Entry,
        AreaHints = new[] { "editEntryContent" },
        TailScript = @"CKEDITOR.replace('content', {
    toolbar: [
    ['Source'],
    ['Undo', 'Redo'],
    ['Bold', 'Italic', 'Underline', 'Strike', '-', 'Subscript', 'Superscript'],
    ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', 'Blockquote'],
    ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
    ['Link', 'Unlink'],
    '/',
    ['Styles', 'Format', 'Font', 'FontSize'],
    ['TextColor', 'BGColor'],
    ['Image', 'Flash', 'Table', 'HorizontalRule', 'RemoveFormat'],
    ['Maximize', 'ShowBlocks']
  ]
  });"
      });

      RegisterWidget(container, new CompositeWidget("AdminCollectionsWidget", "Admin", "CollectionsWidget")
      {
        Description = "The widget shows the collections in a workspace.",
        Assets = new[] { new Asset("AdminDefaultsToggle.js", AdminAssetGroup) },
        SupportedScopes = SupportedScopes.Workspace,
        AreaHints = new[] { "settingsLeft", "settingsRight" }
      });

      RegisterWidget(container, new CompositeWidget("AdminPendingEntriesWidget", "Admin", "PendingEntries")
      {
        Description = "This widget shows the pending and draft entries.",
        Assets = new[] { new Asset("AdminItems.css", AdminAssetGroup) },
        SupportedScopes = SupportedScopes.EntireSite | SupportedScopes.Workspace | SupportedScopes.Collection,
        AreaHints = new[] { "dashboardLeft", "dashboardRight" }
      });

      RegisterWidget(container, new ViewWidget("AdminQuickPubWidget")
      {
        Description = "This widget allows you to quickly pubish or draft new entries.",
        Assets = AsAdminAssets(new[] { "AdminQuickPubWidget.js", "AdminQuickPubWidget.css" }),
        SupportedScopes = SupportedScopes.EntireSite | SupportedScopes.Workspace | SupportedScopes.Collection,
        AreaHints = new[] { "dashboardLeft", "dashboardRight" }
      });

      RegisterWidget(container, new CompositeWidget("AdminRecentAnnotationsWidget", "Admin", "RecentAnnotations")
      {
        Description = "This widget shows a listing of the recent annotations (comments).",
        Assets = AsAdminAssets(new[] { "AdminItems.css" }),
        SupportedScopes = SupportedScopes.EntireSite | SupportedScopes.Workspace | SupportedScopes.Collection,
        AreaHints = new[] { "dashboardLeft", "dashboardRight" }
      });

      RegisterWidget(container, new CompositeWidget("AdminRightNowWidget", "Admin", "RightNow")
      {
        Description = "This widget shows a snapshot of all the counts in your collections to give you an overall view.",
        Assets = AsAdminAssets(new[] { "AdminListings.css", "AdminRightNowWidget.css" }),
        SupportedScopes = SupportedScopes.EntireSite | SupportedScopes.Workspace | SupportedScopes.Collection,
        AreaHints = new[] { "dashboardLeft", "dashboardRight" }
      });

      //RegisterWidget(container, new ViewWidget("AdminTinyMceWidget"); //TODO: register assets

      RegisterWidget(container, new CompositeWidget("AdminAdministratorsWidget", "Admin", "Administrators")
      {
        Description = "This widget allows you to add or remove new admins for the entire site.",
        Assets = AsAdminAssets(new[] { "AdminUsers.js" }),
        SupportedScopes = SupportedScopes.EntireSite,
        AreaHints = new[] { "settingsLeft", "settingsRight" }
      });

      RegisterWidget(container, new CompositeWidget("AdminPeopleWidget", "Admin", "People")
      {
        Description = "This widget allows you to add or remove new authors and contributors to workspaces or collections.",
        Assets = AsAdminAssets(new[] { "AdminUsers.js" }),
        SupportedScopes = SupportedScopes.Workspace | SupportedScopes.Collection,
        AreaHints = new[] { "settingsRight" }
      });

      RegisterWidget(container, new ViewWidget("AdminWorkspacesWidget")
      {
        Description = "The widget shows the workspaces for the entire site.",
        Assets = new[] { new Asset("AdminDefaultsToggle.js", AdminAssetGroup) },
        SupportedScopes = SupportedScopes.EntireSite,
        AreaHints = new[] { "settingsLeft", "settingsRight" }
      });
    }

    IEnumerable<Asset> AsAdminAssets(IEnumerable<string> assets)
    {
      return assets.Select(a => new Asset(a, this.DefaultAssetGroup));
    }

    public override PluginState Setup(IContainer container, string appPath)
    {
      SetupDashboard(container);
      SetupSettings(container);
      return base.Setup(container, appPath);
    }

    public override PluginState Upgrade(IContainer container, string appPath, Version previous, Version current)
    {
      if (previous == ServerApp.Version10) SetupDashboard(container);
      if (previous == ServerApp.Version11) SetupSettings(container);

      return base.Upgrade(container, appPath, previous, current);
    }

    void SetupDashboard(IContainer container)
    {
      SetupIncludeInPageArea(container, "AdminDashboard", "dashboardLeft", "AdminRightNowWidget");
      SetupIncludeInPageArea(container, "AdminDashboard", "dashboardLeft", "AdminRecentAnnotationsWidget");
      SetupIncludeInPageArea(container, "AdminDashboard", "dashboardRight", "AdminQuickPubWidget");
      SetupIncludeInPageArea(container, "AdminDashboard", "dashboardRight", "AdminPendingEntriesWidget");
    }

    void SetupSettings(IContainer container)
    {
      SetupIncludeInPageArea(container, "AdminSettingsEntireSite", "settingsLeft", "AdminWorkspacesWidget");
      SetupIncludeInPageArea(container, "AdminSettingsEntireSite", "settingsRight", "AdminAdministratorsWidget");
      SetupIncludeInPageArea(container, "AdminSettingsWorkspace", "settingsLeft", "AdminCollectionsWidget");
      SetupIncludeInPageArea(container, "AdminSettingsWorkspace", "settingsRight", "AdminPeopleWidget");
      SetupIncludeInPageArea(container, "AdminSettingsCollection", "settingsLeft", "AdminAcceptsWidget");
      SetupIncludeInPageArea(container, "AdminSettingsCollection", "settingsMiddle", "AdminCategoriesWidget");
      SetupIncludeInPageArea(container, "AdminSettingsCollection", "settingsRight", "AdminPeopleWidget");
      //TODO: should be part of blog plugin
      SetupIncludeInPageArea(container, "AdminSettingsCollection", "settingsTabs", new LiteralInclude("<li><a href=\"#blogSettings\">Blog Settings</a></li>"));
      SetupIncludeInPageArea(container, "AdminSettingsCollection", "settingsPanes", "BlogSettingsWidget");
      SetupIncludeInPageArea(container, "AdminEditEntry", "editEntryContent", "AdminCKEditorWidget");
    }
  }
}
