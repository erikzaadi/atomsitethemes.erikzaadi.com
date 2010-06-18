/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections;
  using System.Collections.Generic;
  using System.Net;
  using System.Web.Mvc;

  public class HttpResult : ContentResult
  {
    public HttpResult()
      : base()
    {
      StatusCode = HttpStatusCode.OK;
      StatusDescription = string.Empty;
      Headers = new Dictionary<string, string>();
      Items = new Dictionary<object, object>();
    }

    public string StatusDescription { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public IDictionary<string, string> Headers { get; set; }
    public IDictionary Items { get; set; }

    public override void ExecuteResult(ControllerContext context)
    {
      foreach (string header in Headers.Keys)
        context.HttpContext.Response.AddHeader(header, Headers[header]);
      foreach (object item in Items.Keys)
        context.HttpContext.Items.Add(item, Items[item]);

      context.HttpContext.Response.StatusCode = (int)StatusCode;
      if (StatusDescription != null)
        context.HttpContext.Response.StatusDescription = StatusDescription;
      base.ExecuteResult(context);
    }
  }
}
