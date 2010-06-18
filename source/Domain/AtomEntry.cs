/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.ServiceModel.Syndication;
  using System.Text;
  using System.Web.Routing;
  using System.Xml;
  using System.Xml.Linq;
  using System.Xml.Serialization;

  /// <summary>
  /// Describes an individual entry, acting as a container for metadata and
  /// data associated with the entry. 
  /// </summary>
  [XmlRoot("entry", Namespace="http://www.w3.org/2005/Atom")]
  public class AtomEntry : XmlBase, IXmlSerializable, IComparable<AtomEntry>
  {
    public AtomEntry(XElement xml) : base(xml) { }
    public AtomEntry()
      : this(new XElement(Atom.AtomNs + "entry"))
    {
      SetNamespaces();
    }

    public void SetNamespaces()
    {
      Xml.SetAttributeValue(XNamespace.Xmlns + "atom", Atom.AtomNs.NamespaceName);
      Xml.SetAttributeValue(XNamespace.Xmlns + "svc", Atom.SvcNs.NamespaceName);
      Xml.SetAttributeValue(XNamespace.Xmlns + "thread", Atom.ThreadNs.NamespaceName);
      Xml.SetAttributeValue(XNamespace.Xmlns + "os", Atom.OpenSearchNs.NamespaceName);
    }

    public event Action<AtomEntry> IdChanged;

    public Uri Location { get { return Links.GetLinkUri("self"); } }
    public Uri LocationWeb { get { return Links.GetLinkUri("alternate"); } }

    public bool IsExternal { get { return Content != null && Content.Src != null; } }
    public bool Visible { get { return !Draft && Approved && Published.HasValue && Published.Value < DateTimeOffset.Now; } }

    /// <summary>
    /// Gets or sets whether this entry is a media link entry.  This is an extension.
    /// </summary>
    /// <value>True when entry is media link.</value>
    public bool Media
    {
      get { return GetBooleanWithDefault(Atom.SvcNs + "media", false); }
      set { SetBoolean(Atom.SvcNs + "media", value); }
    }

    /// <summary>
    /// Gets or sets the authors.
    /// </summary>
    /// <value>The authors.</value>
    public IEnumerable<AtomPerson> Authors
    {
      get { return GetXmlValues<AtomPerson>(Atom.AtomNs + "author"); }
      set { SetXmlValues<AtomPerson>(Atom.AtomNs + "author", value); }
    }

    /// <summary>
    /// Gets or sets the categories.
    /// </summary>
    /// <value>The categories.</value>
    public IEnumerable<AtomCategory> Categories
    {
      get { return GetXmlValues<AtomCategory>(Atom.AtomNs + "category"); }
      set { SetXmlValues<AtomCategory>(Atom.AtomNs + "category", value); }
    }

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    /// <value>The content.</value>
    public AtomContent Content
    {
      get { return GetXmlValue<AtomContent>(Atom.AtomNs + "content"); }
      set { SetXmlValue<AtomContent>(Atom.AtomNs + "content", value); }
    }

    /// <summary>
    /// Gets or sets the contributors.
    /// </summary>
    /// <value>The contributors.</value>
    public IEnumerable<AtomPerson> Contributors
    {
      get { return GetXmlValues<AtomPerson>(Atom.AtomNs + "contributor"); }
      set { SetXmlValues<AtomPerson>(Atom.AtomNs + "contributor", value); }
    }

    public IEnumerable<AtomPerson> People
    {
      get { return Authors.Union(Contributors); }
    }

    /// <summary>
    /// Gets or sets the id which also changes the links.  The id should not
    /// change once it has been published.
    /// </summary>
    /// <value>The id.</value>
    public Id Id
    {
      get { return GetUri(Atom.AtomNs + "id"); }
      set { SetUri(Atom.AtomNs + "id", value); OnIdChanged(); }
    }

    protected void OnIdChanged()
    {
      if (IdChanged != null) IdChanged(this);
    }

    /// <summary>
    /// Gets or sets the links.
    /// </summary>
    /// <value>The links.</value>
    public IEnumerable<AtomLink> Links
    {
      get { return GetXmlValues<AtomLink>(Atom.AtomNs + "link"); }
      set { SetXmlValues<AtomLink>(Atom.AtomNs + "link", value); }
    }

    /// <summary>
    /// Gets or sets the published.
    /// </summary>
    /// <value>The published date.</value>
    public DateTimeOffset? Published
    {
      get { return (DateTimeOffset?)Xml.Element(Atom.AtomNs + "published"); }
      set { Xml.Elements(Atom.AtomNs + "published").Remove(); if (value.HasValue) Xml.Add(new XElement(Atom.AtomNs + "published", value)); }
    }

    /// <summary>
    /// Gets or sets when the entry was edited.  This is part of the Atom Publishing Protocol.  This value should be updated
    /// each time an entry is editied.
    /// </summary>
    /// <value>The edited date.</value>
    public DateTimeOffset? Edited
    {
      get { return (DateTimeOffset?)Xml.Element(Atom.AppNs + "edited"); }
      set { Xml.Elements(Atom.AppNs + "edited").Remove(); if (value.HasValue) Xml.Add(new XElement(Atom.AppNs + "edited", value)); }
    }

    /// <summary>
    /// Gets or sets the rights.
    /// </summary>
    /// <value>The rights.</value>
    public AtomText Rights
    {
      get { return GetXmlValue<AtomText>(Atom.AtomNs + "rights"); }
      set { SetXmlValue<AtomText>(Atom.AtomNs + "rights", value); }
    }

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    /// <value>The source.</value>
    //public AtomSource Source { get; set; }

    /// <summary>
    /// Gets or sets the summary.
    /// </summary>
    /// <value>The summary.</value>
    public AtomText Summary
    {
      get { return GetXmlValue<AtomText>(Atom.AtomNs + "summary"); }
      set { SetXmlValue<AtomText>(Atom.AtomNs + "summary", value); }
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

    /// <summary>
    /// Gets or sets the total number of annotations.  This is part of the Atom
    /// Threading Extensions.
    /// </summary>
    /// <see cref="http://www.ietf.org/rfc/rfc4685.txt"/>
    public int? Total
    {
      get { return (int?)Xml.Element(Atom.ThreadNs + "total") ?? 0; }
      set { Xml.Elements(Atom.ThreadNs + "total").Remove(); if (value.HasValue) Xml.Add(new XElement(Atom.ThreadNs + "total", value)); }
    }

    /// <summary>
    /// Gets or sets the total number of occurances of a rating. This is an extension.
    /// </summary>
    public int RatingCount
    {
      get { return (int?)Xml.Element(Atom.SvcNs + "ratingCount") ?? 0; }
      set { Xml.Elements(Atom.SvcNs + "ratingCount").Remove(); Xml.Add(new XElement(Atom.SvcNs + "ratingCount", value)); }
    }

    /// <summary>
    /// Gets or sets the sum of all the rating values. This is an extension.
    /// </summary>
    public int RatingSum
    {
      get { return (int?)Xml.Element(Atom.SvcNs + "ratingSum") ?? 0; }
      set { Xml.Elements(Atom.SvcNs + "ratingSum").Remove(); Xml.Add(new XElement(Atom.SvcNs + "ratingSum", value)); }
    }

    /// <summary>
    /// Gets the rating of the entry based on the number of ratings and the sum of all those ratings.
    /// This value is calculated from the ratingCount and ratingSum.
    /// </summary>
    public float Rating
    {
      get { return RatingCount > 0 ? (float)RatingSum / (float)RatingCount : 0F; }
    }

    /// <summary>
    /// Gets or sets the IP addresses of the raters of an entry.
    /// </summary>
    public IEnumerable<string> Raters
    {
      get { return GetValue<string>(Atom.SvcNs + "raters") == null ? Enumerable.Empty<string>().ToArray() : GetValue<string>(Atom.SvcNs + "raters").Split(';'); }
      set { SetValue<string>(Atom.SvcNs + "raters", string.Join(";", value.ToArray())); }
    }

    /// <summary>
    /// Gets or sets which resource this is a reply to.  This is part of the Atom
    /// Threading Extensions.
    /// </summary>
    /// <see cref="http://www.ietf.org/rfc/rfc4685.txt"/>
    public ThreadInReplyTo InReplyTo
    {
      get { return GetXmlValue<ThreadInReplyTo>(Atom.ThreadNs + "in-reply-to"); }
      set { SetXmlValue<ThreadInReplyTo>(Atom.ThreadNs + "in-reply-to", value); }
    }

    /// <summary>
    /// Gets or sets the publication control settings.  This is part of the Atom Publishing
    /// Protocol.
    /// </summary>
    /// <value>The pub control.</value>
    public AppControl Control
    {
      get { return GetXmlValue<AppControl>(Atom.AppNs + "control"); }
      set { SetXmlValue<AppControl>(Atom.AppNs + "control", value); }
    }

    public string AnnotationType
    {
      get { return GetValue<string>(Atom.SvcNs + "annotationType"); }
      set { SetValue<string>(Atom.SvcNs + "annotationType", value); }
    }

    public bool Draft
    {
      get { return Control != null && Control.Draft.HasValue ? Control.Draft.Value : false; }
    }

    public bool Approved
    {
      get { return Control != null && Control.Approved.HasValue ? Control.Approved.Value : true; }
    }

    public bool AllowAnnotate
    {
      get { return Control != null && Control.AllowAnnotate.HasValue ? Control.AllowAnnotate.Value : true; }
    }

    /// <summary>
    /// Gets the content of the post, or the summary if there is no content.
    /// This is a null safe convienence method.
    /// </summary>
    public AtomText Text
    {
      get
      {
        return (Content != null && !string.IsNullOrEmpty(Content.Text)) ? Content.ToText() :
          (Summary != null && !string.IsNullOrEmpty(Summary.Text)) ? Summary :
          new AtomText() { Text = string.Empty };
      }
    }

    public bool IsExtended
    {
      get { return (Content != null ? Content.IsExtended : false); }
    }

    /// <summary>
    /// Gets the content before a split when extended.
    /// </summary>
    public AtomContent ContentBeforeSplit
    {
      get { return IsExtended ? Content.BeforeSplit : Content; }
    }

    /// <summary>
    /// Gets the content after a split when extended.
    /// </summary>
    public AtomContent ContentAfterSplit
    {
      get { return IsExtended ? Content.AfterSplit : Content; }
    }


    /// <summary>
    /// Gets the date the entry was published or if there is no published date,
    /// it returns the last updated date.  This is a null safe convienence method.
    /// </summary>
    public DateTimeOffset Date
    {
      get
      {
        return Published.HasValue ? Published.Value :
          Updated;
      }
    }

    public bool IsOwnedBy(AtomEntry parent)
    {
      return Authors.Intersect(parent.Authors).Count() < Authors.Count();
    }

    /// <summary>
    /// Returns true if this object is equal to <c>obj</c>.
    /// </summary>
    /// <param name="obj">Object you wish to compare to.</param>
    /// <returns>true if this object is equal to <c>obj</c>.</returns>
    public override bool Equals(object obj)
    {
      if (obj != null && obj.GetType().Equals(this.GetType()))
      {
        AtomEntry other = obj as AtomEntry;
        if ((object)other != null)
        {
          return this.Id.Equals(other.Id);
          //bool equal = true;
          //equal &= (other.Title == null && Title == null) | (other.Title != null && other.Title.Equals(Title));
          //equal &= (other.Summary == null && Summary == null) | (other.Summary != null && other.Summary.Equals(Summary));
          //equal &= (other.Links == null && Links == null) | (other.Links != null && other.Links.SequenceEqual(Links));
          //equal &= (other.Id == null && Id == null) | (other.Id != null && other.Id.Equals(Id));
          //equal &= other.Updated.ToUniversalTime().Equals(Updated.ToUniversalTime());
          //equal &= (other.Published == null && Published == null) | (other.Published.HasValue && Published.HasValue && other.Published.Value.ToUniversalTime().Equals(Published.Value.ToUniversalTime()));
          //equal &= (other.Authors == null && Authors == null) | (other.Authors != null && other.Authors.SequenceEqual(Authors));
          //equal &= (other.Contributors == null && Contributors == null) | (other.Contributors != null && other.Contributors.SequenceEqual(Contributors));
          //equal &= (other.Content == null && Content == null) | (other.Content != null && other.Content.Equals(Content));
          //equal &= (other.Categories == null && Categories == null) | (other.Categories != null && other.Categories.SequenceEqual(Categories));
          //equal &= (other.Rights == null && Rights == null) | (other.Rights != null && other.Rights.Equals(Rights));
          //equal &= (other.InReplyTo == null && InReplyTo == null) | (other.InReplyTo != null && other.InReplyTo.Equals(InReplyTo));
          ////TODO: extensions
          //return equal;
        }
      }
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return Id.GetHashCode();
    }

    //TODO: should this be moved as private into AppCollection?
    public string BuildPath(string parentPath, string requestedSlug)
    {
      //use title when slug not given
      string entryName = (!string.IsNullOrEmpty(requestedSlug) ? requestedSlug :
        Title != null ? Title.Text : "").CleanSlug();
      return !string.IsNullOrEmpty(parentPath) ? parentPath + "," + entryName : entryName;
    }

    public void Save(string path)
    {
      Xml.Save(path);
    }

    public static AtomEntry Load(Uri uri)
    {
      return Load(SyndicationItem.Load(new XmlTextReader(uri.AbsoluteUri)));
    }

    protected static AtomEntry Load(SyndicationItem item)
    {
      using (MemoryStream ms = new MemoryStream())
      {
        XmlWriter w = new XmlTextWriter(ms, Encoding.UTF8);
        item.GetAtom10Formatter().WriteTo(w);
        w.Flush();
        AtomEntry entry = new AtomEntry();
        ms.Position = 0;
        XmlReader r = new XmlTextReader(ms);
        entry.ReadXml(r);
        return entry;
      }
    }

    public static AtomEntry Load(string path)
    {
      AtomEntry entry = new AtomEntry();
      entry.Xml = XElement.Load(path, LoadOptions.None);
      return entry;
    }

    #region IXmlSerializable Members

    public System.Xml.Schema.XmlSchema GetSchema()
    {
      throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
      Xml = XElement.ReadFrom(reader) as XElement;
    }

    public void WriteXml(XmlWriter writer)
    {
      //TODO: use base.WriteXml
      bool start = false;
      if (writer.WriteState == System.Xml.WriteState.Start)
      {
        start = true;
        writer.WriteStartDocument();
        writer.WriteStartElement("entry", Atom.AtomNs.NamespaceName);
      }
      foreach (XAttribute att in Xml.Attributes())
      {
        if (att.IsNamespaceDeclaration && att.Value == Atom.AtomNs.NamespaceName) continue;
        writer.WriteAttributeString(att.Name.LocalName, att.Name.NamespaceName, att.Value);
      }
      foreach (XNode el in Xml.Nodes())
      {
        el.WriteTo(writer);
      }
      if (start)
      {
        writer.WriteEndElement();
        writer.WriteEndDocument();
      }
    }

    #endregion

    #region IComparable<AtomEntry> Members

    public int CompareTo(AtomEntry other)
    {
      return this.Date.CompareTo(other.Date);
    }

    #endregion

    public override string ToString()
    {
      return Title.Text.ToString();
    }
  }
}
