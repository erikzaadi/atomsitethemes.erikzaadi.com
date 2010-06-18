/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml.Linq;
  using System.Collections;
  using System.ComponentModel.DataAnnotations;

  public class AppCollection : XmlBase
  {
    public AppCollection() : this(new XElement(Atom.AppNs + "collection")) { }
    public AppCollection(XElement xml) : base(xml) { }

    /// <summary>
    /// Gets or sets the title of the collection.  This value is copied to the feed.
    /// </summary>
    /// <value>The title.</value>
    [Required]
    public AtomText Title
    {
      get { return GetXmlValue<AtomText>(Atom.AtomNs + "title"); }
      set { SetXmlValue<AtomText>(Atom.AtomNs + "title", value); }
    }

    /// <summary>
    /// Gets or sets the location of the collection feed (private).  Note this is different from
    /// a normal feed (public) as it may contain secure information.
    /// </summary>
    /// <value>The href.</value>
    public Uri Href
    {
      get { return GetUriProperty("href"); }
      set { SetUriProperty("href", value); }
    }

    /// <summary>
    /// Gets or sets whether this is the default collection in the workspace.
    /// </summary>
    /// <value>True for default, false otherwise.</value>
    public bool Default
    {
      get { return GetBooleanPropertyWithDefault(Atom.SvcNs + "default", false); }
      set { SetBooleanProperty(Atom.SvcNs + "default", value); }
    }

    /// <summary>
    /// Gets or sets the role matrix which determines the actions that
    /// each role is allowed to perform.  Actions specified here will
    /// override those actions specified at a higher level.
    /// </summary>
    public RoleMatrix RoleMatrix
    {
      get { return GetXmlValue<RoleMatrix>(Atom.SvcNs + "roleMatrix"); }
      set { SetXmlValue<RoleMatrix>(Atom.SvcNs + "roleMatrix", value); }
    }

    /// <summary>
    /// Gets or sets the widget area includes to display for this collection.
    /// Note: the includes are combined with includes in the other scopes.
    /// </summary>
    public IEnumerable<ServiceWidget> Widgets
    {
      get { return GetXmlValues<ServiceWidget>(Atom.SvcNs + "widget"); }
      set { SetXmlValues<ServiceWidget>(Atom.SvcNs + "widget", value); }
    }

    /// <summary>
    /// Gets or sets the page area includes to display for this collection.
    /// Note: the includes are combined with includes in the other scopes.
    /// </summary>
    public IEnumerable<ServicePage> Pages
    {
      get { return GetXmlValues<ServicePage>(Atom.SvcNs + "page"); }
      set { SetXmlValues<ServicePage>(Atom.SvcNs + "page", value); }
    }

    /// <summary>
    /// Gets or sets whether the dates are important for this collection.  A blog is an 
    /// example of a dated collection.  You may use a non-dated collection for a group of
    /// documentation articles. This value changes how id's are generated for the entries
    /// in a collection. 
    /// 
    /// Dated: tag:example.com,2008-10-10:blog,MyBlogPost
    /// Non-Dated: tag:example.com,2000:info,About
    /// 
    /// Dated: http://example.com/blog/2008/10/10/MyBlogPost.xhtml
    /// Non-Dated: http://example.com/info/About.xhtml
    /// </summary>
    /// <value>True for dated, false otherwise.</value>
    public bool Dated
    {
      get { return GetBooleanPropertyWithDefault(Atom.SvcNs + "dated", true); }
      set { SetBooleanProperty(Atom.SvcNs + "dated", value); }
    }

    /// <summary>
    /// Gets or sets whether any type of annotation is enabled.
    /// Affects trackbacks, pingbacks, comments, etc.
    /// </summary>
    /// <value>True to turn on, false to turn off.</value>
    public bool AnnotationsOn
    {
      get { return GetBooleanPropertyWithDefault(Atom.SvcNs + "annotationsOn", true); }
      set { SetBooleanProperty(Atom.SvcNs + "annotationsOn", value); }
    }

    /// <summary>
    /// Gets or sets whether extended entries are enabled.
    /// </summary>
    /// <value>True to turn on, false to turn off.</value>
    public bool ExtendedEntriesOn
    {
      get { return GetBooleanPropertyWithDefault(Atom.SvcNs + "extendedEntriesOn", true); }
      set { SetYesNoBooleanProperty(Atom.SvcNs + "extendedEntriesOn", value); }
    }

    /// <summary>
    /// Gets or sets whether syndication feeds are available for this collection.
    /// </summary>
    /// <value>True to turn on, false to turn off.</value>
    public bool SyndicationOn
    {
      get { return GetBooleanPropertyWithDefault(Atom.SvcNs + "syndicationOn", true); }
      set { SetYesNoBooleanProperty(Atom.SvcNs + "syndicationOn", value); }
    }

    /// <summary>
    /// Gets or sets the route name used for showing the default view for a collection.
    /// </summary>
    /// <value>Route Name</value>
    public string DefaultView
    {
      get { return GetProperty<string>(Atom.SvcNs + "defaultView"); }
      set { SetProperty<string>(Atom.SvcNs + "defaultView", value); }
    }

    /// <summary>
    /// Gets or sets the route name used for showing the default view for an entry in this collection.
    /// </summary>
    /// <value>Route Name</value>
    public string DefaultEntryView
    {
      get { return GetProperty<string>(Atom.SvcNs + "defaultEntryView"); }
      set { SetProperty<string>(Atom.SvcNs + "defaultEntryView", value); }
    }
    
    ///// <summary>
    ///// Gets or sets the entry id that is the default entry for this collection.
    ///// </summary>
    ///// <value>Entry Id in tag format</value>
    //public Id DefaultEntryId
    //{
    //  get { return GetUriProperty(Atom.SvcNs + "defaultEntryId"); }
    //  set { SetUriProperty(Atom.SvcNs + "defaultEntryId", value); }
    //}

    /// <summary>
    /// Gets or sets whether this collection is visible.
    /// </summary>
    public bool Visible
    {
      get { return GetBooleanPropertyWithDefault(Atom.SvcNs + "visible", true); }
      set { SetBooleanProperty(Atom.SvcNs + "visible", value); }
    }

    /// <summary>
    /// Gets or sets the mime types that can be posted to the collection.
    /// </summary>
    public IEnumerable<AppAccept> Accepts
    {
      get { return GetXmlValues<AppAccept>(Atom.AppNs + "accept"); }
      set { SetXmlValues<AppAccept>(Atom.AppNs + "accept", value); }
    }

    /// <summary>
    /// Gets or sets the categories for the collection.
    /// </summary>
    public IEnumerable<AppCategories> Categories
    {
      get { return GetXmlValues<AppCategories>(Atom.AppNs + "categories"); }
      set { SetXmlValues<AppCategories>(Atom.AppNs + "categories", value); }
    }

    public IEnumerable<AtomCategory> AllCategories
    {
      get { return Categories.SelectMany(c => c.Categories); }
    }

    /// <summary>
    /// Gets or sets the id in the tag URI scheme. This is an extension. The value will be
    /// copied to the atom feed.  The date has no meaning other than that specified by RFC 4151.
    /// 
    /// tag:example.com,2000:name
    /// </summary>
    /// <seealso cref="http://www.faqs.org/rfcs/rfc4151.html"/>
    /// <value>The tag.</value>
    [Required]
    public Id Id
    {
      get { return GetUri(Atom.AtomNs + "id"); }
      set { SetUri(Atom.AtomNs + "id", value); }
    }

    /// <summary>
    /// Gets or sets the subtitle.  This is an extension. The value will be
    /// copied to the atom feed.
    /// </summary>
    /// <value>The subtitle.</value>
    public AtomText Subtitle
    {
      get { return GetXmlValue<AtomText>(Atom.AtomNs + "subtitle"); }
      set { SetXmlValue<AtomText>(Atom.AtomNs + "subtitle", value); }
    }

    /// <summary>
    /// Gets or sets the icon link location. This is an extension. The value will be
    /// copied to the atom feed.
    /// </summary>
    /// <value>The icon.</value>
    public Uri Icon
    {
      get { return GetUri(Atom.AtomNs + "icon"); }
      set { SetUri(Atom.AtomNs + "icon", value); }
    }

    /// <summary>
    /// Gets or sets the logo link location. This is an extension. The value will be
    /// copied to the atom feed.
    /// </summary>
    /// <value>The logo.</value>
    public Uri Logo
    {
      get { return GetUri(Atom.AtomNs + "logo"); }
      set { SetUri(Atom.AtomNs + "logo", value); }
    }

    /// <summary>
    /// Gets or sets the rights. This is an extension. The value will be
    /// copied to the atom feed.
    /// </summary>
    /// <value>The rights.</value>
    public AtomText Rights
    {
      get { return GetXmlValue<AtomText>(Atom.AtomNs + "rights"); }
      set { SetXmlValue<AtomText>(Atom.AtomNs + "rights", value); }
    }

    /// <summary>
    /// Gets or sets the author ids. This is an extension. The authors 
    /// can modify a collection according to the role matrix.
    /// </summary>
    /// <see cref="RoleMatrix"/>
    /// <value>The authors.</value>
    public IEnumerable<string> Authors
    {
      get { return GetValues<string>(Atom.SvcNs + "author"); }
      set { SetValues<string>(Atom.SvcNs + "author", value); }
    }

    /// <summary>
    /// Gets or sets the contributor ids. This is an extension. The value will be
    /// copied to the atom feed.  The contributors can update entries in a collection
    /// according to the role matrix.
    /// </summary>
    /// <see cref="RoleMatrix"/>
    /// <value>The contributors.</value>
    public IEnumerable<string> Contributors
    {
      get { return GetValues<string>(Atom.SvcNs + "contributor"); }
      set { SetValues<string>(Atom.SvcNs + "contributor", value); }
    }


    /// <summary>
    /// Gets or sets the maximum number of entries per feed document.
    /// The default value is 10.
    /// </summary>
    public int PageSize
    {
      get { return (int?)Xml.Attribute(Atom.SvcNs + "pageSize") ?? 10; }
      set { Xml.Attributes(Atom.SvcNs + "pageSize").Remove(); Xml.Add(new XAttribute(Atom.SvcNs + "pageSize", value)); }
    }

    /// <summary>
    /// Gets all authors and contributors for the collection.
    /// </summary>
    public IEnumerable<string> People
    {
      get { return Authors.Union(Contributors); }
    }

    public Scope Scope
    {
      get { return new Scope() { Workspace = Id.Workspace, Collection = Id.Collection }; }
    }

    /// <summary>
    /// Gets or sets the theme for this collection.
    /// This value will override the value set on the workspace.
    /// </summary>
    /// <value>The name.</value>
    public string Theme
    {
      get { return GetProperty<string>(Atom.SvcNs + "theme"); }
      set { SetProperty<string>(Atom.SvcNs + "theme", value); }
    }

    /// <summary>
    /// Determines if the collection will accept the resource based on the given mime-type.
    /// </summary>
    /// <param name="type">The mime type of the entry or media.</param>
    /// <returns>True if the mime type is acceptable.</returns>
    public bool CanAccept(string type)
    {
      //A value of "application/atom+xml;type=entry" MAY appear in any app:accept list of media ranges 
      //and indicates that Atom Entry Documents can be POSTed to the Collection. 
      //If no app:accept element is present, clients SHOULD treat this as equivalent to an app:accept
      //element with the content "application/atom+xml;type=entry".
      //An empty accept means the collection is readonly

      //TODO: support */* and image/* notatations.
      if (Accepts.Count() == 1 && string.IsNullOrEmpty(Accepts.First().Value)) return false;
      else if (Accepts.Count() == 0) return type == Atom.ContentTypeEntry;
      return Accepts.Select(a => a.Value).Contains(type);
    }

    public bool AcceptsEntries
    {
      get { return CanAccept(Atom.ContentTypeEntry); }
    }

    public bool AcceptsMedia
    {
      get { return !OnlyAcceptsEntries && Accepts.Count() > 0 && !string.IsNullOrEmpty(Accepts.First().Value); }
    }

    public bool OnlyAcceptsEntries
    {
      get
      {
        //TODO: support */* and image/* notatations.
        if (Accepts.Select(a => a.Value).Contains("*/*")) return false;
        if (Accepts.Count() == 0) return true;
        if (Accepts.Count() == 1 && Accepts.First().Value == Atom.ContentTypeEntry) return true;
        return false;
      }
    }

    public bool OnlyAcceptsMedia
    {
      get { return !AcceptsEntries && AcceptsMedia; }
    }

    public IEnumerable<Uri> CategorySchemes
    {
      get { return Categories.Select(c => c.Scheme).Where(s => s != null); }
    }

    //public bool OnlyAcceptsMedia()
    //{
    //  //TODO: support */* and image/* notatations.
    //  if (Accepts.Count() == 0) return false;
    //  if (!Accepts.Contains(Atom.ContentTypeEntry)) return true;
    //  return false;
    //}
  }
}
