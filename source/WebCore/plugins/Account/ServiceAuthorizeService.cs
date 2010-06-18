/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Linq;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using System.Collections.Generic;

  /// <summary>
  /// Authorizes based on values set within the AppService.
  /// &lt;admin&gt;, &lt;author&gt;, &lt;contributor&gt;
  /// </summary>
  public class ServiceAuthorizeService : IAuthorizeService
  {
    protected IAppServiceRepository AppServiceRepository { get; private set; }
    protected ILogService LogService { get; private set; }

    public ServiceAuthorizeService(IAppServiceRepository svcRepo, ILogService logger)
    {
      this.AppServiceRepository = svcRepo;
      this.LogService = logger;
    }

    public bool IsInRole(User user, Scope scope, AuthRoles role)
    {
      return (GetRoles(user, scope) & role) == role;
    }

    /// <summary>
    /// Get the scopes which the user is allowed to work with
    /// based on their authorization.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public IEnumerable<Scope> GetScopes(User user)
    {
      IList<Scope> scopes = new List<Scope>();
      if (user == null) return scopes;
      
      bool admin = false;
      if (IsInRole(user, Scope.EntireSite, AuthRoles.Administrator))
      {
        scopes.Add(Scope.EntireSite);
        admin = true;
      }

      foreach (AppWorkspace w in AppServiceRepository.GetService().Workspaces)
      {
        bool owner = false;
        if (w.People.Intersect(user.Ids).Count() > 0 || admin)
        {
          scopes.Add(new Scope() { Workspace = w.Name ?? Atom.DefaultWorkspaceName });
          owner = true;
        }
        foreach (AppCollection c in w.Collections)
        {
          if (c.People.Intersect(user.Ids).Count() > 0 || owner)
          {
            scopes.Add(new Scope() { Workspace = c.Id.Workspace, Collection = c.Id.Collection });
          }
        }
      }
      return scopes;
    }

    /// <summary>
    /// Gets all the roles for a user for the given scope.  The roles for the user are collected
    /// recursively by looking at the service, workspace, and collection levels.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public AuthRoles GetRoles(User user, Scope scope)
    {
      AppService appService = AppServiceRepository.GetService();
      AuthRoles roles = AuthRoles.None;
      if (user != null && user.IsAuthenticated)
      {
        roles |= AuthRoles.User; //all authenticated are users

        //if (scope.IsEntireSite)
        //{
          if (appService.Admins.Where(a => user.Ids.Select(i => i.ToUpperInvariant())
            .Contains(a.ToUpperInvariant())).SingleOrDefault() != null) roles |= AuthRoles.Administrator;
        //}
        //else
        if (!scope.IsEntireSite)
        {
          //get roles at workspace level
          AppWorkspace ws = appService.GetWorkspace(scope.Workspace);
          if (ws.Authors.Where(a => user.Ids.Select(i => i.ToUpperInvariant())
            .Contains(a.ToUpperInvariant())).SingleOrDefault() != null) roles |= AuthRoles.Author;
          if (ws.Contributors.Where(a => user.Ids.Select(i => i.ToUpperInvariant())
            .Contains(a.ToUpperInvariant())).SingleOrDefault() != null) roles |= AuthRoles.Contributor;

          if (scope.IsCollection) //continue at collection level getting scopes
          {
            AppCollection c = ws.GetCollection(scope.Collection);
            if (c.Authors.Where(a => user.Ids.Select(i => i.ToUpperInvariant())
            .Contains(a.ToUpperInvariant())).SingleOrDefault() != null)
              roles |= AuthRoles.Author;

            if (c.Contributors.Where(a => user.Ids.Select(i => i.ToUpperInvariant())
            .Contains(a.ToUpperInvariant())).SingleOrDefault() != null)
              roles |= AuthRoles.Contributor;
          }
        }
      }
      else
      {
        //all non-authenticated are anonymous
        roles |= AuthRoles.Anonymous;
      }
      return roles;
    }
    
    /// <summary>
    /// Gets whether the user is authorized to perform the action within
    /// the given context based on the role matrix.  The role matrix
    /// is first checked at the collection level and then it bubbles up
    /// to the workspace level and onto the service level and then
    /// finally the default built-in role matrix.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="id">The entry Id or the collection Id.</param>
    /// <param name="action">The action.</param>
    /// <returns></returns>
    public bool IsAuthorized(User user, Scope scope, AuthAction action)
    {
      return IsAuthorized(GetRoles(user, scope), scope, action);
    }

    /// <summary>
    /// Gets whether any of the roles are authorized to perform the action within
    /// the given context based on the role matrix.  The role matrix
    /// is first checked at the collection level and then it bubbles up
    /// to the workspace level and onto the service level and then
    /// finally the default built-in role matrix.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="id">The entry Id or the collection Id.</param>
    /// <param name="action">The action.</param>
    /// <returns></returns>
    public bool IsAuthorized(AuthRoles roles, Scope scope, AuthAction action)
    {
      RoleMatrix rm = null;
      RoleAction ra = null;
      AppService appService = AppServiceRepository.GetService();

      if (!scope.IsEntireSite)
      {
        var w = appService.GetWorkspace(scope.Workspace);

        if (scope.IsCollection)
        {
          //try collection level first
          rm = w.GetCollection(scope.Collection).RoleMatrix;
          if (rm != null) ra = rm.RoleActions.Where(a => a.Name == action.ToString()).FirstOrDefault();
        }

        //try workspace level next
        if (ra == null)
        {
          rm = w.RoleMatrix;
          if (rm != null) ra = rm.RoleActions.Where(a => a.Name == action.ToString()).FirstOrDefault();
        }
      }

      //service level
      if (ra == null)
      {
        rm = appService.RoleMatrix;
        if (rm != null) ra = rm.RoleActions.Where(a => a.Name == action.ToString()).FirstOrDefault();
      }

      //use default role matrix
      if (ra == null)
      {
        rm = RoleMatrix.Default;
        if (rm != null) ra = rm.RoleActions.Where(a => a.Name == action.ToString()).FirstOrDefault();
      }

      if (ra == null)
      {
        LogService.Warn("Action not found in any role matrix.");
        return false;
      }

      return ((ra.AuthRoles & roles) > AuthRoles.None);
    }
  }
}
