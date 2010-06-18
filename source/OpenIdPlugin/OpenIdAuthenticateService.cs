/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.OpenIdPlugin
{
  using System.Security.Principal;
  using System.Web;
  using System.Web.Security;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using AtomSite.WebCore;
  using DotNetOpenAuth.Messaging;
  using DotNetOpenAuth.OpenId;
  using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
  using DotNetOpenAuth.OpenId.RelyingParty;

  public class OpenIdAuthenticateService : IAuthenticateService
  {
    protected readonly ILogService LogService;
    protected readonly IUserRepository UserRepository;

    public OpenIdAuthenticateService(ILogService logger, IUserRepository userRepo)
    {
      LogService = logger;
      UserRepository = userRepo;
    }

    public virtual void Authenticate(ServerApp app)
    {
      HttpContext ctx = app.Context;

      //only authenticate when not already authenticated since this works side by side with forms auth
      if (ctx.User == null || ctx.User.Identity == null || !ctx.User.Identity.IsAuthenticated)
      {
        //detect authenticated response for open id
        var openid = new OpenIdRelyingParty();
        var response = openid.GetResponse();
        if (response != null && response.Status == AuthenticationStatus.Authenticated)
        {
          User user = UserRepository.GetUser(response.ClaimedIdentifier);
          bool newUser = (user == null);
          if (newUser) user = new User()
          {
            Ids = new string[] { response.ClaimedIdentifier },
            Name = response.FriendlyIdentifierForDisplay
          };
          //Update user information
          ClaimsResponse fetch = response.GetExtension<ClaimsResponse>();
          if (fetch != null)
          {
            if (!string.IsNullOrEmpty(fetch.Nickname)) user.Name = fetch.Nickname;
            if (!string.IsNullOrEmpty(fetch.Email)) user.Email = fetch.Email;
          }
          if (newUser) UserRepository.CreateUser(user);
          else UserRepository.UpdateUser(user);

          FormsAuthentication.SetAuthCookie(response.ClaimedIdentifier, false);
          //FormsAuthentication.RedirectFromLoginPage(openid.Response.ClaimedIdentifier, false);
          //send back to the right page
          string returnUrl = ctx.Request.QueryString["ReturnUrl"];
          if (!string.IsNullOrEmpty(returnUrl))
          {
            returnUrl = HttpUtility.UrlDecode(returnUrl);
            ctx.Response.Redirect(returnUrl);
          }
        }
        else if (response != null && response.Status != AuthenticationStatus.Authenticated)
        {
          LogService.Error("Open ID authentication for {0} was unsuccessful with status of {1}.",
            response.FriendlyIdentifierForDisplay, response.Status.ToString());
          //FormsAuthentication.RedirectToLoginPage("error=" + HttpUtility.UrlEncode("Open ID authentication was unsuccessful."));
          string returnUrl = ctx.Request.QueryString["ReturnUrl"];
          ctx.Response.Redirect(FormsAuthentication.LoginUrl + "?ReturnUrl="
            + HttpUtility.UrlEncode(returnUrl) + "&error=" + HttpUtility.UrlEncode("Open ID authentication was unsuccessful."));
        }
        else if (ctx.User == null || ctx.User.Identity == null)
        {
          //set to anon
          ctx.User = new GenericPrincipal(
          new User() { Name = string.Empty }, new string[0]);
        }

        //detect login request for open id
        string identifier = ctx.Request.Form["openid_identifier"];
        if (!string.IsNullOrEmpty(identifier))
        {
          try
          {
            var req = openid.CreateRequest(Identifier.Parse(identifier));
            var fields = new ClaimsRequest();
            fields.Email = DemandLevel.Require;
            fields.Nickname = DemandLevel.Require;
            req.AddExtension(fields);
            req.RedirectToProvider();
          }
          catch (ProtocolException pe)
          {
            LogService.Error(pe);
            //FormsAuthentication.RedirectToLoginPage("error=" + HttpUtility.UrlEncode(oie.Message));
            string returnUrl = ctx.Request.QueryString["ReturnUrl"];
            ctx.Response.Redirect(FormsAuthentication.LoginUrl + "?ReturnUrl="
            + HttpUtility.UrlEncode(returnUrl) + "&error=" + HttpUtility.UrlEncode(pe.Message));
          }
        }
      }
    }
    public virtual void PostAuthenticate(ServerApp app) { }
  }
}
