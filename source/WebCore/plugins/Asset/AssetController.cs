using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AtomSite.Domain;
using System.Net;

namespace AtomSite.WebCore
{
  public class AssetController : BaseController
  {
    protected IAssetService AssetService { get; set; }

    public AssetController(IAssetService assetService) : base()
    {
      AssetService = assetService;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GroupCss(string group)
    {
      return Group(AssetType.Css, "text/css", group);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GroupJs(string group)
    {
      return Group(AssetType.Js, "text/javascript", group);
    }

    protected ActionResult Group(AssetType assetType, string contentType, string group)
    {
      var c = AssetService.GetGroupCombined(assetType, group, Url);
      if (NoneMatch(c.Md5))
      {
        TimeSpan ts = new TimeSpan(30, 0, 0, 0);
        Response.Cache.SetCacheability(HttpCacheability.Public);
        Response.Cache.SetExpires(DateTime.Now.Add(ts));
        Response.Cache.SetMaxAge(ts);
        Response.Cache.SetValidUntilExpires(true);
        Response.Cache.SetLastModified(c.LastModified);
        Response.Cache.SetETag(c.Md5);
        return new ContentResult()
        {
          Content = c.Content,
          ContentType = contentType,
        };
      }
      else
      {
        return new HttpResult() { StatusCode = HttpStatusCode.NotModified };
      }
    }
 
  }
}
