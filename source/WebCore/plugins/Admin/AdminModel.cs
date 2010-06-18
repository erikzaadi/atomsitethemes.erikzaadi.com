/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;
  using AtomSite.Domain;

  public class AdminModel : PageModel
  {
    public AdminModel()
    {
      Errors = new List<string>();
      Notifications = new Dictionary<string, string>();
    }

    public string SiteDisplayUrl
    {
      get
      {
        string url = base.Service.Base.ToString();
        url = url.Replace("http://", string.Empty);
        if (url.EndsWith("/")) url = url.Substring(0, url.Length - 1);
        return url;
      }
    }

    public Uri SiteUri { get { return base.Service.Base; } }

    public IList<string> Errors { get; set; }
    public IDictionary<string, string> Notifications { get; set; }

    public AppCollection TargetCollection
    {
      get
      {
        //only return an authorized scope
        var scopes = AuthorizeService.GetScopes(User);
        var colls = Service.GetCollections(scopes).Where(c => c.AcceptsEntries);
        AppCollection coll = null;

        if (base.Scope.IsCollection)
        {
          coll = colls.Where(c => c.Id.Workspace == Scope.Workspace && c.Id.Collection == Scope.Collection).FirstOrDefault();
        }
        else if (base.Scope.IsWorkspace)
        {
          coll = colls.Where(c => c.Id.Workspace == Scope.Workspace).FirstOrDefault();
        }

        if (coll == null)
          coll = colls.Where(c => c.Default).FirstOrDefault();
        return coll;
      }
    }

    public Scope BaseScope
    {
      get
      {
        var scopes = AuthorizeService.GetScopes(User);
        if (!Scope.InScope(scopes) && scopes.Count() > 0)
        {
          return scopes.First();
        }
        return Scope;
      }
    }

    public bool ScopeAcceptsEntries()
    {
      return Service.GetCollections(Scope).Where(c => c.AcceptsEntries).Count() > 0;
    }

    public bool ScopeAcceptsMedia()
    {
      return Service.GetCollections(Scope).Where(c => c.AcceptsMedia).Count() > 0;
    }
  }
}
