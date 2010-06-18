/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Linq;
  using System.Web.Mvc;
  using AtomSite.Domain;
  public partial class AdminController : BaseController
  {

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Get)]
    public ViewResult Settings(string workspace, string collection)
    {
      string view = "AdminSettings";
      AdminModel m = null;
      if (Scope.IsEntireSite)
      {
        var esm = TempData["model"] as AdminSettingsEntireSiteModel;
        if (esm == null)
        {
          esm = new AdminSettingsEntireSiteModel();
          esm.SiteAddress = AppService.Base;
          esm.DefaultSubdomain = AppService.DefaultSubdomain;
          esm.ServiceType = AppService.ServiceType.ToString();
          esm.Secure = AppService.Secure;
        }
        view = "AdminSettingsEntireSite";
        m = esm;
      }
      else if (Scope.IsWorkspace)
      {
        var wm = TempData["model"] as AdminSettingsWorkspaceModel;
        if (wm == null)
        {
          wm = new AdminSettingsWorkspaceModel();
          //wm.Workspace = workspace;
          wm.Title = Workspace.Title.Text;
          wm.Subtitle = Workspace.Subtitle != null ? Workspace.Subtitle.Text : null;
          wm.Name = Workspace.Name;
        }
        view = "AdminSettingsWorkspace";
        m = wm;
      }
      else
      {
        var cm = TempData["model"] as AdminSettingsCollectionModel;
        if (cm == null)
        {
          cm = new AdminSettingsCollectionModel();
          cm.CollectionId = Collection.Id;
          cm.Title = Collection.Title.Text;
          cm.Subtitle = Collection.Subtitle != null ? Collection.Subtitle.Text : null;
          cm.Owner = Collection.Id.Owner;
          cm.Name = Collection.Id.Collection;
          cm.Dated = Collection.Dated;
          cm.Visible = Collection.Visible;
          cm.AnnotationsOn = Collection.AnnotationsOn;
          cm.SyndicationOn = Collection.SyndicationOn;
        }
        view = "AdminSettingsCollection";
        m = cm;
      }
      if (m == null) m = new AdminModel();
      if (TempData["new"] != null) m.Notifications.Add((string)TempData["new"], "was created successfully! Changes may take some time before they appear.");
      else if (TempData["saved"] != null) m.Notifications.Add("Saved", "settings successfully! Changes may take some time before they appear.");
      return View(view, "Admin", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Get)]
    public ViewResult NewWorkspace()
    {
      var m = new AdminSettingsWorkspaceModel();
      return View("AdminSettingsWorkspace", "Admin", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult NewWorkspace(AdminSettingsWorkspaceModel m)
    {
      try
      {
        if (string.IsNullOrEmpty(m.Name)) throw new Exception("Name is required.");
        if (m.Name.ToLowerInvariant() != m.Name.CleanSlug().ToLowerInvariant()) throw new Exception("Name has invalid characters.");
        string workspace = m.Name.CleanSlug().ToLowerInvariant();
        AppWorkspace w = new AppWorkspace() { Name = workspace };
        if (string.IsNullOrEmpty(m.Title)) throw new Exception("Title is required.");
        w.Title = new AtomTitle() { Text = m.Title };
        w.Subtitle = string.IsNullOrEmpty(m.Subtitle) ? null : new AtomSubtitle() { Text = m.Subtitle };
        
        AppService.AddWorkspace(w);
        AtomPubService.UpdateService(AppService);
        TempData["new"] = "New Workspace";

        return RedirectToAction("Settings", new { workspace = workspace });
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        m.Errors.Add(ex.Message);
      }
      return View("AdminSettingsWorkspace", "Admin", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult UpdateWorkspace(string workspace, AdminSettingsWorkspaceModel m)
    {
      try
      {
        AppWorkspace w = AppService.GetWorkspace(workspace);
        if (string.IsNullOrEmpty(m.Title)) throw new Exception("Title is required.");
        w.Title = new AtomTitle() { Text = m.Title };
        w.Subtitle = string.IsNullOrEmpty(m.Subtitle) ? null : new AtomSubtitle() { Text = m.Subtitle };

        AtomPubService.UpdateService(AppService);
        TempData["saved"] = true;

        return RedirectToAction("Settings", new { workspace = workspace });
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        m.Errors.Add(ex.Message);
      }
      return View("AdminSettingsWorkspace", "Admin", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Get)]
    public ViewResult NewCollection()
    {
      AdminSettingsCollectionModel cm = new AdminSettingsCollectionModel();
      return View("AdminSettingsCollection", "Admin", cm);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult NewCollection(string workspace, AdminSettingsCollectionModel m)
    {
      try
      {
        if (string.IsNullOrEmpty(m.Owner)) throw new Exception("Owner is required.");
        if (!string.IsNullOrEmpty(workspace) && workspace != Atom.DefaultWorkspaceName)
        {
          if (!m.Owner.StartsWith(workspace)) throw new Exception("Owner must start with workspace name: workspace.example.com");
        }

        if (string.IsNullOrEmpty(m.Name)) throw new Exception("Name is required.");
        if (m.Name.ToLowerInvariant() != m.Name.CleanSlug().ToLowerInvariant()) throw new Exception("Name has invalid characters.");
        string collection = m.Name.ToLowerInvariant();

        if (Workspace.Collections.Where(coll => coll.Id.Collection == collection).Count() > 0) 
            throw new Exception("The collection already exists.  Please choose a different name.");
        
        AppWorkspace w = AppService.GetWorkspace(workspace);
        bool @default = w.Collections.Count() == 0;

        AppCollection c = new AppCollection() { Id = new Id(m.Owner, collection), Dated = m.Dated ?? false, Default = @default };

        if (string.IsNullOrEmpty(m.Title)) throw new Exception("Title is required.");
        c.Title = new AtomTitle() { Text = m.Title };
        c.Subtitle = string.IsNullOrEmpty(m.Subtitle) ? null : new AtomSubtitle() { Text = m.Subtitle };
        c.AnnotationsOn = m.AnnotationsOn ?? false;
        c.Visible = m.Visible ?? false;
        c.SyndicationOn = m.SyndicationOn ?? false;
        c.Href = new Uri(collection + ".atom", UriKind.Relative);

        w.AddCollection(c);
        AtomPubService.UpdateService(AppService);
        ServerApp.Restart();
        TempData["new"] = "New Collection";
        return RedirectToAction("Settings", new { workspace = workspace, collection = collection });
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        m.Errors.Add(ex.Message);
      }
      return View("AdminSettingsCollection", "Admin", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult UpdateCollection(string workspace, string collection, AdminSettingsCollectionModel m)
    {
      try
      {
        AppCollection c = AppService.GetCollection(workspace, collection);
        m.CollectionId = c.Id;
        if (string.IsNullOrEmpty(m.Title)) throw new Exception("Title is required.");
        c.Title = new AtomTitle() { Text = m.Title };
        c.Subtitle = string.IsNullOrEmpty(m.Subtitle) ? null : new AtomSubtitle() { Text = m.Subtitle };
        c.AnnotationsOn = m.AnnotationsOn ?? false;
        c.Visible = m.Visible ?? false;
        c.SyndicationOn = m.SyndicationOn ?? false;
        AtomPubService.UpdateService(AppService);
        ServerApp.Restart();
        TempData["saved"] = true;
        return RedirectToAction("Settings", new { workspace = workspace, collection = collection });
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        m.Errors.Add(ex.Message);
      }
      TempData["model"] = m;
      return RedirectToAction("Settings", new { workspace = workspace, collection = collection });
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult SetDefaultWorkspace(string workspace)
    {
      //TODO: move to service
      try
      {
        AppWorkspace w = AppService.GetWorkspace(workspace);
        AppWorkspace defaultW = AppService.GetWorkspace();
        if (defaultW != w)
        {
          defaultW.Default = false;
          w.Default = true;
          AtomPubService.UpdateService(AppService);
        }
        return PartialView("AdminWorkspacesWidget", new AdminModel());
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { error = ex.Message });
      }
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult SetDefaultCollection(string workspace, string collection)
    {
      //TODO: move to service
      try
      {
        AppCollection c = AppService.GetCollection(workspace, collection);
        AppCollection defaultC = AppService.GetWorkspace(workspace).GetCollection();
        if (defaultC != c)
        {
          defaultC.Default = false;
          c.Default = true;
          AtomPubService.UpdateService(AppService);
        }
        return CollectionsWidget(workspace);
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { error = ex.Message });
      }
    }

    [ScopeAuthorize(EntireSite=true), AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult Administrators()
    {
      var m = new AdminUsersModel();
      m.Title = "Administrators";
      m.AddLinks.Add("Add", Url.Action("SelectAdministrator"));
      m.CanRemove = true;
      m.GetRemoveHref = (u) => Url.Action("RemoveAdministrator", "Admin", new { userId = u.Ids.First() });
      m.CanEdit = true;
      int total; //TODO: paging
      m.Users = UserRepository.GetUsers(0, int.MaxValue, out total)
        .Where(u => u.Ids.Intersect(AppService.Admins).Count() == 1).ToPagedList(0, int.MaxValue, total);

      return PartialView("AdminUsersWidget", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult People(string workspace, string collection)
    {
      var m = new AdminUsersModel();
      m.Title = "Authors and Contributors";
      m.AddLinks.Add("Add Author", Url.Action("SelectAuthor", "Admin", new { workspace = Scope.Workspace, collection = Scope.Collection }));
      m.AddLinks.Add("Add Contributor", Url.Action("SelectContributor", "Admin", new { workspace = Scope.Workspace, collection = Scope.Collection }));
      m.CanRemove = true;
      m.GetRemoveHref = (u) => Url.Action("RemovePerson", "Admin", new { workspace = Scope.Workspace, collection = Scope.Collection, userId = u.Ids.First() });
      m.CanEdit = true;
      m.Service = AppService;
      int total; //TODO: paging
      var w = AppService.GetWorkspace(Scope.Workspace);
      if (Scope.IsCollection)
      {
        m.Users = UserRepository.GetUsers(0, int.MaxValue, out total)
          .Where(u => u.Ids.Intersect(w.GetCollection(Scope.Collection).Authors.Union(w.GetCollection(Scope.Collection).Contributors)).Count() == 1).ToPagedList(0, int.MaxValue, total);
      }
      else
      {
        m.Users = UserRepository.GetUsers(0, int.MaxValue, out total)
          .Where(u => u.Ids.Intersect(w.Authors.Union(w.Contributors)).Count() == 1).ToPagedList(0, int.MaxValue, total);
      }
      return PartialView("AdminUsersWidget", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult SelectAuthor(string workspace, string collection)
    {
      AdminUserSelectModel m = new AdminUserSelectModel();
      m.GetPostHref = (u) => Url.Action("AddAuthor", "Admin", new { workspace = Scope.Workspace, collection = Scope.Collection, userId = u.Ids.First() });
      m.CancelHref = Url.Action("People", new { workspace = workspace, collection = collection });
      m.SelectionTitle = "Select New Author";
      int total; //TODO: paging
      m.Users = UserRepository.GetUsers(0, int.MaxValue, out total)
        .Where(u => u.Ids.Intersect(Workspace.Authors).Count() == 0).ToPagedList(0, int.MaxValue, total);

      return PartialView("AdminUserSelect", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult SelectContributor(string workspace, string collection)
    {
      AdminUserSelectModel m = new AdminUserSelectModel();
      m.GetPostHref = (u) => Url.Action("AddContributor", "Admin", new { workspace = Scope.Workspace, collection = Scope.Collection, userId = u.Ids.First() });
      m.CancelHref = Url.Action("People", new { workspace = workspace, collection = collection });
      m.SelectionTitle = "Select New Contributor";
      int total; //TODO: paging
      m.Users = UserRepository.GetUsers(0, int.MaxValue, out total)
        .Where(u => u.Ids.Intersect(Workspace.Contributors).Count() == 0).ToPagedList(0, int.MaxValue, total);

      return PartialView("AdminUserSelect", m);
    }

    [ScopeAuthorize(EntireSite = true), AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult SelectAdministrator()
    {
      AdminUserSelectModel m = new AdminUserSelectModel();
      m.GetPostHref = (u) => Url.Action("AddAdministrator", "Admin", new { userId = u.Ids.First() });
      m.CancelHref = Url.Action("Administrators");
      m.SelectionTitle = "Select New Administrator";
      int total; //TODO: paging
      m.Users = UserRepository.GetUsers(0, int.MaxValue, out total)
        .Where(u => u.Ids.Intersect(AppService.Admins).Count() == 0).ToPagedList(0, int.MaxValue, total);

      return PartialView("AdminUserSelect", m);
    }

    [ScopeAuthorize(EntireSite=true), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult AddAdministrator(string userId)
    {
      //TODO: move to service
      try
      {
        var admins = AppService.Admins.ToList();
        if (!admins.Contains(userId)) admins.Add(userId);
        AppService.Admins = admins;
        AtomPubService.UpdateService(AppService);
        return Administrators();
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { error = ex.Message });
      }
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult AddAuthor(string workspace, string collection, string userId)
    {
      //TODO: move logic to service
      try
      {
        AppWorkspace w = AppService.GetWorkspace(workspace);
        if (Scope.IsCollection)
        {
          AppCollection c = w.GetCollection(collection);
          var authors = c.Authors.ToList();
          if (!authors.Contains(userId)) authors.Add(userId);
          c.Authors = authors;
        }
        else
        {
          var authors = w.Authors.ToList();
          if (!authors.Contains(userId)) authors.Add(userId);
          w.Authors = authors;
        }
        AtomPubService.UpdateService(AppService);
        return People(workspace, collection);
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { error = ex.Message });
      }
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult AddContributor(string workspace, string collection, string userId)
    {
      //TODO: move to service
      try
      {
        AppWorkspace w = AppService.GetWorkspace(workspace);
        if (Scope.IsCollection)
        {
          AppCollection c = w.GetCollection(collection);
          var contribs = c.Contributors.ToList();
          if (!contribs.Contains(userId)) contribs.Add(userId);
          c.Contributors = contribs;
        }
        else
        {
          var contribs = w.Contributors.ToList();
          if (!contribs.Contains(userId)) contribs.Add(userId);
          w.Contributors = contribs;
        }
        AtomPubService.UpdateService(AppService);
        return People(workspace, collection);
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { error = ex.Message });
      }
    }

    [ScopeAuthorize(EntireSite=true), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult RemoveAdministrator(string userId)
    {
      //TODO: move to service
      try
      {
        var admins = AppService.Admins.ToList();
        if (userId == ((User)User.Identity).Ids.First())
          throw new Exception("Can't remove yourself as an administrator.");
        if (admins.Contains(userId)) admins.Remove(userId);
        AppService.Admins = admins;
        AtomPubService.UpdateService(AppService);
        return Administrators();
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { error = ex.Message });
      }
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult RemovePerson(string workspace, string collection, string userId)
    {
      //TODO: move to service
      try
      {
        var w = AppService.GetWorkspace(workspace);
        if (Scope.IsCollection)
        {
          var c = w.GetCollection(collection);
          if (c.Authors.Contains(userId))
          {
            var p = c.Authors.ToList();
            p.Remove(userId);
            c.Authors = p;
          }
          if (c.Contributors.Contains(userId))
          {
            var p = c.Contributors.ToList();
            p.Remove(userId);
            c.Contributors = p;
          }
        }
        else
        {
          if (w.Authors.Contains(userId))
          {
            var p = w.Authors.ToList();
            p.Remove(userId);
            w.Authors = p;
          }
          if (w.Contributors.Contains(userId))
          {
            var p = w.Contributors.ToList();
            p.Remove(userId);
            w.Contributors = p;
          }
        }
        AtomPubService.UpdateService(AppService);
        return People(Scope.Workspace, Scope.Collection);
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { error = ex.Message });
      }
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Post)]
    public JsonResult ToggleAccepts(string workspace, string collection, string accepts)
    {
      //TODO: move to service
      try
      {
          string[] vals;
        //update the collection
        var c = AppService.GetCollection(workspace, collection);
        //when nothing selected, make readonly
        if (accepts == null || accepts.Length == 0) vals = new string[] { string.Empty };
        else vals = accepts.Split(',').Select(a => a.Trim()).ToArray();
        
        //when readonly is selected and it wasn't already selected, remove other selections
        if (!c.Accepts.Select(a => a.Value).Contains(string.Empty) && vals.Length > 1 && vals.Contains(string.Empty)) vals = new string[] { string.Empty };
        else if (vals.Length > 1) vals = vals.Where(a => a != string.Empty).ToArray();
        //when just Atom.ContentTypeEntry set to null as that is default
        c.Accepts = vals.Length == 1 && vals[0] == Atom.ContentTypeEntry ? null :
            vals.Select(a=>AppAccept.AllAccepts.Where(aa => aa.Value == a).FirstOrDefault() ?? new AppAccept() { Value = a });
        AtomPubService.UpdateService(AppService);
        return Json(new { accepts = string.Join(",", vals) });
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { error = ex.Message });
      }
    }

    [ScopeAuthorize, AcceptVerbs(HttpVerbs.Post)]
    public JsonResult DeleteWorkspace(string workspace)
    {
      //TODO: move to service
      try
      {
        throw new NotImplementedException();
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { error = ex.Message });
      }
    }

    [ScopeAuthorize, AcceptVerbs(HttpVerbs.Post)]
    public JsonResult DeleteCollection(string workspace)
    {
      //TODO: move to service
      try
      {
        throw new NotImplementedException();
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { error = ex.Message });
      }
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Post)]
    public JsonResult AddCategory(string workspace, string collection, string term, string scheme)
    {
      try
      {
        Uri schemeUri = !string.IsNullOrEmpty(scheme) ? new Uri(scheme) : null;
        AtomCategory cat = AtomPubService.AddCategory(Collection.Id, term, schemeUri);
        return Json(new { term = cat.Term, label = cat.Label, scheme = cat.Scheme });
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { error = ex.Message });
      }
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult Categories(string workspace, string collection, string scheme)
    {
      Uri schemeUri = !string.IsNullOrEmpty(scheme) ? new Uri(scheme) : null;
      AdminCategoriesModel m = new AdminCategoriesModel();
      m.Categories = Collection.Categories.Where(c => c.Scheme == schemeUri).SingleOrDefault();

      Collection.Categories.Select(c => c.Scheme).ToList()
        .ForEach(s => m.Schemes.Add(s == null ? "Default" : s.ToString(),
        Url.Action("Categories", "Admin", new { workspace = Scope.Workspace, collection = Scope.Collection, scheme = s })));
      //TODO: metrics
      return PartialView("AdminCategoriesWidget", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Post)]
    public JsonResult RemoveCategory(string workspace, string collection, string term, string scheme)
    {
      try
      {
        Uri schemeUri = !string.IsNullOrEmpty(scheme) ? new Uri(scheme) : null;
        AtomPubService.RemoveCategory(Collection.Id, term, schemeUri);
        return Json(new { success = true });
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { error = ex.Message });
      }
    }

    [ScopeAuthorize(EntireSite=true), AcceptVerbs(HttpVerbs.Post)]
    public ActionResult UpdateEntireSite(AdminSettingsEntireSiteModel m)
    {
      try
      {
        AppService.Base = m.SiteAddress;
        AppService.ServiceType = (ServiceType)Enum.Parse(typeof(ServiceType), m.ServiceType);
        AppService.DefaultSubdomain = !string.IsNullOrEmpty(m.DefaultSubdomain) ? m.DefaultSubdomain : null;
        AppService.Secure = m.Secure ?? false;

        AtomPubService.UpdateService(AppService);
        ServerApp.Restart();
        TempData["saved"] = true;
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        m.Errors.Add(ex.Message);
      }
      TempData["model"] = m;
      return RedirectToAction("Settings");
    }

    [ScopeAuthorize(Roles=AuthRoles.AuthorOrAdmin), AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult Accepts(string workspace, string collection)
    {
      var m = new AdminAcceptsModel();
      bool isDefault = Collection.Accepts.Count() == 0;
      m.AcceptChecks = AppAccept.AllAccepts.Union(Collection.Accepts)
        .Select(a => new AcceptCheck() { Accept = a.Value, Checked = Collection.Accepts.Contains(a), Default = isDefault && Collection.CanAccept(a.Value) });
      m.CurrentAccepts = isDefault ? Atom.ContentTypeEntry : string.Join(",", Collection.Accepts.Select(a => a.Value).ToArray());

      return PartialView("AdminAcceptsWidget", m);
    }
  }
}
