/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.TwitterPlugin
{
  using System;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using AtomSite.WebCore;

  public class TwitterController : AtomSite.WebCore.BaseController
  {
    protected readonly TwitterService TwitterService;
    protected readonly IAppServiceRepository AppServiceRepository;

    public TwitterController(TwitterService twitterService, IAppServiceRepository svcRepo)
    {
      TwitterService = twitterService;
      AppServiceRepository = svcRepo;
    }

    public ActionResult Widget(Include include)
    {
      TwitterTimeline timeline = new TwitterTimeline();
      var i = new TwitterInclude(include);
      try
      {
        timeline = TwitterService.GetTimeline(i.Username, i.Count);
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
      }
      return PartialView("TwitterWidget", timeline);
    }

    [ScopeAuthorize(Action = AuthAction.UpdateServiceDoc, Roles = AuthRoles.AuthorOrAdmin)]
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Config(TwitterConfigModel m)
    {
      if (m.IncludePath != null)
      {
        var include = AppService.GetInclude<TwitterInclude>(m.IncludePath);
        m.Username = include.Username;
        m.Count = include.Count;
      }
      return PartialView("TwitterConfig", m);
    }

    [ScopeAuthorize(Action = AuthAction.UpdateServiceDoc, Roles = AuthRoles.AuthorOrAdmin)]
    [AcceptVerbs(HttpVerbs.Post)]
    [ActionName("Config")]
    public ActionResult PostConfig(TwitterConfigModel m)
    {
      if (string.IsNullOrEmpty(m.Username) || m.Username.Trim().Length == 0)
        ModelState.AddModelError("username", "Please supply a Twitter user to show the public feed.");

      if (!ModelState.IsValidField("count")) ModelState.AddModelError("count", "Please enter a valid number for the count.");

      if (ModelState.IsValid)
      {
        var appSvc = AppServiceRepository.GetService();
        var include = appSvc.GetInclude<TwitterInclude>(m.IncludePath);
        include.Username = m.Username;
        include.Count = m.Count.Value;
        AppServiceRepository.UpdateService(appSvc);
        return Json(new { success = true, includePath = m.IncludePath });
      }
      return PartialView("TwitterConfig", m);
    }
  }
}