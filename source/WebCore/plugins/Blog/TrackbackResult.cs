/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Xml.Linq;
  using AtomSite.Domain;

  public class TrackbackResult : XmlBase
  {
    public TrackbackResult() : base(new XElement("result")) { }
    public bool Error
    {
      get { return !GetBooleanWithDefault("error", false); }
      set { SetBoolean("error", !value, "1", "0"); }
    }
    public string Message
    {
      get { return GetValue<string>("message"); }
      set { SetValue<string>("message", value); }
    }
  }
}
