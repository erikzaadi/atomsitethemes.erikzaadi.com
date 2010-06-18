/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using AtomSite.WebCore.Properties;
  using StructureMap;

  public class AdminService
  {
    protected ILogService LogService;
    protected IAtomPubService AtomPubService;
    protected IAnnotateService AnnotateService;
    protected IAuthorizeService AuthorizeService;
    protected IRouteService RouteService;
    protected IThemeService ThemeService;
    protected IAppServiceRepository AppServiceRepository;

    public AdminService(IAtomPubService atompub, IAnnotateService anno, IAuthorizeService auth,
      ILogService logger, IRouteService route, IThemeService themeSvc, IAppServiceRepository svcRepo)
    {
      AtomPubService = atompub;
      AnnotateService = anno;
      AuthorizeService = auth;
      RouteService = route;
      LogService = logger;
      ThemeService = themeSvc;
      AppServiceRepository = svcRepo;
      atompub.SettingEntryLinks += (e) => SetLinks(e);
    }

    protected User GetUser()
    {
      return Thread.CurrentPrincipal.Identity as User;
    }

    private void SetLinks(AtomEntry e)
    {
      var list = e.Links.ToList();

      //if (e.Approved) list.Merge(new AtomLink() { Rel = "admin-approve", Href = RouteService.RouteUrl("AdminRoute", new { controller = "Admin", action = "EditEntry" }) });
      //else list.Merge(new AtomLink() { Rel = "admin-approve", Href = RouteService.RouteUrl("AdminRoute", new { controller = "Admin", action = "EditEntry" }) });

      if (AuthorizeService.IsAuthorized(GetUser(), e.Id.ToScope(), AuthAction.DeleteEntryOrMedia))
      {
        list.Merge(new AtomLink() { Rel = "delete", Href = RouteService.RouteUrl("AtomPubEntryEdit", e.Id, AbsoluteMode.Force) });
      }

      list.Merge(new AtomLink() { Rel = "admin-edit", Href = RouteService.RouteUrl(e.Media?"AdminEditMedia":"AdminEditEntry", new { id = e.Id.ToString() }, AbsoluteMode.Force) });

      e.Links = list;
    }

    #region Plugins

    //TODO: move all this to Plugin Engine


    public IEnumerable<PluginState> GetPlugins()
    {
      AppService appSvc = AtomPubService.GetService();
      return appSvc.Plugins.OrderByDescending(p => p.Merit);
    }

    public Plugin GetPlugin(string fullNameOfType, IContainer container, string appPath)
    {
      AppService appSvc = AtomPubService.GetService();
      string type = appSvc.Plugins.Where(p => p.Type.StartsWith(fullNameOfType)).Single().Type;
      IPlugin plugin = (IPlugin)container.GetInstance(Type.GetType(type));
      return plugin.GetPluginEntry(container, appPath);
    }

    public PluginState LoadAndInstallPlugin(Stream plugStream, string filename, string contentType, string appPath)
    {
      //TODO: auth

      if (!(contentType == "application/zip" || contentType == "application/x-zip-compressed" || contentType == "application/octet-stream"))
        throw new InvalidContentTypeException(contentType);

      string zipPath = Path.Combine(Path.Combine(appPath, Settings.Default.PluginsDir), filename);
      LogService.Info("Loading plugin into {0}", zipPath);
      //copy to bin directory
      using (FileStream fs = File.OpenWrite(zipPath))
      {
        byte[] bytes = new byte[plugStream.Length];
        int length = plugStream.Read(bytes, 0, (int)plugStream.Length);
        fs.Write(bytes, 0, length);
      }

      LogService.Info("Trying to install plugin...");
      PluginState ps = PluginEngine.InstallFromZip(zipPath, appPath, false);
      LogService.Info("Plugin installation for {0} resulted in plugin status of {1}", ps.Type, ps.Status);
      RegisterPlugin(ps);
      return ps;
    }

    public void InstallPlugin(string pluginType, string appPath, IContainer container)
    {
      //TODO: auth
      LogService.Info("Installing plugin {0}", pluginType);
      PluginState ps = PluginEngine.InstallFromZip(pluginType, appPath);
      RegisterPlugin(pluginType, ps);
    }

    protected void RegisterPlugin(string oldType, PluginState pluginEntry)
    {
      IList<PluginState> plugins = AtomPubService.GetService().Plugins.ToList();

      LogService.Info("Registering plugin {0}", pluginEntry.Type);
      var existing = plugins.Where(p => p.Type == oldType).SingleOrDefault();
      if (existing != null) plugins[plugins.IndexOf(existing)] = pluginEntry;
      else plugins.Add(pluginEntry);
      AppService svc = AtomPubService.GetService();
      svc.Plugins = plugins;
      AtomPubService.UpdateService(svc);
    }

    protected void RegisterPlugin(PluginState pluginEntry)
    {
      RegisterPlugin(pluginEntry.Type, pluginEntry);
    }

    public void SetupPlugin(string pluginType, string appPath, IContainer container)
    {
      //TODO: auth
      IList<PluginState> plugins = AtomPubService.GetService().Plugins.ToList();
      PluginState pluginEntry = plugins.Where(p => p.Type == pluginType).Single();
      LogService.Info("Finding plugin {0}", pluginType);
      Type type = Type.GetType(pluginType);
      IPlugin plugin = (IPlugin)container.GetInstance(type);

      LogService.Info("Setting up plugin {0}", pluginType);
      pluginEntry = plugin.Setup(container, appPath);

      //update registry
      RegisterPlugin(pluginEntry);
    }

    public void UninstallPlugin(string pluginType, string appPath, IContainer container)
    {
      //TODO: auth
      IList<PluginState> plugins = AtomPubService.GetService().Plugins.ToList();
      PluginState pluginEntry = plugins.Where(p => p.Type == pluginType).Single();
      Type type = Type.GetType(pluginType);
      IPlugin plugin = (IPlugin)container.GetInstance(type);
      LogService.Info("Uninstalling plugin");
      pluginEntry = plugin.Uninstall(container, appPath);
      //update registry
      RegisterPlugin(pluginEntry);
    }

    public void SetPluginEnabled(string type, bool enabled)
    {
      //TODO: authorization
      AppService appSvc = AtomPubService.GetService();
      PluginState plugin = appSvc.Plugins.Where(p => p.Type == type).Single();
      plugin.Status = enabled ? PluginStatus.Enabled : PluginStatus.Disabled;
      AtomPubService.UpdateService(appSvc);
    }

    public int SetPluginMerit(string type, string change)
    {
      AppService appSvc = AtomPubService.GetService();
      PluginState plugin = appSvc.Plugins.Where(p => p.Type == type).Single();
      switch (change)
      {
        case "next":
          if (Merit.HasNext(plugin.Merit)) plugin.Merit = (int)Merit.GetNext(plugin.Merit);
          else throw new Exception("There is no next merit.");
          break;
        case "prev":
          if (Merit.HasPrev(plugin.Merit)) plugin.Merit = (int)Merit.GetPrev(plugin.Merit);
          else throw new Exception("There is no previous merit.");
          break;
        case "add":
          if (plugin.Merit != int.MaxValue) plugin.Merit = plugin.Merit+1;
          else throw new Exception("The merit is already at max value.");
          break;
        case "subtract":
          if (plugin.Merit != int.MinValue) plugin.Merit = plugin.Merit - 1;
          else throw new Exception("The merit is already at min value.");
          break;
        default:
          throw new BaseException("The merit change value of '{0}' is not recognized.", change);
      }
      AtomPubService.UpdateService(appSvc);
      return plugin.Merit;
    }

    #endregion Plugins

    public AppService AddTarget(Scope scope, string targetName, bool isPage)
    {
      if (!AuthorizeService.IsAuthorized(GetUser(), scope, AuthAction.UpdateServiceDoc))
        throw new UserNotAuthorizedException(GetUser().Name, "UpdateServiceDoc");

      LogService.Info("Adding the {0} named {1} to scope {2}.", isPage ? "page" : "widget", targetName, scope);

      AppService appSvc = AppServiceRepository.GetService();
      if (isPage)
      {
        appSvc.AddPage(scope, targetName);
      }
      else
      {
        appSvc.AddWidget(scope, targetName);
      }

      AppServiceRepository.UpdateService(appSvc);
      return appSvc;
    }

    public AppService RemoveTarget(Scope scope, string targetName, bool isPage)
    {
      if (!AuthorizeService.IsAuthorized(GetUser(), scope, AuthAction.UpdateServiceDoc))
        throw new UserNotAuthorizedException(GetUser().Name, "UpdateServiceDoc");

      LogService.Info("Removing the {0} named {1} from scope {2}.", isPage ? "page" : "widget", targetName, scope);

      AppService appSvc = AppServiceRepository.GetService();
      if (isPage)
      {
        appSvc.RemovePage(scope, targetName);
      }
      else
      {
        appSvc.RemoveWidget(scope, targetName);
      }

      AppServiceRepository.UpdateService(appSvc);
      return appSvc;
    }

    public AppService AddArea(Scope scope, string targetName, bool isPage, string areaName)
    {
      if (!AuthorizeService.IsAuthorized(GetUser(), scope, AuthAction.UpdateServiceDoc))
        throw new UserNotAuthorizedException(GetUser().Name, "UpdateServiceDoc");

      LogService.Info("Adding the area {0} to the {1} named {2} to scope {3}.", areaName, isPage ? "page" : "widget", targetName, scope);

      AppService appSvc = AppServiceRepository.GetService();
      if (isPage)
      {
        appSvc.AddArea<ServicePage>(scope, targetName, areaName);
      }
      else
      {
        appSvc.AddArea<ServiceWidget>(scope, targetName, areaName);
      }

      AppServiceRepository.UpdateService(appSvc);
      return appSvc;
    }

    public AppService RemoveArea(Scope scope, string targetName, bool isPage, string areaName)
    {
      if (!AuthorizeService.IsAuthorized(GetUser(), scope, AuthAction.UpdateServiceDoc))
        throw new UserNotAuthorizedException(GetUser().Name, "UpdateServiceDoc");

      LogService.Info("Removing the area {0} from the {1} named {2} from scope {3}.", areaName, isPage ? "page" : "widget", targetName, scope);

      AppService appSvc = AppServiceRepository.GetService();
      if (isPage)
      {
        appSvc.RemoveArea<ServicePage>(scope, targetName, areaName);
      }
      else
      {
        appSvc.RemoveArea<ServiceWidget>(scope, targetName, areaName);
      }

      AppServiceRepository.UpdateService(appSvc);
      return appSvc;
    }

    public AppService AddInclude(Scope scope, string targetName, bool isPage, string areaName, string includeName)
    {
      if (!AuthorizeService.IsAuthorized(GetUser(), scope, AuthAction.UpdateServiceDoc))
        throw new UserNotAuthorizedException(GetUser().Name, "UpdateServiceDoc");

      LogService.Info("Including {0} into area {1} of {2} named {3} in scope {4}.", includeName, areaName, isPage ? "page" : "widget", targetName, scope);
      AppService appSvc = AppServiceRepository.GetService();
      if (isPage)
      {
        appSvc.AddInclude<ServicePage>(scope, targetName, areaName, includeName);
      }
      else
      {
        appSvc.AddInclude<ServiceWidget>(scope, targetName, areaName, includeName);
      }

      AppServiceRepository.UpdateService(appSvc);
      return appSvc;
    }

    public AppService RemoveInclude(Scope scope, string targetName, bool isPage, string areaName, int includeIdx)
    {
      if (!AuthorizeService.IsAuthorized(GetUser(), scope, AuthAction.UpdateServiceDoc))
        throw new UserNotAuthorizedException(GetUser().Name, "UpdateServiceDoc");

      LogService.Info("Removing include at index {0} from area {1} of {2} named {3} in scope {4}.", includeIdx, areaName, isPage ? "page" : "widget", targetName, scope);

      AppService appSvc = AppServiceRepository.GetService();
      if (isPage)
      {
        appSvc.RemoveInclude<ServicePage>(scope, targetName, areaName, includeIdx);
      }
      else
      {
        appSvc.RemoveInclude<ServiceWidget>(scope, targetName, areaName, includeIdx);
      }

      AppServiceRepository.UpdateService(appSvc);
      return appSvc;
    }

    public AppService MoveInclude(Scope scope, string targetName, bool isPage, string areaName, int includeIdx, bool down)
    {
      if (!AuthorizeService.IsAuthorized(GetUser(), scope, AuthAction.UpdateServiceDoc))
        throw new UserNotAuthorizedException(GetUser().Name, "UpdateServiceDoc");

      LogService.Info("Moving {5} include at index {0} in area {1} of {2} named {3} in scope {4}.", includeIdx, areaName,
        isPage ? "page" : "widget", targetName, scope, down ? "down" : "up");

      AppService appSvc = AppServiceRepository.GetService();
      ServiceArea area = appSvc.GetArea(scope, targetName, isPage, areaName);
      var includes = area.Includes.ToList();

      if (includeIdx == 0 && !down) throw new Exception("This widget include is already at the top.");
      if (includeIdx == includes.Count - 1 && down) throw new Exception("This widget include is already at the bottom.");

      var include = includes.ElementAt(includeIdx);
      includes.Remove(include);
      int newIdx = includeIdx + (down ? 1 : -1);
      includes.Insert(newIdx, include);
      area.Includes = includes;
      AppServiceRepository.UpdateService(appSvc);
      return appSvc;
    }
  }
}
