/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.BlogMLPlugin
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Web.Mvc;
  using AtomSite.WebCore;

  public class BlogMLModel : AdminModel
  {
    public BlogMLModel()
    {
      ImportMode = AtomSite.Plugins.BlogMLPlugin.ImportMode.Merge.ToString();
    }
     
    public string EntryCollectionId { get; set; }
    public string PagesCollectionId { get; set; }
    public string MediaCollectionId { get; set; }
    public string ImportMode { get; set; }

    public IEnumerable<SelectListItem> EntrySelections
    {
      get
      {
        var scopes = AuthorizeService.GetScopes(User);
        return Service.Workspaces.SelectMany(w => w.Collections).Where(c => c.AcceptsEntries && scopes.Contains(c.Id.ToScope()))
          .Select(c => new SelectListItem()
          {
            Value = c.Id.ToString(),
            Text = c.Id.ToScope().Workspace + " - " + c.Id.ToScope().Collection,
            Selected = c.Id.ToString() == EntryCollectionId
          });
      }
    }

    public IEnumerable<SelectListItem> PagesSelections
    {
      get
      {
        var selections = EntrySelections.ToList();
        foreach (var sli in selections)
        {
          sli.Selected = sli.Value == PagesCollectionId;
        }

        //don't select same collection as in EntrySelections
        //TODO: add logic to find a collection not "dated"
        if (selections.Count() > 1 && string.IsNullOrEmpty(PagesCollectionId))
        {
          foreach (var i in selections) i.Selected = !i.Selected;
        }
        selections.Add(new SelectListItem() { Value = string.Empty, Text = "(ignore)" });
        return selections;
      }
    }

    public IEnumerable<SelectListItem> MediaSelections
    {
      get
      {
        var scopes = AuthorizeService.GetScopes(User);
        var selections = Service.Workspaces.SelectMany(w => w.Collections).Where(c => c.AcceptsMedia && scopes.Contains(c.Id.ToScope()))
          .Select(c => new SelectListItem()
          {
            Value = c.Id.ToString(),
            Text = c.Id.ToScope().Workspace + " - " + c.Id.ToScope().Collection,
            Selected = c.Id.ToString() == MediaCollectionId
          }).ToList();
        selections.Add(new SelectListItem() { Value = string.Empty, Text = "(ignore)" });
        return selections;
      }
    }

    public IEnumerable<SelectListItem> ImportModeSelections 
    { 
      get 
      {
        yield return new SelectListItem() { Text = "Merge", Value = "Merge", Selected = ("Merge" == ImportMode) };
        yield return new SelectListItem() { Text = "Overwrite", Value = "Overwrite", Selected = ("Overwrite" == ImportMode)};
      }
    } 
  }
}
