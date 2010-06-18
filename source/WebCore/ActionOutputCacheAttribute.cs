//http://blog.codeville.net/2008/10/15/partial-output-caching-in-aspnet-mvc/
namespace AtomSite.WebCore
{
  using System;
  using System.IO;
  using System.Reflection;
  using System.Text;
  using System.Web;
  using System.Web.Caching;
  using System.Web.Mvc;
  using System.Web.UI;
  using AtomSite.Domain;
  using StructureMap;

  public class ActionOutputCacheAttribute : ActionFilterAttribute
  {
    private static MethodInfo switchWriterMethod = ServerApp.CurrentTrustLevel > AspNetHostingPermissionLevel.Medium ?
      typeof(HttpResponse).GetMethod("SwitchWriter", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic) :
      null;

    public ActionOutputCacheAttribute(int cacheDuration)
      : this(cacheDuration, false)
    { }

    public ActionOutputCacheAttribute(int cacheDuration, bool anonOnly)
    {
      this.cacheDuration = cacheDuration;
      this.anonOnly = anonOnly;
    }

    private int cacheDuration;
    private bool anonOnly;
    private TextWriter originalWriter;
    private string cacheKey;

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      //We can't cache if no switch writer
      if (true || switchWriterMethod == null) return;

      this.cacheKey = ComputeCacheKey(filterContext);
      string cachedOutput = (string)filterContext.HttpContext.Cache[this.cacheKey];
      if (cachedOutput != null)
        filterContext.Result = new ContentResult { Content = cachedOutput };
      else
        this.originalWriter = (TextWriter)switchWriterMethod.Invoke(HttpContext.Current.Response, new object[] { new HtmlTextWriter(new StringWriter()) });
    }

    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
      //We can't cache if no switch writer
      if (switchWriterMethod == null) return;

      if (originalWriter != null) // Must complete the caching
      {
        HtmlTextWriter cacheWriter = (HtmlTextWriter)switchWriterMethod.Invoke(HttpContext.Current.Response, new object[] { originalWriter });
        string textWritten = ((StringWriter)cacheWriter.InnerWriter).ToString();
        filterContext.HttpContext.Response.Write(textWritten);

        filterContext.HttpContext.Cache.Add(cacheKey, textWritten, null, DateTime.Now.AddSeconds(cacheDuration), Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
      }
    }

    private string ComputeCacheKey(ActionExecutingContext filterContext)
    {
      //if anon only return guid when not anon
      if (anonOnly && !filterContext.HttpContext.User.Identity.IsAuthenticated) return Guid.NewGuid().ToString();

      var keyBuilder = new StringBuilder();
      keyBuilder.Append(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName);
      keyBuilder.Append(filterContext.ActionDescriptor.ActionName);

      //factor authorization roles into cache key
      BaseController c = filterContext.Controller as BaseController;
      if (c != null)
      {
        IAuthorizeService auth = ((IContainer)filterContext.HttpContext.Application["container"]).GetInstance<IAuthorizeService>();
        AuthRoles roles = auth.GetRoles((User)filterContext.HttpContext.User.Identity, c.Scope);
        keyBuilder.AppendFormat("r" + roles);
      }
      //do not factor route into caching since may be cached across multiple pages
      //foreach (var pair in filterContext.RouteData.Values)
      //  keyBuilder.AppendFormat("rd{0}_{1}_", pair.Key.GetHashCode(), pair.Value.GetHashCode());
      foreach (var pair in filterContext.ActionParameters)
      {
        if (pair.Value != null)
          keyBuilder.AppendFormat("ap{0}_{1}_", pair.Key.GetHashCode(), pair.Value.GetHashCode());
      }
      return keyBuilder.ToString();
    }
  }
}
