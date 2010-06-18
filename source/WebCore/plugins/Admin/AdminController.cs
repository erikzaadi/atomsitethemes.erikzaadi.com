/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Linq;
  using System.Web.Mvc;
  using System.Web.UI;
  using AtomSite.Domain;
  using AtomSite.Repository;

  [OutputCache(Location=OutputCacheLocation.None)]
  public partial class AdminController : BaseController
  {
    protected AdminService AdminService { get; private set; }
    protected IUserRepository UserRepository { get; private set; }
    protected IAtomPubService AtomPubService { get; private set; }
    protected IAnnotateService AnnotateService { get; private set; }
    protected IAtomEntryRepository AtomEntryRepository { get; private set; }
    protected IThemeService ThemeService { get; private set; }

    public AdminController(IUserRepository userRepo, IAtomPubService atompub, IAnnotateService annotate,
      IAtomEntryRepository atomRepo, AdminService admin, IThemeService theme)
    {
      AdminService = admin;
      UserRepository = userRepo;
      AtomPubService = atompub;
      AnnotateService = annotate;
      AtomEntryRepository = atomRepo;
      ThemeService = theme;
    }

    [ScopeAuthorize(AnyScope=true)]
    public ActionResult Dashboard(string workspace, string collection)
    {
      //redirect to appropriate scope if can't access this scope
      var scopes = AuthorizeService.GetScopes(User.Identity as User);
      if (!Scope.InScope(scopes))
      {
        return RedirectToAction("Dashboard", new { workspace = scopes.First().Workspace, collection = scopes.First().Collection });
      }

      return View("AdminDashboard", "Admin", new AdminModel());
    }

    [ScopeAuthorize]
    public PartialViewResult PendingEntries(string workspace, string collection)
    {
      var entries = AtomPubService.GetEntries(new EntryCriteria()
      {
        WorkspaceName = workspace,
        CollectionName = collection,
        Authorized = true,
        Pending = true,
      }, 0, 10).ToPagedList(0, 10);
      return PartialView("AdminPendingEntriesWidget", new AdminEntriesModel() { Entries = entries });
    }

    [ScopeAuthorize]
    public PartialViewResult RightNow(string workspace, string collection)
    {
      var m = new AdminRightNowModel();
      m.ReleaseName = ServerApp.CurrentRelease;
      m.Version = ServerApp.CurrentVersion.ToString();

      var scopes = AuthorizeService.GetScopes(User.Identity as User);
      m.HighestScope = scopes.OrderByDescending(s => s.IsEntireSite)
        .ThenByDescending(s => s.IsWorkspace).ThenByDescending(s => s.IsCollection).First();

      scopes = scopes.Where(s => s.IsCollection);

      foreach (Scope s in scopes)
      {
        //what is current theme
        //m.CurrentThemes.Add(s, ThemeViewEngine.GetThemeName(AppService.GetWorkspace(s.Workspace), 
        //  AppService.GetCollection(s.Workspace, s.Collection)));

        //entry stats
        ScopeMetric metrics = new ScopeMetric() { Scope = s };
        //total entries
        metrics.Metrics.Add("entries-total", AtomEntryRepository.GetEntriesCount(
          SelectionCriteria.EntriesAll(s.Workspace, s.Collection, null, null)));

        //entries published
        metrics.Metrics.Add("entries-pub", AtomEntryRepository.GetEntriesCount(
          SelectionCriteria.EntriesPublished(s.Workspace, s.Collection, null, null)));

        //entries pending (not approved)
        metrics.Metrics.Add("entries-pend", AtomEntryRepository.GetEntriesCount(
          SelectionCriteria.EntriesPending(s.Workspace, s.Collection, null, null)));

        //entries draft
        metrics.Metrics.Add("entries-draft", AtomEntryRepository.GetEntriesCount(
          SelectionCriteria.EntriesPending(s.Workspace, s.Collection, null, null)));

        if (AppService.GetCollection(s.Workspace, s.Collection).AnnotationsOn)
        {
          //total annotations
          metrics.Metrics.Add("ann-total", AtomEntryRepository.GetEntriesCount(
          SelectionCriteria.AnnotationsAll(s.Workspace, s.Collection, null)));

          //annotations published
          metrics.Metrics.Add("ann-pub", AtomEntryRepository.GetEntriesCount(
          SelectionCriteria.AnnotationsPublished(s.Workspace, s.Collection, null)));

          //annotations pending
          metrics.Metrics.Add("ann-pend", AtomEntryRepository.GetEntriesCount(
          SelectionCriteria.AnnotationsPending(s.Workspace, s.Collection, null)));

          //annotation spam (not yet supported
          metrics.Metrics.Add("ann-spam", AtomEntryRepository.GetEntriesCount(
          SelectionCriteria.AnnotationsSpam(s.Workspace, s.Collection, null)));
        }
        m.Metrics.Add(metrics);
      }

      return PartialView("AdminRightNowWidget", m);
    }


    [ScopeAuthorize]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
    [System.Web.Mvc.ValidateInput(false)]
    public JsonResult QuickPub(string id, string title, string content, string submit)
    {
      try
      {
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content)) throw new Exception("Both the title and content are required.");
        AtomEntry entry = new AtomEntry();
        entry.Title = new AtomTitle() { Text = title };
        entry.Content = new AtomContent() { Text = content, Type = "html" };
        if (submit != "Publish") entry.Control = new AppControl() { Draft = true };
        entry = AtomPubService.CreateEntry(id, entry, null);
        string message = (entry.Published.HasValue) ? "<strong>Entry Published</strong>" : "<strong>Draft Saved</strong>";
        message += string.Format(" <a href='{0}'>View entry</a> | <a href='{1}'>Edit entry</a>",
          entry.LocationWeb, Url.Action("EditEntry", "Admin", new { id = entry.Id }));
        return Json(new { message = message });
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { error = ex.Message });
      }
    }

    [ScopeAuthorize]
    public PartialViewResult CollectionsWidget(string workspace)
    {
      var m = new AdminCollCountModel();
      m.Collections = Workspace.Collections.Select(c => new CollectionCount()
      {
        Collection = c,
        Count = AtomEntryRepository.GetEntriesCount(new EntryCriteria()
          {
            WorkspaceName = c.Id.Workspace,
            CollectionName = c.Id.Collection,
            Authorized = true
          })
      });

      return PartialView("AdminCollectionsWidget", m);
    }
  }
}
