/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.Rater
{
  using System;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using AtomSite.WebCore;

  public class RaterController : BaseController
  {
    protected IRaterService RaterService { get; private set; }

    public RaterController(IRaterService raterService)
    {
      this.RaterService = raterService;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult RateEntry(string workspace, string collection, int? year,
      int? month, int? day, string path, float? rating)
    {
      if (!rating.HasValue) throw new Exception("Rate called without posting a rating.");
      float result = this.RaterService.Rate(EntryId,
        rating.Value, (User)User.Identity, Request.UserHostAddress).Rating;
      return new ContentResult()
      {
        Content = result.ToString("0.0"),
        ContentType = "text/plain"
      };
    }

    public ActionResult Rater(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      RaterModel rate = this.RaterService.GetRaterModel(EntryId,
        (User)User.Identity, Request.UserHostAddress);

      return PartialView("RaterWidget", rate);
    }
  }
}
