/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Collections.Generic;
  using System.Xml.Linq;
  /// <summary>
  /// Describes information about the feed the entry belongs to.
  /// TODO: make this class based on xml
  /// </summary>
  public class AtomSource : XmlBase
  {
    #region constructors

    public AtomSource() : this(new XElement(Atom.AtomNs + "source")) { }
    public AtomSource(XElement xml) : base(xml) { }

    #endregion

    #region public properties

    public IEnumerable<AtomPerson> Authors { get; set; }
    public IEnumerable<AtomCategory> Categories { get; set; }
    public IEnumerable<AtomPerson> Contributors { get; set; }
    public AtomGenerator Generator { get; set; }
    public Uri Icon { get; set; }
    public Uri Id { get; set; }
    public IEnumerable<AtomLink> Links { get; set; }
    public Uri Logo { get; set; }
    public AtomText Rights { get; set; }
    public AtomText Subtitle { get; set; }
    public AtomText Title { get; set; }
    public DateTimeOffset Updated { get; set; }
    public IEnumerable<XElement> Extensions { get; set; }

    #endregion
  }
}
