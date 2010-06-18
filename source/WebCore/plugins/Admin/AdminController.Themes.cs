using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AtomSite.Domain;
using System.Web.Mvc;
using AtomSite.Repository;
using System.IO;

namespace AtomSite.WebCore
{
  public partial class AdminController : BaseController
  {

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    public ViewResult Theme(string workspace, string collection)
    {
      var m = new AdminThemeModel();
      m.CurrentThemeName = ThemeService.GetThemeName(Scope);
      m.InheritedThemeName = ThemeService.GetInheritedThemeName(Scope);
      m.Inherited = ThemeService.GetThemeName(Scope) == null;
      m.Theme = ThemeService.GetTheme(m.InheritedThemeName);
      m.ThemeLocation = ThemeService.GetThemePath(m.InheritedThemeName);
      m.InstalledThemes = ThemeService.GetInstalledThemes().Select(t => ThemeService.GetTheme(t));
      if (TempData["error"] != null) m.Errors.Add((string)TempData["error"]);
      if (TempData["applied"] != null) m.Notifications.Add("Success", "The theme was successfully updated.");
      else if (TempData["deleted"] != null) m.Notifications.Add("Success", "The theme was successfully deleted.");
      //temporary notification
      else if (m.Errors.Count == 0) m.Notifications.Add("So you've mastered CSS,", "then help us improve the theme system by creating a theme.");
      return View("AdminTheme", "Admin", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
      [AcceptVerbs(HttpVerbs.Get)]
    public ViewResult ThemeUpload()
    {
        var m = new AdminModel();
        if (TempData["success"] != null) m.Notifications.Add("Success", 
            string.Format("The theme '{0}' was uploaded and installed successfully.", TempData["success"]));
        return View("AdminThemeUpload", "Admin", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult ThemeUpload(HttpPostedFileBase uptheme)
    {
        AdminModel model = new AdminModel();
        try
        {
            if (uptheme.ContentLength == 0) throw new Exception("Empty file uploaded");

            LogService.Info("Theme uploaded filename={0} contentType={1} contentLength={2}",
              uptheme.FileName, uptheme.ContentType, uptheme.ContentLength);

            string theme = ThemeService.InstallTheme(uptheme.InputStream, Path.GetFileName(uptheme.FileName), uptheme.ContentType);
            TempData["success"] = theme;
            return RedirectToAction("ThemeUpload");
        }
        catch (Exception ex)
        {
            LogService.Error(ex);
            model.Errors.Add(ex.Message);
        }
        return View("AdminThemeUpload", "Admin", model);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    public PartialViewResult ThemeInfo(string workspace, string collection, string theme)
    {
      var m = new AdminThemeModel();
      m.CurrentThemeName = ThemeService.GetThemeName(Scope);
      m.InheritedThemeName = ThemeService.GetInheritedThemeName(Scope);
      if (theme == "(inherit)")
      {
        theme = ThemeService.GetInheritedThemeName(Scope.ToAbove());
        m.Inherited = true;
      }
      m.Theme = ThemeService.GetTheme(theme);
      m.ThemeLocation = ThemeService.GetThemePath(theme);
      m.InstalledThemes = ThemeService.GetInstalledThemes().Select(t => ThemeService.GetTheme(t));
      return PartialView("AdminThemeInfo", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    public ActionResult ChangeTheme(string workspace, string collection, string theme, string submit)
    {
      try
      {
        if (theme == "(inherit)") theme = null;
        if (submit == "Apply Theme")
        {
            this.ThemeService.SetTheme(Scope, theme);
          TempData["applied"] = true;
        }
        else if (submit == "Delete Theme")
        {
            this.ThemeService.DeleteTheme(theme);
            TempData["deleted"] = true;
        }
        else throw new NotImplementedException();
        return RedirectToAction("Theme", new { workspace = workspace, collection = collection });
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        TempData["error"] = ex.Message;
        return RedirectToAction("Theme", new { workspace = workspace, collection = collection });
      }
    }
  }
}
