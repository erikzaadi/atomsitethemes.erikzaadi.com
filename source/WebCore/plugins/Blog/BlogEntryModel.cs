/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Domain;
  
  public class BlogEntryModel : EntryModel
  {
    //public AtomEntry Entry { get; set; }

    public bool ShowApproveAll()
    {
      return AuthorizeService.IsAuthorized(User, Scope, AuthAction.ApproveAnnotation);
    }

    public bool CanDelete()
    {
      return AuthorizeService.IsAuthorized(User, Scope, AuthAction.DeleteEntryOrMedia);
    }
    public bool CanEdit()
    {
      return AuthorizeService.IsAuthorized(User, Scope, AuthAction.UpdateEntryOrMedia);
    }
  }
}
