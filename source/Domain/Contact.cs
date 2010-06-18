/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Xml.Linq;

  [Flags]
  public enum ContactMode
  {
    Email = 1,
    Annotate = 2,
    EmailAndAnnotate = Email | Annotate
  }

  public class Contact : XmlBase
  {
    public static readonly XName ContactXName = Atom.SvcNs + "contact";

    public Contact() : this(new XElement(Atom.SvcNs + "contact")) { }
    public Contact(XElement xml) : base(xml) { }

    public ContactMode Mode
    {
      get
      {
        string mode = (string)Xml.Attribute("mode");
        if (string.IsNullOrEmpty(mode)) return ContactMode.Email;
        switch (mode.ToLowerInvariant())
        {
          case "email": return ContactMode.Email;
          case "annotate": return ContactMode.Annotate;
          case "emailandannotate": return ContactMode.Email;
          default: return ContactMode.Email;//throw new InvalidCastException(string.Format("The service type of {0} is unrecognized.", type));
        }
      }
      set { SetProperty<string>("mode", value.ToString()); }
    }

    public string To
    {
      get { return GetProperty<string>("to"); }
      set { SetProperty<string>("to", value); }
    }

    public string Host
    {
      get { return GetProperty<string>("host"); }
      set { SetProperty<string>("host", value); }
    }

    public string UserName
    {
      get { return GetProperty<string>("userName"); }
      set { SetProperty<string>("userName", value); }
    }

    public string Password
    {
      get { return GetProperty<string>("password"); }
      set { SetProperty<string>("password", value); }
    }

    public int? Port
    {
      get { return GetInt32Property("port"); }
      set { SetInt32Property("port", value); }
    }
  }
}
