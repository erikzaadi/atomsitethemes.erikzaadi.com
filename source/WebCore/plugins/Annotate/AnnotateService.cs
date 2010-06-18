/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using StructureMap;
  using System.Web.Routing;

  public class AnnotateService : IAnnotateService
  {
    public event Action<AtomEntry> EntryAnnotated;
    public event Action<Id, AtomEntry, string> AnnotatingEntry;

    protected AppService AppService;
    protected IAtomPubService AtomPubService;
    protected IAtomEntryRepository AtomEntryRepository;
    protected IMediaRepository MediaRepository;
    protected IAuthorizeService AuthorizeService;
    protected IContainer Container;
    protected ILogService LogService;

    public AnnotateService(IContainer container, IAtomPubService atompub, IAppServiceRepository svcRepo, IAuthorizeService auth,
      IAtomEntryRepository entryRepo, IMediaRepository mediaRepo, ILogService logger)
    {
      AppService = svcRepo.GetService();
      AtomPubService = atompub;
      AuthorizeService = auth;
      AtomEntryRepository = entryRepo;
      MediaRepository = mediaRepo;
      Container = container;
      LogService = logger;

      atompub.SettingEntryLinks += (e) => SetLinks(e);
      atompub.SettingFeedLinks += (f) => SetLinks(f);
    }

    protected void SetLinks(AtomFeed f)
    {
      LogService.Debug("AnnotateService.SetLinks feedId={0}", f.Id);
      var links = f.Links.ToList();

      var url = new UrlHelper(Container.GetInstance<RequestContext>());

      //atom threading extension
      if (f.Id != null && f.Id.Scheme == "tag")
      {
        links.Merge(new AtomLink
        {
          Href = url.RouteIdUri("AnnotateAnnotationsFeed", f.Id, AbsoluteMode.Force),
          Rel = "replies",
          Type = Atom.ContentType,
          Updated = DateTimeOffset.UtcNow
        });
      }
      f.Links = links;
    }

    protected void SetLinks(AtomEntry e)
    {
      LogService.Debug("AnnotateService.SetLinks entryId={0}", e.Id);
      var links = e.Links.ToList();
      var url = new UrlHelper(Container.GetInstance<RequestContext>());
      //atom threading extension
      if (e.InReplyTo != null)
      {
        e.InReplyTo.Href = url.RouteIdUri("AtomPubResource", e.InReplyTo.Ref, AbsoluteMode.Force);
        links.Merge(new AtomLink
        {
          Rel = "related",
          Type = "text/html",
          Href = url.RouteIdUri("AtomPubResource", e.InReplyTo.Ref, AbsoluteMode.Force)
        });
      }

      //atom threading extension
      links.Merge(new AtomLink
      {
        Href = url.RouteIdUri("AnnotateEntryAnnotationsFeed", e.Id, AbsoluteMode.Force),
        Rel = "replies",
        Type = Atom.ContentType,
        Count = e.Total,
        Updated = DateTimeOffset.UtcNow
      });
      e.Links = links;
    }

    protected User GetUser()
    {
      return Thread.CurrentPrincipal.Identity as User;
    }

    protected void SetPerson(AtomEntry entry)
    {
      entry.SetPerson(AuthorizeService);
    }

    public virtual AtomEntry Annotate(Id entryId, AtomEntry entry, string slug)
    {
      LogService.Info("AnnotateService.Annotate entryId={0} slug={1}", entryId, slug);

      //authorization
      if (!AuthorizeService.IsAuthorized(GetUser(), entryId.ToScope(), AuthAction.Annotate))
        throw new UserNotAuthorizedException(GetUser().Name, AuthAction.Annotate.ToString());

      AppCollection coll = AppService.GetCollection(entryId);

      //make sure type is accepted
      if (!coll.CanAccept(Atom.ContentTypeEntry))
        throw new InvalidContentTypeException(Atom.ContentTypeEntry);

      entry.SetNamespaces(); //TODO: is there a better place for this?

      //build id onto parent's id
      AtomEntry parent = AtomPubService.GetEntry(entryId);
      entry.Id = new Id(parent.Id.Owner, parent.Id.Date, parent.Id.Collection, entry.BuildPath(parent.Id.EntryPath, slug));
      
      var url = new UrlHelper(Container.GetInstance<RequestContext>());
      //this annotation is a reply to the parent entry, TODO: leave off href for later calc based on id?
      entry.InReplyTo = new ThreadInReplyTo()
      {
        Ref = parent.Id,
        Href = parent.IsExternal ? parent.Content.Src : url.RouteIdUri("AtomPubEntry", entry.Id, AbsoluteMode.Force),
        Type = parent.IsExternal ? parent.Content.Type : Atom.ContentTypeEntry
      };

      if (!entry.Published.HasValue) entry.Published = DateTimeOffset.UtcNow;
      entry.Updated = DateTimeOffset.UtcNow;
      entry.Edited = DateTimeOffset.UtcNow;

      if (entry.Authors.Count() == 0) entry.SetPerson(AuthorizeService, true);

      //entry.IdChanged += (e) => e.UpdateLinks(UrlHelper.RouteIdUri);

      //OnAnnotate(parent, entryId, entry, slug);
      if (AnnotatingEntry != null) AnnotatingEntry(entryId, entry, slug);

      if (entry.Authors.Count() == 0 || entry.Authors.First().Name == null)
        throw new AnnotationNotAllowedException(entry.Id, entry.AnnotationType, "the author cannot be determined");

      entry = AtomEntryRepository.CreateEntry(entry);
      if (EntryAnnotated != null) EntryAnnotated(entry);
      return entry;
    }

    public virtual AtomEntry MediaAnnotate(Id entryId, Stream stream, string slug, string contentType)
    {
      LogService.Info("AnnotateService.MediaAnnotate entryId={0} slug={1} contentType={2}", entryId, slug, contentType);
      throw new NotImplementedException();
    }

    public virtual AtomFeed GetAnnotations(Id id, bool deep, int pageIndex, int pageSize)
    {
      LogService.Info("AnnotateService.GetAnnotations: {0} deep={1}", id, deep);

      //authorization
      if (!AuthorizeService.IsAuthorized(GetUser(), id.ToScope(), AuthAction.GetAnnotations))
        throw new UserNotAuthorizedException(GetUser().Name, AuthAction.GetAnnotations.ToString());

      return GetAnnotations(new EntryCriteria()
      {
        EntryId = id.EntryPath != null ? id : null,
        WorkspaceName = id.EntryPath == null ? id.Workspace : null,
        CollectionName = id.EntryPath == null ? id.Collection : null,
        SortMethod = id.EntryPath == null ? SortMethod.DateDesc : SortMethod.Default,
        Authorized = AuthorizeService.IsAuthorized(GetUser(), id.ToScope(), AuthAction.GetEntryOrMedia),
        Annotations = true,
        Deep = deep
      }, pageIndex, pageSize);
    }

    public AtomFeed GetAnnotations(bool deep, int pageIndex, int pageSize)
    {
      LogService.Info("AnnotateService.GetAnnotations deep={0}", deep);
      EntryCriteria criteria = new EntryCriteria()
      {
        Annotations = true,
        SortMethod = SortMethod.Default,
        Authorized = AuthorizeService.IsAuthorized(GetUser(), new Scope(), AuthAction.GetEntryOrMedia),
        Deep = deep
      };
      return GetAnnotations(criteria, pageIndex, pageSize);
    }

    public AtomFeed GetAnnotations(string workspace, bool deep, int pageIndex, int pageSize)
    {
      LogService.Info("AnnotateService.GetAnnotations workspace={0} deep={1}", workspace, deep);
      EntryCriteria criteria = new EntryCriteria()
      {
        Annotations = true,
        WorkspaceName = workspace,
        SortMethod = SortMethod.Default,
        Authorized = AuthorizeService.IsAuthorized(GetUser(), new Scope() { Workspace = workspace }, AuthAction.GetEntryOrMedia),
        Deep = deep
      };
      return GetAnnotations(criteria, pageIndex, pageSize);
    }

    protected virtual AtomFeed GetAnnotations(EntryCriteria criteria, int pageIndex, int pageSize)
    {
      AtomFeed feed = new AtomFeed();
      feed.Subtitle = new AtomText(Atom.AtomNs + "subtitle") { Text = "Annotations" };
      if (criteria.EntryId != null) //get annotations for an entry
      {
        AtomEntry parent = AtomEntryRepository.GetEntry(criteria.EntryId);
        feed.Title = parent.Title;
        feed.Id = parent.Id;
        //feed.Authors = parent.Authors;
        //feed.Contributors = parent.Contributors;
      }
      else if (criteria.CollectionName != null) //get annotations for a collection
      {
        AppCollection coll = AppService.GetCollection(criteria.WorkspaceName, criteria.CollectionName);
        feed.Id = coll.Id;
        feed.Title = coll.Title;
        //TODO: feed.Authors = coll.Authors;
        //TODO: feed.Contributors = coll.Contributors;
      }
      else if (criteria.WorkspaceName != null) //get annotations for an entire workspace
      {
        AppWorkspace ws = AppService.GetWorkspace(criteria.WorkspaceName);
        feed.Title = ws.Title;
        //TODO: feed.Authors = ws.Authors;
        //TODO: feed.Contributors = ws.Contributors;
      }
      else
      {
        feed.Title = new AtomTitle { Text = "Annotations" };
      }
      int total;
      feed.Entries = AtomEntryRepository.GetEntries(criteria, pageIndex, pageSize, out total);
      feed.TotalResults = total;
      //foreach (AtomEntry e in feed.Entries)
      //{
      //  e.UpdateLinks(this.UrlHelper.RouteIdUri);
      //}
      return feed;
    }

    public virtual AnnotationState GetAnnotationState(AppCollection coll, Id entryId)
    {
      LogService.Info("AnnotateService.GetAnnotationState entryId={0}", entryId);

      if (!coll.AnnotationsOn)
      {
        return AnnotationState.Off;
      }
      else if (!AuthorizeService.IsAuthorized(GetUser(), entryId.ToScope(), AuthAction.Annotate))
      {
        return AnnotationState.Unauthorized;
      }
      else if (!AtomEntryRepository.GetEntry(entryId).AllowAnnotate)
      {
        return AnnotationState.Closed;
      }
      //TODO: handle expired
      return AnnotationState.On;
    }
  }
}
