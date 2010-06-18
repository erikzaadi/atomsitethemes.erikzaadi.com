/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System.Collections.Generic;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;

  public class CdnAssetLocations : XmlBase
  {
    public CdnAssetLocations() : this(new XElement(Atom.SvcNs + "cdn")) { }
    public CdnAssetLocations(XElement xml) : base(xml) { }

    public IEnumerable<CdnAssetLocation> Assets
    {
      get { return GetXmlValues<CdnAssetLocation>(Atom.SvcNs + "asset"); }
      set { SetXmlValues<CdnAssetLocation>(Atom.SvcNs + "asset", value); }
    }

    public static readonly CdnAssetLocations Default = new CdnAssetLocations(XElement.Load(new XmlTextReader(
      Assembly.GetExecutingAssembly().GetManifestResourceStream("AtomSite.Domain.CdnAssetLocations.xml"))));
  }

  public class CdnAssetLocation : XmlBase
  {
    public CdnAssetLocation() : this(new XElement(Atom.SvcNs + "asset")) { }
    public CdnAssetLocation(XElement xml) : base(xml) { }
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
}
