/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Domain;

  public interface ICleanContentService
  {
    void CleanContentTrusted(AtomContent content);
    void CleanContentFully(AtomContent content);
  }

  public class FailedToCleanContentException : BaseException
  {
    public FailedToCleanContentException(string error) :
      base("Could not clean content due to: {0}", error) { }
  }
}
