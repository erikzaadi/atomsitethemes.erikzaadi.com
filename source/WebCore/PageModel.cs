/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
    using AtomSite.Domain;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using StructureMap;
    using System.Web.Mvc;

    public class PageModel : BaseModel
    {
        public PageModel()
            : base()
        {
            PageWidth = "doc1";
            PageTemplate = "yui-t5";
            PageName = string.Empty;
            Assets = new List<Asset>();
            TailScript = string.Empty;
        }

        public string PageName { get; protected set; }
        public string ParentName { get; protected set; }
        public ICollection<Asset> Assets { get; protected set; }
        public string PageWidth { get; protected set; }
        public string PageTemplate { get; protected set; }

        public string TailScript { get; protected set; }

        public void AddToTailScript(string script)
        {
            //if (TailScript == null) TailScript = string.Empty;
            TailScript += Environment.NewLine + script;
        }

        /// <summary>
        /// Gets the name of the aspx page (without .aspx extension) when using Web Forms view engine
        /// to update model with dependencies and other configuration. Also sets the asset groups this
        /// page requires.
        /// 
        /// TODO: this should be abstracted into view engine somehow
        /// </summary>
        /// <param name="page"></param>
        /// <param name="assetGroups"></param>
        /// <returns></returns>
        public void UpdatePageModel(System.Web.UI.Page page, string parentName)
        {
            string virpath = page.AppRelativeVirtualPath;
            int start = page.AppRelativeVirtualPath.LastIndexOf('/') == -1 ? 0 :
              page.AppRelativeVirtualPath.LastIndexOf('/') + 1;
            int length = page.AppRelativeVirtualPath.LastIndexOf(".aspx") - start;
            string pageName = page.AppRelativeVirtualPath.Substring(start, length);

            IContainer container = (IContainer)page.Application["Container"];
            PageName = pageName;
            ParentName = parentName;
            OnUpdatePageModel(container);
            UpdateThemeFromRequest(page);
        }

        private void UpdateThemeFromRequest(System.Web.UI.Page page)
        {
            if (page.Session == null)
                return;
            if (page.Session["PageWidth"] != null)
                PageWidth = page.Session["PageWidth"].ToString();

            if (page.Session["PageTemplate"] != null)
                PageTemplate = page.Session["PageTemplate"].ToString();
        }

        protected virtual void OnUpdatePageModel(IContainer container)
        {
            //global
            IAssetService assetSvc = container.GetInstance<IAssetService>();
            assetSvc.UpdatePageModel(this);

            //page
            IPage page = container.TryGetInstance<IPage>(PageName);
            if (page != null)
            {
                if (ParentName == null) ParentName = page.Parent;
                IPage parent = container.TryGetInstance<IPage>(ParentName);
                if (parent != null) parent.UpdatePageModel(this);
                page.UpdatePageModel(this);
            }

            //using the page name, find width and template using scope fallback rules
            var pages = Service.GetServicePages(Scope, PageName, ParentName);

            var pageWidth = pages.Where(p => p.Width != null).LastOrDefault();
            if (pageWidth != null) PageWidth = pageWidth.Width;
            var pageTemplate = pages.Where(p => p.Template != null).LastOrDefault();
            if (pageTemplate != null) PageTemplate = pageTemplate.Template;

            var includes = Service.GetIncludes(Scope, PageName, ParentName);

            //let each widget update page model
            foreach (Include include in includes)
            {
                var w = container.TryGetInstance<IWidget>(include.Name);
                if (w != null) w.UpdatePageModel(this, include);
            }
        }


        ///// <summary>
        ///// Gets the name of the aspx page (without .aspx extension) when using Web Forms view engine
        ///// to update model with dependencies and other configuration.
        ///// </summary>
        ///// <param name="page"></param>
        ///// <returns></returns>
        //public virtual void UpdatePageModel(System.Web.UI.Page page)
        //{
        //    OnUpdatePageModel(page, new string[] { Asset.DefaultGroup });
        //}  
    }

    public class ErrorModel : PageModel
    {
        public ErrorModel() { }
        public ErrorModel(HandleErrorInfo error)
        {
            HandleErrorInfo = error;
        }
        public HandleErrorInfo HandleErrorInfo { get; set; }
    }
}
