/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Domain;

  public class AddCommentModel : BaseModel
  {
    public AtomAuthor AnonAuthor { get; set; }
    public AnnotationState AnnotationState { get; set; }
  }
}
