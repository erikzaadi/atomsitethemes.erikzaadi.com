/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Linq;
  using System.Web;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using AtomSite.Repository;

  public class WidgetController : BaseController
  {
    protected IAtomPubService AtomPubService { get; private set; }
    protected IAnnotateService AnnotateService { get; private set; }
    protected IAppServiceRepository AppServiceRepository { get; private set; }
    protected IAtomEntryRepository AtomEntryRepository { get; private set; }

    public WidgetController(IAtomPubService atompub, IAnnotateService annotate, IAppServiceRepository svcRepo, IAtomEntryRepository entryRepo)
      : base()
    {
      AtomPubService = atompub;
      AnnotateService = annotate;
      AppServiceRepository = svcRepo;
      AtomEntryRepository = entryRepo;
    }

    [ActionOutputCache(2 * MIN)]
    public PartialViewResult Entry(string widgetName, Include include)
    {
      var i = new IdInclude(include);
      Id entryId = EntryId;
      if (i.Id != null) entryId = i.Id;
      if (entryId == null) throw new ArgumentException("Unable to determine entry based on include or context.");
      EntryModel model = new EntryModel();
      try
      {
        model.Entry = AtomPubService.GetEntry(entryId);
      }
      catch (ResourceNotFoundException ex)
      {
        LogService.Error(ex);
      }
      return PartialView(widgetName, model);
    }

    [ActionOutputCache(2 * MIN)]
    public PartialViewResult SizeAnnotations(string widgetName, Include include)
    {
      var i = new FeedInclude(include);
      if (!i.Count.HasValue) i.Count = 6;
      FeedModel model = new FeedModel() { Feed = GetFeed(i, true) };
      return PartialView(widgetName, model);
    }

    [ActionOutputCache(2 * MIN)]
    public PartialViewResult SizeFeed(string widgetName, Include include)
    {
      var i = new FeedInclude(include);
      if (!i.Count.HasValue) i.Count = 6;
      FeedModel model = new FeedModel() { Feed = GetFeed(i, false) };
      return PartialView(widgetName, model);
    }

    [ActionOutputCache(30 * MIN)]
    public PartialViewResult FullFeed(string widgetName, Include include)
    {
      var i = new FeedInclude(include);
      if (!i.Count.HasValue) i.Count = int.MaxValue;
      FeedModel model = new FeedModel() { Feed = GetFeed(i, false) };
      return PartialView(widgetName, model);
    }

    [ScopeAuthorize(Action = AuthAction.UpdateServiceDoc, Roles = AuthRoles.AuthorOrAdmin)]
    [AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult LiteralConfig(LiteralConfigModel m)
    {
      var include = AppService.GetInclude<LiteralInclude>(m.IncludePath);
      m.Html = HttpUtility.HtmlDecode(include.Html); //needs decoded or it gets double encoded
      return PartialView("WidgetLiteralConfig", m);
    }

    [ScopeAuthorize(Action = AuthAction.UpdateServiceDoc, Roles = AuthRoles.AuthorOrAdmin)]
    [AcceptVerbs(HttpVerbs.Post)]
    [ActionName("LiteralConfig")]
    [ValidateInput(false)]
    public ActionResult PostLiteralConfig(LiteralConfigModel m)
    {
      if (string.IsNullOrEmpty(m.Html)) 
        ModelState.AddModelError("html", "You must supply some html to literally include into the output.");

      if (ModelState.IsValid)
      {
        var appSvc = AppServiceRepository.GetService();
        var include = appSvc.GetInclude<LiteralInclude>(m.IncludePath);
        include.Html = m.Html;
        AppServiceRepository.UpdateService(appSvc);
        return Json(new { success = true, includePath = m.IncludePath });
      }

      return PartialView("WidgetLiteralConfig", m);
    }

    [ScopeAuthorize(Action = AuthAction.UpdateServiceDoc, Roles = AuthRoles.AuthorOrAdmin)]
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult EntryConfig(EntryConfigModel m)
    {
      if (m.IncludePath != null)
      {
        var include = AppService.GetInclude<IdInclude>(m.IncludePath);
        if (include.Id != null) m.SelectedId = include.Id.ToString();
      }
      LoadEntryConfig(m);
      return PartialView("WidgetEntryConfig", m);
    }

    [ScopeAuthorize(Action = AuthAction.UpdateServiceDoc, Roles = AuthRoles.AuthorOrAdmin)]
    [AcceptVerbs(HttpVerbs.Post)]
    [ActionName("EntryConfig")]
    [ValidateInput(false)]
    public ActionResult PostEntryConfig(EntryConfigModel m)
    {
      if (m.SelectedId == null)
        ModelState.AddModelError("selectedId", "You must select an entry from the list.");

      if (ModelState.IsValid)
      {
        var appSvc = AppServiceRepository.GetService();
        var include = appSvc.GetInclude<IdInclude>(m.IncludePath);
        include.Id = m.SelectedId;
        AppServiceRepository.UpdateService(appSvc);
        return Json(new { success = true, includePath = m.IncludePath });
      }
      LoadEntryConfig(m);
      return PartialView("WidgetEntryConfig", m);
    }

    private void LoadEntryConfig(EntryConfigModel m)
    {
      if (m.SelectedId != null)
      {
        var entry = AtomPubService.GetEntry(m.SelectedId);
        m.SelectedTitle = entry.Title.ToString();
      }

      m.Scopes = AuthorizeService.GetScopes(this.User.Identity as User);
      var scope = new Scope() { Workspace = m.CurrentWorkspace, Collection = m.CurrentCollection };
      if (m.CurrentWorkspace == null)
      {
        scope = m.Scopes.First();
        m.CurrentWorkspace = scope.Workspace;
        m.CurrentCollection = scope.Collection;
      }     

      m.Entries = AtomPubService.GetEntries(new EntryCriteria()
      {
        WorkspaceName = scope.Workspace,
        CollectionName = scope.Collection,
        Authorized = false,
        Published = true
      }, (m.Page ?? 1) - 1, 50);
    }
    
    [ScopeAuthorize(Action = AuthAction.UpdateServiceDoc, Roles = AuthRoles.AuthorOrAdmin)]
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult FeedConfig(FeedConfigModel m)
    {
      if (m.IncludePath != null)
      {
        var include = AppService.GetInclude<FeedInclude>(m.IncludePath);
        m.SelectedId = include.Id == null ? null : include.Id.ToString();
        m.Count = include.Count;
        m.Title = include.Title;
      }
      LoadFeedConfig(m);
      return PartialView("WidgetFeedConfig", m);
    }

    [ScopeAuthorize(Action = AuthAction.UpdateServiceDoc, Roles = AuthRoles.AuthorOrAdmin)]
    [AcceptVerbs(HttpVerbs.Post)]
    [ActionName("FeedConfig")]
    [ValidateInput(false)]
    public ActionResult PostFeedConfig(FeedConfigModel m)
    {
      if (m.SelectedId == null)
        ModelState.AddModelError("selectedId", "You must select a collection from the list.");

      if (!ModelState.IsValidField("count")) ModelState.AddModelError("count", "Please enter a valid number for the count.");
      if (!ModelState.IsValidField("title")) ModelState.AddModelError("title", "Please enter a valid title.");

      if (ModelState.IsValid)
      {
        var appSvc = AppServiceRepository.GetService();
        var include = appSvc.GetInclude<FeedInclude>(m.IncludePath);
        include.Id = m.SelectedId;
        include.Title = string.IsNullOrEmpty(m.Title) || m.Title.Trim().Length == 0 ? null : m.Title.Trim();
        include.Count = m.Count;
        AppServiceRepository.UpdateService(appSvc);
        return Json(new { success = true, includePath = m.IncludePath });
      }
      LoadFeedConfig(m);
      return PartialView("WidgetFeedConfig", m);
    }

    private void LoadFeedConfig(FeedConfigModel m)
    {
      var scopes = AuthorizeService.GetScopes(this.User.Identity as User);

      m.Collections = scopes.Where(s => s.IsCollection)
        .Select(s => AppService.GetCollection(s.Workspace, s.Collection));
    }

    [ScopeAuthorize(Action = AuthAction.UpdateServiceDoc, Roles = AuthRoles.AuthorOrAdmin)]
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult ScopeConfig(ScopeConfigModel m)
    {
      var include = AppService.GetInclude<ScopeInclude>(m.IncludePath);
      m.SelectedScope = include.ScopeName;
      return PartialView("WidgetScopeConfig", m);
    }

    [ScopeAuthorize(Action = AuthAction.UpdateServiceDoc, Roles = AuthRoles.AuthorOrAdmin)]
    [AcceptVerbs(HttpVerbs.Post)]
    [ActionName("ScopeConfig")]
    [ValidateInput(false)]
    public ActionResult PostScopeConfig(ScopeConfigModel m)
    {
      if (string.IsNullOrEmpty(m.SelectedScope))
        ModelState.AddModelError("selectedScope", "You must select a scope for widget to target the data in.");

      if (ModelState.IsValid)
      {
        var appSvc = AppServiceRepository.GetService();
        var include = appSvc.GetInclude<ScopeInclude>(m.IncludePath);
        include.ScopeName = m.SelectedScope;
        AppServiceRepository.UpdateService(appSvc);
        return Json(new { success = true, includePath = m.IncludePath });
      }

      return PartialView("WidgetScopeConfig", m);
    }

    protected AtomFeed GetFeed(FeedInclude include, bool isAnnotations)
    {
      Id id = (EntryId != null) ? EntryId : (Collection != null ? Collection.Id : null);
      if (include.Id != null) id = include.Id;
      if (id == null) throw new ArgumentException("Unable to determine feed based on include or context.");
      
      AtomFeed feed = !isAnnotations ? AtomPubService.GetFeed(id, 0, include.Count.Value) :
        AnnotateService.GetAnnotations(id, true, 0, include.Count ?? 10);
      
      if (!string.IsNullOrEmpty(include.Title))
      {
        feed.Title = new AtomTitle() { Text = include.Title };
      }
      return feed;
    }

  }
}
