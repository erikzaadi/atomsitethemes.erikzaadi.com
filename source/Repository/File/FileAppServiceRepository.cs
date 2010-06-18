/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository.File
{
  using System.IO;
  using System.Web.Hosting;
  using AtomSite.Domain;

  /// <summary>
  /// This class stores service documents to the file system.
  /// </summary>
  public class FileAppServiceRepository : IAppServiceRepository
  {
    static readonly XmlCache<AppService> services = new XmlCache<AppService>();

    readonly PathResolver pathResolver;

    public FileAppServiceRepository()
      : this(HostingEnvironment.ApplicationPhysicalPath)
    { }

    public FileAppServiceRepository(string storePath)
    {
      pathResolver = new PathResolver(storePath);
    }

    public AppService UpdateService(AppService service)
    {
      services.Put(pathResolver.ServiceDocPath, service);
      return service;
    }

    public AppService GetService()
    {
      try
      {
        //TODO: why the cast needed?
        return (AppService)services.Get(pathResolver.ServiceDocPath);
      }
      catch (FileNotFoundException)
      {
        throw new ResourceNotFoundException("service document", pathResolver.ServiceDocPath);
      }
    }
  }
}
