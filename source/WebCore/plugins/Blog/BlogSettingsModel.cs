/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Domain;
  using System.Web.Mvc;

  [Bind(Include = "BloggingOn,TrackbacksOn")]
  public class BlogSettingsModel : AdminModel
  {
    public bool? BloggingOn { get; set; }
    public bool? TrackbacksOn { get; set; }
  }
}
