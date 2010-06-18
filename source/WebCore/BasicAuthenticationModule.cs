/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore.Modules
{
  using System;
  using System.Security.Principal;
  using System.Text;
  using System.Web;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using StructureMap;

  public class BasicAuthenticationModule : IHttpModule
  {
    public const string AuthenticationType = "Basic";
    const string HttpAuthorizationHeader = "Authorization";  // HTTP1.1 Authorization header
    public const string BasicHeader = "Basic";
    const string HttpWWWAuthenticateHeader = "WWW-Authenticate"; // HTTP1.1 Basic Challenge Scheme Name
    const string Realm = "AtomPub"; // HTTP.1.1 Basic Challenge Realm

    public void AuthenticateUser(Object source, EventArgs e)
    {
      HttpContext ctx = ((HttpApplication)source).Context;
      string auth = ctx.Request.Headers[HttpAuthorizationHeader];
      if (!string.IsNullOrEmpty(auth) && auth.StartsWith(BasicHeader))
      {
        string basic = Encoding.ASCII.GetString(Convert.FromBase64String(auth.Split(' ')[1]));
        //get the plugin
        IContainer container = (IContainer)((HttpApplication)source).Application["Container"];
        IUserRepository userRepo = container.GetInstance<IUserRepository>();
        User user = userRepo.GetUser(basic.Split(':')[0]);
        if (user != null && user.CheckPassword(basic.Split(':')[1]))
        {
          user.AuthenticationType = AuthenticationType;
          ctx.User = new GenericPrincipal(user, new string[0]);
        }

        if (ctx.User == null || ctx.User.Identity == null)
        {
          //set to anon
          ctx.User = new GenericPrincipal(
          new User() { Name = string.Empty, AuthenticationType = AuthenticationType }, new string[0]);
        }
      }
    }

    public void IssueAuthenticationChallenge(object source, EventArgs e)
    {
      HttpApplication application = (HttpApplication)source;

      if (application.Context.Response.StatusCode == 401)
      {
        application.Context.Response.AddHeader(HttpWWWAuthenticateHeader,
            BasicHeader + " realm=\"" + Realm + "\"");
      }
    }

    #region IHttpModule Members
    public void Init(HttpApplication context)
    {
      //
      // Subscribe to the authenticate event to perform the
      // authentication.
      //
      context.AuthenticateRequest += new EventHandler(this.AuthenticateUser);
      //
      // Subscribe to the EndRequest event to issue the
      // challenge if necessary.
      //
      context.EndRequest += new EventHandler(this.IssueAuthenticationChallenge);
    }

    public void Dispose()
    {
      //
      // Do nothing here
      //
    }
    #endregion
  }
}
