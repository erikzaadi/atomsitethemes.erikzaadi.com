/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Diagnostics;
  using System.IO;
  using AtomSite.Repository;
  using AtomSite.Repository.File;
  using StructureMap.Configuration.DSL;

  public class FileRegistry : Registry
  {
    public FileRegistry(string appPath)
    {
      TraceSource s = new TraceSource("AtomSite");
      For<ILogService>().Singleton().Add(new TraceSourceLogService(s));

      For<IThemeService>().Singleton().Add<ThemeService>().Ctor<string>("appPath").Is(appPath);
      For<IAssetService>().Singleton().Add<AssetService>();

      For<IAppCategoriesRepository>().Add<FileAppCategoriesRepository>().Ctor<string>("storePath").Is(appPath);
      For<IAppServiceRepository>().Add<FileAppServiceRepository>().Ctor<string>("storePath").Is(appPath);
      For<IAtomEntryRepository>().Add<FileAtomEntryRepository>().Ctor<string>("storePath").Is(appPath);
      For<IMediaRepository>().Add<FileMediaRepository>().Ctor<string>("storePath").Is(appPath);
      For<IUserRepository>().Add<FileUserRepository>().Ctor<string>("storePath").Is(Path.Combine(appPath, FileUserRepository.DefaultUsersFileName));

      For<IRouteService>().HybridHttpOrThreadLocalScoped().Add<RouteService>();
      //IncludeConfigurationFromConfigFile = true; //TODO: doesn't work
    }
  }
}
