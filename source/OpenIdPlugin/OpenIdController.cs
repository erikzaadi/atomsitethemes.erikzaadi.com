/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.OpenIdPlugin
{
  using System.Web.Mvc;
  using System.Web.UI;
  using AtomSite.WebCore;

  [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = 30 * MIN, VaryByParam = "*")]
  public class OpenIdController : BaseController
  {
    [AcceptVerbs("GET")]
    public ActionResult OpenIdLogin(string returnUrl)
    {
      if (this.Request.IsAjaxRequest())
      {
        return PartialView("OpenIdLoginWidget");
      }
      return RedirectToAction("Login", "Account");
    }
  }
}
