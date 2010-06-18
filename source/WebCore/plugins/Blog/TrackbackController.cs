/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Linq;
  using System.Web.Mvc;
  using System.Xml;
  using System.Xml.Linq;

  public class TrackbackController : BaseController
  {
    protected ITrackbackService TrackbackService { get; private set; }

    public TrackbackController(ITrackbackService trackback)
    {
      TrackbackService = trackback;
    }

    public ActionResult TrackbackWidget(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      return new ContentResult() { Content = TrackbackService.GetTrackbackMetadata(EntryId) };
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Trackback(string workspace, string collection, int? year,
      int? month, int? day, string path, string title, string excerpt, string url, string blog_name)
    {
      LogService.Info("Trackback posted for {0}", EntryId);
      // http://www.movabletype.org/docs/mttrackback.html
      return new XmlWriterResult((x) =>
          TrackbackService.ReceiveTrackback(EntryId, title, excerpt, url, blog_name,
          Request.UserHostAddress, Request.UrlReferrer).Xml.WriteTo(x));
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Pingback(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      LogService.Info("Pingback posted for {0}", EntryId);
      //methodCall[methodName/text()='pingback.ping']/params/param[1]/value/string/text()
      XDocument doc = XDocument.Load(new XmlTextReader(this.Request.InputStream));
      Uri sourceUri = new Uri(doc.Descendants("string").First().Value);
      //methodCall[methodName/text()='pingback.ping']/params/param[2]/value/string/text()
      Uri targetUri = new Uri(doc.Descendants("string").Last().Value);
      PingbackResult result = TrackbackService.ReceivePingback(EntryId, sourceUri, targetUri,
          Request.UserHostAddress, Request.UrlReferrer);
      return new XmlWriterResult((x) => result.Xml.WriteTo(x));
    }

  }
}
