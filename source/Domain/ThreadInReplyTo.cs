/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Xml.Linq;

  /// <summary>
  /// Identifies which resource this is an annotation of.  For example, a comment in-reply-to a blog post.
  /// </summary>
  public class ThreadInReplyTo : XmlBase
  {
    public ThreadInReplyTo() : this(new XElement(Atom.ThreadNs + "in-reply-to")) { }
    public ThreadInReplyTo(XElement xml) : base(xml) { }

    /// <summary>
    /// Gets or sets the ref.
    /// </summary>
    /// <value>The href.</value>
    public Uri Ref
    {
      get { return GetUriProperty("ref"); }
      set { SetUriProperty("ref", value); }
    }

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
    /// Gets or sets the source.
    /// </summary>
    /// <value>The source.</value>
    public Uri Source
    {
      get { return GetUriProperty("source"); }
      set { SetUriProperty("source", value); }
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
    /// Returns true if this object is equal to <c>obj</c>.
    /// </summary>
    /// <param name="obj">Object you wish to compare to.</param>
    /// <returns>true if this object is equal to <c>obj</c>.</returns>
    public override bool Equals(object obj)
    {
      if (obj != null && obj.GetType().Equals(this.GetType()))
      {
        ThreadInReplyTo other = obj as ThreadInReplyTo;
        if ((object)other != null)
        {
          return other.Ref == Ref & other.Href == Href & other.Source == Source & other.Type == Type;
        }
      }
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      if (this.Href != null && this.Ref != null && this.Source != null && this.Type != null)
        return this.Href.GetHashCode() ^ this.Ref.GetHashCode() ^ this.Source.GetHashCode() ^ this.Type.GetHashCode();
      else if (this.Href != null && this.Ref != null && this.Source != null)
        return this.Href.GetHashCode() ^ this.Ref.GetHashCode() ^ this.Source.GetHashCode();
      else if (this.Href != null && this.Ref != null)
        return this.Href.GetHashCode() ^ this.Ref.GetHashCode();
      else if (this.Href != null)
        return this.Href.GetHashCode();
      else
        return base.GetHashCode();
    }
  }
}
