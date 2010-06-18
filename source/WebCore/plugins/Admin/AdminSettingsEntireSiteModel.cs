/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.Web.Mvc;

  [Bind(Include = "SiteAddress,ServiceType,DefaultSubdomain,Secure")]
  public class AdminSettingsEntireSiteModel : AdminSettingsModel
  {
    [Required]
    public Uri SiteAddress { get; set; }

    [Required]
    public string ServiceType { get; set; }

    [RegularExpression("^[a-zA-Z][a-zA-Z0-9\\-]*$",
      ErrorMessage="The default subdomain must start with a letter and only contain valid characters.")]
    public string DefaultSubdomain { get; set; }

    public bool? Secure { get; set; }
  }
}
