/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System.Collections.Generic;
  using System.Xml.Linq;
  using System;
  using System.Linq;

  public class Plugin : AtomEntry
  {
    public Plugin() : base(new XElement(Atom.PluginNs + "plugin")) { }
    public Plugin(XElement xml) : base(xml) { }

    public string Type
    {
      get { return GetValue<string>(Atom.PluginNs + "type"); }
      set { SetValue<string>(Atom.PluginNs + "type", value); }
    }

    public IEnumerable<string> CompatibleVersions
    {
      get { return GetValues<string>(Atom.PluginNs + "compatibleVersion"); }
      set { SetValues<string>(Atom.PluginNs + "compatibleVersion", value); }
    }

    public IEnumerable<FilePart> Files
    {
      get { return GetXmlValues<FilePart>(Atom.PluginNs + "file"); }
      set { SetXmlValues<FilePart>(Atom.PluginNs + "file", value); }
    }

    public IEnumerable<WidgetPart> Widgets
    {
      get { return GetXmlValues<WidgetPart>(Atom.PluginNs + "widget"); }
      set { SetXmlValues<WidgetPart>(Atom.PluginNs + "widget", value); }
    }
    public IEnumerable<PagePart> Pages
    {
      get { return GetXmlValues<PagePart>(Atom.PluginNs + "page"); }
      set { SetXmlValues<PagePart>(Atom.PluginNs + "page", value); }
    }
    public IEnumerable<RoutePart> Routes
    {
      get { return GetXmlValues<RoutePart>(Atom.PluginNs + "route"); }
      set { SetXmlValues<RoutePart>(Atom.PluginNs + "route", value); }
    }
    public IEnumerable<ViewEnginePart> ViewEngines
    {
      get { return GetXmlValues<ViewEnginePart>(Atom.PluginNs + "viewEngine"); }
      set { SetXmlValues<ViewEnginePart>(Atom.PluginNs + "viewEngine", value); }
    }


    public IEnumerable<FilePart> AllFiles()
    {
      List<FilePart> files = new List<FilePart>();
      files.AddRange(Files);
      files.AddRange(Widgets.SelectMany(w => w.Files));
      files.AddRange(Pages.SelectMany(p => p.Files));
      return files.Distinct();
    }

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }


  public class WidgetPart : PluginPart
  {
    public WidgetPart() : base(Atom.PluginNs + "widget") { }
  }

  public class PagePart : PluginPart
  {
    public PagePart() : base(Atom.PluginNs + "page") { }
  }

  public abstract class PluginPart : XmlBase
  {
    public PluginPart(XName type) : this(new XElement(type)) { }
    public PluginPart(XElement xml) : base(xml) { }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public AtomText Name
    {
      get { return GetXmlValue<AtomText>(Atom.PluginNs + "name"); }
      set { SetXmlValue<AtomText>(Atom.PluginNs + "name", value); }
    }

    /// <summary>
    /// Gets or sets the description.  This is an extension. 
    /// </summary>
    /// <value>The description.</value>
    public AtomContent Description
    {
      get { return GetXmlValue<AtomContent>(Atom.PluginNs + "description"); }
      set { SetXmlValue<AtomContent>(Atom.PluginNs + "description", value); }
    }

    public IEnumerable<string> Scopes
    {
      get { return GetValues<string>(Atom.PluginNs + "scope"); }
      set { SetValues<string>(Atom.PluginNs + "scope", value); }
    }

    public IEnumerable<FilePart> Files
    {
      get { return GetXmlValues<FilePart>(Atom.PluginNs + "file"); }
      set { SetXmlValues<FilePart>(Atom.PluginNs + "file", value); }
    }
  }

  public class FilePart : XmlBase
  {
    public FilePart() : this(new XElement(Atom.PluginNs + "file")) { }
    public FilePart(XElement xml) : base(xml) { }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
      get { return GetProperty<string>("name"); }
      set { SetProperty<string>("name", value); }
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Md5Sum
    {
      get { return GetProperty<string>("md5"); }
      set { SetProperty<string>("md5", value); }
    }

    public override bool Equals(object obj)
    {
      if (obj != null && obj.GetType().Equals(this.GetType()))
      {
        PluginState other = obj as PluginState;
        if ((object)other != null)
        {
          return other.Name == this.Name; //use md5?
        }
      }
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return this.Name.GetHashCode(); //use md5?
    }
  }


  public class RoutePart : XmlBase
  {
    public RoutePart() : this(new XElement(Atom.PluginNs + "route")) { }
    public RoutePart(XElement xml) : base(xml) { }
    
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
      get { return GetProperty<string>("name"); }
      set { SetProperty<string>("name", value); }
    }

    public string Url
    {
      get { return GetProperty<string>("url"); }
      set { SetProperty<string>("url", value); }
    }
  }

  public class ViewEnginePart : XmlBase
  {
    public ViewEnginePart() : this(new XElement(Atom.PluginNs + "viewEngine")) { }
    public ViewEnginePart(XElement xml) : base(xml) { }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public AtomText Name
    {
      get { return GetXmlValue<AtomText>(Atom.PluginNs + "name"); }
      set { SetXmlValue<AtomText>(Atom.PluginNs + "name", value); }
    }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public AtomContent Description
    {
      get { return GetXmlValue<AtomContent>(Atom.PluginNs + "description"); }
      set { SetXmlValue<AtomContent>(Atom.PluginNs + "description", value); }
    }
  }
}
