/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using AtomSite.Domain;

  public class CategoriesModel : BaseModel
  {
    public IEnumerable<AtomCategory> Categories { get; set; }
    public Id Id { get; set; }
  }
}
