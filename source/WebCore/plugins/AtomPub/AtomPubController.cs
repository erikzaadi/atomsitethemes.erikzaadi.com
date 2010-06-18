/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.IO;
  using System.Net;
  using System.Web;
  using System.Web.Mvc;
  using System.Xml;
  using System.Xml.Linq;
  using AtomSite.Domain;

  //[HandleSiteError(View = "Error")]
  public class AtomPubController : BaseController
  {
    protected IAtomPubService AtomPubService;
    public AtomPubController(IAtomPubService atompub)
      : base()
    {
      AtomPubService = atompub;
    }

    public ActionResult ServiceIndex()
    {
      FeedModel model = new FeedModel();
      model.Feed = AtomPubService.GetFeed(0, 10);
      return View("AtomPubIndex", model);
    }

    public ActionResult WorkspaceIndex(string workspace)
    {
      FeedModel model = new FeedModel();
      model.Feed = AtomPubService.GetFeed(workspace, 0, 10);
      return View("AtomPubIndex", model);
    }

    public ActionResult CollectionIndex(string workspace, string collection)
    {
      FeedModel model = new FeedModel();
      model.Feed = AtomPubService.GetFeed(Collection.Id, 0, 10);
      return View("AtomPubIndex", model);
    }

    public ActionResult Resource(string workspace, string collection)
    {
      EntryModel model = new EntryModel()
      {
        Entry = AtomPubService.GetEntry(EntryId)
      };
      return View("AtomPubResource", model);
    }

    [AcceptVerbs(HttpVerbs.Get), ActionName("Service")]
    public ActionResult GetService()
    {
      AppService s = AtomPubService.GetService();
      return new XmlWriterResult((w) => s.Xml.WriteTo(w))
      {
        ContentType = Atom.ContentTypeService
      };
    }

    [AcceptVerbs(HttpVerbs.Get), ActionName("Categories")]
    public ActionResult GetCategories(string workspace, string collection, string scheme)
    {
      //TODO: need support in AtomPubService
      throw new NotImplementedException();
    }

    [AcceptVerbs(HttpVerbs.Get), ActionName("Collection")]
    public ActionResult GetCollection(string workspace, string collection, int? page)
    {
      int pageIndex = (page ?? 1) - 1;
      AtomFeed feed = AtomPubService.GetCollectionFeed(Collection.Id, pageIndex);
      return new XmlWriterResult((w) => feed.Xml.WriteTo(w))
      {
        ContentType = Atom.ContentTypeFeed
      };
    }

    /// <summary>
    /// Post an atom entry or media resource to a collection.
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    [AcceptVerbs(HttpVerbs.Post), ActionName("Collection")]
    public ActionResult PostCollection(string workspace, string collection)
    {
      if (HttpContext.Request.ContentType.ToUpper().StartsWith(Atom.ContentType.ToUpper()))
      {
        AtomEntry entry = new AtomEntry();
        //note: don't dispose of incoming stream or it will close the response, still true?
        XmlReader reader = new XmlTextReader(HttpContext.Request.InputStream);
        entry.Xml = XElement.Load(reader);
        entry = AtomPubService.CreateEntry(Collection.Id, entry, GetSlug());
        return new XmlWriterResult((w) => entry.Xml.WriteTo(w))
        {
          StatusCode = HttpStatusCode.Created,
          ContentType = Atom.ContentTypeEntry,
          ETag = AtomPubService.GetEntryEtag(entry.Id),
          Location = entry.Location,
          ContentLocation = entry.Location
        };
      }
      else //not atom so must be media
      {
        AtomEntry mediaLinkEntry = AtomPubService.CreateMedia(Collection.Id, HttpContext.Request.InputStream,
          GetSlug(), HttpContext.Request.ContentType);
        string contentType;
        return new XmlWriterResult((w) => mediaLinkEntry.Xml.WriteTo(w))
        {
          StatusCode = HttpStatusCode.Created,
          ContentType = Atom.ContentTypeEntry,
          ETag = AtomPubService.GetMediaEtag(mediaLinkEntry.Id, out contentType),
          Location = mediaLinkEntry.Location,
          ContentLocation = mediaLinkEntry.Location
        };
      }
    }

    [AcceptVerbs(HttpVerbs.Get), ActionName("Entry")]
    public ActionResult GetEntry(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      Id id = Collection.Id.AddPath(year, month, day, path);
      string etag = AtomPubService.GetEntryEtag(id);
      if (NoneMatch(etag)) //only get data when user needs it
      {
        AtomEntry entry = AtomPubService.GetEntry(id);
        return new XmlWriterResult((w) => entry.Xml.WriteTo(w))
        {
          ContentType = Atom.ContentTypeEntry,
          ETag = AtomPubService.GetEntryEtag(id),
        };
      }
      else
      {
        return new HttpResult() { StatusCode = HttpStatusCode.NotModified };
      }
    }

    [AcceptVerbs(HttpVerbs.Put), ActionName("Entry")]
    public ActionResult PutEntry(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      if (!HttpContext.Request.ContentType.ToUpper().StartsWith(Atom.ContentType.ToUpper()))
        throw new InvalidContentTypeException(HttpContext.Request.ContentType);

      Id id = Collection.Id.AddPath(year, month, day, path);
      string etag = AtomPubService.GetEntryEtag(id);

      if (Match(etag)) //only update when user had latest data
      {
        AtomEntry entry = new AtomEntry();
        //note: don't dispose of incoming stream or it will close the response, still true?
        XmlReader reader = new XmlTextReader(HttpContext.Request.InputStream);
        entry.Xml = XElement.Load(reader);
        entry = AtomPubService.UpdateEntry(id, entry, GetSlug());

        return new XmlWriterResult((w) => entry.Xml.WriteTo(w))
        {
          ContentType = Atom.ContentTypeEntry,
          ETag = AtomPubService.GetEntryEtag(id)
          //locations?
        };
      }
      else
      {
        return new HttpResult() { StatusCode = HttpStatusCode.PreconditionFailed };
      }
    }

    [AcceptVerbs(HttpVerbs.Delete), ActionName("Entry")]
    public ActionResult DeleteEntry(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      Id id = Collection.Id.AddPath(year, month, day, path);
      string etag = AtomPubService.GetEntryEtag(id);
      if (Match(etag))
      {
        AtomPubService.DeleteEntry(id);
        return new EmptyResult();
      }
      else
      {
        return new HttpResult() { StatusCode = HttpStatusCode.PreconditionFailed };
      }
    }

    [AcceptVerbs(HttpVerbs.Get), ActionName("Media")]
    public ActionResult GetMedia(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      Id id = Collection.Id.AddPath(year, month, day, path);
      string contentType;
      string etag = AtomPubService.GetMediaEtag(id, out contentType);

      if (NoneMatch(etag)) //only get data when user needs it
      {
        Stream stream = AtomPubService.GetMedia(id, out contentType);
        return new StreamResult()
        {
          Stream = stream,
          ContentType = contentType,
          ETag = etag
        };
      }
      else
      {
        return new HttpResult() { StatusCode = HttpStatusCode.NotModified };
      }
    }

    [AcceptVerbs(HttpVerbs.Put), ActionName("Media")]
    public ActionResult PutMedia(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      Id id = Collection.Id.AddPath(year, month, day, path);
      string contentType;
      string etag = AtomPubService.GetMediaEtag(id, out contentType);

      if (Match(etag)) //only update when user had latest data
      {
        AtomEntry entry = AtomPubService.UpdateMedia(id, HttpContext.Request.InputStream,
          HttpContext.Request.ContentType);
        return new EmptyResult();
      }
      else
      {
        return new HttpResult() { StatusCode = HttpStatusCode.PreconditionFailed };
      }
    }

    [AcceptVerbs(HttpVerbs.Delete), ActionName("Media")]
    public ActionResult DeleteMedia(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      Id id = Collection.Id.AddPath(year, month, day, path);
      string contentType;
      string etag = AtomPubService.GetMediaEtag(id, out contentType);
      if (Match(etag))
      {
        AtomPubService.DeleteMedia(id);
        return new EmptyResult();
      }
      else
      {
        return new HttpResult() { StatusCode = HttpStatusCode.PreconditionFailed };
      }
    }

    [AcceptVerbs(HttpVerbs.Head), ActionName("Entry")]
    public ActionResult HeadEntry(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      string etag = AtomPubService.GetEntryEtag(
        Collection.Id.AddPath(year, month, day, path));
      HttpResult result = new HttpResult() { ContentType = Atom.ContentTypeEntry };
      result.Headers["ETag"] = etag;
      return result;
    }

    [AcceptVerbs(HttpVerbs.Head), ActionName("Media")]
    public ActionResult HeadMedia(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      string contentType;
      string etag = AtomPubService.GetMediaEtag(Collection.Id.AddPath(year, month, day, path),
        out contentType);
      HttpResult result = new HttpResult() { ContentType = contentType };
      result.Headers["ETag"] = etag;
      return result;
    }

    [AcceptVerbs(HttpVerbs.Put), ActionName("Service")]
    public ActionResult PutService()
    {
      throw new NotImplementedException();
    }

    [AcceptVerbs(HttpVerbs.Get), ActionName("Feed")]
    public ActionResult GetFeed(string workspace, string collection, string type, int? page)
    {
      if (!Collection.SyndicationOn) throw new ResourceNotFoundException(type, "feed");

      int pageIndex = (page ?? 1) - 1;
      AtomFeed feed = AtomPubService.GetFeed(Collection.Id, pageIndex, 10);

      //if extended entries are on then only show content before split
      if (Collection.ExtendedEntriesOn)
      {
        foreach (AtomEntry e in feed.Entries)
        {
          e.Content = e.IsExtended ? e.ContentBeforeSplit : e.Content;
        }
      }

      return GetFeedResult(feed, type);
    }

    [AcceptVerbs(HttpVerbs.Post), ActionName("ApproveEntry")]
    public ActionResult ApproveEntry(string workspace, string collection, int? year,
      int? month, int? day, string path, bool? approved)
    {
      AtomPubService.ApproveEntry(Collection.Id.AddPath(year, month, day, path), approved ?? true);
      return new EmptyResult();
    }

    [AcceptVerbs(HttpVerbs.Post), ActionName("ApproveAll")]
    public ActionResult ApproveAll(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      //TODO: use dep inj
      int count = AtomPubService.ApproveAll(Collection.Id.AddPath(year, month, day, path));
      return new ContentResult()
      {
        Content = count.ToString(),
        ContentType = "text/plain"
      };
    }
  }
}
