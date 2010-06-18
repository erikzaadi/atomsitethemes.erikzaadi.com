/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore.Modules
{
  using System;
  using System.Web;
  using System.Web.Configuration;

  public class FormsBypassModule : IHttpModule
  {
    public virtual void Init(HttpApplication context)
    {
      context.EndRequest += OnEndRequest;
    }

    public virtual void Dispose() { }

    void OnEndRequest(object sender, EventArgs e)
    {
      OnEndRequest(((HttpApplication)sender).Context);
    }

    protected virtual void OnEndRequest(HttpContext context)
    {
      if (context.Response.StatusCode == 302 &&
                context.Items.Contains("FormsAuthBypass") && (bool)context.Items["FormsAuthBypass"])
      {
        context.Response.Clear();
        //context.Response.Headers.Remove("Location"); //only works with IIS7
        context.Response.StatusCode = 401;
        context.Response.StatusDescription = "Access Denied";

        CustomErrorsSection customErrorsSection = (CustomErrorsSection)context.GetSection("system.web/customErrors");
        string url = customErrorsSection.DefaultRedirect;
        // Get the collection
        foreach (CustomError e in customErrorsSection.Errors)
        {
          if (context.Response.StatusCode == e.StatusCode)
            url = e.Redirect ?? url;
        }
        context.Server.Transfer(url);
      }
    }
  }
}