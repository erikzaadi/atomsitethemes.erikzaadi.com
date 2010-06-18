/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.Rater
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using AtomSite.Domain;

  public class RaterAppCollection : AppCollection
  {
    public RaterAppCollection(AppCollection coll) : base(coll.Xml) { }

    /// <summary>
    /// Gets or sets whether ratings are allowed on entries within this collection.
    /// </summary>
    /// <value>True to turn on, false to turn off.</value>
    public bool RatingsOn
    {
      get { return GetBooleanPropertyWithDefault(Atom.SvcNs + "ratingsOn", true); }
      set { SetBooleanProperty(Atom.SvcNs + "ratingsOn", value); }
    }
  }
}
