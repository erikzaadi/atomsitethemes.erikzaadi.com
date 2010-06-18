/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using System.Web.Mvc;
    using AtomSite.Domain;
  public partial class AdminController : BaseController
  {
    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    public ViewResult Tools(string workspace, string collection)
    {
      var m = new AdminModel();
      //m.Notifications.Add("Already have a blog?", "Help build an import tool to convert other blog formats into Atom format.");

      if (TempData["error"] != null) m.Errors.Add((string)TempData["error"]);
      if (TempData["notify"] != null) m.Notifications.Add(((KeyValuePair<string, string>)TempData["notify"]).Key, ((KeyValuePair<string, string>)TempData["notify"]).Value);
      return View("AdminTools", "Admin", m);
    }
  }
}
