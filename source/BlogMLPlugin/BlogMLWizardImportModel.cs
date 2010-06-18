/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.BlogMLPlugin
{
  using System.Web;
  using AtomSite.WebCore;

  public class BlogMLWizardImportModel : PageModel
  {
    public string BlogName { get; set; }
    public string PagesName { get; set; }
    public string MediaName { get; set; }
    public string Owner { get; set; }
    public int Year { get; set; }
    public HttpPostedFileBase BlogMLFile { get; set; }
    public string Error { get; set; }
  }
}
