/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore.Modules
{
  using System;
  using System.Linq;
  using System.Security;
  using System.Security.Cryptography;
  using System.Security.Principal;
  using System.Text;
  using System.Web;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using StructureMap;

  public class WsseAuthenticationModule : IHttpModule
  {
    public const string AuthenticationType = "WSSE Token";
    const string HttpAuthorizationHeader = "Authorization";  // HTTP1.1 Authorization header
    public const string XwsseHeader = "X-WSSE";
    const string HttpWWWAuthenticateHeader = "WWW-Authenticate"; // HTTP1.1 Basic Challenge Scheme Name
    const string Realm = "AtomPub"; // HTTP.1.1 Basic Challenge Realm
    const int MINUTE_RANGE = 10; // in case server or client time is not accurate

    public void AuthenticateUser(Object source, EventArgs e)
    {
      HttpContext ctx = ((HttpApplication)source).Context;
      string auth = ctx.Request.Headers[HttpAuthorizationHeader];
      if (!string.IsNullOrEmpty(auth) && auth.Contains("WSSE profile=\"UsernameToken\""))
      {
        string xwsse = ctx.Request.Headers[XwsseHeader];

        if (xwsse == null) throw new ArgumentNullException(xwsse);

        //extract data from token
        string name = xwsse.Split(',').Where(s => s.ToLower().Contains("user")).Single().Split('"')[1];
        string passDigest = xwsse.Split(',').Where(s => s.ToLower().Contains("pass")).Single().Split('"')[1];
        string nonce = xwsse.Split(',').Where(s => s.ToLower().Contains("nonce")).Single().Split('"')[1];
        string date = xwsse.Split(',').Where(s => s.ToLower().Contains("created")).Single().Split('"')[1];

        //is token stale?
        var d = DateTimeOffset.Parse(date);
        if (d < DateTimeOffset.UtcNow.AddMinutes(-MINUTE_RANGE) || d > DateTimeOffset.UtcNow.AddMinutes(MINUTE_RANGE))
        {
          //IContainer c = (IContainer)((HttpApplication)source).Application["Container"];
          //ILogService log = c.GetInstance<ILogService>();
          //container.GetInstance<ILogService>().Info("WSSE date: {0}, local date: {1}", date, DateTimeOffset.UtcNow);
          throw new TokenExpiredException();
        }

        //get the plugin
        IContainer container = (IContainer)((HttpApplication)source).Application["Container"];
        IUserRepository userRepo = container.GetInstance<IUserRepository>();

        User user = userRepo.GetUser(name);
        if (user != null)
        {
          user.AuthenticationType = AuthenticationType;
          bool authenticated = passDigest == Convert.ToBase64String(
             SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(nonce + date + user.GetClearPassword())));
          if (authenticated) ctx.User = new GenericPrincipal(user, new string[0]);
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
            "WSSE realm=\"" + Realm + "\", profile=\"UsernameToken\"");
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

  public class TokenExpiredException : SecurityException { };
}
