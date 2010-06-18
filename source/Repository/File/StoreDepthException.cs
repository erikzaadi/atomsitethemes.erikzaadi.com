/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository.File
{
  using AtomSite.Domain;

  public class StoreDepthException : BaseException
  {
    public StoreDepthException(string depth, string collectionName)
      : base("The storage depth of \"{0}\" is not supported on the \"{1}\" collection.", depth, collectionName) { }
  }
}
