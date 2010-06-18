/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Security.Principal;
  using System.Xml.Linq;

  [Serializable]
  public class User : XmlBase, IIdentity
  {
    public User() : this(new XElement(Atom.SvcNs + "user")) { }
    public User(XElement xml) : base(xml) { }

    public string Name
    {
      get { return GetProperty<string>("name"); }
      set { SetProperty<string>("name", value); }
    }
    public IEnumerable<string> Ids
    {
      get { return GetValues<string>(Atom.SvcNs + "id"); }
      set { SetValues<string>(Atom.SvcNs + "id", value); }
    }
    public string FullName
    {
      get { return GetValue<string>(Atom.SvcNs + "fullName"); }
      set { SetValue<string>(Atom.SvcNs + "fullName", value); }
    }
    public string Email
    {
      get { return GetValue<string>(Atom.SvcNs + "email"); }
      set { SetValue<string>(Atom.SvcNs + "email", value); }
    }

    public Uri Uri
    {
      get { return GetUri(Atom.SvcNs + "uri"); }
      set { SetUri(Atom.SvcNs + "uri", value); }
    }
    public string Password
    {
      get { return GetValue<string>(Atom.SvcNs + "password"); }
      set { SetValue<string>(Atom.SvcNs + "password", value); }
    }
    public string PasswordFormat
    {
      get { return GetValue<string>(Atom.SvcNs + "passwordFormat"); }
      set { SetValue<string>(Atom.SvcNs + "passwordFormat", value); }
    }

    public string GetClearPassword()
    {
      //TODO: handle encrypted
      return Password;
    }

    public bool CheckPassword(string password)
    {
      //TODO: handle various formats such as encryption and hashing
      return password == Password;
    }

    public AtomPerson ToAtomPerson(XName type)
    {
      return new AtomPerson(type)
      {
        Id = Ids.First(),
        Name = Name,
        Email = Email,
        Uri = Uri
      };
    }

    public AtomAuthor ToAtomAuthor()
    {
      return new AtomAuthor()
      {
        Id = Ids.First(),
        Name = Name,
        Email = Email,
        Uri = Uri
      };
    }

    public AtomContributor ToAtomContributor()
    {
      return new AtomContributor()
      {
        Id = Ids.First(),
        Name = Name,
        Email = Email,
        Uri = Uri
      };
    }

    #region IIdentity Members

    public string AuthenticationType { get; set; }

    public bool IsAuthenticated
    {
      get { return !string.IsNullOrEmpty(Name); }
    }

    #endregion
  }
}
