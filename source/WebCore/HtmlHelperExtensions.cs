/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Globalization;
  using System.Linq;
  using System.Web;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using AtomSite.Utils;
  using AtomSite.WebCore.Properties;
  using StructureMap;
  using System.Collections.Generic;
    using System.Xml.Linq;

  public static class HtmlHelperExtensions
  {
    public static string DateTimeAbbreviation(this HtmlHelper helper, DateTimeOffset date)
    {
      return DateTimeAbbreviation(helper, date, (d, tz) =>
          d.ToString("G") + " - " + Settings.Default.TimeZoneDisplay);
    }

    public static void RenderContent<T>(this HtmlHelper<T> helper, AtomContent content) where T : BaseModel
    {
        if (content.Type == "xhtml")
        {
            // split amoung inline includes
            string innerXml = string.Empty;
            foreach (XNode e in content.Xml.Element(Atom.XhtmlNs + "div").Nodes().ToList())
            {
                // if it is an include, render it
                if (e.NodeType == System.Xml.XmlNodeType.Element && ((XElement)e).Name == Include.IncludeXName)
                {
                    RenderIncludes(helper, Enumerable.Repeat(new Include((XElement)e), 1));
                }
                else // otherwise just print content to output
                {
                    //TODO: add base to links when needed, see NeedBase
                    helper.ViewContext.HttpContext.Response.Write(e.ToString().Replace(" xmlns=\"http://www.w3.org/1999/xhtml\"", string.Empty));
                }
            }
        }
        else
        {
            helper.ViewContext.HttpContext.Response.Write(content.ToString());
        }
    }

    public static void RenderSubWidgets<T>(this HtmlHelper<T> helper, string widgetName, string areaName) where T : BaseModel
    {
      BaseController controller = helper.ViewContext.Controller as BaseController;
      if (controller != null)
      {
        var includes =
          //get all the widgets at the default level and the widget level for all scopes
        controller.AppService.Widgets.Where(w => w.Name == widgetName)
        .SelectMany(w => w.Areas).Where(area => area.Name == areaName).SelectMany(area => area.Includes);

        if (controller.Workspace != null)
          includes = includes.Concat(controller.Workspace.Widgets.Where(w => w.Name == widgetName)
            .SelectMany(w => w.Areas).Where(area => area.Name == areaName).SelectMany(area => area.Includes));

        if (controller.Collection != null)
          includes = includes.Concat(controller.Collection.Widgets.Where(w => w.Name == widgetName)
            .SelectMany(w => w.Areas).Where(area => area.Name == areaName).SelectMany(area => area.Includes));

        RenderIncludes(helper, includes);
      }
    }

    static void RenderIncludes<T>(this HtmlHelper<T> helper, IEnumerable<Include> includes) where T : BaseModel
    {
      //do actual widget rendering
      foreach (Include include in includes)
      {
        RenderInclude(helper, include);
      }
    }

    public static void RenderInclude<T>(this HtmlHelper<T> helper, Include include) where T : BaseModel
    {
      if (include == null) return;

      IContainer container = (IContainer)helper.ViewContext.HttpContext.Application["container"];
      IWidget widget = container.TryGetInstance<IWidget>(include.Name);

      if (widget == null) container.GetInstance<ILogService>().Warn(include.Name +
          " include is not found. Either plugin is disabled or uninstalled. Please update configuration.");
      else
      {
        if (widget.IsEnabled(helper.ViewData.Model, include))
          widget.Render(helper.ViewContext, include);
      }
    }

    public static void RenderWidgets<T>(this HtmlHelper<T> helper, string areaName) where T : PageModel
    {
      BaseController controller = helper.ViewContext.Controller as BaseController;
      if (controller != null)
      {
        var includes =
          //get all the includes at the default level and the page level for all scopes
        controller.AppService.Pages.Where(p => p.Name == helper.ViewData.Model.ParentName || p.Name == helper.ViewData.Model.PageName)
          .SelectMany(p => p.Areas).Where(area => area.Name == areaName).SelectMany(area => area.Includes);

        if (controller.Workspace != null)
          includes = includes.Concat(controller.Workspace.Pages.Where(p => p.Name == helper.ViewData.Model.ParentName || p.Name == helper.ViewData.Model.PageName)
            .SelectMany(p => p.Areas).Where(area => area.Name == areaName).SelectMany(area => area.Includes));

        if (controller.Collection != null)
          includes = includes.Concat(controller.Collection.Pages.Where(p => p.Name == helper.ViewData.Model.ParentName || p.Name == helper.ViewData.Model.PageName)
            .SelectMany(p => p.Areas).Where(area => area.Name == areaName).SelectMany(area => area.Includes));

        RenderIncludes(helper, includes);
      }
    }

    public static void RenderPartial<T>(this HtmlHelper<T> helper, string partialViewName, BaseModel model) where T : BaseModel
    {
      BaseController controller = helper.ViewContext.Controller as BaseController;
      if (controller != null)
      {
        model.Service = helper.ViewData.Model.Service;
        model.Workspace = helper.ViewData.Model.Workspace;
        model.Collection = helper.ViewData.Model.Collection;
        model.User = helper.ViewData.Model.User;
        model.EntryId = helper.ViewData.Model.EntryId;
        model.AuthorizeService = helper.ViewData.Model.AuthorizeService;
        //model.Theme = helper.ViewData.Model.Theme;
        System.Web.Mvc.Html.RenderPartialExtensions.RenderPartial(helper, partialViewName, model);
      }
    }

    public static string ScriptSources<T>(this HtmlHelper<T> helper) where T : PageModel
    {
      UrlHelper url = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
      IAssetService assetSvc = AssetService.GetCurrent(helper.ViewContext.RequestContext);
      IEnumerable<string> scripts = assetSvc.GetAssetSources(AssetType.Js, helper.ViewData.Model, url); 
      string html = string.Empty;
      foreach (string script in scripts)
      {
        html += string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>{1}",
          script, Environment.NewLine);
      }
      return html;
    }

    public static string StyleLinks<T>(this HtmlHelper<T> helper) where T : PageModel
    {
      UrlHelper url = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
      IAssetService assetSvc = AssetService.GetCurrent(helper.ViewContext.RequestContext);
      IEnumerable<string> styles = assetSvc.GetAssetSources(AssetType.Css, helper.ViewData.Model, url); 
      string html = string.Empty;
      foreach (string style in styles)
      {
        html += string.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />{1}",
          style, Environment.NewLine);
      }
      return html;
    }

    public static string DateTimeFormat(this HtmlHelper helper, DateTimeOffset date, string formatString)
    {
      TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(Settings.Default.TimeZoneInfoId);
      DateTimeOffset dto = TimeZoneInfo.ConvertTime(date, tz);
      return dto.ToString(formatString);
    }

    public static string DateTimeAbbreviation(this HtmlHelper helper, DateTimeOffset date,
        Func<DateTimeOffset, TimeZoneInfo, string> format)
    {
      TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(Settings.Default.TimeZoneInfoId);
      DateTimeOffset dto = TimeZoneInfo.ConvertTime(date, tz);
      return string.Format("<abbr title=\"{0}\">{1}</abbr>",
          HttpUtility.HtmlAttributeEncode(dto.ToString("f") + " " + tz.DisplayName),
          format(dto, tz));
    }

    public static string DateTimeAgoAbbreviation(this HtmlHelper helper, DateTimeOffset date)
    {
      TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(Settings.Default.TimeZoneInfoId);
      DateTimeOffset app = date.ToUniversalTime().Add(tz.BaseUtcOffset);
      return string.Format("<abbr class=\"timeago\" title=\"{0}Z\">on {1} - {2}</abbr>",
          date.ToUniversalTime().ToString("s"),
          app.ToString("G"),
          Settings.Default.TimeZoneDisplay);
    }

    public static string GravatarImg(this HtmlHelper helper, string email, int size)
    {
      if (email == null) email = string.Empty;
      //TODO: pull alt from config
      UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
      string alt = helper.ViewContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) +
        helper.ViewContext.HttpContext.Request.ApplicationPath + urlHelper.ImageSrc("noav.png").Substring(1);
      string src = "http://gravatar.com/avatar/" + 
                SecurityHelper.HashIt(email.ToLowerInvariant(), "MD5", false)
                + ".jpg?s=" + size + "&amp;d=" + alt;
      return string.Format("<img class=\"avatar\" src=\"{0}\" width=\"{1}\" height=\"{1}\" alt=\"Gravatar\" />",
        src, size);      
    }
  }
}
