/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using AtomSite.Domain;

  public class BlogAppCollection : AppCollection
  {
    public BlogAppCollection(AppCollection coll) : base(coll.Xml) { }

    /// <summary>
    /// Gets or sets whether blogging is enabled for this collection.
    /// </summary>
    /// <value>True to turn on, false to turn off.</value>
    public bool BloggingOn
    {
      get { return GetBooleanPropertyWithDefault(Atom.SvcNs + "bloggingOn", false); }
      set { SetBooleanProperty(Atom.SvcNs + "bloggingOn", value); }
    }

    /// <summary>
    /// Gets or sets whether trackbacks are sent and recieved on entries
    /// within this collection.
    /// </summary>
    /// <value>True to turn on, false to turn off.</value>
    public bool TrackbacksOn
    {
      get { return GetBooleanPropertyWithDefault(Atom.SvcNs + "trackbacksOn", true); }
      set { SetBooleanProperty(Atom.SvcNs + "trackbacksOn", value); }
    }
  }
}
