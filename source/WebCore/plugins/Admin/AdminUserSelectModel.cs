/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;
  using AtomSite.Domain;

  public class AdminUserSelectModel : BaseModel
  {
    public string SelectionTitle { get; set; }
    public IPagedList<User> Users { get; set; }
    public Func<User, string> GetPostHref { get; set; }
    public string CancelHref { get; set; }
  }
}
