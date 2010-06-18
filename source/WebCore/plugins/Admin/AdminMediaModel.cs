/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Web.Mvc;
  using AtomSite.Domain;

  public class AdminMediaModel : AdminModel
  {
    public bool IsNew { get { return Entry.Id == null; } }
    public AtomEntry Entry { get; set; }
  }

  public class AdminMediaModelBinder : DefaultModelBinder
  {

  }
}
