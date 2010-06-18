/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Linq;
  using AtomSite.Domain;

  public class ScopeInclude : Include
  {
    private static readonly string[] scopes = { "site", "workspace", "collection" };

    public ScopeInclude() : base() { }
    public ScopeInclude(Include include) : base(include) { }
     
    public string ScopeName
    {
      get 
      { 
        string s = GetValue<string>(Atom.SvcNs + "scope");
        if (s != null && scopes.Contains(s.ToLower())) return s.ToLower();
        return null;
      }
      set { SetValue<string>(Atom.SvcNs + "scope", value.ToLower()); }
    }
  }
}
