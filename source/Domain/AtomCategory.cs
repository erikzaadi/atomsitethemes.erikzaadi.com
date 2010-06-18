/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Xml.Linq;

  /// <summary>
  /// Specifies a category that the feed belongs to. A feed may have multiple category elements.
  /// Taken verbatim from http://www.atomenabled.org/developers/syndication/.
  /// </summary>
  public class AtomCategory : XmlBase
  {
    public AtomCategory() : this(new XElement(Atom.AtomNs + "category")) { }
    public AtomCategory(XElement xml) : base(xml) { }

    /// <summary>
    /// Gets or sets the term.
    /// </summary>
    /// <value>The term.</value>
    public string Term
    {
      get { return GetProperty<string>("term"); }
      set { SetProperty<string>("term", value); }
    }

    /// <summary>
    /// Gets or sets the scheme.
    /// </summary>
    /// <value>The scheme.</value>
    public Uri Scheme
    {
      get { return GetUriProperty("scheme"); }
      set { SetUriProperty("scheme", value); }
    }

    /// <summary>
    /// Gets or sets the label.
    /// </summary>
    /// <value>The label.</value>
    public string Label
    {
      get { return GetProperty<string>("label"); }
      set { SetProperty<string>("label", value); }
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      return Label ?? Term;
    }

    /// <summary>
    /// Returns true when scheme and term matches.  Ignores label.
    /// </summary>
    /// <param name="obj">Object you wish to compare to.</param>
    /// <returns>true if this object is equal to <c>obj</c>.</returns>
    public override bool Equals(object obj)
    {
      if (obj != null && obj.GetType().Equals(this.GetType()))
      {
        AtomCategory other = obj as AtomCategory;
        if ((object)other != null)
        {
          return other.Term == Term & other.Scheme == Scheme;
        }
      }
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      if (Scheme != null)
        return Term.GetHashCode() ^ Scheme.GetHashCode();
      else
        return Term.GetHashCode();
    }
  }
}
