/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Web.Mvc;
  using AtomSite.Domain;

  public static class AdminHtmlHelperExtensions
  {
    public static void RenderScope<T>(this HtmlHelper<T> helper, Scope scope, bool top) where T : PageModel
    {
      string tag = top ? "View<a href='javascript:return false'><em>{2}</em> {3} <small>{4}</small></a>" :
        "<li{0}><a href='{1}'><em>{2}</em> {3} <small>{4}</small></a></li>";

      string type = string.Empty;
      string name = string.Empty;
      string note = string.Empty;
      string attrib = string.Empty;
      string workspace = helper.ViewContext.HttpContext.Request.QueryString["workspace"];
      string collection = helper.ViewContext.HttpContext.Request.QueryString["collection"];

      if (!top && (workspace == scope.Workspace && collection == scope.Collection))
      {
        attrib = " class='current'";
      }

      UriBuilder uri = new UriBuilder(helper.ViewContext.HttpContext.Request.Url);
      if (scope.IsEntireSite)
      {
        uri.Query = string.Empty;
        type = "Entire Site";
        note = "(all workspaces)";
      }
      else if (scope.IsWorkspace)
      {
        uri.Query = "workspace=" + scope.Workspace;
        type = !top ? "&#160;Workspace" : "Workspace";
        name = scope.Workspace;
        note = helper.ViewData.Model.Service.GetWorkspace(scope.Workspace).Default ? "(default)" : string.Empty;
      }
      else if (scope.IsCollection)
      {
        uri.Query = "workspace=" + scope.Workspace + "&collection=" + scope.Collection;
        type = !top ? "&#160;&#160;Collection" : "Collection";
        name = scope.Collection;
        note = helper.ViewData.Model.Service.GetCollection(scope.Workspace, scope.Collection).Default ? "(default)" : string.Empty;
      }
      string url = uri.ToString();

      tag = string.Format(tag, attrib, url, type, name, note);
      helper.ViewContext.HttpContext.Response.Write(tag);
    }
  }
}
