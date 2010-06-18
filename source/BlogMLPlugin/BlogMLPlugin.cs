/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.BlogMLPlugin
{
  using System.Collections.Generic;
  using System.Web.Mvc;
  using System.Web.Routing;
  using AtomSite.Domain;
  using AtomSite.WebCore;
  using StructureMap;

  public class BlogMLPlugin : BasePlugin
  {
    public BlogMLPlugin(ILogService logger)
      : base(logger)
    {
      DefaultMerit = (int)MeritLevel.Default;
      CanUninstall = true;
      DefaultAssetGroup = "admin";
    }

    public override PluginState Setup(IContainer container, string appPath)
    {
      base.SetupIncludeInPageArea(container, "WizardSetupChoice", "setupOptions", "BlogMLWizardChoiceWidget");
      base.SetupIncludeInPageArea(container, "AdminTools", "toolsImport", "BlogMLImportToolWidget");
      base.SetupIncludeInPageArea(container, "AdminTools", "toolsExport", "BlogMLExportToolWidget");
      return base.Setup(container, appPath);
    }

    public override PluginState Uninstall(IContainer container, string appPath)
    {
      Plugin plugin = GetEmbeddedPluginEntry();
      UninstallPluginFiles(container, plugin, appPath);
      return base.Uninstall(container, appPath);
    }

    public override PluginState Upgrade(IContainer container, string appPath, System.Version previous, System.Version current)
    {
      Plugin plugin = GetEmbeddedPluginEntry();
      return base.Upgrade(container, appPath, previous, current);
    }

    public override void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets)
    {
      container.Configure(a => a.For<IBlogMLService>().Singleton().Add<BlogMLService>());

      RegisterController<BlogMLController>(container);

      RegisterPage(container, new Page("BlogMLWizardImport", "Wizard")
      {
        Assets = new[] { new Asset("jquery.multifile-1.46.js") },
        SupportedScopes = SupportedScopes.EntireSite
      });

      RegisterWidget(container, new CompositeWidget("BlogMLImportToolWidget", "BlogML", "ImportToolWidget")
      {
        Description = "Widget adds a tool you can use to import data from a BlogML file.",
        Assets = new[] { new Asset("jquery.multifile-1.46.js") },
        TailScript = "$(function() {$('input[name=blogml]').MultiFile({ max: 1, accept: 'xml' });});",
        SupportedScopes = SupportedScopes.EntireSite,
        AreaHints = new[] { "toolsImport" }
      });
      RegisterWidget(container, new CompositeWidget("BlogMLExportToolWidget", "BlogML", "ExportToolWidget")
      {
        Description = "Widget adds a tool you can use to export your data to a BlogML file.",
        SupportedScopes = SupportedScopes.EntireSite,
        AreaHints = new[] { "toolsExport" }
      });
      RegisterWidget(container, new ViewWidget("BlogMLWizardChoiceWidget")
      {
        Description = "Widget adds a new BlogML import choice during the wizard setup.",
        SupportedScopes = SupportedScopes.EntireSite,
        AreaHints = new[] { "setupOptions" }
      });

      routes.Insert(0, new SiteRoute()
      {
        Name = "BlogMLImport",
        Route = new Route("BlogML/WizardImport",
          new RouteValueDictionary(new { controller = "BlogML", action = "WizardImport" }), new MvcRouteHandler()),
        Merit = (int)MeritLevel.Max
      });
    }
  }
}
