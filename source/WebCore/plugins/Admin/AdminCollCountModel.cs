using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AtomSite.Domain;

namespace AtomSite.WebCore
{
  public class AdminCollCountModel : AdminModel
  {
    public IEnumerable<CollectionCount> Collections;
  }

  public class CollectionCount
  {
    public AppCollection Collection { get; set; }
    public int Count { get; set; }
  }
}
