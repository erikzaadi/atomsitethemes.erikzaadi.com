/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System.Collections.Generic;
  using System.Xml.Linq;
  using System;
  using System.Linq;
  using System.ComponentModel;
  using System.Web;

  public enum PluginStatus : int
  {
    Incompatible = -1,
    NotInstalled = 0,
    NeedSetup,
    Enabled,
    Disabled,
  }

  public class PluginState : XmlBase
  {
    public PluginState() : this(new XElement(Atom.SvcNs + "plugin")) { }
    public PluginState(XElement xml) : base(xml) { }

    public PluginState(string type)
      : this()
    {
      Type = type;
    }

    public string Type
    {
      get { return GetProperty<string>("type"); }
      set { SetProperty<string>("type", value); }
    }

    public int Merit
    {
      get { return base.GetInt32Property("merit"); }
      set { base.SetInt32Property("merit", new int?(value)); }
    }

    public PluginStatus Status
    {
      get { return (PluginStatus)Enum.Parse(typeof(PluginStatus), base.GetProperty<string>("status")); }
      set { SetProperty<string>("status", value.ToString()); }
    }

    public bool Enabled
    {
      get { return Status == PluginStatus.Enabled; }
    }

    public bool Installed
    {
      get { return Status > PluginStatus.NotInstalled; }
    }

    public bool Setup
    {
      get { return Status > PluginStatus.NeedSetup; }
    }

    public string Name
    {
      get { return Type.Split(',')[0].Split('.').Last(); }
    }

    public string Version
    {
      //return Type.GetType(type).Assembly.GetName().Version.ToString();
      //AtomSite.WebCore.WizardPlugin, AtomSite.WebCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
      get { return Type.Split(',')[2].Split('=')[1]; }
    }

    public string Assembly
    {
      get { return Type.Split(',')[1] + ".dll"; }
    }

    

    public override bool Equals(object obj)
    {
      if (obj != null && obj.GetType().Equals(this.GetType()))
      {
        PluginState other = obj as PluginState;
        if ((object)other != null)
        {
          return other.Type == this.Type;
        }
      }
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return this.Type.GetHashCode();
    }
  }
}
