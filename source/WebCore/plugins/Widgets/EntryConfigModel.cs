/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using AtomSite.Domain;

  public class EntryConfigModel : ConfigModel
  {
    public string SelectedTitle { get; set; }
    public string SelectedId { get; set; }
    public string CurrentWorkspace { get; set; }
    public string CurrentCollection { get; set; }
    public int? Page { get; set; }
    public IEnumerable<Scope> Scopes { get; set; }
    public IPagedList<AtomEntry> Entries { get; set; }
    public Id GetSelectedId()
    {
      if (SelectedId != null) return new Uri(SelectedId);
      return null;
    }
  }
}