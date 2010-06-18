/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System.Collections.Generic;
  using System.Xml.Linq;

  public abstract class TargetBase : XmlBase
  {
    public TargetBase(XName type) : this(new XElement(type)) { }
    public TargetBase(XElement xml) : base(xml) { }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name
    {
      get { return GetProperty<string>("name"); }
      set { SetProperty<string>("name", value); }
    }

    /// <summary>
    /// Gets or sets the areas.
    /// </summary>
    public IEnumerable<ServiceArea> Areas
    {
      get { return GetXmlValues<ServiceArea>(Atom.SvcNs + "area"); }
      set { SetXmlValues<ServiceArea>(Atom.SvcNs + "area", value); }
    }
  }

  public class ServiceWidget : TargetBase
  {
    public ServiceWidget() : base(Atom.SvcNs + "widget") { }
    public ServiceWidget(XElement xml) : base(xml) { }
  }

  public class ServicePage : TargetBase
  {
    public ServicePage() : base(Atom.SvcNs + "page") { }
    public ServicePage(XElement xml) : base(xml) { }


    ///// <summary>
    ///// Gets or sets the group to determine if it applies to a group of pages.
    ///// </summary>
    //public string Group
    //{
    //  get { return GetProperty<string>("group"); }
    //  set { SetProperty<string>("group", value); }
    //}

    /// <summary>
    /// Gets or sets the name of the YUI doc that controls the page width
    /// #doc - 750px centered (good for 800x600)
    /// #doc2 - 950px centered (good for 1024x768)
    /// #doc3 - 100% fluid (good for everybody)
    /// #doc4 - 974px fluid (good for 1024x768)
    /// #doc-custom - an example of a custom page width
    /// </summary>
    public string Width
    {
      get { return GetProperty<string>("width"); }
      set { SetProperty<string>("width", value); }
    }

    /// <summary>
    /// Gets or sets the name of the YUI template that controls the column width and orientation.
    /// .yui-t1 - Two columns, narrow on left, 160px
    /// .yui-t2 - Two columns, narrow on left, 180px
    /// .yui-t3 - Two columns, narrow on left, 300px
    /// .yui-t4 - Two columns, narrow on right, 180px
    /// .yui-t5 - Two columns, narrow on right, 240px
    /// .yui-t6 - Two columns, narrow on right, 300px
    /// </summary>
    public string Template
    {
      get { return GetProperty<string>("template"); }
      set { SetProperty<string>("template", value); }
    }
  }

  public class ServiceArea : XmlBase
  {
    public ServiceArea() : this(new XElement(Atom.SvcNs + "area")) { }
    public ServiceArea(XElement xml) : base(xml) { }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name
    {
      get { return GetProperty<string>("name"); }
      set { SetProperty<string>("name", value); }
    }

    /// <summary>
    /// Gets or sets includes, typically with a name of the widget to display.
    /// </summary>
    public IEnumerable<Include> Includes
    {
      get { return GetXmlValues<Include>(Include.IncludeXName); }
      set { SetXmlValues<Include>(Include.IncludeXName, value); }
    }
  }

  public class Include : XmlBase
  {
    public readonly static XName IncludeXName = Atom.SvcNs + "include";

    public Include() : this(new XElement(IncludeXName)) { }
    public Include(Include i) : base(i.Xml) { }
    public Include(XElement xml) : base(xml) { }

    /// <summary>
    /// Gets or sets the name, which is typically the name of the widget to display.
    /// </summary>
    public string Name
    {
      get { return GetProperty<string>("name"); }
      set { SetProperty<string>("name", value); }
    }

    public override bool Equals(object obj)
    {
      if (obj == null) return false;
      Include other = obj as Include;
      if (other == null) return false;
      return this.Xml.ToString() == other.Xml.ToString();
    }

    public override int GetHashCode()
    {
      return this.Xml.ToString().GetHashCode();
    }
  }

  public class ConfigInclude : Include
  {
    public ConfigInclude() : base() { }
    public ConfigInclude(XElement xml) : base(xml) { }
    public ConfigInclude(Include include) : this(include.Xml) { }

    /// <summary>
    /// Gets or sets the name, which is typically the name of the widget to display.
    /// </summary>
    public string IncludePath
    {
      get { return GetProperty<string>("path"); }
      set { SetProperty<string>("path", value); }
    }
  }
}
