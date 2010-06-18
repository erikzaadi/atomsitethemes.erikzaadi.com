/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Linq;
  using System.Web.UI.WebControls;
  using System.Xml.Linq;
  using AtomSite.Domain;

  public class PingbackResult : XmlBase
  {
    public PingbackResult() : base(new XElement("methodResponse")) { }
    public string Success
    {
      get { return Xml.Element("params").Element("param").Element("value").Element("string").Value; }
      set
      {
        Xml.Elements("params").Remove(); Xml.Add(new XElement("params",
      new XElement("param", new XElement("value", new XElement("string", value)))));
      }
    }
    public int? FaultCode
    {
      get
      {
        return (int?)Xml.Element("fault").Element("value").Element("struct").Elements("member")
            .Where(m => (string)m.Element("name") == "faultCode").Single().Element("value").Element("int");
      }
    }
    public string FaultString
    {
      get
      {
        return (string)Xml.Element("fault").Element("value").Element("struct").Elements("member")
            .Where(m => (string)m.Element("name") == "faultString").Single().Element("value").Element("string");
      }
    }
    public void SetFault(int faultCode, string faultString)
    {
      Xml.Elements("params").Remove(); Xml.Add(new XElement("fault",
    new XElement("value", new XElement("struct",
        new XElement("member",
            new XElement("name", "faultCode"),
            new XElement("value", new XElement("int", faultCode))),
        new XElement("member",
            new XElement("name", "faultString"),
            new XElement("value", new XElement("string", faultString)))))));
    }
  }
}
