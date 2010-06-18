/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Net;
  using System.Web;
  using System.Web.Mvc;
  using System.Xml;
  using System.Xml.Linq;
  using AtomSite.Domain;

  public class AnnotateController : BaseController
  {
    protected IAnnotateService AnnotateService;
    protected IAtomPubService AtomPubService;
    public AnnotateController(IAnnotateService anno, IAtomPubService atompub)
      : base()
    {
      this.AnnotateService = anno;
      this.AtomPubService = atompub;
    }

    /// <summary>
    /// Post a new entry to an existing entry.  This will create an annotation.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    [AcceptVerbs(HttpVerbs.Post), ActionName("Entry")]
    public virtual ActionResult AnnotateEntry(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      if (HttpContext.Request.ContentType.ToUpper().StartsWith(Atom.ContentType.ToUpper()))
      {
        AtomEntry entry = new AtomEntry();
        //note: don't dispose of incoming stream or it will close the response, still true?
        XmlReader reader = new XmlTextReader(HttpContext.Request.InputStream);
        entry.Xml = XElement.Load(reader);
        Id id = Collection.Id.AddPath(year, month, day, path);
        entry = AnnotateService.Annotate(id, entry, GetSlug());
        return new XmlWriterResult((w) => entry.Xml.WriteTo(w))
        {
          StatusCode = HttpStatusCode.Created,
          ContentType = Atom.ContentTypeEntry,
          ETag = AtomPubService.GetEntryEtag(entry.Id),
          Location = entry.Location,
          ContentLocation = entry.Location
        };
      }
      else //media annotation
      {
        throw new NotImplementedException();
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult AnnotationsFeed(string workspace, string collection, int? year,
      int? month, int? day, string path, string type, int? page)
    {
      Id id = EntryId != null ? EntryId : Collection.Id;
      int pageIndex = (page ?? 1) - 1;
      AtomFeed feed = AnnotateService.GetAnnotations(id, true, pageIndex, 10);
      return GetFeedResult(feed, type);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Annotations(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      AtomFeed feed = AnnotateService.GetAnnotations(EntryId, true, 0, int.MaxValue);
      return PartialView("AnnotateListWidget", new FeedModel() { Feed = feed });
    }
  }
}
