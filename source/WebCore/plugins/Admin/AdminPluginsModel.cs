/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;
  using AtomSite.Domain;
  using StructureMap;

  public class AdminPluginsModel : AdminModel
  {
    public IEnumerable<PluginState> Plugins { get; set; }
    public string ChangedType { get; set; }
    public string MessageForType { get; set; }

    public bool HasPluginEntry(string type)
    {
      IContainer container = HttpContext.Current.Application["container"] as Container;
      if (Plugins.Where(p => p.Type == type).First().Setup)
      {
        try
        {
          IPlugin plugin = (IPlugin)container.GetInstance(Type.GetType(type));
          return plugin.GetPluginEntry(container, HttpContext.Current.Server.MapPath("~/")) != null;
        }
        catch (Exception ex)
        {
          Logger.Error(ex);
        }
      }
      return false;
    }

    public bool CanUninstall(string type)
    {
      IContainer container = HttpContext.Current.Application["container"] as Container;
      if (Plugins.Where(p => p.Type == type).First().Setup)
      {
        try
        {
          IPlugin plugin = (IPlugin)container.GetInstance(Type.GetType(type));
          return plugin.CanUninstall;
        }
        catch (Exception ex)
        {
          Logger.Error(ex);
        }
      }
      return false;
    }
  }
}
