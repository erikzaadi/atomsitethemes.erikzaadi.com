/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using AtomSite.Domain;

  public class FeedConfigModel : ConfigModel
  {
    public bool HasTitle { get; set; }
    public string Title { get; set; }
    public bool HasCount { get; set; }
    public int? Count { get; set; }
    public string SelectedId { get; set; }
    public IEnumerable<AppCollection> Collections { get; set; }
    public Id GetSelectedId()
    {
      if (SelectedId != null) return new Uri(SelectedId);
      return null;
    }
  }
}