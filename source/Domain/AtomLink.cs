/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Linq;
  using System.Xml.Linq;
using System.Collections;
  using System.Collections.Generic;

  public static class AtomLinkExtensions
  {
    public static void Merge(this IList<AtomLink> links, AtomLink link)
    {
      //remove old link
      var old = links.Where(l => l.Rel == link.Rel && l.Type == link.Type).ToList();
      for (int i = 0; i < old.Count(); i++) links.Remove(old[i]);

      //add new link
      links.Add(link);
    }
    public static Uri GetLinkUri(this IEnumerable<AtomLink> links, string relation)
    {
      var link = links.Where(l => l.Rel == relation).FirstOrDefault();
      if (link != null) return link.Href;
      return null;
    }
  }


  /// <summary>
	/// Identifies a related Web resource.
	/// Taken verbatim from http://www.atomenabled.org/developers/syndication/.
	/// </summary>
	public class AtomLink : XmlBase
	{
		public AtomLink() : this(new XElement(Atom.AtomNs + "link")) { }
		public AtomLink(XElement xml) : base(xml) { }

		/// <summary>
		/// Gets or sets the href.
		/// </summary>
		/// <value>The href.</value>
		public Uri Href
		{
			get { return GetUriProperty("href"); }
			set { SetUriProperty("href", value); }
		}

		/// <summary>
		/// Gets or sets the relationship.
		/// </summary>
		/// <value>The relationship.</value>
		public string Rel
		{
			get { return GetProperty<string>("rel"); }
			set { SetProperty<string>("rel", value); }
		}

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public string Type
		{
			get { return GetProperty<string>("type"); }
			set { SetProperty<string>("type", value); }
		}

		/// <summary>
		/// Gets or sets the link language.
		/// </summary>
		/// <value>The link language.</value>
		public string HrefLang
		{
			get { return GetProperty<string>("hreflang"); }
			set { SetProperty<string>("hreflang", value); }
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get { return GetProperty<string>("title"); }
			set { SetProperty<string>("title", value); }
		}

		/// <summary>
		/// Gets or sets the length.
		/// </summary>
		/// <value>The length.</value>
		public string Length
		{
			get { return GetProperty<string>("length"); }
			set { SetProperty<string>("length", value); }
		}

		/// <summary>
		/// Gets or sets the number of annotations.  This is part of the Atom
		/// Threading Extensions.
		/// </summary>
		/// <see cref="http://www.ietf.org/rfc/rfc4685.txt"/>
		public int? Count
		{
			get { return (int?)Xml.Attribute(Atom.ThreadNs + "count"); }
			set { Xml.Attributes(Atom.ThreadNs + "count").Remove(); if (value.HasValue) Xml.Add(new XAttribute(Atom.ThreadNs + "count", value)); }
		}

		/// <summary>
		/// Gets or sets the last time an annotation was made.  This is part of the Atom
		/// Threading Extensions.
		/// </summary>
		/// <see cref="http://www.ietf.org/rfc/rfc4685.txt"/>
        public DateTimeOffset? Updated
        {
            get { return (DateTimeOffset?)Xml.Attribute(Atom.ThreadNs + "updated"); }
          set { Xml.Attributes(Atom.ThreadNs + "updated").Remove(); if (value.HasValue) Xml.Add(new XAttribute(Atom.ThreadNs + "updated", value)); }
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
				AtomLink other = obj as AtomLink;
				if ((object)other != null)
				{
					return other.Href == Href & other.Type == Type & other.HrefLang == HrefLang & other.Title == Title &
						other.Length == Length & other.Count == Count;// &
						//(other.Updated ?? Updated) == null ? true : other.Updated.Value.ToUniversalTime().Equals(Updated.Value.ToUniversalTime());
				}
			}
			return base.Equals(obj);
		}

    public override int GetHashCode()
    {
      if (this.Href != null && this.Rel != null && this.Title != null && this.Type != null)
        return this.Href.GetHashCode() ^ this.Rel.GetHashCode() ^ this.Title.GetHashCode() ^ this.Type.GetHashCode();
      else if (this.Href != null && this.Rel != null && this.Title != null)
        return this.Href.GetHashCode() ^ this.Rel.GetHashCode() ^ this.Title.GetHashCode();
      else if (this.Href != null && this.Rel != null)
        return this.Href.GetHashCode() ^ this.Rel.GetHashCode();
      else if (this.Href != null)
        return this.Href.GetHashCode();
      else
        return base.GetHashCode();
    }
	}
}
