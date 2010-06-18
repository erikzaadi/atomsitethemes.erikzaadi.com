/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using System.Xml.Linq;
  using System.Xml.Serialization;

  [XmlRoot("service", Namespace="http://www.w3.org/2007/app")]
  public class AppService : XmlBase, IXmlSerializable
  {
//    public static AppService DefaultService = new AppService(new XElement(
//  @"<service xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom' xmlns:svc='http://atomsite.net/info/Service' xmlns:file='http://atomsite.net/info/FileRepository' xmlns:plug='http://atomsite.net/info/Plugins' svc:theme='blue' xml:base='http://localhost:1333/'>
//  <svc:admin>Admin</svc:admin>
//  <workspace svc:default='true'>
//    <atom:title>example.com</atom:title>
//    <atom:subtitle>The life and times of a blogger.</atom:subtitle>
//    <collection href='blog.atom' svc:default='true' svc:bloggingOn='true'>
//      <atom:title>My Blog</atom:title>
//      <atom:id>tag:example.com,2009:blog</atom:id>
//    </collection>
//    <collection href='pages.atom' svc:dated='false' svc:ratingsOn='false' svc:annotationsOn='false' svc:bloggingOn='true'>
//      <atom:title>My Pages</atom:title>
//      <atom:id>tag:example.com,2009:pages</atom:id>
//    </collection>
//    <collection href='media.atom' svc:visible='false' svc:syndicationOn='no'>
//      <atom:title>My Media</atom:title>
//      <atom:id>tag:example.com,2009:media</atom:id>
//      <accept svc:ext='png'>image/png</accept>
//      <accept svc:ext='jpg'>image/jpeg</accept>
//      <accept svc:ext='gif'>image/gif</accept>
//    </collection>
//  </workspace>
//</service>"));

    public AppService() : this(new XElement(Atom.AppNs + "service")) { }
    public AppService(XElement xml)
      : base(xml)
    {
      xml.SetAttributeValue(XNamespace.Xmlns + "atom", Atom.AtomNs.NamespaceName);
      xml.SetAttributeValue(XNamespace.Xmlns + "svc", Atom.SvcNs.NamespaceName);
    }

    public IEnumerable<AppWorkspace> Workspaces
    {
      get { return GetXmlValues<AppWorkspace>(Atom.AppNs + "workspace"); }
      set { SetXmlValues<AppWorkspace>(Atom.AppNs + "workspace", value); }
    }

    /// <summary>
    /// Gets or sets how the service will function with multiple workspaces.
    /// </summary>
    public ServiceType ServiceType
    {
      get
      {
        string type = (string)Xml.Attribute(Atom.SvcNs + "type");
        if (string.IsNullOrEmpty(type)) return ServiceType.Single;
        switch (type.ToLowerInvariant())
        {
          case "multifolder": return ServiceType.MultiFolder;
          case "multisubdomain": return ServiceType.MultiSubdomain;
          case "single": return ServiceType.Single;
          default: return ServiceType.Single;//throw new InvalidCastException(string.Format("The service type of {0} is unrecognized.", type));
        }
      }
      set { SetProperty<string>(Atom.SvcNs + "type", value.ToString()); }
    }

    /// <summary>
    /// Gets or sets the default sub domain.  This should be left blank if not using a subdomain.
    /// </summary>
    /// <value>The subdomain.</value>
    public string DefaultSubdomain
    {
      get { return GetProperty<string>(Atom.SvcNs + "defaultSubdomain"); }
      set { SetProperty<string>(Atom.SvcNs + "defaultSubdomain", value); }
    }

    /// <summary>
    /// Gets or sets whether the service supports SSL for secure publishing.
    /// Secure = https:// and Non-secure = http://
    /// </summary>
    /// <value>True for default, false otherwise.</value>
    public bool Secure
    {
      get { return GetBooleanProperty(Atom.SvcNs + "secure") ?? false; }
      set { SetBooleanProperty(Atom.SvcNs + "secure", value == false ? new bool?() : new bool?(value)); }
    }


    /// <summary>
    /// Gets or sets the theme for all collections in all workspaces.
    /// This value can be overridden by the value set on the workspace and/or collection.
    /// </summary>
    /// <value>The name.</value>
    public string Theme
    {
      get { return GetProperty<string>(Atom.SvcNs + "theme"); }
      set { SetProperty<string>(Atom.SvcNs + "theme", value); }
    }

    /// <summary>
    /// Gets or sets the administrators which have access to all workspaces
    /// and all collections according to the role matrix.  These are the
    /// only users that can add new workspaces.
    /// </summary>
    /// <value>The administrators.</value>
    public IEnumerable<string> Admins
    {
      get { return GetValues<string>(Atom.SvcNs + "admin"); }
      set { SetValues<string>(Atom.SvcNs + "admin", value); }
    }

    /// <summary>
    /// Gets or sets the plugin state which contains the merit, installed state, and
    /// enabled state for each plugin.
    /// </summary>
    public IEnumerable<PluginState> Plugins
    {
      get { return GetXmlValues<PluginState>(Atom.SvcNs + "plugin"); }
      set { SetXmlValues<PluginState>(Atom.SvcNs + "plugin", value); }
    }

    /// <summary>
    /// Gets or sets the widget area includes to display for all workspaces and collections.
    /// Note: the includes are combined with includes in the other scopes.
    /// </summary>
    public IEnumerable<ServiceWidget> Widgets
    {
      get { return GetXmlValues<ServiceWidget>(Atom.SvcNs + "widget"); }
      set { SetXmlValues<ServiceWidget>(Atom.SvcNs + "widget", value); }
    }

    /// <summary>
    /// Gets or sets the page area includes to display for all workspaces and collections.
    /// Note: the includes are combined with includes in the other scopes.
    /// </summary>
    public IEnumerable<ServicePage> Pages
    {
      get { return GetXmlValues<ServicePage>(Atom.SvcNs + "page"); }
      set { SetXmlValues<ServicePage>(Atom.SvcNs + "page", value); }
    }

    /// <summary>
    /// Gets or sets the role matrix which determines the actions that
    /// each role is allowed to perform.
    /// </summary>
    public RoleMatrix RoleMatrix
    {
      get { return GetXmlValue<RoleMatrix>(Atom.SvcNs + "roleMatrix"); }
      set { SetXmlValue<RoleMatrix>(Atom.SvcNs + "roleMatrix", value); }
    }

    /// <summary>
    /// Gets a workspace by the name of the workspace or returns the default
    /// workspace when name is not specified or resolved.
    /// </summary>
    /// <param name="workspace">Name of workspace.</param>
    /// <returns>AppWorkspace</returns>
    public AppWorkspace GetWorkspace(string workspace)
    {
      AppWorkspace w = null;
      if (!string.IsNullOrEmpty(workspace))
        w = Workspaces.Where(ws => ws.Name == workspace || (string.IsNullOrEmpty(ws.Name) && workspace == Atom.DefaultWorkspaceName)).SingleOrDefault();

      //get default when not found
      if (w == null)
        w = GetWorkspace();

      return w;
    }

    /// <summary>
    /// Gets the default workspace.
    /// </summary>
    /// <returns>AppWorkspace</returns>
    public AppWorkspace GetWorkspace()
    {
      AppWorkspace w = Workspaces.Where(ws => ws.Default).SingleOrDefault();
      if (w == null) throw new ResourceNotFoundException("default workspace", "(default)");
      return w;
    }

    /// <summary>
    /// Gets a collection by the workspace name and the collection name.
    /// </summary>
    /// <param name="workspace">Name of workspace.</param>
    /// <param name="collection">Name of collection.</param>
    /// <returns>AppCollection</returns>
    public AppCollection GetCollection(string workspace, string collection)
    {
      return GetWorkspace(workspace).GetCollection(collection);
    }

    /// <summary>
    /// Gets a collection by the id.
    /// </summary>
    /// <param name="id">Id of collection.</param>
    /// <returns>AppCollection</returns>
    public AppCollection GetCollection(Id id)
    {
      return GetWorkspace(id.Workspace).GetCollection(id.Collection);
    }

    /// <summary>
    /// Gets the default collection from the default workspace.
    /// </summary>
    /// <returns></returns>
    public AppCollection GetCollection()
    {
      return GetWorkspace().GetCollection();
    }

    public IEnumerable<AppCollection> GetCollections(IEnumerable<Scope> scopes)
    {
      return scopes.Where(s => s.IsCollection).Select(s => GetCollection(s.Workspace, s.Collection));
    }

    public IEnumerable<AppCollection> GetCollections(Scope scope)
    {
      if (scope.IsCollection) return Enumerable.Repeat(GetCollection(scope.Workspace, scope.Collection), 1);
      if (scope.IsWorkspace) return GetWorkspace(scope.Workspace).Collections;
      return Workspaces.SelectMany(w => w.Collections);      
    }

    public IEnumerable<ServicePage> GetServicePages(Scope scope, string pageName, string parentName)
    {
      //find service pages that apply to this page based on name and parent using fallback rules
      var pages = Pages.Where(p => p.Name == parentName || p.Name == pageName);

      if (scope.IsWorkspace || scope.IsCollection) pages = pages
        .Concat(this.GetWorkspace(scope.Workspace).Pages
          .Where(p => p.Name == parentName || p.Name == pageName));

      if (scope.IsCollection) pages = pages
        .Concat(this.GetCollection(scope.Workspace, scope.Collection).Pages
          .Where(p => p.Name == parentName || p.Name == pageName));

      return pages;
    }

    public IEnumerable<Include> GetIncludes(Scope scope, string pageName, string parentName)
    {
      //using the page name, we can lookup all widgets that will display on the page
      //this allows us to update the page model, specifically for adding script and
      //style dependencies
      var pages = GetServicePages(scope, pageName, parentName);

      var includes =
        //get all the widgets at the default level and the page level for all scopes
          pages.SelectMany(w => w.Areas).SelectMany(area => area.Includes);

      var includeNames = includes.Select(i => i.Name).Distinct();

      //concatenate all the sub-widgets
      includes = includes.Concat(Widgets.Where(w => includeNames.Contains(w.Name))
        .SelectMany(w => w.Areas).SelectMany(area => area.Includes));

      if (scope.IsWorkspace || scope.IsCollection)
        includes = includes.Concat(this.GetWorkspace(scope.Workspace).Widgets.Where(w => includeNames.Contains(w.Name))
          .SelectMany(w => w.Areas).SelectMany(area => area.Includes));

      if (scope.IsCollection)
        includes = includes.Concat(this.GetCollection(scope.Workspace, scope.Collection).Widgets.Where(w => includeNames.Contains(w.Name))
          .SelectMany(w => w.Areas).SelectMany(area => area.Includes));

      return includes;
    }

    /// <summary>
    /// Gets the author by name from the authors in a workspace, collection, or the
    /// authors and administrators at the service level.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    //public AtomPerson GetAuthorByName(Id id, string userId)
    //{
    //  //bubble up
    //  AtomPerson p = GetCollection(id).Authors.Where(a => a.Name == name).SingleOrDefault();
    //  if (p == null) p = GetWorkspace(id.Workspace).Authors.Where(a => a.Name == name).SingleOrDefault();
    //  if (p == null) p = Admins.Where(a => a.Name == name).Select(a =>
    //    new AtomPerson(Atom.AtomNs + "author") { Name = a.Name, Email = a.Email, Uri = a.Uri }).SingleOrDefault();
    //  if (p == null) throw new AuthorOrAdminNotFoundException(name);
    //  return p;
    //}

    public void Save(string path)
    {
      Xml.Save(path);
    }

    public static AppService Load(string path)
    {
      AppService service = new AppService();
      service.Xml = XElement.Load(path, LoadOptions.None);
      return service;
    }

    #region IXmlSerializable Members

    public System.Xml.Schema.XmlSchema GetSchema()
    {
      throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
      Xml = XElement.Load(reader, LoadOptions.SetBaseUri);
    }

    public void WriteXml(XmlWriter writer)
    {
      //TODO: use base.WriteXml
      bool start = false;
      if (writer.WriteState == System.Xml.WriteState.Start)
      {
        start = true;
        writer.WriteStartDocument();
        writer.WriteStartElement("service", Atom.AppNs.NamespaceName);
      }
      if (string.IsNullOrEmpty(writer.LookupPrefix(Atom.AtomNs.NamespaceName)))
        writer.WriteAttributeString("xmlns", "atom", null, Atom.AtomNs.NamespaceName);
      foreach (XAttribute att in Xml.Attributes())
      {
        if (att.IsNamespaceDeclaration && (att.Value == Atom.AppNs.NamespaceName)) continue;
        writer.WriteAttributeString(att.Name.LocalName, att.Name.NamespaceName, att.Value);
      }
      foreach (XNode el in Xml.Nodes())
      {
        el.WriteTo(writer);
      }
      if (start)
      {
        writer.WriteEndElement();
        writer.WriteEndDocument();
      }
    }

    #endregion

    public IEnumerable<ServicePage> GetPages(Scope scope)
    {
      return scope.IsEntireSite ? Pages :
                scope.IsWorkspace ? GetWorkspace(scope.Workspace).Pages :
                GetCollection(scope.Workspace, scope.Collection).Pages;
    }

    public ServicePage GetPage(Scope scope, string pageName)
    {
      return GetPages(scope).Where(p => p.Name == pageName).Single();
    }

    public IEnumerable<ServiceWidget> GetWidgets(Scope scope)
    {
      return scope.IsEntireSite ? Widgets :
                scope.IsWorkspace ? GetWorkspace(scope.Workspace).Widgets :
                GetCollection(scope.Workspace, scope.Collection).Widgets;
    }

    public ServiceWidget GetWidget(Scope scope, string widgetName)
    {
      return GetWidgets(scope).Where(p => p.Name == widgetName).Single();
    }

    public ServiceArea GetArea(Scope scope, string targetName, bool isPage, string areaName)
    {
      TargetBase t = isPage ? (TargetBase)GetPage(scope, targetName) : (TargetBase)GetWidget(scope, targetName);
      return t.Areas.Where(a => a.Name == areaName).Single();
    }

    private IEnumerable<T> GetTargets<T>(Scope scope) where T : TargetBase
    {
      return typeof(T) == typeof(ServicePage) ? 
        GetPages(scope).Cast<T>() : GetWidgets(scope).Cast<T>();
    }

    private T AddTarget<T>(Scope scope, string targetName) where T : TargetBase
    {
      IEnumerable<T> targets = GetTargets<T>(scope);
      var list = targets.ToList();

      if (list.Where(p => p.Name == targetName).Count() > 0)
        throw new Exception("Target already exists in scope."); // TODO: create real exception

      T target = Activator.CreateInstance<T>();
      target.Name = targetName;
      list.Add(target);

      if (target is ServicePage)
      {
        if (scope.IsEntireSite) Pages = list.Cast<ServicePage>();
        else if (scope.IsWorkspace) GetWorkspace(scope.Workspace).Pages = list.Cast<ServicePage>();
        else GetCollection(scope.Workspace, scope.Collection).Pages = list.Cast<ServicePage>();
      }
      else
      {
        if (scope.IsEntireSite) Widgets = list.Cast<ServiceWidget>();
        else if (scope.IsWorkspace) GetWorkspace(scope.Workspace).Widgets = list.Cast<ServiceWidget>();
        else GetCollection(scope.Workspace, scope.Collection).Widgets = list.Cast<ServiceWidget>();
      }
      return target;
    }

    public ServicePage AddPage(Scope scope, string pageName)
    {
      return AddTarget<ServicePage>(scope, pageName);
      //IEnumerable<ServicePage> pages = GetPages(scope);

      //var list = pages.ToList();

      //if (list.Where(p => p.Name == pageName).Count() > 0)
      //  throw new Exception("Page already exists in scope."); // TODO: create real exception

      //var page = new ServicePage() { Name = pageName };
      //list.Add(page);
      //pages = list;

      //return page;
    }

    public ServiceWidget AddWidget(Scope scope, string widgetName)
    {
      return AddTarget<ServiceWidget>(scope, widgetName);
      //IEnumerable<ServiceWidget> widgets = GetWidgets(scope);

      //var list = widgets.ToList();

      //if (list.Where(p => p.Name == widgetName).Count() > 0)
      //  throw new Exception("Widget already exists in scope."); // TODO: create real exception

      //var w = new ServiceWidget() { Name = widgetName };
      //list.Add(w);
      //widgets = list;

      //return w;
    }

    private void RemoveTarget<T>(Scope scope, string targetName) where T : TargetBase
    {
      IEnumerable<T> targets = GetTargets<T>(scope);
      var list = targets.ToList();
      var remove = list.Where(t => t.Name == targetName).ToArray();
      for (int i = remove.Length - 1; i >= 0; i--)
      {
        list.Remove(remove[i]);

        // update AppService at correct level
        if (remove[i] is ServicePage)
        {
          if (scope.IsEntireSite) Pages = list.Cast<ServicePage>();
          else if (scope.IsWorkspace) GetWorkspace(scope.Workspace).Pages = list.Cast<ServicePage>();
          else GetCollection(scope.Workspace, scope.Collection).Pages = list.Cast<ServicePage>();
        }
        else
        {
          if (scope.IsEntireSite) Widgets = list.Cast<ServiceWidget>();
          else if (scope.IsWorkspace) GetWorkspace(scope.Workspace).Widgets = list.Cast<ServiceWidget>();
          else GetCollection(scope.Workspace, scope.Collection).Widgets = list.Cast<ServiceWidget>();
        }
      }
    }

    public void RemovePage(Scope scope, string pageName)
    {
      RemoveTarget<ServicePage>(scope, pageName);
    }

    public void RemoveWidget(Scope scope, string widgetName)
    {
      RemoveTarget<ServiceWidget>(scope, widgetName);
    }

    public ServiceArea AddArea<T>(Scope scope, string targetName, string areaName) where T : TargetBase
    {
      IEnumerable<T> targets = GetTargets<T>(scope);
      T target = targets.Where(p => p.Name == targetName).SingleOrDefault();
      if (target == null) target = AddTarget<T>(scope, targetName);

      if (target.Areas.Where(a => a.Name == areaName).Count() > 0)
        throw new Exception("Area already exists in target."); // TODO: create real exception

      var list = target.Areas.ToList();
      var area = new ServiceArea() { Name = areaName };
      list.Add(area);
      target.Areas = list;
      return area;
    }

    public void RemoveArea<T>(Scope scope, string targetName, string areaName) where T : TargetBase
    {
      IEnumerable<T> targets = GetTargets<T>(scope);

      T target = targets.Where(p => p.Name == targetName).Single();
      var list = target.Areas.ToList();
      var area = list.Find(a => a.Name == areaName);
      list.Remove(area);
      target.Areas = list;
    }

    public Include AddInclude<T>(Scope scope, string targetName, string areaName, string includeName) where T : TargetBase
    {
      IEnumerable<T> targets = GetTargets<T>(scope);
      T target = targets.Where(p => p.Name == targetName).SingleOrDefault();
      if (target == null) target = AddTarget<T>(scope, targetName);

      var area = target.Areas.Where(a => a.Name == areaName).SingleOrDefault();
      if (area == null) area = AddArea<T>(scope, targetName, areaName);

      var list = area.Includes.ToList();
      var include = new Include() { Name = includeName };
      list.Add(include);
      area.Includes = list;
      return include;
    }

    public void RemoveInclude<T>(Scope scope, string targetName, string areaName, int includeIdx) where T : TargetBase
    {
      IEnumerable<T> targets = GetTargets<T>(scope);

      var area = targets.Where(p => p.Name == targetName).Single().Areas.Where(a => a.Name == areaName).Single();
      var list = area.Includes.ToList();
      list.RemoveAt(includeIdx);
      area.Includes = list;
    }

    public AppWorkspace AddWorkspace(AppWorkspace w)
    {
        //TODO: detect duplicates
      var ws = Workspaces.ToList();
      ws.Add(w);
      Workspaces = ws;
      return w;
    }
    public AppWorkspace RemoveWorkspace(AppWorkspace w)
    {
        //TODO: detect duplicates
      var ws = Workspaces.ToList();
      ws.Remove(w);
      Workspaces = ws;
      return w;
    }

    public string AddAdmin(string p)
    {
        var admins = Admins.ToList();
        admins.Add(p);
        Admins = admins;
        return p;
    }

    public string GetIncludePath(Include include)
    {
      var area = new ServiceArea(include.Xml.Parent);
      TargetBase target = null;
      if (area.Xml.Parent.Name.LocalName == "page")
      {
        target = new ServicePage(area.Xml.Parent);
      }
      else
      {
        target = new ServiceWidget(area.Xml.Parent);
      }
      if (target.Xml.Parent.Name.LocalName == "workspace")
      {
        return string.Format("include-{0}-{1}-{2}-{3}-{4}", new AppWorkspace(target.Xml.Parent).Name ?? Atom.DefaultWorkspaceName,
          target.Xml.Name.LocalName, target.Name, area.Name, area.Includes.ToList().IndexOf(include));
      }
      else if (target.Xml.Parent.Name.LocalName == "collection")
      {
        var coll = new AppCollection(target.Xml.Parent);
        return string.Format("include-{0}-{1}-{2}-{3}-{4}-{5}", coll.Scope.Workspace ?? Atom.DefaultWorkspaceName,
          coll.Scope.Collection, target.Xml.Name.LocalName, target.Name, area.Name, area.Includes.ToList().IndexOf(include));
      }
      return string.Format("include-{0}-{1}-{2}-{3}", target.Xml.Name.LocalName, target.Name, area.Name, area.Includes.ToList().IndexOf(include));
    }

    public T GetInclude<T>(string includePath) where T : Include
    {
      var paths = includePath.Split('-');
      Scope scope = Scope.EntireSite;
      string targetType = "page";
      string targetName = null;
      string areaName = null;
      int includeIdx = 0;
      if (paths.Length == 7)
      {
        scope = new Scope() { Workspace = paths[1], Collection = paths[2] };
        targetType = paths[3];
        targetName = paths[4];
        areaName = paths[5];
        includeIdx = int.Parse(paths[6]);
      }
      else if (paths.Length == 6)
      {
        scope = new Scope() { Workspace = paths[1] };
        targetType = paths[2];
        targetName = paths[3];
        areaName = paths[4];
        includeIdx = int.Parse(paths[5]);
      }
      else
      {
        targetType = paths[1];
        targetName = paths[2];
        areaName = paths[3];
        includeIdx = int.Parse(paths[4]);
      }
      var include = GetArea(scope, targetName, (targetType == "page"), areaName).Includes.ElementAt(includeIdx);
      var i = Activator.CreateInstance<T>();
      i.Xml = include.Xml;
      return i;
    }

    public IEnumerable<string> GetPeopleInScope(Scope scope)
    {
      if (scope.IsCollection)
      {
        var c = GetCollection(scope.Workspace, scope.Collection);
        foreach (var p in c.People) yield return p;
      }
      if (scope.IsWorkspace || scope.IsCollection)
      {
        var w = GetWorkspace(scope.Workspace);
        foreach (var p in w.People) yield return p;
      }
      foreach (var p in Admins) yield return p;
    }
  }
}
