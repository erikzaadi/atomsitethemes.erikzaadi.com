/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Security.Cryptography;
  using System.Web.Mvc;
  using System.Xml;
  using System.Xml.Linq;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using StructureMap;

  public interface IPlugin
  {
    string DefaultAssetGroup { get; set; }
    int DefaultMerit { get; set; }
    bool CanUninstall { get; set; }
    PluginState Setup(IContainer container, string appPath);
    PluginState Upgrade(IContainer container, string appPath, Version previous, Version current);
    Plugin GetPluginEntry(IContainer container, string appPath);
    PluginState Uninstall(IContainer container, string appPath);
    void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets);
  }

  public abstract class BasePlugin : IPlugin
  {
    protected ILogService LogService;
    public BasePlugin(ILogService logger)
    {
      LogService = logger;
      DefaultMerit = (int)MeritLevel.Default;
      DefaultAssetGroup = Asset.DefaultGroup;
    }

    #region IPlugin Members

    public string DefaultAssetGroup { get; set; }
    public int DefaultMerit { get; set; }
    public bool CanUninstall { get; set; }

    public virtual PluginState Setup(IContainer container, string appPath)
    {
      var state = GetPluginState();
      state.Status = PluginStatus.Enabled;
      return state;
    }

    public virtual PluginState Upgrade(IContainer container, string appPath, Version previous, Version current)
    {
      var state = GetPluginState();
      state.Status = PluginStatus.Enabled;
      return state;
    }

    public virtual PluginState Uninstall(IContainer container, string appPath)
    {
      var state = GetPluginState();
      state.Status = PluginStatus.NotInstalled;
      return state;
    }

    public abstract void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets);

    public virtual Plugin GetPluginEntry(IContainer container, string appPath)
    {
      return GetEmbeddedPluginEntry();
    }
    #endregion

    protected virtual PluginState GetPluginState()
    {
      return new PluginState()
      {
        Type = this.GetType().AssemblyQualifiedName,
        Merit = DefaultMerit
      };
    }

    protected virtual Plugin GetEmbeddedPluginEntry()
    {
      string name = this.GetType().Assembly.GetManifestResourceNames()
        .Where(n => n.EndsWith(this.GetType().Name + "." + this.GetType().Assembly.GetName().Version.ToString()
          + ".xml")).SingleOrDefault();

      if (name != null)
      {
        using (Stream s = this.GetType().Assembly.GetManifestResourceStream(name))
        {
          return new Plugin() { Xml = XElement.Load(new XmlTextReader(s)) };
        }
      }
      return null;
    }

    #region Registration

    protected virtual void RegisterPage<T>(IContainer container) where T : IPage
    {
      var p = Activator.CreateInstance<T>();
      container.Configure(c =>
      {
        c.For<IPage>().Add(p).Named(p.Name);
      });
    }
    protected virtual void RegisterPage(IContainer container, IPage page)
    {
      container.Configure(c =>
      {
        c.For<IPage>().Add(page).Named(page.Name);
      });
    }
    protected virtual void RegisterPage(IContainer container, string pageName)
    {
      RegisterPage(container, pageName, new string[] { });
    }
    protected virtual void RegisterPage(IContainer container, string pageName, string[] assets)
    {
      RegisterPage(container, pageName, DefaultAssetGroup, assets);
    }
    protected virtual void RegisterPage(IContainer container, string pageName, string assetGroup, string[] assets)
    {
      RegisterPage(container, pageName, assets.Select(a => new Asset(a, assetGroup, AssetScope.Page)));
    }
    protected virtual void RegisterPage(IContainer container, string pageName, IEnumerable<Asset> assets)
    {
      IPage page = new Page(pageName) { Assets = assets };
      RegisterPage(container, page);
    }

    protected virtual void RegisterRoutes<T>(IContainer container, List<SiteRoute> routes) where T : BaseRouteRegistrar
    {
      T registrar = (T)Activator.CreateInstance(typeof(T), container.GetInstance<IAppServiceRepository>(),
        routes, this.DefaultMerit);
      registrar.RegisterRoutes();
    }

    protected virtual void RegisterRoutes<T>(IContainer container, List<SiteRoute> routes, string routeServiceType) where T : BaseRouteRegistrar
    {
      T registrar = (T)Activator.CreateInstance(typeof(T), container.GetInstance<IAppServiceRepository>(),
        routes, this.DefaultMerit);
      registrar.RouteServiceType = routeServiceType;
      registrar.RegisterRoutes();
    }

    protected virtual void RegisterController<T>(IContainer container) where T : IController
    {
      string name = typeof(T).Name;
      container.Configure(c =>
      {
        c.For<IController>().Add<T>().Named(name);
      });
    }

    protected virtual void RegisterWidget<T>(IContainer container) where T : IWidget
    {
      var w = Activator.CreateInstance<T>();
      RegisterWidget(container, w);
    }

    protected virtual void RegisterWidget(IContainer container, IWidget w)
    {
      container.Configure(c =>
      {
        c.For<IWidget>().Add(w).Named(w.Name);
      });
    }

    protected virtual void RegisterViewWidget(IContainer container, string widgetName)
    {
      RegisterViewWidget(container, widgetName, new string[] { });
    }

    protected virtual void RegisterViewWidget(IContainer container, string widgetName, string[] assetNames)
    {
      RegisterViewWidget(container, widgetName, assetNames.Select(assetName =>
        new Asset(assetName, DefaultAssetGroup, AssetScope.Widget)),
        string.Empty);
    }
    protected virtual void RegisterViewWidget(IContainer container, string widgetName, string[] assetNames, string tailScript)
    {
      RegisterViewWidget(container, widgetName, assetNames.Select(assetName =>
        new Asset(assetName, DefaultAssetGroup, AssetScope.Widget)), tailScript);
    }

    protected virtual void RegisterViewWidget(IContainer container, string widgetName, IEnumerable<Asset> assets, string tailScript)
    {
      IWidget w = new ViewWidget(widgetName) { Assets = assets, TailScript = tailScript };
      RegisterWidget(container, w);
    }

    protected virtual void RegisterCompositeWidget(IContainer container, string widgetName, string controller,
      string action)
    {
      RegisterCompositeWidget(container, widgetName, controller, action, new Asset[] { }, string.Empty);
    }

    protected virtual void RegisterCompositeWidget(IContainer container, string widgetName, string controller,
      string action, string[] assetNames)
    {
      RegisterCompositeWidget(container, widgetName, controller, action,
        assetNames.Select(assetName => new Asset(assetName, this.DefaultAssetGroup, AssetScope.Widget)), string.Empty);
    }

    protected virtual void RegisterCompositeWidget(IContainer container, string widgetName, string controller,
      string action, string[] assetNames, string tailScript)
    {
      RegisterCompositeWidget(container, widgetName, controller, action,
        assetNames.Select(assetName => new Asset(assetName, this.DefaultAssetGroup, AssetScope.Widget)), tailScript);
    }

    protected virtual void RegisterCompositeWidget(IContainer container, string widgetName, string controller,
      string action, IEnumerable<Asset> assets, string tailScript)
    {
      IWidget w = new CompositeWidget(widgetName, controller, action) { Assets = assets, TailScript = tailScript };
      RegisterWidget(container, w);
    }

    protected virtual void RegisterGlobalAsset(ICollection<Asset> assets, string assetName)
    {
      assets.Add(new Asset(assetName, DefaultAssetGroup, AssetScope.Global));
    }

    protected virtual void RegisterFrameworkAsset(ICollection<Asset> assets, string assetName)
    {
      assets.Add(new Asset(assetName, Asset.NoGroup, AssetScope.Global));
    }

    //protected virtual void RegisterAsset(ICollection<Asset> assets, string assetName, AssetScope assetScope)
    //{
    //  assets.Add(new Asset(assetName, DefaultAssetGroup, assetScope));
    //}
    //protected virtuavoid RegisterAsset(ICollection<Asset> assets, Asset asset)
    //{
    //  assets.Add(asset);
    //}

    #endregion Registration

    #region Installation

    protected virtual void SetupIncludeInPageArea(IContainer container, string pageName, string areaName, string widgetName)
    {
      SetupIncludeInTargetArea<ServicePage>(container, pageName, areaName, new Include() { Name = widgetName });
    }

    protected void SetupIncludeInPageArea(IContainer container, string pageName, string areaName, Include include)
    {
      SetupIncludeInTargetArea<ServicePage>(container, pageName, areaName, include);
    }

    protected virtual void SetupIncludeInWidgetArea(IContainer container, string targetWidgetName, string areaName, string widgetName)
    {
      SetupIncludeInTargetArea<ServiceWidget>(container, targetWidgetName, areaName, new Include() { Name = widgetName });
    }

    protected void SetupIncludeInWidgetArea(IContainer container, string targetWidgetName, string areaName, Include include)
    {
      SetupIncludeInTargetArea<ServiceWidget>(container, targetWidgetName, areaName, include);
    }

    protected virtual void SetupIncludeInTargetArea<T>(IContainer container,
      string targetName, string areaName, Include include) where T : TargetBase
    {

      IAppServiceRepository svcRepo = container.GetInstance<IAppServiceRepository>();
      AppService svc = svcRepo.GetService();
      bool changed = false;

      IList<T> allTargets = null;
      if (svc.Pages.ToList().GetType() == typeof(List<T>))
      {
        allTargets = svc.Pages.Cast<T>().ToList();
      }
      else if (svc.Widgets.ToList().GetType() == typeof(List<T>))
      {
        allTargets = svc.Widgets.Cast<T>().ToList();
      }

      //IList<T> allTargets = svc.Pages.ToList();// svc.Widgets != null ? svc.Widgets.ToList() : new List<Widgets>();
      T target = allTargets.Where(t => t.Name == targetName).SingleOrDefault();
      if (target == null)
      {
        LogService.Info("Adding widgets for target {0}", targetName ?? "default");
        target = Activator.CreateInstance<T>();
        target.Name = targetName;
        allTargets.Add(target);
        changed = true;
      }
      IList<ServiceArea> areas = target.Areas.ToList();// != null ? widgets.Areas.ToList() : new List<Area>();
      ServiceArea area = areas.Where(a => a.Name == areaName).SingleOrDefault();
      if (area == null)
      {
        LogService.Info("Adding area {0} for target {1}", areaName, targetName ?? "default");
        area = new ServiceArea() { Name = areaName };
        areas.Add(area);
        changed = true;
      }
      IList<Include> includes = area.Includes.ToList();// != null ? area.Includes.ToList() : new List<Include>();
      Include inc = includes.Where(i => include.Name == i.Name).SingleOrDefault();
      if (inc == null)
      {
        LogService.Info("Including {0} in area {1} for target {2}", include.Name, areaName, targetName ?? "default");
        includes.Add(include);
        changed = true;
      }
      else
      {
        LogService.Info("{0} already included in area {1} for target {2}", include.Name, areaName, targetName ?? "default");
      }

      //update
      if (changed)
      {
        area.Includes = includes;
        target.Areas = areas;
        if (svc.Pages.ToList().GetType() == typeof(List<T>))
        {
          svc.Pages = allTargets.Cast<ServicePage>();
        }
        else if (svc.Widgets.ToList().GetType() == typeof(List<T>))
        {
          svc.Widgets = allTargets.Cast<ServiceWidget>();
        }
        svcRepo.UpdateService(svc);
      }
    }

    //protected virtual IEnumerable<FilePart> InstallEmbeddedFiles(string appPath)
    //{
    //  var files = new List<FilePart>();
    //  foreach (string name in this.GetType().Assembly.GetManifestResourceNames())
    //  {
    //    files.Add(WriteManifestFile(name, appPath));
    //  }
    //  return files;
    //}


    //protected virtual string FilePathToManifestName(string name)
    //{
    //  return this.GetType().Namespace + "." + name.Replace('\\', '.');
    //}

    //protected virtual string ManifestNameToFilePath(string name)
    //{
    //  string file = name.Substring(this.GetType().Namespace.Length + 1);
    //  string filename = file.Substring(0, file.LastIndexOf('.')).Replace('.', Path.DirectorySeparatorChar) +
    //        file.Substring(file.LastIndexOf('.'));
    //  return filename;
    //}

    //protected virtual void InstallPluginFiles(Plugin plugin, string appPath)
    //{
    //  var files = plugin.Pages.SelectMany(p => p.Files).Concat(
    //    plugin.Widgets.SelectMany(w => w.Files));

    //  foreach (FilePart f in files)
    //  {
    //    //skip plugin entry meta file
    //    if (f.Name == this.GetType().FullName + ".xml") continue;

    //    //TODO: existing file? only overwrite if md5 don't match?
    //    var manifestName = FilePathToManifestName(f.Name);
    //    FilePart fp = WriteManifestFile(manifestName, appPath);
    //    //update MD5
    //    f.Md5Sum = fp.Md5Sum;
    //  }
    //}

    protected virtual void UninstallPluginFiles(IContainer container, Plugin plugin, string appPath)
    {
      if (plugin == null) throw new PluginEntryNotFoundException(this.GetType().ToString());

      IAppServiceRepository svcRepo = container.GetInstance<IAppServiceRepository>();
      AppService svc = svcRepo.GetService();
      //var installed = svc.Plugins.Where(p => p.Installed && p.Type != plugin.Type);

      var otherPlugins = container.GetAllInstances<IPlugin>()
        .Where(p => p.GetPluginEntry(container, appPath) != null)
        .Select(p => p.GetPluginEntry(container, appPath)).ToList();

      otherPlugins.Remove(plugin);

      //get all files being used
      var inUseFiles = otherPlugins.SelectMany(p => p.AllFiles()).Distinct();

      //only remove files unique to this plugin
      var files = plugin.AllFiles().Except(inUseFiles);

      foreach (FilePart f in files)
      {
        string path = Path.Combine(appPath, f.Name);
        if (f.Md5Sum != null)
        {
          //only remove file if it hasn't changed
          MD5 md5 = new MD5CryptoServiceProvider();
          using (Stream s = File.OpenRead(path))
          {
            if (f.Md5Sum != Convert.ToBase64String(md5.ComputeHash(s)))
            {
              LogService.Info("Skipping plugin file uninstallation because file was changed: {0}", f.Name);
            }
          }
        }
        File.Delete(path);
        LogService.Info("Deleting the plugin file: {0}", f.Name);
      }
    }

    //protected FilePart WriteManifestFile(string name, string appPath)
    //{
    //  FilePart f = new FilePart();
    //  string filename = ManifestNameToFilePath(name);
    //  f.Name = filename;
    //  string path = Path.Combine(appPath, filename);

    //  //create dir if not exists
    //  string dir = Path.GetDirectoryName(path);
    //  if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

    //  //compute MD5 and write file to disk
    //  MD5 md5 = new MD5CryptoServiceProvider();
    //  using (Stream s = this.GetType().Assembly.GetManifestResourceStream(name))
    //  {
    //    f.Md5Sum = Convert.ToBase64String(md5.ComputeHash(s));
    //    var bStr = new byte[s.Length];
    //    s.Read(bStr, 0, (int)s.Length);
    //    File.WriteAllBytes(path, bStr);
    //  }
    //  LogService.Info("Installing plugin file: {0}", filename);
    //  return f;
    //}

    //protected virtual IEnumerable<string> SimpleInstall(string appPath)
    //{
    //  var files = new List<string>();
    //  string asmPath = Path.GetDirectoryName(this.GetType().Assembly.Location);
    //  if (asmPath != Path.Combine(appPath, "bin"))
    //  {
    //    //copy theme and script files
    //    files.AddRange(CopyFiles(Path.Combine(asmPath, "js"), Path.Combine(appPath, "js")));
    //    files.AddRange(CopyFiles(Path.Combine(asmPath, "themes"), Path.Combine(appPath, "themes")));
    //  }
    //  return files;
    //}

    //protected virtual IEnumerable<string> CopyFiles(string sourcePath, string destPath)
    //{
    //  var files = new List<string>();
    //  foreach (string file in Directory.GetFiles(sourcePath))
    //  {
    //    var dest = Path.Combine(destPath, Path.GetFileName(file));
    //    File.Copy(file, dest, true);
    //    files.Add(dest);
    //  }
    //  foreach (string dir in Directory.GetDirectories(sourcePath))
    //  {
    //    files.AddRange(CopyFiles(dir, Path.Combine(destPath, Path.GetDirectoryName(dir))));
    //  }
    //  return files;
    //}

    #endregion Installation

    protected void UninstallInclude(IContainer container, Func<Include, bool> criteria)
    {
      IAppServiceRepository svcRepo = container.GetInstance<IAppServiceRepository>();
      AppService svc = svcRepo.GetService();

      var includes = svc.Pages.SelectMany(p => p.Areas).SelectMany(a => a.Includes).Concat(
          svc.Widgets.SelectMany(w => w.Areas).SelectMany(a => a.Includes)).Where(criteria);

      if (includes.Count() == 0) return;
      bool changed = false;
      foreach (Include i in includes)
      {
        LogService.Info("Removing include {0}", i.Name);
        ((XElement)i).Remove();
        changed = true;
      }
      if (changed) svcRepo.UpdateService(svc);
    }

    protected void UninstallIncludeFromPageArea(IContainer container, string pageName, string areaName, Func<Include, bool> criteria)
    {
      UninstallIncludeFromTarget<ServicePage>(container, pageName, areaName, criteria);
    }

    protected void UninstallIncludeFromWidgetArea(IContainer container, string targetWidgetName, string areaName, Func<Include, bool> criteria)
    {
      UninstallIncludeFromTarget<ServiceWidget>(container, targetWidgetName, areaName, criteria);
    }

    private void UninstallIncludeFromTarget<T>(IContainer container, string targetName, string areaName, Func<Include, bool> criteria) where T : TargetBase
    {
      IAppServiceRepository svcRepo = container.GetInstance<IAppServiceRepository>();
      AppService svc = svcRepo.GetService();

      IList<T> allTargets = null;
      if (svc.Pages.ToList().GetType() == typeof(List<T>))
      {
        allTargets = svc.Pages.Cast<T>().ToList();
      }
      else if (svc.Widgets.ToList().GetType() == typeof(List<T>))
      {
        allTargets = svc.Widgets.Cast<T>().ToList();
      }

      T target = allTargets.Where(t => t.Name == targetName).SingleOrDefault();
      if (target == null) return; //can't find

      IList<ServiceArea> areas = target.Areas.ToList();
      ServiceArea area = areas.Where(a => a.Name == areaName).SingleOrDefault();
      if (area == null) return; //can't find

      IList<Include> includes = area.Includes.ToList();
      Include inc = includes.Where(criteria).SingleOrDefault();
      if (inc == null) return; //can't find

      LogService.Info("Removing include {0} from area {1} for target {2}", inc.Name, areaName, targetName ?? "default");
      includes.Remove(inc);

      area.Includes = includes;
      target.Areas = areas;
      if (svc.Pages.ToList().GetType() == typeof(List<T>))
      {
        svc.Pages = allTargets.Cast<ServicePage>();
      }
      else if (svc.Widgets.ToList().GetType() == typeof(List<T>))
      {
        svc.Widgets = allTargets.Cast<ServiceWidget>();
      }
      svcRepo.UpdateService(svc);
    }
  }
}
