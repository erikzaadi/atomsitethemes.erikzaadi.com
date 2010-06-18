/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Domain;
  using System;
  using System.Security.Principal;
  using System.Collections.Generic;
  using System.Linq;

  public interface IAuthorizeService
  {
    bool IsInRole(User user, Scope scope, AuthRoles roles);
    AuthRoles GetRoles(User user, Scope scope);
    bool IsAuthorized(AuthRoles roles, Scope scope, AuthAction action);
    bool IsAuthorized(User user, Scope scope, AuthAction action);
    IEnumerable<Scope> GetScopes(User user);
  }

  public static class AuthorizeExtensions
  {
    public static void Auth(this IAuthorizeService auth, Scope scope, AuthAction action)
    {
      User user = System.Threading.Thread.CurrentPrincipal.Identity as User;
      if (!auth.IsAuthorized(auth.GetRoles(user, scope), scope, action))
        throw new UserNotAuthorizedException(user.Name, action.ToString());
    }

    //public static void Auth(this IAuthorizeService auth, AuthAction action)
    //{
    //  Auth(auth, null, action);
    //}

    public static void SetPerson(this AtomEntry entry, IAuthorizeService auth, bool forceAuthor)
    {
      //TODO: how to handle existing people, validate them?

      //determine if current user is an author or contributor
      User u = System.Threading.Thread.CurrentPrincipal.Identity as User;

      if (!u.IsAuthenticated) return;
      List<AtomPerson> authors = entry.Authors.ToList();
      if (auth.GetRoles(u, entry.Id.ToScope()) >= AuthRoles.Author || forceAuthor)
      {
        AtomPerson author = u.ToAtomAuthor();
        if (!authors.Contains(author))
        {
          authors.Add(author); //add
          entry.Authors = authors; //update
        }
      }
      else if (auth.GetRoles(u, entry.Id.ToScope()) >= AuthRoles.Contributor)
      {
        List<AtomPerson> contribs = entry.Contributors.ToList();
        AtomPerson contrib = u.ToAtomContributor();
        if (!contribs.Contains(contrib))
        {
          contribs.Add(contrib); //add
          entry.Contributors = contribs; //update
        }
      }
      else //default to author
      {
        AtomPerson author = u.ToAtomAuthor();
        if (!authors.Contains(author))
        {
          authors.Add(author); //add
          entry.Authors = authors; //update
        }
      }
    }
    public static void SetPerson(this AtomEntry entry, IAuthorizeService auth)
    {
      SetPerson(entry, auth, false);
    }
  }
}
