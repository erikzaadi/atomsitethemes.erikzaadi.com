/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Linq;
  using System.Xml.Linq;

  /// <summary>
  /// Contains or links to the complete content of the entry. Content must be provided if there is no alternate link, and should be provided if there is no summary.
  /// Taken verbatim from http://www.atomenabled.org/developers/syndication/.
  /// </summary>
  public class AtomContent : XmlBase
  {
    public AtomContent() : this(new XElement(Atom.AtomNs + "content")) { }
    public AtomContent(XElement xml) : base(xml) { }

    /// <summary>
    /// Gets or sets the type, can be "html", "text", "xhtml" or other mime-type.
    /// </summary>
    /// <value>The type.</value>
    public string Type
    {
      get { return GetProperty<string>("type"); }
      set { SetProperty<string>("type", value); }
    }

    /// <summary>
    /// Gets or sets the SRC.
    /// </summary>
    /// <value>The SRC.</value>
    public Uri Src
    {
      get { return GetUriProperty("src"); }
      set { SetUriProperty("src", value); }
    }

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    /// <value>The content.</value>
    public string Text
    {
      get
      {
        //TODO: other types of xml?
        if (Type == "xhtml")
        {
          return Xml.Element(Atom.XhtmlNs + "div").ToString();
        }
        return Xml.Value;
      }
      set
      {
        //TODO: unit test
        Xml.Elements().Remove(); Xml.Value = string.Empty;
        if (value == null) return;
        if (Type == "xhtml")
        {
          //TODO: verify contains (or add) root "div" node in xhtml namespace
          Xml.Add(XElement.Parse(value));
        }
        else Xml.SetValue(value);
      }
    }

    public bool NeedBase(Uri currentUri)
    {
      if (Base == null) return false;
      if (Type != "xhtml") return false;
      return (Base != currentUri);
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that is ready for insertion into a web page. It
    /// does not include the "&lt;div>" tag if the content is xhtml.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      if (Type == "xhtml")
      {
        string innerXml = string.Empty;
        Xml.Element(Atom.XhtmlNs + "div").Nodes().ToList().ForEach(e => innerXml += e.ToString());

        //TODO: add base to links when needed, see NeedBase
        return innerXml.Replace(" xmlns=\"http://www.w3.org/1999/xhtml\"", string.Empty);
      }
      return Text;
    }

    public AtomText ToText()
    {
      return new AtomText { Text = this.ToString() };
    }

    public bool IsExtended
    {
      get
      {
        return Type == "xhtml" ? 
        ExtendedMoreComment != null :
        Text.Contains("<!--more-->");
      }
    }

    protected XComment ExtendedMoreComment
    {
      //when xhtml, verify comment is direct child of xhtml div tag
      get
      {
        return Type == "xhtml" ? 
          (XComment)Xml.Element(Atom.XhtmlNs + "div").Nodes()
            .Where(n => n is XComment && ((XComment)n).Value == "more")
            .FirstOrDefault() : null;
      }
    }

    public AtomContent BeforeSplit
    {
      get
      {
        if (!IsExtended) return this;
        if (Type == "xhtml")
        {
          return new AtomContent(new XElement(Xml))
          {
            Text = new XElement(Atom.XhtmlNs + "div",
              Xml.Element(Atom.XhtmlNs + "div").Nodes().Where(e => e.IsBefore(ExtendedMoreComment))).ToString()
          };
        }
        else
        {
          return new AtomContent(new XElement(Xml))
          {
            Text = Text.Substring(0, Text.IndexOf("<!--more-->"))
          };
        }
      }
    }

    public AtomContent AfterSplit
    {
      get
      {
        if (!IsExtended) return this;
        if (Type == "xhtml")
        {
          return new AtomContent(new XElement(Xml))
          {
            Text = new XElement(Atom.XhtmlNs + "div",
              Xml.Element(Atom.XhtmlNs + "div").Nodes().Where(e => e.IsAfter(ExtendedMoreComment))).ToString()
          };
        }
        else
        {
          return new AtomContent(new XElement(Xml))
          {
            Text = Text.Substring(Text.IndexOf("<!--more-->") + "<!--more-->".Length)
          };
        }
      }
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
        AtomContent other = obj as AtomContent;
        if ((object)other != null)
        {
          return other.Type == Type & other.Src == Src
						& other.Text == Text & other.Lang == Lang;
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
      else if (this.Src != null)
        return this.Src.GetHashCode();
      else
        return base.GetHashCode();
    }
  }
}
