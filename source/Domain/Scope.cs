using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomSite.Domain
{
  [Flags]
  public enum SupportedScopes
  {
    None = 0,
    EntireSite = 1,
    Workspace = 2,
    Collection = 4,
    Entry = 8,
    Inline = 16,
    All = EntireSite | Workspace | Collection | Entry | Inline
  }

  public struct Scope
  {
    public static Scope EntireSite = new Scope();

    public string Workspace { get; set; }
    public string Collection { get; set; }
    public bool IsEntireSite { get { return Workspace == null && Collection == null; } }
    public bool IsWorkspace { get { return Workspace != null && Collection == null; } }
    public bool IsCollection { get { return Workspace != null && Collection != null; } }

    public bool InScope(IEnumerable<Scope> scopes)
    {
      string workspace = Workspace;
      string collection = Collection;
      if ((IsEntireSite || IsWorkspace || IsCollection) && scopes.Where(s => s.IsEntireSite).Count() > 0) return true;
      if ((IsWorkspace || IsCollection) && scopes.Where(s => s.IsWorkspace && s.Workspace == workspace).Count() > 0) return true;
      if (IsCollection && scopes.Where(s => s.Workspace == workspace && s.Collection == collection).Count() > 0) return true;
      return false;
    }

    //public bool InScope(AppCollection c)
    //{
    //  return Scopes
    //    .Where(s => s.IsEntireSite || (s.IsCollection && c.Id.Workspace == s.Workspace && c.Id.Collection == s.Collection))
    //    .Count() > 0;
    //}
    //public bool IsInScope(AppWorkspace w)
    //{
    //  return Scopes
    //    .Where(s => s.IsEntireSite || (s.IsWorkspace && s.Workspace == (w.Name ?? Atom.DefaultWorkspaceName)))
    //    .Count() > 0;
    //}

    public override bool Equals(object obj)
    {
      if (obj != null && obj.GetType().Equals(this.GetType()))
      {
        return this.ToString().Equals(obj.ToString());
      }
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return this.ToString().GetHashCode();
    }

    public Scope ToAbove()
    {
      if (IsEntireSite) throw new InvalidOperationException("No more levels above.");
      if (IsCollection) return new Scope() { Workspace = Workspace };
      return new Scope();
    }

    public override string ToString()
    {
      return IsEntireSite ? "Entire Site" :
        IsWorkspace ? "Workspace " + Workspace : 
        "Workspace " + Workspace + " - Collection " + Collection;
    }
  }
}
