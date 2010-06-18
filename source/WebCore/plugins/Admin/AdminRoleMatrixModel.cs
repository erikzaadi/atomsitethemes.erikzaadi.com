/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using System.Linq;
  using AtomSite.Domain;

  public class AdminRoleMatrixModel : AdminModel
  {
    public RoleMatrix RoleMatrix { get; set; }
  }
}
