/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;
  using AtomSite.Domain;
  using System.Collections;

  public class AdminUsersModel : AdminModel
  {
    public AdminUsersModel()
    {
      AddLinks = new Dictionary<string, string>();
    }

    public string Title { get; set; }
    public IPagedList<User> Users { get; set; }
    public IDictionary<string, string> AddLinks { get; set; }
    public bool CanRemove { get; set; }
    public Func<User, string> GetRemoveHref { get; set; }
    public bool CanEdit { get; set; }

    public string Filter { get; set; }
    public int AllCount { get; set; }
    public int AdminsCount { get; set; }
    public int AuthorsCount { get; set; }
    public int ContribsCount { get; set; }
    //public int UsersCount { get; set; }

    public string GetRoles(User u)
    {
      var roles = new List<string>();
      var scopes = AuthorizeService.GetScopes(u);
      foreach (Scope scope in scopes)
      {
        if (AuthorizeService.IsInRole(u, scope, AuthRoles.Administrator))
        {
          if (!roles.Contains("Admin")) roles.Add("Admin");
        }
        if (AuthorizeService.IsInRole(u, scope, AuthRoles.Author))
        {
          if (!roles.Contains("Author")) roles.Add("Author");
        }
        if (AuthorizeService.IsInRole(u, scope, AuthRoles.Contributor))
        {
          if (!roles.Contains("Contributor")) roles.Add("Contributor");
        }
      }
      if (roles.Count == 0) roles.Add("User");
      return string.Join(", ", roles.OrderBy(s => s).ToArray());
    }
  }
}
