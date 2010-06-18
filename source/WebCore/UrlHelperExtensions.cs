/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Web.Hosting;
  using System.Web.Mvc;
  using System.Web.Routing;
  using AtomSite.Domain;
  using AtomSite.Utils;
  using AtomSite.WebCore.Properties;

  public static class UrlHelperExtensions
  {
    /// <summary>
    /// Route's to collection, entry, or media given the id.
    /// </summary>
    /// <param name="helper"></param>
    /// <param name="routeName">Name of the route.</param>
    /// <param name="id">Id of the resource.</param>
    /// <returns></returns>
    public static Uri RouteIdUri(this UrlHelper helper, string routeName, Id id)
    {
      return helper.GetRouteService(routeName).RouteUrl(routeName, id);
    }
    public static string RouteIdUrl(this UrlHelper helper, string routeName, Id id)
    {
      return RouteIdUri(helper, routeName, id).ToString();
    }

    /// <summary>
    /// Route's to collection, entry, or media given the id.
    /// </summary>
    /// <param name="helper"></param>
    /// <param name="routeName">Name of the route.</param>
    /// <param name="id">Id of the resource.</param>
    /// <returns></returns>
    public static Uri RouteIdUri(this UrlHelper helper, string routeName, Id id, AbsoluteMode abMode)
    {
      return helper.GetRouteService(routeName).RouteUrl(routeName, id, abMode);
    }
    public static string RouteIdUrl(this UrlHelper helper, string routeName, Id id, AbsoluteMode abMode)
    {
      return RouteIdUri(helper, routeName, id, abMode).ToString();
    }

    /// <summary>
    /// Route's to collection, entry, or media given the id.
    /// </summary>
    /// <param name="helper"></param>
    /// <param name="routeName">Name of the route.</param>
    /// <param name="id">Id of the resource.</param>
    /// <returns></returns>
    public static Uri RouteIdUri(this UrlHelper helper, string routeName, Id id, object routeData)
    {
      return helper.GetRouteService(routeName).RouteUrl(routeName, id, routeData);
    }
    public static string RouteIdUrl(this UrlHelper helper, string routeName, Id id, object routeData)
    {
      return RouteIdUri(helper, routeName, id, routeData).ToString();
    }

    /// <summary>
    /// Route's to collection, entry, or media given the id.
    /// </summary>
    /// <param name="helper"></param>
    /// <param name="routeName">Name of the route.</param>
    /// <param name="id">Id of the resource.</param>
    /// <param name="routeData">The data for the route.</param>
    /// <returns></returns>
    public static Uri RouteIdUri(this UrlHelper helper, string routeName, Id id, object routeData, AbsoluteMode abMode)
    {
      return helper.GetRouteService(routeName).RouteUrl(routeName, id, routeData, abMode);
    }
    public static string RouteIdUrl(this UrlHelper helper, string routeName, Id id, object routeData, AbsoluteMode abMode)
    {
      return RouteIdUri(helper, routeName, id, routeData, abMode).ToString();
    }


    /// <summary>
    /// Gets a route url using the correct route service.
    /// </summary>
    /// <param name="helper"></param>
    /// <param name="routeName">Name of the route.</param>
    /// <param name="forceAbsolute">Forces full url path.</param>
    /// <returns></returns>
    public static Uri RouteUriEx(this UrlHelper helper, string routeName, AbsoluteMode abMode)
    {
      return helper.GetRouteService(routeName).RouteUrl(routeName, abMode);
    }
    public static string RouteUrlEx(this UrlHelper helper, string routeName, AbsoluteMode abMode)
    {
      return RouteUriEx(helper, routeName, abMode).ToString();
    }

    /// <summary>
    /// Gets a route url using the correct route service.
    /// </summary>
    /// <param name="helper"></param>
    /// <param name="routeName">Name of the route.</param>
    /// <param name="forceAbsolute">Forces full url path.</param>
    /// <returns></returns>
    public static Uri RouteUriEx(this UrlHelper helper, string routeName)
    {
      return helper.GetRouteService(routeName).RouteUrl(routeName);
    }
    public static string RouteUrlEx(this UrlHelper helper, string routeName)
    {
      return RouteUriEx(helper, routeName).ToString();
    }

    /// <summary>
    /// Gets a route url using the correct route service.
    /// </summary>
    /// <param name="helper"></param>
    /// <param name="routeName">Name of the route.</param>
    /// <param name="forceAbsolute">Forces full url path.</param>
    /// <param name="routeData">The data for the route.</param>
    /// <returns></returns>
    public static Uri RouteUriEx(this UrlHelper helper, string routeName, object routeData)
    {
      return helper.GetRouteService(routeName).RouteUrl(routeName, routeData);
    }
    public static string RouteUrlEx(this UrlHelper helper, string routeName, object routeData)
    {
      return RouteUriEx(helper, routeName, routeData).ToString();
    }

    /// <summary>
    /// Gets a route url using the correct route service.
    /// </summary>
    /// <param name="helper"></param>
    /// <param name="routeName">Name of the route.</param>
    /// <param name="routeData">The data for the route.</param>
    /// <param name="abMode">Forces full url path.</param>
    /// <returns></returns>
    public static Uri RouteUriEx(this UrlHelper helper, string routeName, object routeData, AbsoluteMode abMode)
    {
      return helper.GetRouteService(routeName).RouteUrl(routeName, routeData, abMode);
    }
    public static string RouteUrlEx(this UrlHelper helper, string routeName, object routeData, AbsoluteMode abMode)
    {
      return RouteUriEx(helper, routeName, routeData, abMode).ToString();
    }

    public static string GetNextPage(this UrlHelper helper, IEnumerable<AtomLink> links)
    {
      AtomLink link = links.Where(l => l.Rel == "next" && l.Type != Atom.ContentTypeFeed).FirstOrDefault();
      if (link != null) return link.Href.ToString();
      return null;
    }

    public static string GetPrevPage(this UrlHelper helper, IEnumerable<AtomLink> links)
    {
      AtomLink link = links.Where(l => l.Rel == "previous" && l.Type != Atom.ContentTypeFeed).FirstOrDefault();
      if (link != null) return link.Href.ToString();
      return null;
    }

    public static IEnumerable<AtomLink> GetPagingLinks(this UrlHelper helper, string routeName, Id id, RouteValueDictionary routeData, int total, int pageIndex, int pageSize, string contentType, AbsoluteMode mode)
    {
      int lastIndex = (int)Math.Ceiling((double)total / (double)pageSize) - 1;
      List<AtomLink> links = new List<AtomLink>();
      links.Add(new AtomLink { Href = helper.RouteIdUri(routeName, id, GetPageRouteData(0), mode), Rel = "first", Type = contentType });
      links.Add(new AtomLink { Href = helper.RouteIdUri(routeName, id, GetPageRouteData(lastIndex), mode), Rel = "last", Type = contentType });
      if (pageIndex > 0)
        links.Add(new AtomLink { Href = helper.RouteIdUri(routeName, id, GetPageRouteData(pageIndex - 1), mode), Rel = "previous", Type = contentType });
      if (pageIndex < lastIndex)
        links.Add(new AtomLink { Href = helper.RouteIdUri(routeName, id, GetPageRouteData(pageIndex + 1), mode), Rel = "next", Type = contentType });
      return links;
    }
    public static IEnumerable<AtomLink> GetPagingLinks(this UrlHelper helper, string routeName, Id id, RouteValueDictionary routeData, int total, int pageIndex, int pageSize, string contentType)
    {
      return GetPagingLinks(helper, routeName, id, routeData, total, pageIndex, pageSize, contentType, AbsoluteMode.Default);
    }

    private static RouteValueDictionary GetPageRouteData(int pageIndex)
    {
      RouteValueDictionary routeData = new RouteValueDictionary();
      if (pageIndex > 0) routeData.Add("page", pageIndex + 1);
      return routeData;
    }

    public static string ThemeStyleSheetHref(this UrlHelper helper)
    {
      string theme = ThemeViewEngine.GetCurrentThemeName(helper.RequestContext);
      return helper.Content(string.Format("~/css/{0}/{0}.css", theme));
    }

    public static string AssetPath(this UrlHelper helper, AssetType assetType, string assetName)
    {
      string theme = ThemeViewEngine.GetCurrentThemeName(helper.RequestContext);
      return AssetService.GetCurrent(helper.RequestContext).GetAssetUrl(assetType, theme, assetName, helper);    
    }

    public static string ImageSrc(this UrlHelper helper, string imageFileName)
    {
      return helper.AssetPath(AssetType.Img, imageFileName);
    }

    public static string StyleHref(this UrlHelper helper, string styleFileName)
    {
      return helper.AssetPath(AssetType.Css, styleFileName);
    }

    public static string ScriptSrc(this UrlHelper helper, string scriptFileName)
    {
      return helper.AssetPath(AssetType.Js, scriptFileName);
    }

    public static string GetGravatarHref(this UrlHelper helper, string email, int size)
    {
      return WebHelper.GetGravatarHref(email, size).ToString();
    }

    static IRouteService GetRouteService(this UrlHelper helper, string routeName)
    {
      return RouteService.GetForRoute(helper.RequestContext, routeName);
    }
  }

}
