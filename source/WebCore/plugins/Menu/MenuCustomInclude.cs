/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using System.Xml.Linq;
  using AtomSite.Domain;

  public class MenuCustomInclude : Include
  {
    public MenuCustomInclude(Include include)
      : base(include.Xml)
    { }

    public MenuCustomInclude() : base()
    {
      this.Name = "CustomMenuWidget";
      this.MenuItems = new List<CustomMenuItem>();
    }

    public IEnumerable<CustomMenuItem> MenuItems
    {
      get { return GetXmlValues<CustomMenuItem>(Atom.SvcNs + "item"); }
      set { SetXmlValues<CustomMenuItem>(Atom.SvcNs + "item", value); }
    }
  }

  public class CustomMenuItem : XmlBase
  {
    public CustomMenuItem() : base(new XElement(Atom.SvcNs + "item")) { }
    public CustomMenuItem(XElement xml) : base(xml) { }

    public string Href
    {
      get { return GetProperty<string>("href"); }
      set { SetProperty<string>("href", value); }
    }
    public string Text
    {
      get { return GetProperty<string>("text"); }
      set { SetProperty<string>("text", value); }
    }
    public string Title
    {
      get { return GetProperty<string>("title"); }
      set { SetProperty<string>("title", value); }
    }
    public bool ExactSelect
    {
      get { return GetBooleanPropertyWithDefault("exactSelect", false); }
      set { SetBooleanProperty("exactSelect", value == false ? (bool?)null : true); }
    }
  }
}
