/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;
  using AtomSite.Domain;
  using AtomSite.Plugins.Rater;
  using System.Web.Mvc;

  public class AdminThemeModel : AdminModel
  {
    public IEnumerable<Theme> InstalledThemes { get; set; }
    public Theme Theme { get; set; }
    public string ThemeLocation { get; set; }
    public bool Installed { get { return InstalledThemes.Contains(Theme); } }

    public string ParentThemeName { get; set; }

    public string InheritedThemeName { get; set; }
    public string CurrentThemeName { get; set; }
    public bool Current { get { return CurrentThemeName == Theme.Name; } }
    public bool Inherited { get; set; }

    public bool CanInherit { get { return !Scope.IsEntireSite; } }
    public bool CanDelete { get { return Theme.Name != "default"; } }

    public RaterModel GetRaterModel(string ip, UrlHelper url)
    {
      return new RaterModel()
      {
        PostHref = url.RouteIdUri("RaterRateEntry", Theme.Id),
        EntryId = Theme.Id,
        Rating = Theme.Rating,
        RatingCount = Theme.RatingCount,
        CanRate = false
      };
    }
    public string GetSpecImage(UrlHelper url, string spec)
    {
      var specs = spec.StartsWith("doc") ? Theme.Widths : Theme.Templates;
      if (specs.Contains(spec))
      {
        return string.Format("<img src='{0}' alt='{1} supported' />",
          url.ImageSrc(spec + ".png"), spec);
      }
      return string.Format("<img src='{0}' alt='{1} not supported' />",
        url.ImageSrc(spec + "off.png"), spec);
    }
  }

  //public class InstalledTheme
  //{
  //  public bool Installed { get; set; }
  //  public string Name { get { return Id.EntryPath; } }
  //  public Id Id { get; set; }
  //  public string Version { get; set; }
  //  public float Rating { get; set; }
  //  public int RatingCount { get; set; }
  //  public RaterModel GetRaterModel()
  //  {
  //    return new RaterModel()
  //    {
  //      EntryId = Id,
  //      Rating = Rating,
  //      RatingCount = RatingCount,
  //      CanRate = false
  //    };
  //  }
  //}
}
