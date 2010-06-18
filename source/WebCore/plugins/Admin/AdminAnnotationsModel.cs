/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Domain;
  public class AdminAnnotationsModel : AdminModel
  {
      public IPagedList<AtomEntry> Annotations { get; set; }
      public string Filter { get; set; }
      public string Search { get; set; }
    public int PublishedCount { get; set; }
    public int SpamCount { get; set; }
    public int UnapprovedCount { get; set; }
    public int AllCount { get; set; }
  }
}
