/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using StructureMap;
  using System.Web.Routing;

  public class AtomPubService : IAtomPubService
  {
    public event Action<AtomEntry> SettingEntryLinks;
    public event Action<AtomFeed> SettingFeedLinks;

    public event Action<Id, AtomEntry, string> CreatingEntry;
    public event Action<AtomEntry> EntryCreated;

    public event Action<Id, AtomEntry, string> UpdatingEntry;
    public event Action<AtomEntry> EntryUpdated;

    public event Action<Id> DeletingEntry;
    public event Action<Id> EntryDeleted;

    protected IAppServiceRepository AppServiceRepository;
    protected IAppCategoriesRepository AppCategoriesRepository;
    protected IAtomEntryRepository AtomEntryRepository;
    protected IMediaRepository MediaRepository;
    protected IAuthorizeService AuthorizeService;
    protected IContainer Container;
    protected ILogService LogService;

    public AtomPubService(
        IAppServiceRepository appServiceRepository,
        IAppCategoriesRepository appCategoriesRepository,
        IAtomEntryRepository atomEntryRepository,
        IMediaRepository mediaRepository,
        IAuthorizeService authorizeService,
        IContainer container, 
      ILogService logger)
    {
      this.AppServiceRepository = appServiceRepository;
      this.AppCategoriesRepository = appCategoriesRepository;
      this.AtomEntryRepository = atomEntryRepository;
      this.MediaRepository = mediaRepository;
      this.AuthorizeService = authorizeService;
      this.Container = container;
      this.LogService = logger;
    }

    protected User GetUser()
    {
      return Thread.CurrentPrincipal.Identity as User;
    }

    #region IAtomPubService Members

    public virtual AppService GetService()
    {
      LogService.Info("AtomPubService.GetService");
      AuthorizeService.Auth(Scope.EntireSite, AuthAction.GetServiceDoc);
      AppService service = AppServiceRepository.GetService();
      //TODO: filter workspaces/collections user is not Authorized to use
      //OnGetService(service);
      return service;
    }

    public virtual AppService UpdateService(AppService appService)
    {
      LogService.Info("AtomPubService.UpdateService");
      AuthorizeService.Auth(Scope.EntireSite, AuthAction.UpdateServiceDoc);
      //TODO: combine with existing service doc based on authorization rules
      //OnUpdateService(appService);
      return AppServiceRepository.UpdateService(appService);
    }

    /// <summary>
    /// Get entries by the given criteria.
    /// </summary>
    /// <remarks>This method will likely deprecate many of the specific feed based functions.</remarks>
    /// <param name="criteria"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public IPagedList<AtomEntry> GetEntries(EntryCriteria criteria, int pageIndex, int pageSize)
    {
      LogService.Info("AtomPubService.GetEntries");
      AuthorizeService.Auth(new Scope() { Workspace = criteria.WorkspaceName, Collection = criteria.CollectionName },
        AuthAction.GetFeed);
      
      criteria.Authorized = true;//TODO: AuthorizeService.IsAuthorized(GetUser(), coll.Id, AuthAction.GetEntryOrMedia);
      int total;      
      var entries = AtomEntryRepository.GetEntries(criteria, pageIndex, pageSize, out total);
      foreach (AtomEntry entry in entries) SetLinks(entry);
      PagedList<AtomEntry> pagedList = new PagedList<AtomEntry>(entries, pageIndex, pageSize, total);
      return pagedList;
    }

    /// <summary>
    /// Gets a consolidated feed of all collections in all workspaces.
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public AtomFeed GetFeed(int pageIndex, int pageSize)
    {
      return GetFeed(null, pageIndex, pageSize);
    }

    /// <summary>
    /// Gets a consolidated feed of all collections in a workspace.
    /// </summary>
    /// <param name="workspaceName"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public virtual AtomFeed GetFeed(string workspaceName, int pageIndex, int pageSize)
    {
      LogService.Info("AtomPubService.GetFeed: workspace={0}", workspaceName);
      AuthorizeService.Auth(new Scope() { Workspace = workspaceName }, AuthAction.GetFeed);
      AppService svc = AppServiceRepository.GetService();
      AtomFeed feed = new AtomFeed();
      feed.Generator = new AtomGenerator { Text = "atomsite.net" };
      feed.Id = new Uri("urn:" + Guid.NewGuid().ToString());
      feed.Title = new AtomText() { Text = svc.Base.Authority };
      int grandTotal = 0;

      if (!string.IsNullOrEmpty(workspaceName) || svc.Workspaces.Count() == 1)
      {
        //fill info from workspace
        feed.Title = svc.Workspaces.First().Title;
        feed.Subtitle = svc.Workspaces.First().Subtitle;
        //TODO: other props
      }
      
      //build feed for entire service or a single workspace
      var collections = svc.Workspaces.Where(w => w.Name == workspaceName || workspaceName == null)
        .SelectMany(w => w.Collections)
        .Where(c => c.SyndicationOn && c.Visible);

      List<AtomEntry> entries = new List<AtomEntry>();
      foreach (AppCollection coll in collections)
      {
        if (AuthorizeService.IsAuthorized(GetUser(), coll.Id.ToScope(), AuthAction.GetFeed))
        {
          int total;
          EntryCriteria criteria = new EntryCriteria()
          {
            WorkspaceName = coll.Id.Workspace,
            CollectionName = coll.Id.Collection,
            Authorized = AuthorizeService.IsAuthorized(GetUser(), coll.Id.ToScope(), AuthAction.GetEntryOrMedia),
          };
          entries.AddRange(AtomEntryRepository.GetEntries(criteria, pageIndex, pageSize, out total));
          grandTotal += total;
        }
      }
      feed.TotalResults = grandTotal;
      entries.OrderByDescending(e => e.Date);      
      entries.Take(pageSize);
      feed.Entries = entries;
      SetLinks(feed);
      return feed;
    }

    public virtual AtomFeed GetFeed(Id collectionId, int pageIndex, int pageSize)
    {
      //<atom:link rel="alternate" type="text/html" href="https://atomsite.net/blog.xhtml" />

      LogService.Info("AtomPubService.GetFeed collectionId={0}", collectionId);
      Auth(collectionId, AuthAction.GetFeed);
      AppCollection c = AppServiceRepository.GetService().GetCollection(collectionId);
      EntryCriteria criteria = new EntryCriteria()
      {
        WorkspaceName = collectionId.Workspace,
        CollectionName = collectionId.Collection,
        Authorized = AuthorizeService.IsAuthorized(GetUser(), collectionId.ToScope(), AuthAction.GetEntryOrMedia),
      };
      int total;
      AtomFeed feed = AtomFeed.BuildFeed(c, AtomEntryRepository.GetEntries(criteria, pageIndex, pageSize, out total),
        total);//, pageIndex, pageSize, RouteFunc, "AtomPubFeed", "AtomPubCollectionIndex", true);

      var url = new UrlHelper(Container.GetInstance<RequestContext>());
      feed.Links = feed.Links.Concat(url.GetPagingLinks("AtomPubFeed", feed.Id, null, feed.TotalResults ?? 0, 
        pageIndex, pageSize,Atom.ContentTypeFeed, AbsoluteMode.Force));

      SetLinks(feed);
      return feed;
    }

    public virtual AtomFeed GetFeedByDate(Id collectionId, DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
    {
      LogService.Info("AtomPubService.GetFeedByDate collectionId={0}, startDate={1}, endDate={2}", collectionId, startDate, endDate);
      Auth(collectionId, AuthAction.GetFeed);

      AppCollection c = AppServiceRepository.GetService().GetCollection(collectionId);
      EntryCriteria criteria = new EntryCriteria()
      {
        WorkspaceName = collectionId.Workspace,
        CollectionName = collectionId.Collection,
        StartDate = startDate,
        EndDate = endDate,
        Authorized = AuthorizeService.IsAuthorized(GetUser(), collectionId.ToScope(), AuthAction.GetEntryOrMedia)
      };
      int total;
      AtomFeed feed = AtomFeed.BuildFeed(c, AtomEntryRepository.GetEntries(criteria, pageIndex, pageSize, out total),
        total);//, pageIndex, pageSize, RouteFunc, "AtomPubFeed", "AtomPubCollectionIndex", false);
      if (startDate.DayOfYear == 1 && endDate.DayOfYear > 354)
      {
        feed.Subtitle = new AtomSubtitle() { Text = "For the year " + startDate.Year.ToString("0000") };
      }
      else if (startDate.Day == 1 && endDate.Day == DateTime.DaysInMonth(endDate.Year, endDate.Month))
      {
        feed.Subtitle = new AtomSubtitle() { Text = "For " + startDate.ToString("MMMM yyyy") };
      }
      else if (startDate.Day == endDate.Day)
      {
        feed.Subtitle = new AtomSubtitle() { Text = "For " + startDate.ToString("D") };
      }
      else
      {
        feed.Subtitle = new AtomSubtitle()
        {
          Text = "From " + startDate.ToShortDateString() + " to " +
            endDate.ToShortDateString()
        };
      }
      SetLinks(feed);
      return feed;
    }


    public virtual AtomFeed GetFeedByAuthor(Id collectionId, string authorName, int pageIndex, int pageSize)
    {
      LogService.Info("AtomPubService.GetFeedByAuthor collectionId={0}, authorName={1}", collectionId, authorName);
      Auth(collectionId, AuthAction.GetFeed);

      AppCollection c = AppServiceRepository.GetService().GetCollection(collectionId);
      EntryCriteria criteria = new EntryCriteria()
      {
        WorkspaceName = collectionId.Workspace,
        CollectionName = collectionId.Collection,
        PersonName = authorName,
        PersonType = "author",
        Authorized = AuthorizeService.IsAuthorized(GetUser(), collectionId.ToScope(), AuthAction.GetEntryOrMedia)
      };
      int total;
      AtomFeed feed = AtomFeed.BuildFeed(c, AtomEntryRepository.GetEntries(criteria, pageIndex, pageSize, out total),
        total);//, pageIndex, pageSize, RouteFunc, "AtomPubFeed", null, false);
      feed.Subtitle = new AtomSubtitle() { Text = authorName };
      SetLinks(feed);
      return feed;
    }

    public virtual AtomFeed GetFeedByContributor(Id collectionId, string contributorName, int pageIndex, int pageSize)
    {
      LogService.Info("AtomPubService.GetFeedByContributor collectionId={0}, contributorName={1}", collectionId, contributorName);
      Auth(collectionId, AuthAction.GetFeed);

      AppCollection c = AppServiceRepository.GetService().GetCollection(collectionId);
      int total;
      EntryCriteria criteria = new EntryCriteria()
      {
        WorkspaceName = collectionId.Workspace,
        CollectionName = collectionId.Collection,
        PersonName = contributorName,
        PersonType = "contributor",
        Authorized = AuthorizeService.IsAuthorized(GetUser(), collectionId.ToScope(), AuthAction.GetEntryOrMedia)
      };

      AtomFeed feed = AtomFeed.BuildFeed(c, AtomEntryRepository.GetEntries(criteria, pageIndex, pageSize, out total),
        total);//, pageIndex, pageSize, RouteFunc, "AtomPubFeed", "AtomPubCollectionIndex", false);
      feed.Subtitle = new AtomSubtitle() { Text = contributorName }; 
      SetLinks(feed);
      return feed;
    }

    public virtual AtomFeed GetFeedByPerson(Id collectionId, string personName, int pageIndex, int pageSize)
    {
      LogService.Info("AtomPubService.GetFeedByPerson collectionId={0}, personName={1}", collectionId, personName);
      Auth(collectionId, AuthAction.GetFeed);

      AppCollection c = AppServiceRepository.GetService().GetCollection(collectionId);
      EntryCriteria criteria = new EntryCriteria()
      {
        WorkspaceName = collectionId.Workspace,
        CollectionName = collectionId.Collection,
        PersonName = personName,
        PersonType = "person",
        Authorized = AuthorizeService.IsAuthorized(GetUser(), collectionId.ToScope(), AuthAction.GetEntryOrMedia)
      };
      int total;

      AtomFeed feed = AtomFeed.BuildFeed(c, AtomEntryRepository.GetEntries(criteria, pageIndex, pageSize, out total),
        total);//, pageIndex, pageSize, RouteFunc, "AtomPubFeed", "AtomPubCollectionIndex", false);
      feed.Subtitle = new AtomSubtitle() { Text = personName };
      SetLinks(feed);
      return feed;
    }
    public virtual AtomFeed GetFeedByCategory(Id collectionId, string term, Uri scheme, int pageIndex, int pageSize)
    {
      LogService.Info("AtomPubService.GetFeedByCategory collectionId={0}, term={1}", collectionId, term);
      Auth(collectionId, AuthAction.GetFeed);

      AppCollection c = AppServiceRepository.GetService().GetCollection(collectionId);
      //TODO: support external categories
      AtomCategory category = c.Categories.SelectMany(cats => cats.Categories).Where(cat => cat.Term == term &&
          cat.Scheme == scheme).SingleOrDefault();
      if (category == null) throw new ResourceNotFoundException("category", term);

      EntryCriteria criteria = new EntryCriteria()
      {
        WorkspaceName = collectionId.Workspace,
        CollectionName = collectionId.Collection,
        CategoryTerm = term,
        CategoryScheme = scheme,
        Authorized = AuthorizeService.IsAuthorized(GetUser(), collectionId.ToScope(), AuthAction.GetEntryOrMedia)
      };
      int total;

      AtomFeed feed = AtomFeed.BuildFeed(c, AtomEntryRepository.GetEntries(criteria, pageIndex, pageSize, out total),
        total);//, pageIndex, pageSize, RouteFunc, "AtomPubFeed", "AtomPubCollectionIndex", false);

      feed.Subtitle = new AtomSubtitle { Text = "Browsing " + category.ToString() };
      SetLinks(feed);
      return feed;
    }



    public AtomFeed GetFeedBySearch(string term, int pageIndex, int pageSize)
    {
      LogService.Info("AtomPubService.GetFeedBySearch term={0}", term);
      throw new NotImplementedException();
    }

    public AtomFeed GetFeedBySearch(string workspace, string term, int pageIndex, int pageSize)
    {
      LogService.Info("AtomPubService.GetFeedBySearch workspace={0} term={1} pageIndex={2}", workspace, term, pageIndex);
      AuthorizeService.Auth(new Scope() { Workspace = workspace }, AuthAction.GetFeed);

      EntryCriteria criteria = new EntryCriteria()
      {
        WorkspaceName = workspace,
        SearchTerm = term,
        Authorized = AuthorizeService.IsAuthorized(GetUser(), new Scope() { Workspace = workspace }, AuthAction.GetEntryOrMedia)
      };
      int total;
      //search annotations?

      AtomFeed feed = new AtomFeed();
      feed.Generator = new AtomGenerator { Text = "atomsite.net" };
      AppWorkspace w = AppServiceRepository.GetService().GetWorkspace(workspace);
      feed.Title = w.Title;
      feed.Updated = DateTimeOffset.UtcNow;
      feed.Entries = AtomEntryRepository.GetEntries(criteria, pageIndex, pageSize, out total);
      feed.TotalResults = total;

      //use newest updated date as feed updated date
      if (feed.Entries.Count() > 0)
        feed.Updated = feed.Entries.Max().Updated;

      feed.Subtitle = new AtomSubtitle { Text = "Search for '" + term + "'" };
      SetLinks(feed);
      return feed;
    }
    
    public virtual AtomFeed GetFeedBySearch(Id collectionId, string term, int pageIndex, int pageSize)
    {
      LogService.Info("AtomPubService.GetFeedBySearch collectionId={0} term={1} pageIndex={2}", collectionId, term, pageIndex);
      Auth(collectionId, AuthAction.GetFeed);

      AppCollection c = AppServiceRepository.GetService().GetCollection(collectionId);

      EntryCriteria criteria = new EntryCriteria()
      {
        WorkspaceName = collectionId.Workspace,
        CollectionName = collectionId.Collection,
        SearchTerm = term,
        Authorized = AuthorizeService.IsAuthorized(GetUser(), collectionId.ToScope(), AuthAction.GetEntryOrMedia)
      };
      int total;
      //search annotations?

      AtomFeed feed = AtomFeed.BuildFeed(c, AtomEntryRepository.GetEntries(criteria, pageIndex, pageSize, out total),
        total);//, pageIndex, pageSize, RouteFunc, "AtomPubFeed", "AtomPubCollectionIndex", false);

      feed.Subtitle = new AtomSubtitle { Text = "Search for '" + term + "'" };
      SetLinks(feed);
      return feed;
    }


    public virtual AtomFeed GetCollectionFeed(Id collectionId, int pageIndex)
    {
      LogService.Info("AtomPubService.GetCollectionFeed collectionId={0} pageIndex={1}", collectionId, pageIndex);
      Auth(collectionId, AuthAction.GetCollectionFeed);
      AppCollection c = AppServiceRepository.GetService().GetCollection(collectionId);
      EntryCriteria criteria = new EntryCriteria()
      {
        WorkspaceName = collectionId.Workspace,
        CollectionName = collectionId.Collection,
        Authorized = true,
        SortMethod = SortMethod.EditDesc //according to spec, collection feeds should be ordered by edited
      };
      int total;
      AtomFeed feed = AtomFeed.BuildFeed(c, AtomEntryRepository.GetEntries(criteria, pageIndex, c.PageSize, out total),
        total);//, pageIndex, c.PageSize, RouteFunc, "AtomPubCollection", "AtomPubCollectionIndex", true);
      SetLinks(feed);

      var url = new UrlHelper(Container.GetInstance<RequestContext>());
      feed.Links = feed.Links.Concat(url.GetPagingLinks("AtomPubCollection", feed.Id, null, feed.TotalResults ?? 0, pageIndex, c.PageSize,
        Atom.ContentTypeFeed, AbsoluteMode.Force));
      return feed;
    }

    public virtual AtomEntry CreateEntry(Id collectionId, AtomEntry entry, string slug)
    {
      LogService.Info("AtomPubService.CreateEntry collectionId={0} slug={1}", collectionId, slug);
      Auth(collectionId, AuthAction.CreateEntryOrMedia);
      AppCollection coll = AppServiceRepository.GetService().GetCollection(collectionId);
      if (entry.Control == null) entry.Control = new AppControl();

      //is acceptable type?
      if (!coll.CanAccept(Atom.ContentTypeEntry)) throw new InvalidContentTypeException(Atom.ContentTypeEntry);

      //approval based on role action matrix
      if (!AuthorizeService.IsAuthorized(GetUser(), collectionId.ToScope(), AuthAction.ApproveEntryOrMedia))
        entry.Control.Approved = false;

      if (!entry.Draft && !entry.Published.HasValue) entry.Published = DateTimeOffset.UtcNow;
      if (entry.Updated == default(DateTimeOffset)) entry.Updated = DateTimeOffset.UtcNow;
      entry.Edited = DateTimeOffset.UtcNow;
      if (coll.Dated)
        entry.Id = new Id(collectionId.Owner, entry.Date.UtcDateTime, collectionId.Collection, entry.BuildPath(null, slug));
      else
        entry.Id = new Id(collectionId.Owner, collectionId.Date, collectionId.Collection, entry.BuildPath(null, slug));

      SetPerson(entry);
      SetCategories(entry); 
      SetLinks(entry);
      entry.IdChanged += (e) => SetLinks(e);

      if (CreatingEntry != null) CreatingEntry(collectionId, entry, slug);
      entry = AtomEntryRepository.CreateEntry(entry);
      if (EntryCreated != null) EntryCreated(entry);

      return entry;
    }
    /// <summary>
    /// Get an entry from the collection.  This method supports Anonymous
    /// retrieval.
    /// </summary>
    /// <param name="entryId"></param>
    /// <returns></returns>
    public virtual AtomEntry GetEntry(Id entryId)
    {
      LogService.Info("AtomPubService.GetEntry entryId={0}", entryId);
      AtomEntry entry = AtomEntryRepository.GetEntry(entryId);
      SetLinks(entry);
      //entry.UpdateLinks(RouteFunc);

      //OnGetEntry(entry);

      //Allow authorized users to get entry otherwise only get if visible
      if (AuthorizeService.IsAuthorized(GetUser(), entryId.ToScope(), AuthAction.GetEntryOrMedia))
        return entry;
      else if (entry.Visible)
      {
        //TODO: clean private data
        return entry;
      }
      throw new UserNotAuthorizedException(GetUser().Name, AuthAction.GetEntryOrMedia.ToString());
    }

    public virtual AtomEntry UpdateEntry(Id entryId, AtomEntry entry, string slug)
    {
      LogService.Info("AtomPubService.UpdateEntry entryId={0}", entryId);
      Auth(entryId, AuthAction.UpdateEntryOrMedia);

      //ensure existing entry exists
      AtomEntry old = AtomEntryRepository.GetEntry(entryId);

      //copy old approval setting when not authorized to approve
      if (!AuthorizeService.IsAuthorized(GetUser(), entryId.ToScope(), AuthAction.ApproveEntryOrMedia))
      {
        if (entry.Control != null && entry.Control.Approved.HasValue)
          entry.Control.Approved = old.Control.Approved;
        else
          entry.Control.Approved = false;
      }

      if (!entry.Draft && !entry.Published.HasValue) entry.Published = DateTimeOffset.UtcNow;
      entry.Updated = DateTimeOffset.UtcNow;
      entry.Edited = DateTimeOffset.UtcNow;

      //if (old.Draft) //allow Id to change when old was draft mode, is this safe?
      //{
      //  AppCollection coll = AppServiceRepository.GetService().GetCollection(entryId);
      //  if (coll.Dated)
      //    entry.Id = new Id(coll.Id.Owner, entry.Date.UtcDateTime, coll.Id.Collection, entry.BuildPath(null, slug));
      //  else
      //    entry.Id = new Id(coll.Id.Owner, coll.Id.Date, coll.Id.Collection, entry.BuildPath(null, slug));
      //}
      //else 
        entry.Id = entryId; //reset Id (it shouldn't change)

      if (old.Media) entry.Content = old.Content; //reset Content (it shouldn't change for media link entries)
      //entry.UpdateLinks(RouteFunc);
      SetPerson(entry);
      SetCategories(entry);
      SetLinks(entry);
      entry.IdChanged += (e) => SetLinks(e);// e.UpdateLinks(RouteFunc); //in case of changes during draft

      if (UpdatingEntry != null) UpdatingEntry(entryId, entry, slug);
      entry = AtomEntryRepository.UpdateEntry(entry);
      if (EntryUpdated != null) EntryUpdated(entry);

      return entry;
    }
    
    public virtual void DeleteEntry(Id entryId)
    {
      LogService.Info("AtomPubService.DeleteEntry entryId={0}", entryId);
      Auth(entryId, AuthAction.DeleteEntryOrMedia);
      if (DeletingEntry != null) DeletingEntry(entryId);
      AtomEntryRepository.DeleteEntry(entryId);
      if (EntryDeleted != null) EntryDeleted(entryId);
    }

    public virtual string GetEntryEtag(Id entryId)
    {
      Auth(entryId, AuthAction.PeekEntryOrMedia);
      return AtomEntryRepository.GetEntryEtag(entryId);
    }

    public virtual AtomEntry CreateMedia(Id collectionId, Stream stream, string slug, string contentType)
    {
      LogService.Info("AtomPubService.CreateMedia collectionId={0} slug={1} contentType={2}", collectionId, slug, contentType);
      Auth(collectionId, AuthAction.CreateEntryOrMedia);
      AppCollection coll = AppServiceRepository.GetService().GetCollection(collectionId);
      if (!coll.CanAccept(contentType)) throw new InvalidContentTypeException(contentType);
      AtomEntry entry = new AtomEntry();
      entry.Media = true;
      entry.Title = new AtomText(Atom.AtomNs + "title") { Text = slug != null ? slug : collectionId.Collection };
      entry.Updated = DateTimeOffset.UtcNow;
      entry.Edited = DateTimeOffset.UtcNow;
      if (!entry.Published.HasValue) entry.Published = DateTimeOffset.UtcNow;
      if (coll.Dated)
        entry.Id = new Id(collectionId.Owner, entry.Date.UtcDateTime, collectionId.Collection, entry.BuildPath(null, slug));
      else
        entry.Id = new Id(collectionId.Owner, collectionId.Date, collectionId.Collection, entry.BuildPath(null, slug));

      entry.Content = new AtomContent { Type = contentType };//, Src = UrlHelper.RouteIdUri("AtomPubMedia", entry.Id, AbsoluteMode.Force) };
      SetPerson(entry);
      SetLinks(entry);
      entry.Summary = new AtomText(Atom.AtomNs + "summary") { Text = "" };
      entry.IdChanged += (e) => SetLinks(e);// e.UpdateLinks(RouteFunc);
      
      if (CreatingEntry != null) CreatingEntry(collectionId, entry, slug);
      //OnCreateMedia(entry, collectionId, stream, slug, contentType);
      entry = MediaRepository.CreateMedia(entry, stream);
      if (EntryCreated != null) EntryCreated(entry);
      return entry;
    }
    public virtual Stream GetMedia(Id entryId, out string contentType)
    {
      LogService.Info("AtomPubService.GetMedia entryId={0}", entryId);
      AtomEntry mediaLinkEntry = AtomEntryRepository.GetEntry(entryId);
      contentType = mediaLinkEntry.Content.Type;
       //TODO: remove this hack
      SetLinks(mediaLinkEntry);
      //mediaLinkEntry.Content.Src = UrlHelper.RouteIdUri("AtomPubMedia", mediaLinkEntry.Id, AbsoluteMode.Force);
      
      //Allow authorized users to get media otherwise only get if visible
      if (AuthorizeService.IsAuthorized(GetUser(), entryId.ToScope(), AuthAction.GetEntryOrMedia))
        return MediaRepository.GetMedia(mediaLinkEntry);
      else if (mediaLinkEntry.Visible)
      {
        //TODO: clean private data
        return MediaRepository.GetMedia(mediaLinkEntry);
      }
      throw new UserNotAuthorizedException(GetUser().Name, AuthAction.GetEntryOrMedia.ToString());
    }
    public virtual AtomEntry UpdateMedia(Id entryId, Stream stream, string contentType)
    {
      LogService.Info("AtomPubService.UpdateMedia entryId={0} contentType={1}", entryId, contentType);
      Auth(entryId, AuthAction.UpdateEntryOrMedia);
      //ensure existing entry exists
      AtomEntry mediaLinkEntry = AtomEntryRepository.GetEntry(entryId);
      mediaLinkEntry.Media = true;
      //mediaLinkEntry.UpdateLinks(RouteFunc);
      mediaLinkEntry.Content.Type = contentType;
      mediaLinkEntry.Updated = DateTimeOffset.UtcNow;
      mediaLinkEntry.Edited = DateTimeOffset.UtcNow;
      SetPerson(mediaLinkEntry);
      SetLinks(mediaLinkEntry);
      //OnUpdateMedia(mediaLinkEntry, entryId, stream, contentType);
      mediaLinkEntry = MediaRepository.UpdateMedia(mediaLinkEntry, stream);
      if (EntryUpdated != null) EntryUpdated(mediaLinkEntry);
      return mediaLinkEntry;
    }
    public virtual void DeleteMedia(Id entryId)
    {
      LogService.Info("AtomPubService.DeleteMedia entryId={0}", entryId);
      Auth(entryId, AuthAction.DeleteEntryOrMedia);
      AtomEntry mediaLinkEntry = AtomEntryRepository.GetEntry(entryId);
      //OnDeleteMedia(mediaLinkEntry, entryId);

      if (DeletingEntry != null) DeletingEntry(entryId);
      MediaRepository.DeleteMedia(mediaLinkEntry);
      if (EntryDeleted != null) EntryDeleted(entryId);
    }
    public virtual string GetMediaEtag(Id entryId, out string contentType)
    {
      Auth(entryId, AuthAction.PeekEntryOrMedia);
      AtomEntry mediaLinkEntry = AtomEntryRepository.GetEntry(entryId);
      contentType = mediaLinkEntry.Content.Type;
      return MediaRepository.GetMediaEtag(mediaLinkEntry);
    }

    public virtual void DeleteWorkspace(string workspaceName, bool updateService)
    {
      AppService appSvc = AppServiceRepository.GetService();
      AppWorkspace ws = appSvc.GetWorkspace(workspaceName);
      foreach (AppCollection c in ws.Collections)
      {
        DeleteCollection(c.Id, updateService);
      }

      if (updateService)
      {
        appSvc.RemoveWorkspace(ws);
        UpdateService(appSvc);
      }
    }

    public virtual void DeleteCollection(Id collectionId, bool updateService)
    {
      AtomEntryRepository.DeleteEntries(
        new EntryCriteria()
        {
          WorkspaceName = collectionId.Workspace,
          CollectionName = collectionId.Collection,
          Authorized = true
        });

      if (updateService)
      {
        AppService appSvc = AppServiceRepository.GetService();
        appSvc.GetWorkspace(collectionId.Workspace).RemoveCollection(appSvc.GetCollection(collectionId));
        UpdateService(appSvc);
      }
    }

    public void ApproveEntry(Id entryId, bool approved)
    {
        LogService.Info("AtomPubService.ApproveEntry entryId={0}", entryId);
        AuthorizeService.Auth(entryId.ToScope(), AuthAction.ApproveEntryOrMedia);
        AtomEntry e = AtomEntryRepository.GetEntry(entryId);
        if (e.Control == null) e.Control = new AppControl();
        e.Control.Approved = approved;
        AtomEntryRepository.UpdateEntry(e);
    }

    public int ApproveAll(Id id)
    {
        LogService.Info("AtomPubService.ApproveAll id={0}", id);
        AuthorizeService.Auth(id.ToScope(), AuthAction.ApproveEntryOrMedia);
        return AtomEntryRepository.ApproveAll(id);
    }
    #endregion

    protected void SetLinks(AtomEntry entry)
    {
      LogService.Debug("AtomPubService.SetLinks entryId={0}", entry.Id);
      if (entry.Links == null) entry.Links = new List<AtomLink>();
      var links = entry.Links.ToList();

      var url = new UrlHelper(Container.GetInstance<RequestContext>());
      if (AuthorizeService.IsAuthorized(GetUser(), entry.Id.ToScope(), AuthAction.UpdateEntryOrMedia))
      {
        links.Merge(new AtomLink { Rel = "edit", Href = url.RouteIdUri("AtomPubEntryEdit", entry.Id, AbsoluteMode.Force ), });
      }
      links.Merge(new AtomLink { Rel = "self", Href = url.RouteIdUri("AtomPubEntry", entry.Id, AbsoluteMode.Force) });

      if (entry.Media)
      {
          if (entry.Content == null) entry.Content = new AtomContent();
          entry.Content.Src = url.RouteIdUri("AtomPubMedia", entry.Id, AbsoluteMode.Force);
          if (AuthorizeService.IsAuthorized(GetUser(), entry.Id.ToScope(), AuthAction.UpdateEntryOrMedia))
          {
              links.Merge(new AtomLink { Rel = "edit-media", Href = url.RouteIdUri("AtomPubMediaEdit", entry.Id, AbsoluteMode.Force) });
          }
      }
      else
      {
          links.Merge(new AtomLink { Rel = "alternate", Type="text/html", Href = url.RouteIdUri("AtomPubResource", entry.Id, AbsoluteMode.Force) });
      }

      if (AuthorizeService.IsAuthorized(GetUser(), entry.Id.ToScope(), AuthAction.ApproveEntryOrMedia))
      {
          links.RemoveAll(l => l.Rel == "unapprove" || l.Rel == "approve");
          links.Add(new AtomLink { Rel = entry.Approved ? "unapprove" : "approve", Href = url.RouteIdUri("AtomPubApproveEntry", entry.Id, AbsoluteMode.Force) });
      }

      entry.Links = links;
      if (SettingEntryLinks != null) SettingEntryLinks(entry);
    }

    protected void SetLinks(AtomFeed feed)
    {
      LogService.Debug("AtomPubService.SetLinks collectionId={0}", feed.Id);
      if (feed.Links == null) feed.Links = new List<AtomLink>();
      var links = feed.Links.ToList();

      var url = new UrlHelper(Container.GetInstance<RequestContext>());
      links.Merge(new AtomLink()
      {
        Rel = "service",
        Type = Atom.ContentTypeService,
        Href = url.RouteUriEx("AtomPubService", AbsoluteMode.Force)
      });
      if (feed.Id != null && feed.Id.Scheme == "tag")
      {
        links.Merge(new AtomLink()
        {
          Rel = "self",
          Type = Atom.ContentTypeFeed,
          Href = url.RouteIdUri("AtomPubFeed", feed.Id, AbsoluteMode.Force)
        });
        links.Merge(new AtomLink()
        {
          Rel = "alternate",
          Type = "text/html",
          Href = url.RouteIdUri("AtomPubCollectionIndex", feed.Id, AbsoluteMode.Force)
        });
      }
      feed.Links = links;
      foreach (AtomEntry entry in feed.Entries) SetLinks(entry);
      if (SettingFeedLinks != null) SettingFeedLinks(feed);
    }

    protected void Auth(AuthAction action)
    {
      Auth(null, action);
    }

    protected void Auth(Id id, AuthAction action)
    {
      User user = GetUser();
      if (!AuthorizeService.IsAuthorized(user, id.ToScope(), action))
        throw new UserNotAuthorizedException(user.Name, action.ToString());
    }

    protected void SetPerson(AtomEntry entry)
    {
      if (entry.Authors.Count() == 0) entry.SetPerson(AuthorizeService);
    }

    public AtomCategory AddCategory(Id collectionId, string category, Uri scheme)
    {
      LogService.Info("AtomPubService.AddCategory collectionId={0}, category={1}", collectionId, category);
      AppService service = AppServiceRepository.GetService();
      AppCollection coll = service.GetCollection(collectionId);
      AtomCategory cat = new AtomCategory() { Term = category, Scheme = scheme };
      //TODO: support external categories
      //TODO: support scheme
      bool changed = coll.Categories.First().AddCategory(cat);
      //save when changed
      if (changed)
      {
        LogService.Info("Saving service doc for internal category change.");
        AppServiceRepository.UpdateService(service);
      }
      return cat;
    }

    public void RemoveCategory(Id collectionId, string category, Uri scheme)
    {
      LogService.Info("AtomPubService.RemoveCategory collectionId={0}, category={1}", collectionId, category);
      bool changed = false;
      AppService service = AppServiceRepository.GetService();
      AppCollection coll = service.GetCollection(collectionId);
      AppCategories cats = coll.Categories.Where(c => c.Scheme == scheme).FirstOrDefault();
      if (cats != null)
      {
        var list = cats.Categories.ToList();
        var cat = list.Where(c => c.Term == category).SingleOrDefault();
        if (cat != null)
        {
          list.Remove(cat);
          cats.Categories = list;
          changed = true;
        }
      }
      //save when changed
      if (changed)
      {
        LogService.Info("Saving service doc for internal category change.");
        AppServiceRepository.UpdateService(service);
      }
    }

    //TODO: this is complex, is there a better design?
    protected void SetCategories(AtomEntry entry)
    {
      LogService.Info("AtomPubService.SetCategories entryId={0}", entry.Id);
      AppService service = AppServiceRepository.GetService();
      bool changed = false;
      AppCollection coll = service.GetCollection(entry.Id);
      //get all external categories
      Dictionary<AppCategories, KeyValuePair<AppCategories, bool>> external = new Dictionary<AppCategories, KeyValuePair<AppCategories, bool>>();
      foreach (AppCategories cats in coll.Categories)
      {
        if (cats.IsExternal) external.Add(AppCategoriesRepository.GetCategories(coll.Id, cats), new KeyValuePair<AppCategories, bool>(cats, false));
      }

      //add to either internal or external
      foreach (AtomCategory cat in entry.Categories)
      {
        AppCategories cats = coll.Categories.Where(c => !c.IsExternal && c.Scheme == cat.Scheme).SingleOrDefault();
        if (cats != null && cats.AddCategory(cat))
        {
          LogService.Info("Added internal category {0}", cat.Term);
          changed = true;
        }
        else
        {
          //look in external
          if (external.Keys.Where(e => e.Scheme == cat.Scheme).SingleOrDefault() != null)
          {
            KeyValuePair<AppCategories, KeyValuePair<AppCategories, bool>> ex = external.Where(e => e.Key.Scheme == cat.Scheme).Single();
            if (ex.Key.AddCategory(cat))
            {
              external.Remove(ex.Key);
              external.Add(ex.Key, new KeyValuePair<AppCategories, bool>(ex.Value.Key, true));
              LogService.Info("Added external category {0}", cat.Term);
            }
          }
        }
      }

      //save when changed
      if (changed)
      {
        LogService.Info("Saving service doc for internal category changes.");
        AppServiceRepository.UpdateService(service);
      }

      //save external when changed
      foreach (KeyValuePair<AppCategories, KeyValuePair<AppCategories, bool>> ex in external)
      {
        if (ex.Value.Value)
        {
          LogService.Info("Saving external category changes for {0}", ex.Value.Key.Href);
          AppCategoriesRepository.UpdateCategories(coll.Id, ex.Value.Key, ex.Key);
        }
      }
   }
  }
}
