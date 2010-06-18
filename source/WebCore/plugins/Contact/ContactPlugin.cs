/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Web.Mvc;
  using System.Web.Routing;
  using StructureMap;
  using System.Collections.Generic;
  using AtomSite.Domain;

  public class ContactPlugin : BasePlugin
  {
    public ContactPlugin(ILogService logger) : base(logger) { }

    public override void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets)
    {
      RegisterController<ContactController>(container);
      RegisterWidget(container, new CompositeWidget("ContactWidget", "Contact", "ContactWidget")
      {
          SupportedScopes = Domain.SupportedScopes.All,
          Description = "This widget adds a contact form that user can send an email to you.",
          Assets = new Asset[] { new Asset("Contact.css"), new Asset("Contact.js") },
          AreaHints = new[] { "content" }
      });

      RegisterWidget(container, new CompositeWidget("ContactSettingsWidget", "Contact", "ContactSettings")
      {
          SupportedScopes = Domain.SupportedScopes.Workspace,
          Description = "This widget allows you to modify the contact form settings for any contact forms placed within a workspace.",
          AreaHints = new[] { "settingsPane" }
      });
    }

    public override PluginState Setup(IContainer container, string appPath)
    {
        SetupContactSettings(container);
        return base.Setup(container, appPath);
    }

    public override PluginState Upgrade(IContainer container, string appPath, System.Version previous, System.Version current)
    {
        if (previous <= ServerApp.Version131) SetupContactSettings(container);
        return base.Upgrade(container, appPath, previous, current);
    }

    void SetupContactSettings(IContainer container)
    {
        SetupIncludeInPageArea(container, "AdminSettingsWorkspace", "settingsTabs",
          new LiteralInclude("<li><a href=\"#contactSettings\">Contact Settings</a></li>"));
        SetupIncludeInPageArea(container, "AdminSettingsWorkspace", "settingsPanes", "ContactSettingsWidget");
    }
  }
}
