/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Web.Mvc;
  using System.Web.Routing;
  using System.Xml;
  using System.Xml.Linq;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using AtomSite.WebCore.Properties;
  using ICSharpCode.SharpZipLib.Zip;
  using StructureMap;

  public class PluginEngine
  {
    public static PluginState InstallFromZip(string type, string appPath)
    {
      //TODO: need to replace old version PluginState with new version
      PluginState ps = new PluginState(type);
      string pluginPath = Path.Combine(appPath, Settings.Default.PluginsDir);
      string zipPath = Directory.GetFiles(pluginPath, ps.Name + "*.zip").LastOrDefault();
      return InstallFromZip(zipPath, appPath, true);
    }

    public static PluginState InstallFromZip(string zipPath, string appPath, bool forceInstall)
    {
      string name = Path.GetFileNameWithoutExtension(zipPath);
      //string appPath = Path.GetDirectoryName(Path.GetDirectoryName(zipPath));
      PluginState ps = null;

      //read plugin entry and make sure compatible
      using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipPath)))
      {
        ZipEntry e;
        while ((e = s.GetNextEntry()) != null)
        {
          if (e.Name.EndsWith(name + ".xml"))
          {
            Plugin p = new Plugin(XDocument.Load(new XmlTextReader(s)).Root);
            ps = new PluginState(p.Type);
            var compatible = p.CompatibleVersions.Select(cv => new Version(cv));
            if (!compatible.Contains(ServerApp.CurrentVersion))
              ps.Status = PluginStatus.Incompatible;
            else
              ps.Status = PluginStatus.NeedSetup;
            break;
          }
        }
      }
      if (ps == null) throw new PluginEntryNotFoundException(name); 

      if (ps.Status == PluginStatus.NeedSetup || forceInstall)
      {
        //exclude manifest when unzipping (it is embedded in dll anyway)
        string exclude = string.Format("-{0}$", name + ".xml");
        FastZip zip = new FastZip();
        zip.ExtractZip(zipPath, appPath, FastZip.Overwrite.Always, null, exclude, null, true);
      }
      return ps;
    }

    public static void LoadPlugins(IContainer container, RouteCollection routeTable, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, string appPath, string binPath)
    {
      container.GetInstance<ILogService>().Info("Loading plugins");
      IAppServiceRepository svcRepo = container.GetInstance<IAppServiceRepository>();

      IList<PluginState> pluginStates = svcRepo.GetService().Plugins.ToList();
      List<SiteRoute> routes = new List<SiteRoute>();
      bool changed = false;

      //find plugins
      container.Configure((x) =>
      {
        //plugin all the types into the object factory
        x.Scan((scanner) =>
        {
          container.GetInstance<ILogService>().Info("Scanning for plugins");
          scanner.AssembliesFromPath(binPath);
          scanner.AddAllTypesOf<IPlugin>().NameBy((t) => t.Name);
        });
      });

      //register and install if not already installed
      foreach (IPlugin plugin in container.GetAllInstances<IPlugin>()
        .OrderByDescending(p => p.DefaultMerit)) //TODO: used merit state
      {
        container.GetInstance<ILogService>().Info("Found plugin: {0} with merit {1}", plugin, plugin.DefaultMerit);

        PluginState pluginState = pluginStates
          .Where(e => e.Type == plugin.GetType().AssemblyQualifiedName)
          .SingleOrDefault();
        
        bool upgrade = false;

        //check for older version
        if (pluginState == null)
        {
          pluginState = pluginStates
            .Where(e => e.Type.Split(',')[0] == plugin.GetType().AssemblyQualifiedName.Split(',')[0])
            .SingleOrDefault();
          if (pluginState != null) upgrade = true;
        }
        
        try
        {
        
          //support upgrade of plugin when metadata version is newer than plugin version
          if (upgrade && pluginState.Enabled)
          {
            container.GetInstance<ILogService>().Info("Upgrading plugin");
            int idx = pluginStates.IndexOf(pluginState);
            pluginState = plugin.Upgrade(container, appPath, new Version(pluginState.Version),
              plugin.GetType().Assembly.GetName().Version);
            pluginStates[idx] = pluginState;
            changed = true;
          }

          //setup plugin if need setup or debugging is on and plugin is enabled
          if (pluginState == null || (pluginState.Status == PluginStatus.NeedSetup))
            //|| (this.Context.IsDebuggingEnabled && pluginState.Enabled))
          {
            container.GetInstance<ILogService>().Info("Setting up plugin");
            pluginState = plugin.Setup(container, appPath);
            var existing = pluginStates.Where(p => p.Type == pluginState.Type).SingleOrDefault();
            if (existing != null) pluginStates[pluginStates.IndexOf(existing)] = pluginState;
            else pluginStates.Add(pluginState);
            changed = true;
          }

          //register the enabled plugins
          if (pluginState.Enabled)
          {
            container.GetInstance<ILogService>().Info("Registering plugin");
            var sae = container.GetInstance<IAssetService>();
            plugin.Register(container, routes, ViewEngines.Engines, ModelBinders.Binders, sae.Assets);
          }
        }
        catch (Exception ex)
        {
          container.GetInstance<ILogService>().Error(ex);
        }
      }
      
      //save changes
      if (changed)
      {
        container.GetInstance<ILogService>().Info("Updating plugins metadata");
        AppService appSvc = svcRepo.GetService();
        appSvc.Plugins = pluginStates;
        svcRepo.UpdateService(appSvc);
      }
      
      //update RouteTable
      //routes.Sort((r1, r2) => r1.Merit.CompareTo(r2.Merit));
      foreach (var r in routes) routeTable.Add(r.Name, r.Route);
    }
  }

  [global::System.Serializable]
  public class PluginNotCompatibleException : BaseException
  {
    public PluginNotCompatibleException(string type)
      : base("The plugin '{0}' is not compatible with {1} {2}",
        type, ServerApp.CurrentRelease, ServerApp.CurrentVersion) { }
  }
  [global::System.Serializable]
  public class PluginEntryNotFoundException : BaseException
  {
    public PluginEntryNotFoundException(string type)
      : base("The plugin archive does not appear to be valid as no plugin entry was found for '{0}'", type) { }
  }
}
