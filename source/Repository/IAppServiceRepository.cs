/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository
{
  using AtomSite.Domain;
	
  public interface IAppServiceRepository
	{
		AppService GetService();
		AppService UpdateService(AppService service);
	}
}
