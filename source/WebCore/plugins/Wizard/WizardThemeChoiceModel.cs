/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using AtomSite.Domain;

  public class WizardThemeChoiceModel : PageModel
  {
    public string SelectedTheme { get; set; }
    public IEnumerable<Theme> Themes { get; set; }
    public string Theme { get; set; }
  }
}
