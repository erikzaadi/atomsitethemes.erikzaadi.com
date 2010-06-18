/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Security.Principal;
  using System.Web.Mvc;
  using System.Web.Security;
  using System.Web.UI;
  using AtomSite.Domain;
  using AtomSite.Repository;

  [OutputCache(Location = OutputCacheLocation.None)]
  public class AccountController : BaseController
  {
    protected IUserRepository UserRepository { get; private set; }
    public AccountController(IUserRepository userRepo)
    {
      UserRepository = userRepo;
    }

    [AcceptVerbs("GET")]
    public ActionResult Login(string returnUrl, string error)
    {
      if (User.Identity.IsAuthenticated) return Return(returnUrl);
      if (error != null) ViewData["errors"] = new List<string>() { error };
      var m = new PageModel();
      //use default workspace
      if (Workspace == null) m.Workspace = AppService.GetWorkspace();
      return View("AccountLogin", m);
    }

    [AcceptVerbs("POST")]
    public ActionResult Login(string username, string password, bool? rememberMe, string returnUrl)
    {
      if (User.Identity.IsAuthenticated) return Return(returnUrl);

      // Basic parameter validation
      List<string> errors = new List<string>();

      if (string.IsNullOrEmpty(username))
      {
        errors.Add("You must specify a username.");
      }

      if (errors.Count == 0)
      {
        User user = UserRepository.GetUser(username);
        if (user != null && user.CheckPassword(password))
        {
          FormsAuthentication.SetAuthCookie(username, rememberMe ?? false);
          return Return(returnUrl);
        }
        errors.Add("The username or password provided is incorrect.");
      }

      // If we got this far, something failed, redisplay form
      ViewData["errors"] = errors;
      ViewData["username"] = username;
      
      var m = new PageModel();
      if (Workspace == null) m.Workspace = AppService.GetWorkspace();
      return View("AccountLogin", m);
    }

    public ActionResult Logout()
    {
      FormsAuthentication.SignOut();
      return new RedirectResult(Request.UrlReferrer.ToString());
    }

    public ActionResult Return(string returnUrl)
    {
      if (!string.IsNullOrEmpty(returnUrl)) return new RedirectResult(returnUrl);
      else return new RedirectResult(FormsAuthentication.DefaultUrl);
    }

    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      if (filterContext.HttpContext.User.Identity is WindowsIdentity)
      {
        throw new InvalidOperationException("Windows authentication is not supported.");
      }
    }
  }
}
