/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  public interface IAuthenticateService
  {
    void Authenticate(ServerApp app);
    void PostAuthenticate(ServerApp app);
  }
}
