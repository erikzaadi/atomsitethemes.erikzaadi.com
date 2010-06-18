/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System.Xml.Linq;
  using AtomSite.Utils.Extensions;
 
  /// <summary>
  /// Contains human-readable text, usually in small quantities. The type attribute determines
  /// how this information is encoded (default="text")
  /// Taken verbatim from http://www.atomenabled.org/developers/syndication/.
  /// </summary>
  public class AtomText : XmlBase
  {
    public AtomText(XName name) : this(new XElement(name)) { }
    public AtomText() : this(new XElement(Atom.AtomNs + "text")) { }
    public AtomText(XElement xml) : base(xml) { }

    /// <summary>
    /// Gets or sets the type, usually either "text" or "html"
    /// </summary>
    /// <value>The type.</value>
    public string Type
    {
      get { return GetProperty<string>("type"); }
      set { SetProperty<string>("type", value); }
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text
    {
      get { return Xml.Value; }
      set { Xml.SetValue(value); }
    }

    /// <summary>
    /// Returns a string that does not contain any markup or line breaks;
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      return Xml.Value.StripHtml().Replace("\n", " ").Replace("\r", "").Trim();
    }

    public string ToStringPreview(int length)
    {
      string preview = ToString();
      if (preview.Length > length)
        return ToString().AbbreviateText(length, "…");
      else return preview;
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
        AtomText other = obj as AtomText;
        if ((object)other != null)
        {
          return other.Text == Text & 
            //allow for default value of "text"
            (other.Type == Type || (Type == null && other.Type == "text") || (other.Type == null && Type == "text")) 
            & other.Lang == Lang;
        }
      }
      return base.Equals(obj);
    }
    public override int GetHashCode()
    {
      if (this.Text != null && this.Type != null)
        return this.Text.GetHashCode() ^ this.Type.GetHashCode();
      else if (this.Text != null)
        return this.Text.GetHashCode();
      else
        return base.GetHashCode();
    }
  }

  public class AtomTitle : AtomText
  {
    public AtomTitle() : base(Atom.AtomNs + "title") { }
  }

  public class AtomSubtitle : AtomText
  {
    public AtomSubtitle() : base(Atom.AtomNs + "subtitle") { }
  }

  public class AtomSummary : AtomText
  {
    public AtomSummary() : base(Atom.AtomNs + "summary") { }
  }

  public class AtomRights : AtomText
  {
    public AtomRights() : base(Atom.AtomNs + "rights") { }
  }
}
