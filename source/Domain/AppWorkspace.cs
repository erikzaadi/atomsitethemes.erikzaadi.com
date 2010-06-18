/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml.Linq;

  public class AppWorkspace : XmlBase
  {
    public AppWorkspace() : this(new XElement(Atom.AppNs + "workspace")) { }
    public AppWorkspace(XElement xml) : base(xml) { }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>The title.</value>
    public AtomText Title
    {
      get { return GetXmlValue<AtomText>(Atom.AtomNs + "title"); }
      set { SetXmlValue<AtomText>(Atom.AtomNs + "title", value); }
    }

    /// <summary>
    /// Gets or sets the subtitle.  This is an extension. 
    /// </summary>
    /// <value>The subtitle.</value>
    public AtomText Subtitle
    {
      get { return GetXmlValue<AtomText>(Atom.AtomNs + "subtitle"); }
      set { SetXmlValue<AtomText>(Atom.AtomNs + "subtitle", value); }
    }

    /// <summary>
    /// Gets or sets the name.  This is an extension.
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
      get { return GetProperty<string>(Atom.SvcNs + "name"); }
      set { SetProperty<string>(Atom.SvcNs + "name", value); }
    }

    /// <summary>
    /// Gets or sets whether this is the default workspace.  This is an extension.
    /// </summary>
    /// <value>True for default, false otherwise.</value>
    public bool Default
    {
      get { return GetBooleanProperty(Atom.SvcNs + "default") ?? false; }
      set { SetBooleanProperty(Atom.SvcNs + "default", value); }
    }

    /// <summary>
    /// Gets or sets the role matrix which determines the actions that
    /// each role is allowed to perform.  Actions specified here will
    /// override those actions specified at a higher level.
    /// </summary>
    public RoleMatrix RoleMatrix
    {
      get { return GetXmlValue<RoleMatrix>(Atom.SvcNs + "roleMatrix"); }
      set { SetXmlValue<RoleMatrix>(Atom.SvcNs + "roleMatrix", value); }
    }


    /// <summary>
    /// Gets or sets the widget area includes to display for this workspaces.
    /// Note: the includes are combined with includes in the other scopes.
    /// </summary>
    public IEnumerable<ServiceWidget> Widgets
    {
      get { return GetXmlValues<ServiceWidget>(Atom.SvcNs + "widget"); }
      set { SetXmlValues<ServiceWidget>(Atom.SvcNs + "widget", value); }
    }

    /// <summary>
    /// Gets or sets the page area includes to display for this workspace.
    /// Note: the includes are combined with includes in the other scopes.
    /// </summary>
    public IEnumerable<ServicePage> Pages
    {
      get { return GetXmlValues<ServicePage>(Atom.SvcNs + "page"); }
      set { SetXmlValues<ServicePage>(Atom.SvcNs + "page", value); }
    }

    /// <summary>
    /// Gets or sets the collections.
    /// </summary>
    /// <value>The collections.</value>
    public IEnumerable<AppCollection> Collections
    {
      get { return GetXmlValues<AppCollection>(Atom.AppNs + "collection"); }
      set { SetXmlValues<AppCollection>(Atom.AppNs + "collection", value); }
    }

    /// <summary>
    /// Gets or sets the author ids for the entire workspace.  See role matrix
    /// for actions that an author can perform on any collection in this
    /// workspace.  This is an extension.
    /// </summary>
    /// <value>The authors.</value>
    public IEnumerable<string> Authors
    {
      get { return GetValues<string>(Atom.SvcNs + "author"); }
      set { SetValues<string>(Atom.SvcNs + "author", value); }
    }

    /// <summary>
    /// Gets or sets the contributor ids for the entire workspace.  See role matrix
    /// for actions that a contributor can perform on any collection in this
    /// workspace.  This is an extension.
    /// </summary>
    /// <value>The authors.</value>
    public IEnumerable<string> Contributors
    {
      get { return GetValues<string>(Atom.SvcNs + "contributor"); }
      set { SetValues<string>(Atom.SvcNs + "contributor", value); }
    }

    /// <summary>
    /// Gets all the authors and contributors.
    /// </summary>
    public IEnumerable<string> People
    {
      get { return Authors.Union(Contributors); }
    }

    /// <summary>
    /// Gets or sets the theme that all collections in the workspace will use.
    /// Collections may override this value with their own theme value. This
    /// is an extension.
    /// </summary>
    /// <value>The name.</value>
    public string Theme
    {
      get { return GetProperty<string>(Atom.SvcNs + "theme"); }
      set { SetProperty<string>(Atom.SvcNs + "theme", value); }
    }

    /// <summary>
    /// Gets the collection by the colleciton name.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    public AppCollection GetCollection(string collection)
    {
      AppCollection c = Collections.Where(col => (col.Id != null) && (col.Id.Collection == collection)).SingleOrDefault();
      if (c == null) throw new ResourceNotFoundException("collection", collection);
      return c;
    }

    /// <summary>
    /// Gets the default collection in the workspace.
    /// </summary>
    /// <returns>The app collection.</returns>
    public AppCollection GetCollection()
    {
      AppCollection c = Collections.Where(col => col.Default).SingleOrDefault();
      if (c == null) throw new ResourceNotFoundException("default collection", "(default)");
      return c;
    }

    public AppCollection AddCollection(AppCollection c)
    {
      var cs = Collections.ToList();
      cs.Add(c);
      Collections = cs;
      return c;
    }
    public AppCollection RemoveCollection(AppCollection c)
    {
      var cs = Collections.ToList();
      cs.Remove(c);
      Collections = cs;
      return c;
    }
  }
}
