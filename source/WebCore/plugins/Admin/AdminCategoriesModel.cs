/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using AtomSite.Domain;
  public class AdminCategoriesModel : AdminSettingsModel
  {
    public AdminCategoriesModel()
    {
      //Categories = new List<AtomCategory>();
      Metrics = new Dictionary<AtomCategory, int>();
      Schemes = new Dictionary<string, string>();
    }
    public IDictionary<string, string> Schemes { get; set; }
    public AppCategories Categories { get; set; }
    public IDictionary<AtomCategory, int> Metrics { get; set; }
    public bool IsSelected(string scheme)
    {
      string s = Categories.Scheme == null ? "Default" : Categories.Scheme.ToString();
      return scheme.Equals(s);
    }
  }
}
