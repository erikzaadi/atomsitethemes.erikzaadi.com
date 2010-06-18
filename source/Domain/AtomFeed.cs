/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.ServiceModel.Syndication;
  using System.Text;
  using System.Web;
  using System.Web.Caching;
  using System.Web.Routing;
  using System.Xml;
  using System.Xml.Linq;
  using System.Xml.Serialization;

  /// <summary>
  /// Describes a feed containing multiple entries.
  /// </summary>
  [XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
  public class AtomFeed : XmlBase, IXmlSerializable
  {
    public AtomFeed() : base(new XElement(Atom.AtomNs + "feed")) { }

    public Uri Location { get { return Links.Where(l => l.Rel == "self").Single().Href; } }

    public IEnumerable<AtomPerson> Authors
    {
      get { return GetXmlValues<AtomPerson>(Atom.AtomNs + "author"); }
      set { SetXmlValues<AtomPerson>(Atom.AtomNs + "author", value); }
    }
    public IEnumerable<AtomCategory> Categories
    {
      get { return GetXmlValues<AtomCategory>(Atom.AtomNs + "category"); }
      set { SetXmlValues<AtomCategory>(Atom.AtomNs + "category", value); }
    }
    public IEnumerable<AtomPerson> Contributors
    {
      get { return GetXmlValues<AtomPerson>(Atom.AtomNs + "contributor"); }
      set { SetXmlValues<AtomPerson>(Atom.AtomNs + "contributor", value); }
    }
    public IEnumerable<AtomPerson> People
    {
      get { return Authors.Union(Contributors); }
    }
    public AtomGenerator Generator
    {
      get { return GetXmlValue<AtomGenerator>(Atom.AtomNs + "generator"); }
      set { SetXmlValue<AtomGenerator>(Atom.AtomNs + "generator", value); }
    }
    public Uri Icon
    {
      get { return GetUri(Atom.AtomNs + "icon"); }
      set { SetUri(Atom.AtomNs + "icon", value); }
    }

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    /// <value>The id.</value>
    public Uri Id
    {
      get { return GetUri(Atom.AtomNs + "id"); }
      set { SetUri(Atom.AtomNs + "id", value); }
    }
    public IEnumerable<AtomLink> Links
    {
      get { return GetXmlValues<AtomLink>(Atom.AtomNs + "link"); }
      set { SetXmlValues<AtomLink>(Atom.AtomNs + "link", value); }
    }
    public Uri Logo
    {
      get { return GetUri(Atom.AtomNs + "logo"); }
      set { SetUri(Atom.AtomNs + "logo", value); }
    }

    public AtomText Rights
    {
      get { return GetXmlValue<AtomText>(Atom.AtomNs + "rights"); }
      set { SetXmlValue<AtomText>(Atom.AtomNs + "rights", value); }
    }

    /// <summary>
    /// Gets or sets the sub title description.
    /// </summary>
    /// <value>The subtitle.</value>
    public AtomText Subtitle
    {
      get { return GetXmlValue<AtomText>(Atom.AtomNs + "subtitle"); }
      set { SetXmlValue<AtomText>(Atom.AtomNs + "subtitle", value); }
    }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>The title.</value>
    public AtomText Title
    {
      get { return GetXmlValue<AtomText>(Atom.AtomNs + "title"); }
      set { SetXmlValue<AtomText>(Atom.AtomNs + "title", value); }
    }

    /// <summary>
    /// Gets or sets the updated.
    /// </summary>
    /// <value>The updated.</value>
    public DateTimeOffset Updated
    {
      get { return Xml.Element(Atom.AtomNs + "updated") != null ? (DateTimeOffset)Xml.Element(Atom.AtomNs + "updated") : DateTimeOffset.MinValue; }
      set { Xml.Elements(Atom.AtomNs + "updated").Remove(); Xml.Add(new XElement(Atom.AtomNs + "updated", value)); }
    }

    public IEnumerable<AtomEntry> Entries
    {
      get { return GetXmlValues<AtomEntry>(Atom.AtomNs + "entry"); }
      set { SetXmlValues<AtomEntry>(Atom.AtomNs + "entry", value); }
    }

    /// <summary>
    /// Gets or sets the total number of entries in a feed.
    /// </summary>
    /// <see cref="http://www.opensearch.org/Specifications/OpenSearch/1.1#The_.22totalResults.22_element"/>
    public int? TotalResults
    {
      get { return (int?)Xml.Element(Atom.OpenSearchNs + "totalResults"); }
      set { Xml.Elements(Atom.OpenSearchNs + "totalResults").Remove(); if (value.HasValue) Xml.Add(new XElement(Atom.OpenSearchNs + "totalResults", value)); }
    }

    public bool Owns(AtomEntry entry)
    {
      return entry.People.Where(p => People.Contains(p)).Count() > 0;
    }

    public static AtomFeed BuildFeed(AppCollection coll, IEnumerable<AtomEntry> entries,
      int total)//, int pageIndex, int pageSize), Func<string, Id, RouteValueDictionary, Uri> routeUri,
      //string atomRouteName, string webRouteName, bool addPaging)
    {
      AtomFeed feed = new AtomFeed();
      feed.Generator = new AtomGenerator { Text = "atomsite.net" };
      feed.Title = coll.Title;
      feed.Subtitle = coll.Subtitle;
      feed.Id = coll.Id;
      feed.Logo = coll.Logo;
      feed.Icon = coll.Icon;
      feed.Rights = coll.Rights;
      feed.Updated = DateTimeOffset.UtcNow;
      feed.TotalResults = total;
      feed.Entries = entries;

      //refresh links in case they change
      //foreach (AtomEntry e in feed.Entries) e.UpdateLinks(routeUri);

      //use newest updated date as feed updated date
      if (feed.Entries.Count() > 0)
        feed.Updated = feed.Entries.Max().Updated;

      //feed.GenerateLinks(routeUri, atomRouteName, webRouteName, pageIndex, pageSize, addPaging);

      //TODO: ensure all entries have author
      //NOTE: the below two statements would break any Views built on a FeedModel
      //TODO: save bandwidth, add author at feed level if all authors are the same, remove from entries
      //TODO: save bandwidth, add contrib at feed level if all contrib are the same, remove from entries
      return feed;
    }

    public static AtomFeed LoadWithCaching(Uri uri, int maxEntries, int cacheSeconds)
    {
      lock (HttpRuntime.Cache)
      {
        if (HttpRuntime.Cache["AtomFeed@" + uri.AbsolutePath] != null)
        {
          return HttpRuntime.Cache["AtomFeed@" + uri.AbsolutePath] as AtomFeed;
        }
        else
        {
          AtomFeed feed = Load(uri, maxEntries);
          HttpRuntime.Cache.Add("AtomFeed@" + uri.AbsolutePath, feed, null,
            DateTime.Now.AddSeconds(cacheSeconds), TimeSpan.Zero,
            CacheItemPriority.Normal, null);
          return feed;
        }
      }
    }

    public static AtomFeed Load(Uri uri, int maxEntries)
    {
      return Load(uri.AbsoluteUri, maxEntries);
    }

    public static AtomFeed Load(string uri, int maxEntries)
    {
      try
      {
        return Load(SyndicationFeed.Load(new XmlTextReader(uri)), maxEntries);
      }
      catch (Exception ex)
      {
        //TODO: use logger?
        Trace.WriteLine(ex);
      }      
      return Empty;
    }

    public static readonly AtomFeed Empty = new AtomFeed() { Title = new AtomTitle() { Text = "Empty Feed" }, Entries = new List<AtomEntry>() };

    public static AtomFeed Load(Uri uri)
    {
      return Load(uri.AbsoluteUri, Int32.MaxValue);
    }

    public static AtomFeed Load(string uri)
    {
      return Load(uri, Int32.MaxValue);
    }

    public static AtomFeed Load(SyndicationFeed feed)
    {
      return Load(feed, int.MaxValue);
    }

    public static AtomFeed Load(SyndicationFeed feed, int maxEntries)
    {
      feed.Items = feed.Items.Take(maxEntries);
      using (MemoryStream ms = new MemoryStream())
      {
        XmlWriter w = new XmlTextWriter(ms, Encoding.UTF8);
        feed.GetAtom10Formatter().WriteTo(w);
        w.Flush();
        AtomFeed atomFeed = new AtomFeed();
        ms.Position = 0;
        XmlReader r = new XmlTextReader(ms);
        atomFeed.ReadXml(r);
        return atomFeed;
      }
    }

    public void WriteRssTo(XmlWriter writer)
    {
      using (MemoryStream ms = new MemoryStream())
      {
        XmlWriter w = new XmlTextWriter(ms, Encoding.UTF8);
        Xml.WriteTo(w);
        w.Flush();
        ms.Position = 0;
        SyndicationFeed feed = SyndicationFeed.Load(new XmlTextReader(ms));
        feed.SaveAsRss20(writer);
      }
    }

    #region IXmlSerializable Members

    public System.Xml.Schema.XmlSchema GetSchema()
    {
      throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
      Xml = XElement.Load(reader, LoadOptions.SetBaseUri);
    }

    public void WriteXml(XmlWriter writer)
    {
      //TODO: does it need other prefixes?
      WriteXml("feed", Atom.AtomNs.NamespaceName, writer);
    }


    #endregion
  }
}
