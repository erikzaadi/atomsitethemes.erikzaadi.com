/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using AtomSite.Domain;

  public class ContactAppWorkspace : AppWorkspace
  {
    public ContactAppWorkspace(AppWorkspace ws) : base(ws.Xml) { }

    /// <summary>
    /// Gets or sets contact configuration for <c>ContactPlugin</c>.
    /// </summary>
    /// <value><c>Contact</c> configuration</value>
    public Contact Contact
    {
        get { return GetXmlValue<Contact>(Atom.SvcNs + "contact"); }
        set { SetXmlValue<Contact>(Atom.SvcNs + "contact", value); }
    }
  }
}
