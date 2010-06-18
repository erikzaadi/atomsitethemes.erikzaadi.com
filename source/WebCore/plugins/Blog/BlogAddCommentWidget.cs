/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Domain;

  public class BlogAddCommentWidget : CompositeWidget
  {
    public BlogAddCommentWidget() :
      base("BlogAddCommentWidget", "Blog", "AddComment")
    {
      Description = "This widget shows a form that users can add new comments to the current entry.";
      AddAsset("BlogComment.js");
      AddAsset("BlogComment.css");
      Areas = new[] { "commentator" };
      TailScript += "$(function() { $('#addcomment .error').hide(); });";
      SupportedScopes = SupportedScopes.Entry;
      AreaHints = new[] { "content" };
    }

    public override bool IsEnabled(BaseModel baseModel, Include include)
    {
      BlogAppCollection c = new BlogAppCollection(baseModel.Collection);
      return (c.AnnotationsOn && c.BloggingOn &&
        baseModel.AuthorizeService.IsAuthorized(baseModel.User, baseModel.Scope, AuthAction.Annotate));
    }
  }
}
