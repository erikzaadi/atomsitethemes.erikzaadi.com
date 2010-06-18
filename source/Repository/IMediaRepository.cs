/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository
{
  using System.IO;
  using AtomSite.Domain;

  public interface IMediaRepository
  {
    Stream GetMedia(AtomEntry mediaLinkEntry);
    string GetMediaEtag(AtomEntry mediaLinkEntry);
    void DeleteMedia(AtomEntry mediaLinkEntry);
    AtomEntry CreateMedia(AtomEntry mediaLinkEntry, Stream stream);
    AtomEntry UpdateMedia(AtomEntry mediaLinkEntry, Stream stream);
  }
}
