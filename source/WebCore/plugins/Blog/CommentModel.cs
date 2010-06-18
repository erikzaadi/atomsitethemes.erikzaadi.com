/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Linq;
  using AtomSite.Domain;

  public class CommentModel : BaseModel
  {
    public AtomEntry Comment { get; set; }

    public bool IsOwner()
    {
        bool isOwner = false;
        Service.GetPeopleInScope(Comment.Id.ToScope()).ToList().ForEach(p =>
        {
          if (Comment.People.Select(s => s.Id).Contains(p)) isOwner = true;
        });
        return isOwner;
    }
    
    public bool CanApprove()
    {
      return AuthorizeService.IsAuthorized(User, Scope, AuthAction.ApproveAnnotation);
    }

    public bool CanDelete()
    {
      return AuthorizeService.IsAuthorized(User, Scope, AuthAction.DeleteEntryOrMedia);
    }
  }
}
