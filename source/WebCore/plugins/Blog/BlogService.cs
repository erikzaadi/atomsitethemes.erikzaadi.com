/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Linq;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using StructureMap;
  using System.Web.Routing;

  public class BlogService : IBlogService
  {
    protected IAppServiceRepository AppServiceRepository;
    protected IAtomEntryRepository AtomEntryRepository;
    protected IAnnotateService AnnotateService;
    protected IAuthorizeService AuthorizeService;
    protected IContainer Container;
    protected ILogService LogService;
    protected ICleanContentService CleanContentService;
    protected readonly AppService AppService;

    public BlogService(IAtomPubService atompub, IAnnotateService annotate,
      IAppServiceRepository svcRepo, IContainer container,
        IAtomEntryRepository entryRepo, IAuthorizeService auth, ILogService logger,
      ICleanContentService cleaner)
    {
      this.AppService = svcRepo.GetService();
      this.AppServiceRepository = svcRepo;
      this.AtomEntryRepository = entryRepo;
      this.Container = container;
      this.AnnotateService = annotate;
      this.AuthorizeService = auth;
      this.LogService = logger;
      this.CleanContentService = cleaner;
      atompub.CreatingEntry += OnModifyEntry;
      atompub.UpdatingEntry += OnModifyEntry;
      annotate.AnnotatingEntry += OnAnnotateEntry;
      atompub.SettingEntryLinks += (e) => SetLinks(e);
      atompub.SettingFeedLinks += (f) => SetLinks(f);
    }

    protected void SetLinks(AtomEntry e)
    {
        if (!new BlogAppCollection(AppService.GetCollection(e.Id)).BloggingOn) return;
      LogService.Debug("BlogService.SetLinks entryId={0}", e.Id);
      var links = e.Links.ToList();
      var url = new UrlHelper(Container.GetInstance<RequestContext>());
      if (e.InReplyTo == null)
      {
          links.Merge(new AtomLink()
          {
              Rel = "alternate",
              Type = "text/html",
              Href = url.RouteIdUri("BlogEntry", e.Id, AbsoluteMode.Force)
          });
      }
      else // annotation
      {
          links.Merge(new AtomLink()
          {
              Rel = "alternate",
              Type = "text/html",
              Href = new System.Uri(url.RouteIdUri("BlogEntry", e.InReplyTo.Ref, AbsoluteMode.Force).ToString() + "#" + e.Id.ToWebId())
          });
          e.InReplyTo.Href = url.RouteIdUri("BlogEntry", e.InReplyTo.Ref, AbsoluteMode.Force);
          links.Merge(new AtomLink
          {
              Rel = "related",
              Type = "text/html",
              Href = url.RouteIdUri("BlogEntry", e.InReplyTo.Ref, AbsoluteMode.Force)
          });

          if (AnnotateService.GetAnnotationState(AppService.GetCollection(e.Id), e.Id) == AnnotationState.On)
          {
              links.Merge(new AtomLink
              {
                  Rel = "reply",
                  Type = "text/html",
                  Href = new System.Uri(url.RouteIdUri("BlogEntry", e.InReplyTo.Ref, AbsoluteMode.Force).ToString() + "#addcommentform")
              });
          }
      }
      e.Links = links;
    }
    protected void SetLinks(AtomFeed f)
    {
        if (f.Id != null && f.Id.Scheme == "tag")
        {
            if (!new BlogAppCollection(AppService.GetCollection(f.Id)).BloggingOn) return;
            LogService.Debug("BlogService.SetLinks feedId={0}", f.Id);
            var links = f.Links.ToList();
            var url = new UrlHelper(Container.GetInstance<RequestContext>());
            links.Merge(new AtomLink()
            {
                Rel = "alternate",
                Type = "text/html",
                Href = url.RouteIdUri("BlogListing", f.Id, AbsoluteMode.Force)
            });
            f.Links = links;
        }
    }

    protected void OnModifyEntry(Id collectionId, AtomEntry entry, string slug)
    {
      LogService.Info("BlogService.OnModifyEntry");
      if (new BlogAppCollection(AppService.GetCollection(collectionId)).BloggingOn)
        CleanContentService.CleanContentTrusted(entry.Content);
    }

    protected void OnAnnotateEntry(Id entryId, AtomEntry entry, string slug)
    {
      LogService.Info("BlogService.OnAnnotateEntry");
      if (!new BlogAppCollection(AppService.GetCollection(entryId)).BloggingOn) return;
      //TODO: check if author url (referrer) is blocked

      //TODO: check if content src is blocked

      //TODO: check if spam

      if (entry.AnnotationType == null) entry.AnnotationType = "comment";

      AppCollection coll = AppService.GetCollection(entryId);
      AnnotationState state = AnnotateService.GetAnnotationState(coll, entryId);
      if (state != AnnotationState.On)
        throw new AnnotationNotAllowedException(entryId.ToString(), entry.AnnotationType, "the state is " + state);


      //default title to comment when not given
      if (entry.Title == null || string.IsNullOrEmpty(entry.Title.Text))
        entry.Title = new AtomText(Atom.AtomNs + "title") { Text = "Comment" };

      //approved?
      if (!AuthorizeService.IsAuthorized((User)System.Threading.Thread.CurrentPrincipal.Identity,
        entry.Id.ToScope(), AuthAction.ApproveAnnotation))
      {
        entry.Control = new AppControl() { Approved = false };
      }

      //clean input
      if (entry.Content.Type == "html" || entry.Content.Type == "xhtml")
      {
        if (AuthorizeService.IsAuthorized((User)System.Threading.Thread.CurrentPrincipal.Identity,
          entryId.ToScope(), AuthAction.ApproveAnnotation))
          CleanContentService.CleanContentTrusted(entry.Content);
        else
          CleanContentService.CleanContentFully(entry.Content);
      }
      else if (entry.Content.Type == "text")
      {
          entry.Content.Text = entry.Content.Text.Replace("<", "&lt;").Replace(">", "&gt;");
      }

      else if (entry.Content.Src == null)
          throw new AnnotationNotAllowedException(entryId.ToString(), entry.AnnotationType, "content must be text, html, or external.");

      AtomEntry parent = AtomEntryRepository.GetEntry(entryId);
      //check if there is already a content src annotation with this link
      int total;
      if (entry.IsExternal && AtomEntryRepository.GetEntries(
        new EntryCriteria()
        {
          EntryId = parent.Id,
          Annotations = true,
          Authorized = true,
          Deep = true
        }, 0, int.MaxValue, out total)
        .Where(a => a.Content.Src != null && a.Content.Src == entry.Content.Src).Count() > 0)
        throw new AnnotationNotAllowedException(parent.Id, entry.AnnotationType, "it already contains a link to the source.");

      //TODO: allow categories on annotations?
      entry.Categories = null;
    }

  }
}
