/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;
  using AtomSite.Domain;
  using AtomSite.Repository;

  //[Obsolete]
  public class BlogEntrySimpleWidget : EntryWidget
  {
    public BlogEntrySimpleWidget() : base()
    {
      Name = "BlogEntrySimpleWidget";
      Description = "This widget shows the summary of blog post.";
      OnValidate = (i) => new IdInclude(i).Id != null;
      AreaHints = new[] { "sidetop", "sidemid", "sidebot" };
    }

    public override bool IsEnabled(BaseModel baseModel, Include include)
    {
      return new BlogAppCollection(baseModel.Collection).BloggingOn;
    }
  }
}
