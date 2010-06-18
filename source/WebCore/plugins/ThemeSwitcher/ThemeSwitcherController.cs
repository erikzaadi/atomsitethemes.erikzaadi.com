using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using AtomSite.Domain;

namespace AtomSite.WebCore.plugins.ThemeSwitcher
{
    public class ThemeSwitcherController : BaseController
    {
        public ThemeSwitcherController(IThemeSwitcherService themeSwitcherService)
        {
            ThemeSwitcherService = themeSwitcherService;
        }

        protected IThemeSwitcherService ThemeSwitcherService { get; private set; }
        public ActionResult Download(string ThemeName)
        {
            try
            {
                return File(ThemeSwitcherService.Download(ThemeName), "application/octet-stream", string.Format("{0}.zip", ThemeName));
            }
            catch
            {
                throw;
            }
        }

        public ActionResult ChangeTheme(string ThemeName, string PageTemplate, string PageWidth, string Customizations, bool? HideSwitcher)
        {
            if (HideSwitcher.HasValue && HideSwitcher.Value)
                Session["HideSwitcher"] = true;
            try
            {
                ThemeSwitcherService.ChangeTheme(ThemeName, PageTemplate, PageWidth, Customizations, HttpContext);
            }
            catch (Exception err)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = err.Message });

                TempData["ThemeSwitcherError"] = err.Message;

            }
            if (Request.IsAjaxRequest())
                return Json(new { success = true });

            return Redirect(Url.Content("~/content.xhtml"));
        }

        public ActionResult Widget(Include include)
        {
            if (Session["HideSwitcher"] != null)
                return new EmptyResult();
            ThemeSwitcherViewModel model = ThemeSwitcherService.GetModel(HttpContext, this.Scope);

            if (TempData.ContainsKey("ThemeSwitcherError"))
            {
                model.Message = TempData["ThemeSwitcherError"].ToString();
                model.IsMessageError = true;
            }

            return PartialView("ThemeSwitcherWidget", model);
        }

        public ActionResult ThemeWidget(Include Include)
        {
            var ThemeInclude = new ThemeInclude(Include);

            var theme = ThemeSwitcherService.GetThemeModel(ThemeInclude.ThemeName);

            return PartialView("ThemeWidget", theme);
        }
    }
}