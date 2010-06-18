//http://pietschsoft.com/post/2008/08/Custom-Themes-in-ASPNET-MVC-Updated-for-Preview-5.aspx
namespace AtomSite.WebCore
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Collections.Generic;

    public class ThemeViewEngine : WebFormViewEngine
    {

        public ThemeViewEngine()
        {
            base.ViewLocationFormats = new string[] {
                "~/themes/{1}/{0}.aspx",
                "~/themes/{1}/{0}.ascx",
                "~/themes/default/{0}.aspx",
                "~/themes/default/{0}.ascx"
            };

            base.MasterLocationFormats = new string[] {
                "~/themes/{1}/{0}.master",
                "~/themes/default/{0}.master"
            };

            base.PartialViewLocationFormats = new string[] {
                "~/themes/{1}/{0}.aspx",
                "~/themes/{1}/{0}.ascx",
                "~/themes/default/{0}.aspx",
                "~/themes/default/{0}.ascx"
            };
        }


        public static string GetCurrentThemeName(RequestContext ctx)
        {

            IThemeService themeSvc = ThemeService.GetCurrent(ctx);
            IRouteService routing = RouteService.GetCurrent(ctx);

            if (ctx.HttpContext.Session != null && ctx.HttpContext.Session["theme"] != null)
                return ctx.HttpContext.Session["theme"].ToString();

            return themeSvc.GetInheritedThemeName(routing.GetScope());
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {

            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            if (string.IsNullOrEmpty(viewName))
                throw new ArgumentException("Value is required.", "viewName");

            string themeName = GetCurrentThemeName(controllerContext.RequestContext);

            IList<string> searchedLocations = new List<string>();

            string viewPath = this.GetPath(this.ViewLocationFormats, viewName, themeName, searchedLocations);

            //use best master page when not using default theme
            if (string.IsNullOrEmpty(masterName)) masterName = "Site";

            string masterPath = this.GetPath(this.MasterLocationFormats, masterName, themeName, searchedLocations);

            if (!(string.IsNullOrEmpty(viewPath)) && (!(masterPath == string.Empty) || string.IsNullOrEmpty(masterName)))
                return new ViewEngineResult(this.CreateView(controllerContext, viewPath, masterPath), this);

            return new ViewEngineResult(searchedLocations);
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            if (string.IsNullOrEmpty(partialViewName))
                throw new ArgumentException("Value is required.", partialViewName);

            string themeName = GetCurrentThemeName(controllerContext.RequestContext);

            IList<string> searchedLocations = new List<string>();
            string partialPath = GetPath(this.PartialViewLocationFormats, partialViewName, themeName, searchedLocations);

            if (partialPath == null) return new ViewEngineResult(searchedLocations);
            return new ViewEngineResult(this.CreatePartialView(controllerContext, partialPath), this);
        }

        string GetPath(string[] locations, string viewName, string themeName, IList<string> searchedLocations)
        {
            string path = null;
            for (int i = 0; i < locations.Length; i++)
            {
                path = string.Format(CultureInfo.InvariantCulture, locations[i], viewName, themeName);
                if (this.VirtualPathProvider.FileExists(path)) return path;
                searchedLocations.Add(path);
            }
            return null;
        }

    }
}
