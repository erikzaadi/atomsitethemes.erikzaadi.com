/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;

  public class AdminWidgetSelectModel : AdminModel
  {
    public string Error { get; set; }
    public string SelectionTitle { get; set; }
    public IEnumerable<WidgetSelect> WidgetSelections { get; set; }
    public string CancelHref { get; set; }
  }

  public class WidgetSelect
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public string PostHref { get; set; }
    public string Icon { get; set; }
    public int? ScopeFlags { get; set; }
    public bool IsHint { get; set; }
    public int? ScopeMatch { get; set; }
  }
}