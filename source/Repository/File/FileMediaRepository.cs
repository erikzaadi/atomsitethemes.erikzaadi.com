/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository.File
{
  using System.IO;
  using System.Web.Hosting;
  using AtomSite.Domain;
  using AtomSite.Utils;

  /// <summary>
  /// This class stores media as normal files on the filesystem.  It also
  /// creates and updates media link entries as needed.
  /// </summary>
  public class FileMediaRepository : IMediaRepository
  {
    readonly PathResolver pathResolver;
    readonly IAtomEntryRepository atomEntryRepository;

    public FileMediaRepository(IAppServiceRepository svcRepo, IAtomEntryRepository entryRepo) :
      this(HostingEnvironment.ApplicationPhysicalPath, svcRepo, entryRepo) { }

    public FileMediaRepository(string storePath, IAppServiceRepository svcRepo, IAtomEntryRepository entryRepo)
    {
      pathResolver = new PathResolver(storePath, svcRepo.GetService());
      atomEntryRepository = entryRepo;
    }

    public Stream GetMedia(AtomEntry mediaLinkEntry)
    {
      string path = pathResolver.GetMediaPath(mediaLinkEntry.Id, mediaLinkEntry.Content.Type);
      try
      {
        return System.IO.File.OpenRead(path);
      }
      catch (FileNotFoundException)
      {
        throw new ResourceNotFoundException("media", mediaLinkEntry.Id.ToString());
      }
    }

    public string GetMediaEtag(AtomEntry mediaLinkEntry)
    {
      string path = pathResolver.GetMediaPath(mediaLinkEntry.Id, mediaLinkEntry.Content.Type);
      try
      {
        //contentLength = (int)(new FileInfo(path).Length);
        return FileHelper.ComputeMD5Sum(path);
      }
      catch (FileNotFoundException)
      {
        throw new ResourceNotFoundException("media", mediaLinkEntry.Id.ToString());
      }
    }

    public void DeleteMedia(AtomEntry mediaLinkEntry)
    {
      //TODO: transaction
      atomEntryRepository.DeleteEntry(mediaLinkEntry.Id);
      string path = pathResolver.GetMediaPath(mediaLinkEntry.Id, mediaLinkEntry.Content.Type);
      //TODO: resource not found exception
      System.IO.File.Delete(path);
      AtomSite.Utils.FileHelper.RemoveEmptyPaths(pathResolver.GetCollectionPath(mediaLinkEntry.Id), true);
    }

    public AtomEntry CreateMedia(AtomEntry entry, Stream stream)
    {
      //TODO: transaction

      string path = pathResolver.GetMediaLinkEntryPath(entry.Id);
      Id id = entry.Id;
      int i = 1;
      while (System.IO.File.Exists(path))
      {
        id = new Id(id.Owner, id.Date, id.Collection, entry.Id.EntryPath + i++);
        path = pathResolver.GetEntryPath(id);
      }
      entry.Id = id;

      path = pathResolver.GetMediaPath(entry.Id, entry.Content.Type);
      FileHelper.WriteStream(stream, path);

      //create & return the media link entry
      return atomEntryRepository.CreateEntry(entry);
    }

    public AtomEntry UpdateMedia(AtomEntry mediaLinkEntry, Stream stream)
    {
      //TODO: transaction
      atomEntryRepository.UpdateEntry(mediaLinkEntry);
      string path = pathResolver.GetMediaPath(mediaLinkEntry.Id, mediaLinkEntry.Content.Type);
      //TODO: resource not found exception
      FileHelper.WriteStream(stream, path);
      return mediaLinkEntry;
    }
  }
}
