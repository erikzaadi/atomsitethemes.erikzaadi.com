/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;
  using AtomSite.Domain;

  public class BlogCommentsWidget : CompositeWidget
  {
    public BlogCommentsWidget() :
      base("BlogCommentsWidget", "Blog", "Comments")
    {
      Description = "This widget shows comments (annotations) for the current blog post.";
      AddAsset("BlogComment.js");
      AddAsset("BlogComment.css");
      SupportedScopes = SupportedScopes.Entry;
    }

    public override bool IsEnabled(BaseModel baseModel, Include include)
    {
      BlogAppCollection c = new BlogAppCollection(baseModel.Collection);
      return (c.AnnotationsOn && c.BloggingOn);
    }
  }
}
