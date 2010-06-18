/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;

  public class RoleMatrix : XmlBase
  {
    public RoleMatrix() : this(new XElement(Atom.SvcNs + "roleMatrix")) { }
    public RoleMatrix(XElement xml) : base(xml) { }

    public IEnumerable<RoleAction> RoleActions
    {
      get { return GetXmlValues<RoleAction>(Atom.SvcNs + "roleAction"); }
      set { SetXmlValues<RoleAction>(Atom.SvcNs + "roleAction", value); }
    }

    public static readonly RoleMatrix Default = new RoleMatrix(XElement.Load(new XmlTextReader(
      Assembly.GetExecutingAssembly().GetManifestResourceStream("AtomSite.Domain.RoleMatrix.xml"))));
  }

  public class RoleAction : XmlBase
  {
    public RoleAction() : this(new XElement(Atom.SvcNs + "roleAction")) { }
    public RoleAction(XElement xml) : base(xml) { }
    public string Name
    {
      get { return GetProperty<string>("name"); }
      set { SetProperty<string>("name", value); }
    }

    public AuthRoles AuthRoles
    {
      get
      {
        AuthRoles roles = AuthRoles.None;
        if (Anon) roles |= AuthRoles.Anonymous;
        if (User) roles |= AuthRoles.User;
        if (Contrib) roles |= AuthRoles.Contributor;
        if (Author) roles |= AuthRoles.Author;
        if (Admin) roles |= AuthRoles.Administrator;
        return roles;
      }
    }


    public bool Admin
    {
      get { return GetBooleanPropertyWithDefault("admin", false); }
      set { SetBooleanProperty("admin", value); }
    }
    public bool Author
    {
      get { return GetBooleanPropertyWithDefault("author", false); }
      set { SetBooleanProperty("author", value); }
    }
    public bool Contrib
    {
      get { return GetBooleanPropertyWithDefault("contrib", false); }
      set { SetBooleanProperty("contrib", value); }
    }
    public bool User
    {
      get { return GetBooleanPropertyWithDefault("user", false); }
      set { SetBooleanProperty("user", value); }
    }
    public bool Anon
    {
      get { return GetBooleanPropertyWithDefault("anon", false); }
      set { SetBooleanProperty("anon", value); }
    }
  }

  [Flags]
  public enum AuthRoles
  {
    None=0,
    Anonymous=1,
    User=2,
    Contributor=4,
    Author=8,
    Administrator=16,
      AuthorOrAdmin = Author | Administrator,
      Any = Anonymous | User | Contributor | Author | Administrator
  }

  //TODO: turn AuthAction into static class with static strings so it can be dynamic
  public enum AuthAction
  {
      None,
    GetServiceDoc,
    UpdateServiceDoc,
    GetCollectionFeed,
    CreateEntryOrMedia,
    GetEntryOrMedia,
    UpdateEntryOrMedia,
    DeleteEntryOrMedia,
    PeekEntryOrMedia,

    GetFeed,
    GetAnnotations,
    Annotate,
    ApproveEntryOrMedia,
    ApproveAnnotation,
    RateEntryOrMedia
  }

}
