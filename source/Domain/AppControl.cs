/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System.Xml.Linq;

  public class AppControl : XmlBase
  {
    public AppControl() : this(new XElement(Atom.AppNs + "control")) { }
    public AppControl(XElement xml) : base(xml) { }

    /// <summary>
    /// If this value is true, the entry is in draft mode and will not display public
    /// </summary>
    public bool? Draft
    {
      get { return GetBoolean(Atom.AppNs + "draft"); }
      set { SetYesNoBoolean(Atom.AppNs + "draft", value); }
    }

    /// <summary>
    /// If this value is false, the entry is NOT approved to display to public
    /// </summary>
    public bool? Approved
    {
      get { return GetBoolean(Atom.SvcNs + "approved"); }
      set { SetYesNoBoolean(Atom.SvcNs + "approved", value); }
    }

    /// <summary>
    /// If this value is false, the entry will not allow annotations
    /// </summary>
    public bool? AllowAnnotate
    {
      get { return GetBoolean(Atom.SvcNs + "allowAnnotate"); }
      set { SetYesNoBoolean(Atom.SvcNs + "allowAnnotate", value); }
    }
  }
}
