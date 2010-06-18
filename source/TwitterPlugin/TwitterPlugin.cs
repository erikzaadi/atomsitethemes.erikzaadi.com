/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.TwitterPlugin
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using AtomSite.WebCore;
  using StructureMap;

  public class TwitterPlugin : BasePlugin
  {
    public TwitterPlugin(ILogService logger)
      : base(logger)
    {
      DefaultMerit = (int)MeritLevel.Default;
      CanUninstall = true;
    }

    public override void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets)
    {
      RegisterWidget(container, new CompositeWidget("TwitterWidget", "Twitter", "Widget")
      {
        Description = "This widget displays tweets from a public twitter feed.",
        Assets = new string[] { "TwitterPlugin.css" }.Select(a => new Asset(a)),
        SupportedScopes = SupportedScopes.All,
        OnGetConfigInclude = (p) => new ConfigLinkInclude(p, "Twitter", "Config"),
        OnValidate = (i) =>
        {
          var ti = new TwitterInclude(i);
          return !string.IsNullOrEmpty(ti.Username) && ti.Count > 0;
        },
        AreaHints = new[] { "sidetop", "sidemid", "sidebot" }
      });
      RegisterController<TwitterController>(container);
    }

    public override PluginState Setup(IContainer container, string appPath)
    {
      LogService.Info("Setting up Twitter Plugin");

      //base.SetupIncludeInPageArea(container, "BlogHome", "sidemid", new TwitterInclude("jarrettv", 5));
      //base.SetupIncludeInPageArea(container, "AdminSettingsEntireSite", "settingsTabs", new LiteralInclude("<li><a href=\"#twitterSettings\">Twitter Settings</a></li>"));
      //base.SetupIncludeInPageArea(container, "AdminSettingsEntireSite", "settingsPanes", "TwitterSettingsWidget");

      LogService.Info("Finished Setting up Twitter Plugin");

      return base.Setup(container, appPath);
    }

    public override PluginState Uninstall(IContainer container, string appPath)
    {
      Plugin plugin = GetEmbeddedPluginEntry();
      //base.UninstallIncludeFromPageArea(container, "AdminSettingsEntireSite", "settingsTabs",
      //    (i) => i.Name == "LiteralWidget" && i.Xml.Value.Contains("#twitterSettings"));
      //base.UninstallIncludeFromPageArea(container, "AdminSettingsEntireSite", "settingsPanes",
      //    (i) => i.Name == "TwitterSettingsWidget");
      base.UninstallInclude(container, (i) => i.Name == "TwitterWidget");
      UninstallPluginFiles(container, plugin, appPath);
      return base.Uninstall(container, appPath);
    }
  }
}
