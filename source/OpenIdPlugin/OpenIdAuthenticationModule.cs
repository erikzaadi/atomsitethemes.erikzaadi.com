/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.OpenIdPlugin
{
  using System;
  using System.Security.Principal;
  using System.Web;
  using System.Web.Security;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using AtomSite.WebCore;
  using DotNetOpenAuth;
  using StructureMap;
  using DotNetOpenAuth.OpenId.RelyingParty;
  using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
  using DotNetOpenAuth.OpenId;
  using DotNetOpenAuth.Messaging;

  public class OpenIdAuthenticationModule : IHttpModule
  {
    public void Init(HttpApplication context)
    {
      //no longer used
    }

    public void Dispose() { }
  }
}
