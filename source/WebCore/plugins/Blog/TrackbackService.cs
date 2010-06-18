/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Net;
  using System.Text;
  using System.Web;
  using System.Web.Mvc;
  using System.Xml.Linq;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using AtomSite.Utils;
  using Sgml;
  using StructureMap;
  using System.Web.Routing;

  public class TrackbackService : ITrackbackService
  {
    protected ILogService LogService { get; private set; }
    protected AppService AppService { get; private set; }
    protected IContainer Container { get; private set; }
    protected IAnnotateService AnnotateService { get; private set; }
    protected IAtomEntryRepository AtomEntryRepository { get; private set; }

    public TrackbackService(IAppServiceRepository svcRepo, IAtomPubService atompub,
      IAnnotateService annotate, IAtomEntryRepository entryRepo,
      IContainer container, ILogService logger)
    {
      LogService = logger;
      AppService = svcRepo.GetService();
      Container = container;
      AnnotateService = annotate;
      AtomEntryRepository = entryRepo;

      atompub.EntryCreated += AutoPing;
      atompub.EntryUpdated += AutoPing;
    }

    protected virtual bool IsEnabled(AtomEntry entry)
    {
      BlogAppCollection coll = new BlogAppCollection(AppService.GetCollection(entry.Id));

      if (!coll.AnnotationsOn) return false; //in case they got turned off
      if (!coll.TrackbacksOn) return false;
      if (!entry.AllowAnnotate) return false;
      //TODO: check expired
      return true;
    }

    public virtual void AutoPing(AtomEntry entry)
    {
      LogService.Info("TrackbackService.AutoPing entryId={0}", entry.Id);
      //TODO: work in separate thread (use background worker or not?)

      if (!entry.Visible)
      {
        LogService.Info("Entry is not visible, AutoPing cancelled.");
        return;
      }

      if (!IsEnabled(entry))
      {
        LogService.Info("Annotations and/or trackbacks are disabled, AutoPing cancelled.");
        return;
      }

      IEnumerable<Uri> links = entry.Content.Type != "xhtml" ?
          WebHelper.ExtractLinks(HttpUtility.HtmlDecode(entry.Content.Xml.ToString())) :
          WebHelper.ExtractLinks(entry.Content.Text);

      foreach (Uri link in links)
      {
        try
        {
          string page = null;
          Uri pingbackUrl = null;

          //go ahead and download page
          using (WebClient client = new WebClient())
          {
            page = client.DownloadString(link);
            if (client.Headers["X-Pingback"] != null)
              pingbackUrl = new Uri(client.Headers["X-Pingback"]);
          }

          //try a trackback first since it supports title's and excerpts
          Uri trackbackUrl = DiscoverTrackbackPingUrl(page, link);
          if (trackbackUrl != null)
          {
            if (!SendTrackback(entry, trackbackUrl).Error)
              continue; //success, so move on to next link
          }

          //next try a pingback
          if (pingbackUrl == null) pingbackUrl = DiscoverPingbackLink(page);
          if (pingbackUrl != null)
          {
            if (!SendPingback(entry, pingbackUrl, link).FaultCode.HasValue)
              continue; //success
          }
        }
        catch (Exception ex)
        {
          LogService.Error("Failed to complete ping to link {0}", link.ToString());
          LogService.Error(ex);
        }
      }
    }

    public virtual TrackbackResult SendTrackback(AtomEntry entry, Uri pingUrl)
    {
      LogService.Info("TrackbackService.SendTrackback entryId={0} pingUrl={1}", entry.Id, pingUrl);
      try
      {
        var url = new UrlHelper(Container.GetInstance<RequestContext>());
        NameValueCollection data = new NameValueCollection();
        data.Add("title", entry.Title.ToString());
        data.Add("url", url.RouteIdUrl("BlogEntry", entry.Id, AbsoluteMode.Force));
        data.Add("excerpt", entry.Text.ToStringPreview(96));
        data.Add("blog_name", AppService.GetCollection(entry.Id).Title.ToString());

        using (WebClient client = new WebClient())
        {
          string result = Encoding.Default.GetString(client.UploadValues(pingUrl, data));
          return new TrackbackResult()
          {
            Xml = XElement.Parse(result.Substring(result.IndexOf('<')))
          };
        }
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return new TrackbackResult()
        {
          Error = true,
          Message = ex.Message
        };
      }
    }

    public virtual TrackbackResult ReceiveTrackback(Id entryId, string title, string excerpt,
        string url, string blogName, string ip, Uri referrer)
    {
      LogService.Info("TrackbackService.ReceiveTrackback entryId={0} title={1} url={2}", entryId, title, url);

      if (!IsEnabled(AtomEntryRepository.GetEntry(entryId)))
        return new TrackbackResult() { Error = true, Message = "Trackbacks are disabled." };

      try
      {
        string page = string.Empty;
        string contentType = "text/html";
        using (WebClient client = new WebClient())
        {
          page = client.DownloadString(url);
          if (client.ResponseHeaders["Content-Type"] != null)
            contentType = client.ResponseHeaders["Content-Type"];
        }

        var uh = new UrlHelper(Container.GetInstance<RequestContext>());
        //validate page has a link back
        if (!ContainsLink(page, uh.RouteIdUri("BlogEntry", entryId, AbsoluteMode.Force)))
          throw new AnnotationNotAllowedException(entryId, "trackback", "it does not link back");

        AtomEntry trackback = new AtomEntry();

        //content
        trackback.Content = new AtomContent() { Src = new Uri(url), Type = contentType };

        //title
        if (title == null) title = WebHelper.ExtractTitleForPage(page);
        if (title == null) title = "Trackback";
        trackback.Title = new AtomTitle() { Text = title };

        //summary
        if (excerpt == null) excerpt = WebHelper.ExtractDescriptionForPage(page);
        trackback.Summary = new AtomSummary() { Text = excerpt };

        //author
        trackback.Authors = new List<AtomPerson>() { new AtomAuthor() 
                { 
                    Name = blogName == null ? string.Empty : blogName,
                    Uri = referrer
                } };

        //add extension data?
        trackback.SetValue<string>(Atom.SvcNs + "ip", ip);
        trackback.AnnotationType = "trackback";

        AnnotateService.Annotate(entryId, trackback, null);

        return new TrackbackResult() { Error = false };
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return new TrackbackResult() { Error = true, Message = ex.Message };
      }
    }

    public virtual PingbackResult SendPingback(AtomEntry entry, Uri pingUrl, Uri pageUrl)
    {
      LogService.Info("TrackbackService.SendPingback entryId={0} pingUrl={1} pageUrl={2}", entry.Id, pingUrl, pageUrl);
      try
      {
        var url = new UrlHelper(Container.GetInstance<RequestContext>());
        using (WebClient client = new WebClient())
        {
          string response = client.UploadString(pingUrl,
              new XElement("methodCall",
                  new XElement("methodName", "pingback.ping"),
                  new XElement("params",
                      new XElement("param",
                          new XElement("value",
                          new XElement("string", url.RouteIdUrl("BlogEntry", entry.Id, AbsoluteMode.Force)))),
                      new XElement("param",
                          new XElement("value",
                          new XElement("string", pageUrl))))).ToString());
          return new PingbackResult()
          {
            Xml = XElement.Parse(response.Substring(response.IndexOf('<')))
          };
        }
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        PingbackResult r = new PingbackResult();
        r.SetFault(0, ex.Message);
        return r;
      }
    }

    public virtual PingbackResult ReceivePingback(Id entryId, Uri sourceUrl, Uri targetUrl, string ip, Uri referrer)
    {
      LogService.Info("TrackbackService.ReceivePingback entryId={0} sourceUrl={1} targetUrl={2}", entryId, sourceUrl, targetUrl);

      if (!IsEnabled(AtomEntryRepository.GetEntry(entryId)))
      {
        PingbackResult result = new PingbackResult();
        result.SetFault(49, "Pingbacks are turned off.");
        return result;
      }

      try
      {
        string page = string.Empty;
        string contentType = "text/html";
        using (WebClient client = new WebClient())
        {
          page = client.DownloadString(sourceUrl);
          if (client.ResponseHeaders["Content-Type"] != null)
            contentType = client.ResponseHeaders["Content-Type"];
        }

        //validate page has a link back
        //if (!ContainsLink(page, RouteService.IdToWebHref(entryId, null)))
        //  throw new AnnotationNotAllowedException(entryId, "trackback", "it does not link back");

        AtomEntry pingback = new AtomEntry();

        //content
        pingback.Content = new AtomContent() { Src = sourceUrl, Type = contentType };

        //title
        string title = null;
        if (title == null) title = WebHelper.ExtractTitleForPage(page);
        if (title == null) title = "Pingback";
        pingback.Title = new AtomTitle() { Text = title };

        //summary
        string summary = WebHelper.ExtractDescriptionForPage(page);
        if (summary == null) summary = string.Empty;
        pingback.Summary = new AtomSummary() { Text = summary };

        //author
        pingback.Authors = new List<AtomPerson>() { new AtomAuthor() 
                { 
                    Name = string.Empty,
                    Uri = referrer
                } };

        //add extension data?
        pingback.SetValue<string>(Atom.SvcNs + "ip", ip);
        pingback.AnnotationType = "pingback";

        //TODO: turn this into dependency
        AnnotateService.Annotate(entryId, pingback, null);
        return new PingbackResult() { Success = "Success" };
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        PingbackResult r = new PingbackResult();
        r.SetFault(0, ex.Message);
        return r;
      }
    }

    protected bool ContainsLink(string page, Uri link)
    {
      string url = link.AbsoluteUri;
      string urlEncoded = HttpUtility.UrlEncode(link.AbsoluteUri);
      if (page.Contains(url) || page.Contains(urlEncoded))
        return true;
      return false;
    }


    public string GetTrackbackMetadata(Id entryId)
    {
      var url = new UrlHelper(Container.GetInstance<RequestContext>());

      AtomEntry entry = AtomEntryRepository.GetEntry(entryId);
      string metadata = string.Empty;
      metadata += "\n<!--\n<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"\nxmlns:dc=\"http://purl.org/dc/elements/1.1/\"\nxmlns:trackback=\"http://madskills.com/public/xml/rss/module/trackback/\">\n";
      metadata += string.Format("<rdf:Description\nrdf:about=\"{0}\"\ndc:identifier=\"{1}\"\ndc:title=\"{2}\"\ntrackback:ping=\"{3}\" />\n</rdf:RDF>\n-->\n\n",
          url.RouteIdUrl("BlogEntry", entry.Id, AbsoluteMode.Force), entry.Id, entry.Title.ToString(), url.RouteIdUrl("Trackback", entry.Id, AbsoluteMode.Force));
      return metadata;
    }

    static readonly XNamespace trackback = "http://madskills.com/public/xml/rss/module/trackback/";



    /*
        <rdf:RDF xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
    xmlns:dc="http://purl.org/dc/elements/1.1/"
    xmlns:trackback="http://madskills.com/public/xml/rss/module/trackback/">
<rdf:Description rdf:about="http://www.chrisvandesteeg.nl/2008/06/13/jquery-for-aspnet-mvc-preview-3/"
dc:identifier="http://www.chrisvandesteeg.nl/2008/06/13/jquery-for-aspnet-mvc-preview-3/"
dc:title="jQuery for Asp.net MVC preview 3"
trackback:ping="http://www.chrisvandesteeg.nl/2008/06/13/jquery-for-aspnet-mvc-preview-3/trackback/" />
</rdf:RDF>				*/

    protected Uri DiscoverTrackbackPingUrl(string page, Uri pageUrl)
    {
      int start = 0; int end = 0;
      //TODO: unit test multiple rdf sections
      //TODO: case insensitive?
      while (end < page.Length && start != -1)
      {
        start = page.IndexOf("<rdf:RDF", end);
        if (start > -1)
        {
          end = page.IndexOf("</rdf:RDF>", start);
          XElement x = XElement.Parse(page.Substring(start, end - start) + "</rdf:RDF>");
          XElement desc = x.Descendants().Where(d => d.Attribute(trackback + "ping") != null).SingleOrDefault();
          //TODO: match pageUrl to identifier
          if (desc != null)
          {
            return new Uri(desc.Attribute(trackback + "ping").Value);
          }
        }
      }
      return null;
    }

    protected Uri DiscoverPingbackLink(string page)
    {
      try
      {
        using (SgmlReader reader = new SgmlReader())
        {
          while (reader.ReadToFollowing("link"))
          {
            XElement link = XElement.Load(reader);
            if (link.Attribute("rel") != null && link.Attribute("rel").Value == "pingback")
              return new Uri(link.Attribute("href").Value); //TODO: deal with base Uri?
          }
        }
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
      }
      return null;
    }
  }
}
