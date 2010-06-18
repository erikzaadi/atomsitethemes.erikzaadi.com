/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using AtomSite.Domain;

  public class WizardBasicSettingsModel : PageModel
  {
    public string Address { get; set; }
    public string Owner { get; set; }
    public int Year { get; set; }
    public string WorkspaceSubtitle { get; set; }
    public string BlogTitle { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
  }
}
