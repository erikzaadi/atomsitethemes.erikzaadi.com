/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;
  using AtomSite.Domain;
  using System.Collections;

  public class AdminUserModel : AdminModel
  {
    public bool IsNew { get { return string.IsNullOrEmpty(UserId); } }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string Ids { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Uri { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Roles { get; set; }
  }
}
