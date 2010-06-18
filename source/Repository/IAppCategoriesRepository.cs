/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository
{
  using AtomSite.Domain;

	public interface IAppCategoriesRepository
	{
		AppCategories GetCategories(Id collectionId, AppCategories externalCategories);
		AppCategories CreateCategories(Id collectionId, AppCategories categories);
		AppCategories UpdateCategories(Id collectionId, AppCategories externalCategories, AppCategories categories);
		void DeleteCategories(Id collectionId, AppCategories externalCategories);
	}
}
