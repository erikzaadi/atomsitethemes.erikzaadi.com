/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Web.Mvc;

  [Bind(Include="Name,Title,Subtitle")]
  public class AdminSettingsWorkspaceModel : AdminSettingsModel
  {
    //public string WorkspaceName { get; set; }
    public bool IsNew { get { return Workspace == null; } }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
  }
}
