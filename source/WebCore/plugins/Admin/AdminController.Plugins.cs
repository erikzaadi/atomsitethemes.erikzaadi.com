/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.IO;
  using System.Threading;
  using System.Web;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using StructureMap;
  
  public partial class AdminController : BaseController
  {
    [ScopeAuthorize(EntireSite = true), AcceptVerbs(HttpVerbs.Get)]
    public ViewResult Plugins()
    {
      AdminPluginsModel m = TempData["model"] as AdminPluginsModel;
      if (m == null)
      {
        m = new AdminPluginsModel();
        m.Plugins = AdminService.GetPlugins();
        if (TempData["error"] != null) m.Errors.Add((string)TempData["error"]);
      }
      return View("AdminPlugins", "Admin", m);
    }
    
    [ScopeAuthorize(EntireSite = true)]
    public ActionResult Restart()
    {
      //restart app
      ServerApp.Restart();
      return new EmptyResult();
    }

    [ScopeAuthorize(EntireSite = true), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult SetPluginEnabled(string pluginType, bool enabled)
    {
      return PluginAction(() =>
      {
        AdminService.SetPluginEnabled(pluginType, enabled);
      }, pluginType);
    }

    protected ActionResult PluginAction(Action action, string type)
    {
      return PluginAction(action, type, string.Empty);
    }

    protected ActionResult PluginAction(Action action, string type, string message)
    {
      try
      {
        action();

        //return a refreshed view of plugins
        AdminPluginsModel m = new AdminPluginsModel();
        m.Plugins = AdminService.GetPlugins();
        m.ChangedType = type;
        m.MessageForType = message;
        return PartialView("AdminPluginListing", m);
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { success = false, error = ex.Message });
      }
    }

    [ScopeAuthorize(EntireSite = true), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult SetPluginMerit(string pluginType, string change)
    {
      return PluginAction(() =>
      {
        AdminService.SetPluginMerit(pluginType, change);
      }, pluginType, "<strong>Updated</strong>, please restart.");
    }

    [ScopeAuthorize(EntireSite = true), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult UploadPlugin()
    {
      try
      { 
        AdminPluginsModel m = new AdminPluginsModel();
        //TODO: do not directly access request object
        foreach (string file in Request.Files)
        {
          HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
          if (hpf.ContentLength == 0) throw new Exception("Empty file uploaded");

          LogService.Info("Plugin uploaded filename={0} contentType={1} contentLength={2}",
            hpf.FileName, hpf.ContentType, hpf.ContentLength);

          PluginState ps = AdminService.LoadAndInstallPlugin(hpf.InputStream, Path.GetFileName(hpf.FileName), hpf.ContentType,
            Server.MapPath("~/"));
          if (ps.Status == PluginStatus.Incompatible)
            m.Notifications.Add("Warning,", "the plugin was successfully uploaded but not installed because it " +
              "may not be compatible.  If you'd like to proceed, you can <strong>force</strong> the installation.");
          m.ChangedType = ps.Type;
        }
        m.Plugins = AdminService.GetPlugins();
        TempData["model"] = m;
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        TempData["error"] = ex.Message;
      }
      return RedirectToAction("Plugins");
    }

    [ScopeAuthorize(EntireSite = true), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult ControlPlugin(string pluginType, string change)
    {
      switch (change)
      {
        case "enable": return SetPluginEnabled(pluginType, true);
        case "disable": return SetPluginEnabled(pluginType, false);
        case "install": return InstallPlugin(pluginType);
        case "uninstall": return UninstallPlugin(pluginType);
        case "setup": return SetupPlugin(pluginType);
        default: return PluginAction(() => { throw new Exception("Unknown plugin control requested."); }, pluginType);
      }
    }

    [ScopeAuthorize(EntireSite = true), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult SetupPlugin(string pluginType)
    {
      return PluginAction(() =>
      {
        AdminService.SetupPlugin(pluginType, Server.MapPath("~/"), (IContainer)HttpContext.Application["container"]);
      }, pluginType, "<strong>Setup</strong>, please restart.");
    }

    [ScopeAuthorize(EntireSite = true), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult InstallPlugin(string pluginType)
    {
      return PluginAction(() =>
      {
        AdminService.InstallPlugin(pluginType, Server.MapPath("~/"), (IContainer)HttpContext.Application["container"]);
      }, pluginType, "<strong>Installed</strong> from zip, please restart.");
    }

    [ScopeAuthorize(EntireSite = true), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult UninstallPlugin(string pluginType)
    {
      return PluginAction(() =>
      {
        AdminService.UninstallPlugin(pluginType, Server.MapPath("~/"), (IContainer)HttpContext.Application["container"]);
      }, pluginType, "<strong>Uninstalled</strong>, please restart.");
    }

    [ScopeAuthorize(EntireSite = true), AcceptVerbs(HttpVerbs.Get)]
    public ViewResult Plugin(string type)
    {
      var m = AdminService.GetPlugin(type, (IContainer)HttpContext.Application["container"], Server.MapPath("~/"));
      return View("AdminPlugin", "Admin", new AdminPluginModel() { Plugin = m });
    }

  }
}
