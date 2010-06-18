/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Web.Mvc;
  using AtomSite.Domain;

  public class AdminEntriesModel : AdminModel
  {
    public IPagedList<AtomEntry> Entries { get; set; }
    public string Category { get; set; }
    public string Search { get; set; }
    public string Filter { get; set; }
    public int PublishedCount { get; set; }
    public int DraftCount { get; set; }
    public int UnapprovedCount { get; set; }
    public int AllCount { get; set; }

    public IEnumerable<SelectListItem> GetCategorySelectList()
    {
      if (Scope.IsCollection)
      {
        return Collection.AllCategories.Distinct()
            .Select(c => new SelectListItem() { Text = c.ToString(), Value = c.Term, Selected = c.Term == Category });
      }
      else if (Scope.IsWorkspace)
      {
        return Workspace.Collections.SelectMany(c => c.AllCategories).Distinct()
            .Select(c => new SelectListItem() { Text = c.ToString(), Value = c.Term, Selected = c.Term == Category });
      }
      return Service.Workspaces.SelectMany(w => w.Collections).SelectMany(c => c.AllCategories).Distinct()
            .Select(c => new SelectListItem() { Text = c.ToString(), Value = c.Term, Selected = c.Term == Category });
    }
  }
}
