/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.Rater
{
  using System;
  using AtomSite.WebCore;

  public class RaterModel : BaseModel
  {
    public Uri PostHref { get; set; }
    public float Rating { get; set; }
    public bool CanRate { get; set; }
    public int RatingCount { get; set; }
  }
}
