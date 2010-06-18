/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Xml.Linq;

  /// <summary>
  /// Identifies the software used to generate the feed, for debugging and other purposes. Both the serviceDocUri and version attributes are optional.
  /// Taken verbatim from http://www.atomenabled.org/developers/syndication/.
  /// </summary>
  public class AtomGenerator : XmlBase
  {
    public AtomGenerator() : this(new XElement(Atom.AtomNs + "generator")) { }
    public AtomGenerator(XElement xml) : base(xml) { }

    /// <summary>
    /// Gets or sets the generator URI.
    /// </summary>
    /// <value>The generator URI.</value>
    public Uri Uri
    {
      get { return GetUriProperty("src"); }
      set { SetUriProperty("src", value); }
    }

    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    /// <value>The version.</value>
    public string Version
    {
      get { return GetProperty<string>("version"); }
      set { SetProperty<string>("version", value); }
    }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Text
    {
      get { return Xml.Value; }
      set { Xml.SetValue(value); }
    }
  }
}
