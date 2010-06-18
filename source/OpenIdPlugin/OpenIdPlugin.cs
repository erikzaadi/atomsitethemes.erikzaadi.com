/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.OpenIdPlugin
{
  using System.Collections.Generic;
  using System.Web.Mvc;
  using System.Web.Routing;
  using AtomSite.Domain;
  using AtomSite.WebCore;
  using StructureMap;
  using System.Xml;
  using System.Xml.Linq;
  using System.Linq;
  using System.Diagnostics;
  using System.IO;
  using System;

  public class OpenIdPlugin : BasePlugin
  {
    public OpenIdPlugin(ILogService logger)
      : base(logger)
    {
      DefaultMerit = (int)MeritLevel.Default;
      CanUninstall = true;
    }

    public override PluginState Setup(IContainer container, string appPath)
    {
      base.SetupIncludeInPageArea(container, "AccountLogin", "content", "OpenIdLoginWidget");
      base.SetupIncludeInWidgetArea(container, "BlogAddCommentWidget", "commentator", "OpenIdModalWidget");

      //LogService.Info("Update web.config modules to add OpenIdAuthenticationModule.");
      //string path = System.IO.Path.Combine(appPath, "web.config");
      //XDocument doc = XDocument.Load(path);

      //if (doc.Descendants("add")
      //  .Where(x => (string)x.Attribute("name") == "OpenIdAuthenticationModule").Count() == 0)
      //{
      //  LogService.Info("OpenIdAuthenticationModule not already added, adding...");

      //  var modules = doc.Descendants("httpModules");
      //  modules.First().Elements().Last().AddAfterSelf(
      //    new XElement("add",
      //      new XAttribute("name", "OpenIdAuthenticationModule"),
      //      new XAttribute("type", "AtomSite.Plugins.OpenIdPlugin.OpenIdAuthenticationModule, AtomSite.Plugins.OpenIdPlugin")));

      //  modules = doc.Descendants("modules");
      //  modules.First().Elements().Last().AddAfterSelf(
      //    new XElement("add",
      //      new XAttribute("name", "OpenIdAuthenticationModule"),
      //      new XAttribute("type", "AtomSite.Plugins.OpenIdPlugin.OpenIdAuthenticationModule, AtomSite.Plugins.OpenIdPlugin")));

      //  LogService.Info("Saving web.config");
      //  //#if DEBUG
      //  //      if (DateTime.UtcNow.Subtract(File.GetLastWriteTimeUtc(path)) > new TimeSpan(0, 1, 0)) doc.Save(path);
      //  //#else
      //  doc.Save(path);
      //  //#endif
      //}
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
      if (previous < ServerApp.Version14)
      {
        try
        {
          LogService.Info("Update web.config modules to remove OpenIdAuthenticationModule.");
          string path = System.IO.Path.Combine(appPath, "web.config");
          XDocument doc = XDocument.Load(path);
          var adds = doc.Descendants("add")
            .Where(x => (string)x.Attribute("name") == "OpenIdAuthenticationModule");
          //Debug.Write(adds.Count());
          adds.Remove();
          LogService.Info("Saving web.config.");
          doc.Save(path);
        }
        catch (Exception ex)
        {
          LogService.Error(ex);
        }
      }

      Plugin plugin = GetEmbeddedPluginEntry();
      return base.Upgrade(container, appPath, previous, current);
    }

    public override void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets)
    {
      container.Configure(c => c.For<IAuthenticateService>().Add<OpenIdAuthenticateService>());

      RegisterController<OpenIdController>(container);
      RegisterWidget(container, new ViewWidget("OpenIdLoginWidget")
      {
        Description = "This widget adds a login form that users can login using OpenID.",
        Assets = new[] { "jquery.openid-1.1.js", "OpenId.css" }.Select(s => new Asset(s)),
        TailScript = @"$(function() { $('form.openid').openid(); });",
        SupportedScopes = SupportedScopes.All,
        AreaHints = new[] { "content" }
      });
      RegisterWidget<OpenIdModalWidget>(container);
    }
  }
}
