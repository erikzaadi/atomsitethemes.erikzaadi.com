/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Domain;
  using System.Web.UI;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;

  public class BaseModel
  {
    public User User { get; set; }
    public ILogService Logger { get; set; }
    public IAuthorizeService AuthorizeService { get; set; }
    public AppService Service { get; set; }
    public AppWorkspace Workspace { get; set; }
    public AppCollection Collection { get; set; }
    public Scope Scope { get; set; }
    public Id EntryId { get; set; }
  }

  public class ConfigModel : BaseModel
  {
    public string IncludePath { get; set; }
  }
}
