/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Domain;

  public class AdminEntryModel : AdminModel
  {
    public AtomEntry Entry { get; set; }
  }

  public class AdminEditEntryModel : AdminModel
  {
		public virtual string Id { get; set; }
		public virtual string Title { get; set; }
		public virtual string Slug { get; set; }
		public virtual string Content { get; set; }
		public virtual string Summary { get; set; }
		public virtual bool? Draft { get; set; }
		public virtual bool? Approved { get; set; }
		public virtual string PublishedDate { get; set; }
		public virtual string PublishedTime { get; set; }
		public virtual string Submit { get; set; }
		public virtual string[] Categories { get; set; }
		public virtual bool? AllowAnnotations { get; set; }
  }
}
