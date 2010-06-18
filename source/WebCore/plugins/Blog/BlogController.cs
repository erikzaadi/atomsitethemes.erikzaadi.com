/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Linq;
  using System.Net;
  using System.Web;
  using System.Web.Mvc;
  using System.Xml;
  using System.Xml.Linq;
  using AtomSite.Domain;

  //[HandleError(ExceptionType = typeof(ResourceNotFoundException), View = "NotFound")]
  //[HandleError(View = "Error")]
  public class BlogController : BaseController
  {
    protected IAtomPubService AtomPubService { get; private set; }
    protected IAnnotateService AnnotateService { get; private set; }
    protected IBlogService BlogService { get; private set; }

    public BlogController(IAtomPubService atompub, IAnnotateService annotate, IBlogService blog)
      : base()
    {
      AtomPubService = atompub;
      AnnotateService = annotate;
      BlogService = blog;
    }

    //[ActionOutputCache(60, true)]
    public ViewResult Home(string workspace)
    {
      var m = new PageModel();
      //use default collection
      if (Workspace == null) m.Workspace = AppService.GetWorkspace();
      if (Collection == null) m.Collection = AppService.GetCollection();
      return View("BlogHome", m);
    }

    public ActionResult Default(string workspace, string collection)
    {
      string view = "BlogListing";
      if (!string.IsNullOrEmpty(Collection.DefaultView)) view = Collection.DefaultView;
      return RedirectToRoute(view);
    }
    public ActionResult EntryDefault(string workspace, string collection, int? year, int? month, int? day, string path)
    {
      string view = "BlogEntry";
      if (!string.IsNullOrEmpty(Collection.DefaultEntryView)) view = Collection.DefaultEntryView;
      return Redirect(Url.RouteIdUrl(view, EntryId));
    }

    [ActionOutputCache(30 * SEC, true)]
    public PartialViewResult Comments(string workspace, string collection, int? year, int? month, int? day, string path)
    {
      FeedModel model = new FeedModel();
      model.Feed = AnnotateService.GetAnnotations(EntryId, true, 0, int.MaxValue);

      return PartialView("BlogCommentsWidget", model);
    }

    //[ActionOutputCache(60 * SEC, true)]
    public virtual ActionResult Listing(string workspace, string collection, int? page)
    {
      FeedModel model = new FeedModel();
      model.Feed = AtomPubService.GetFeed(Collection.Id, (page ?? 1) - 1, Collection.PageSize);
      var links = model.Feed.Links.ToList();
      links.Merge(new AtomLink()
      {
        Rel = "wlwmanifest",
        Type = "application/wlwmanifest+xml",
        Href = Url.RouteIdUri("BlogWriterManifest", Collection.Id)
      });

      //TODO: use default view

      Url.GetPagingLinks("BlogListing", Collection.Id, null, model.Feed.TotalResults ?? 0,
        (page ?? 1) - 1, Collection.PageSize, "text/html").ToList().ForEach(l => links.Merge(l));
      model.Feed.Links = links;

      return View("BlogListing", model);
    }

    //[ActionOutputCache(120 * SEC, true)]
    public virtual ActionResult Entry(string workspace, string collection, int? year, int? month, int? day, string path)
    {
      if (!Collection.Visible) throw new ResourceNotFoundException("entry", EntryId.ToString());

      AtomEntry entry = AtomPubService.GetEntry(EntryId);
      BlogAppCollection coll = new BlogAppCollection(Collection);
      if (coll.SyndicationOn && coll.TrackbacksOn)
      {
        Response.AddHeader("X-Pingback", Url.RouteIdUrl("Pingback", entry.Id, AbsoluteMode.Force).ToString());
      }
      string view = "BlogEntry";
      if (!string.IsNullOrEmpty(Collection.DefaultEntryView)) view = Collection.DefaultEntryView;
      return View(view, new BlogEntryModel() { Entry = entry });
    }

    /// <summary>
    /// Post a new entry to an existing entry.  This will create an annotation.  This method
    /// adds Ajax return support.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult PostComment(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      if (HttpContext.Request.ContentType.ToUpper().StartsWith(Atom.ContentType.ToUpper()))
      {
        AtomEntry entry = new AtomEntry();
        //note: don't dispose of incoming stream or it will close the response, still true?
        XmlReader reader = new XmlTextReader(HttpContext.Request.InputStream);
        entry.Xml = XElement.Load(reader);
        entry = AnnotateService.Annotate(EntryId, entry, GetSlug());

        //if this was an ajax request, just return the annotation xhtml + cookie rather than the atom
        if (this.Request.IsAjaxRequest())
        {
          if (!User.Identity.IsAuthenticated)
          {
            //set cookie, TODO: move this to javascript?
            HttpCookie cookie = new HttpCookie("annotation");
            cookie["name"] = entry.Authors.First().Name;
            cookie["email"] = entry.Authors.First().Email;
            cookie["uri"] = entry.Authors.First().Uri != null ? entry.Authors.First().Uri.ToString() : "";
            cookie.Expires = DateTime.Today.AddYears(1);
            Response.AppendCookie(cookie);
          }
          return PartialView("BlogComment", new CommentModel() { Comment = entry });
        }
        else
        {
          return new XmlWriterResult((w) => entry.Xml.WriteTo(w))
          {
            StatusCode = HttpStatusCode.Created,
            ContentType = Atom.ContentTypeEntry,
            ETag = AtomPubService.GetEntryEtag(entry.Id),
            Location = Url.RouteIdUri("BlogEntry", entry.Id, AbsoluteMode.Force),
            ContentLocation = Url.RouteIdUri("BlogEntry", entry.Id, AbsoluteMode.Force)
          };
        }
      }
      else //media annotation
      {
        throw new NotImplementedException();
      }
    }

    [ActionOutputCache(10 * MIN)]
    public PartialViewResult SearchWidget(Include include)
    {
      string scope = new ScopeInclude(include).ScopeName ?? "workspace";
      string url = null;
      switch (scope)
      {
        case "site": url = Url.RouteUrlEx("BlogSearchService"); break;
        case "workspace": url = Url.RouteUrlEx("BlogSearchWorkspace", new { workspace = AppService.GetWorkspace().Name ?? Atom.DefaultWorkspaceName }); break;
        case "collection": url = Url.RouteIdUrl("BlogSearch", AppService.GetCollection().Id); break;
        default: throw new ArgumentException("The search scope is an unrecognized value.");
      }

      return PartialView("BlogSearchWidget",
        new BlogSearchModel() { SearchUrl = url });
    }

    public ActionResult AddComment(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      return PartialView("BlogAddCommentWidget", new AddCommentModel()
      {
        EntryId = EntryId,
        AnonAuthor = GetAnnonAuthor(),
        AnnotationState = AnnotateService.GetAnnotationState(Collection, EntryId)
      });
    }

    //[ActionOutputCache(10 * MIN, true)]
    public ActionResult Category(string workspace, string collection, string term, string scheme, int? page)
    {
      if (!Collection.Visible) throw new ResourceNotFoundException("collection", collection);
      int pageIndex = (page ?? 1) - 1;
      Id id = Collection.Id;
      Uri schemeUri = !string.IsNullOrEmpty(scheme) ? new Uri(scheme) : null;
      AtomFeed feed = AtomPubService.GetFeedByCategory(id, term, schemeUri,
          pageIndex, 10);
      return View("BlogListing", new FeedModel() { Feed = feed });
    }

    //[ActionOutputCache(10 * MIN, true)]
    public ActionResult Date(string workspace, string collection, int year, int? month, int? day)
    {
      if (!Collection.Visible) throw new ResourceNotFoundException("collection", collection);
      Id id = Collection.Id;
      DateTime start, end;
      if (month.HasValue && day.HasValue)
      {
        start = new DateTime(year, month.Value, day.Value);
        end = start.AddDays(1).AddTicks(-1);
      }
      else if (month.HasValue)
      {
        start = new DateTime(year, month.Value, 1);
        end = start.AddMonths(1).AddTicks(-1);
      }
      else
      {
        start = new DateTime(year, 1, 1);
        end = start.AddYears(1).AddTicks(-1);
      }

      AtomFeed feed = AtomPubService.GetFeedByDate(id, start, end, 0, int.MaxValue);
      return View("BlogListing", new FeedModel() { Feed = feed });
    }

    //[ActionOutputCache(20 * MIN, true)]
    public ActionResult Person(string workspace, string collection, string person, int? page)
    {
      if (!Collection.Visible) throw new ResourceNotFoundException("collection", collection);
      int pageIndex = (page ?? 1) - 1;
      Id id = Collection.Id;
      AtomFeed feed = AtomPubService.GetFeedByPerson(id, person, pageIndex, Collection.PageSize);
      return View("BlogListing", new FeedModel() { Feed = feed });
    }

    //[ActionOutputCache(20 * MIN, true)]
    public ActionResult Author(string workspace, string collection, string author, int? page)
    {
      if (!Collection.Visible) throw new ResourceNotFoundException("collection", collection);
      int pageIndex = (page ?? 1) - 1;
      Id id = Collection.Id;
      AtomFeed feed = AtomPubService.GetFeedByAuthor(id, author, pageIndex, Collection.PageSize);
      return View("BlogListing", new FeedModel() { Feed = feed });
    }

    //[ActionOutputCache(20 * MIN, true)]
    public ActionResult Contributor(string workspace, string collection, string contributor, int? page)
    {
      if (!Collection.Visible) throw new ResourceNotFoundException("collection", collection);
      int pageIndex = (page ?? 1) - 1;
      Id id = Collection.Id;
      AtomFeed feed = AtomPubService.GetFeedByContributor(id, contributor, pageIndex, Collection.PageSize);
      return View("BlogListing", new FeedModel() { Feed = feed });
    }

    public ActionResult Search(string workspace, string collection, string term, int? page)
    {
      if (!Collection.Visible) throw new ResourceNotFoundException("collection", collection);
      int pageIndex = (page ?? 1) - 1;
      AtomFeed feed = null;

      //TODO: support entire site, multifolder, etc.
      if (workspace == null && collection == null)
      {
        feed = AtomPubService.GetFeedBySearch(Workspace.Name, term, pageIndex, 20);
      }
      else if (collection == null)
      {
        feed = AtomPubService.GetFeedBySearch(workspace, term, pageIndex, 20);
      }
      else
      {
        Id id = Collection.Id;
        feed = AtomPubService.GetFeedBySearch(id, term, pageIndex, Collection.PageSize);
      }
      return View("BlogListing", new FeedModel() { Feed = feed });
    }

    //[ActionOutputCache(10 * MIN, true)]
    public ActionResult WriterManifest(string workspace, string collection)
    {
      if (!Collection.Visible) throw new ResourceNotFoundException("collection", collection);
      XNamespace wlwNs = "http://schemas.microsoft.com/wlw/manifest/weblog";
      Id id = Collection.Id;

      //look at least at workspace level to find media collection
      Scope scope = Scope.IsCollection ? Scope.ToAbove() : Scope;
      var mediaColl = AppService.GetCollections(scope).Where(c => c.AcceptsMedia).FirstOrDefault();

      return new XmlWriterResult((w) =>
        new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
          new XElement(wlwNs + "manifest",
            new XElement(wlwNs + "options",
          //Work-around for bug in wlw, <imageEndpoint>{image collection URI}</imageEndpoint>.
              mediaColl != null ? new XElement(wlwNs + "imageEndpoint", mediaColl.Href) : null,
              new XElement(wlwNs + "supportsExtendedEntries", Collection.ExtendedEntriesOn ? "Yes" : "No"),
              Collection.Categories.FirstOrDefault() != null ? new XElement(wlwNs + "categoryScheme",
                Collection.Categories.First().Scheme) : null
              ))).WriteTo(w));
    }

    //[ActionOutputCache(10 * MIN, true)]
    //[HandleSiteError(ExceptionType = typeof(ResourceNotFoundException), View = "NotFound")]
    public ActionResult Sitemap(string workspace, string collection)
    {
      if (!Collection.Visible) throw new ResourceNotFoundException("collection", collection);
      XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
      Id id = Collection.Id;
      return new XmlWriterResult((w) =>
        new XDocument(
          new XElement(ns + "urlset",
            new XElement(ns + "url",
              new XElement(ns + "loc", null),//RouteService.IdToWebHref(id, null)),
              new XElement(ns + "changefreq", "daily"),
              new XElement(ns + "priority", "1.0")),
             new XElement(ns + "url",
              new XElement(ns + "loc", null),//RouteService.IndexHref(id)),
              new XElement(ns + "changefreq", "daily"),
              new XElement(ns + "priority", "0.9")),
          //TODO: date
          //TODO: people
          //TODO: categories
             AtomPubService.GetFeed(id, 0, int.MaxValue).Entries.Select(e =>
               new XElement(ns + "url",
              new XElement(ns + "loc", Url.RouteIdUrl("BlogEntry", e.Id, AbsoluteMode.Force)),
              new XElement(ns + "changefreq", "monthly"),
              new XElement(ns + "lastmod", e.Updated))
            ))).WriteTo(w));
      /*
      <?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
   <url>
      <loc>http://www.example.com/</loc>
      <lastmod>2005-01-01</lastmod>
      <changefreq>monthly</changefreq>
      <priority>0.8</priority>
   </url>
</urlset> */
    }

    [ScopeAuthorize, AcceptVerbs(HttpVerbs.Post)]
    public ActionResult UpdateSettings(string workspace, string collection, BlogSettingsModel m)
    {
      try
      {
        BlogAppCollection c = new BlogAppCollection(AppService.GetCollection(workspace, collection));
        c.BloggingOn = m.BloggingOn ?? false;
        c.TrackbacksOn = m.TrackbacksOn ?? false;
        AtomPubService.UpdateService(AppService);
        ServerApp.Restart();
        TempData["saved"] = true;
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        m.Errors.Add(ex.Message);
      }
      return RedirectToAction("Settings", "Admin", new { workspace = workspace, collection = collection });
    }

    [ScopeAuthorize, AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult BlogSettings(string workspace, string collection)
    {
      BlogSettingsModel m = new BlogSettingsModel();
      BlogAppCollection c = new BlogAppCollection(Collection);
      m.BloggingOn = c.BloggingOn;
      m.TrackbacksOn = c.TrackbacksOn;
      return PartialView("BlogSettingsWidget", m);
    }


    [ScopeAuthorize, AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult BlogSettingsEntireSite()
    {
      var m = new BlogSettingsEntireSiteModel();
      var svc = new BlogAppService(AppService);
      m.BlogPageExt = svc.BlogPageExt;
      m.ExtSelections = BlogSettingsEntireSiteModel.Extensions.Select(e =>
        new SelectListItem() { Value = e.Key, Text = e.Value, Selected = e.Key == svc.BlogPageExt });
      return PartialView("BlogSettingsEntireSiteWidget", m);
    }

    [ScopeAuthorize, AcceptVerbs(HttpVerbs.Post)]
    public ActionResult UpdateSettingsEntireSite(BlogSettingsEntireSiteModel m)
    {
      try
      {
        var svc = new BlogAppService(AppService);
        svc.BlogPageExt = m.BlogPageExt ?? string.Empty;
        AtomPubService.UpdateService(AppService);
        ServerApp.Restart();
        TempData["saved"] = true;
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        m.Errors.Add(ex.Message);
      }
      return RedirectToAction("Settings", "Admin");
    }

    protected AtomAuthor GetAnnonAuthor()
    {
      HttpCookie cookie = Request.Cookies["annotation"];
      AtomAuthor author = new AtomAuthor();
      author.Name = string.Empty;
      author.Email = string.Empty;
      if (cookie != null)
      {
        author.Name = Server.UrlDecode(cookie.Values["name"]);
        author.Email = cookie.Values["email"];
        if (!string.IsNullOrEmpty(cookie.Values["uri"]))
          author.Uri = new Uri(cookie.Values["uri"]);
      }
      return author;
    }
  }
}
