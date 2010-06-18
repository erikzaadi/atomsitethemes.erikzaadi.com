/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;
  using AtomSite.Domain;

  public class AnnotateListWidget : CompositeWidget
  {
    public AnnotateListWidget() :
      base("AnnotateListWidget", "Annotate", "Annotations")
    {
      Description = "This widget shows annotations for the current entry.";
      SupportedScopes = SupportedScopes.Entry;
      AreaHints = new[] { "content" };
    }

    public override bool IsEnabled(BaseModel baseModel, Include include)
    {
      return baseModel.Collection.AnnotationsOn;
    }
  }
}
