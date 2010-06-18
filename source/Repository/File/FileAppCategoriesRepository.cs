/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository.File
{
  using System;
  using System.IO;
  using System.Web.Hosting;
  using AtomSite.Domain;

  /// <summary>
  /// This class stores external categories documents to the file system.
  /// </summary>
  public class FileAppCategoriesRepository : IAppCategoriesRepository
  {
    readonly PathResolver pathResolver;

    public FileAppCategoriesRepository(IAppServiceRepository svcRepo)
      : this(HostingEnvironment.ApplicationPhysicalPath, svcRepo) { }

    public FileAppCategoriesRepository(string storePath, IAppServiceRepository svcRepo)
    {
      pathResolver = new PathResolver(storePath, svcRepo.GetService());
    }

    public AppCategories GetCategories(Id collectionId, AppCategories externalCats)
    {
      string path = pathResolver.GetCategoriesPath(collectionId, externalCats);
      try
      {
        return AppCategories.Load(path);//TODO: cache
      }
      catch (FileNotFoundException)
      {
        throw new ResourceNotFoundException("categories document", path);
      }
    }

    public AppCategories CreateCategories(Id collectionId, AppCategories categories)
    {
      throw new NotImplementedException();
    }

    public AppCategories UpdateCategories(Id collectionId, AppCategories externalCats, AppCategories categories)
    {
      string path = pathResolver.GetCategoriesPath(collectionId, externalCats);
      categories.Save(path); //TODO: cache
      return categories;
    }

    public void DeleteCategories(Id collectionId, AppCategories externalCategories)
    {
      throw new NotImplementedException();
    }

  }
}
