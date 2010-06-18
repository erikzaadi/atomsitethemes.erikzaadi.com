/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.BlogMLPlugin
{
  using AtomSite.Domain;
  using AtomSite.WebCore;
  using BlogML.Xml;

  public interface IBlogMLService
  {
    void Import(Id entryCollectionId, Id pageCollectionId, Id mediaCollectionId, ImportMode mode, BlogMLBlog blog);
    Progress GetProgress();
    BlogMLBlog Export(Id entryCollectionId, Id pageCollectionId, Id mediaCollectionId);
  }
}
