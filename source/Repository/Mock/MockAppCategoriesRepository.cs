using System;
using AtomSite.Domain;

namespace AtomSite.Repository.Mock
{
    /// <summary>
    /// Mock Implementation of IAppCategoriesRepository
    ///  - data isn't put anywhere, just stored in memory
    /// </summary>
    public class MockAppCategoriesRepository : IAppCategoriesRepository
    {

        public AppCategories GetCategories(Id collectionId, AppCategories externalCategories)
        {
            throw new NotImplementedException();
        }

        public AppCategories CreateCategories(Id collectionId, AppCategories categories)
        {
            throw new NotImplementedException();
        }

        public AppCategories UpdateCategories(Id collectionId, AppCategories externalCategories, AppCategories categories)
        {
            throw new NotImplementedException();
        }

        public void DeleteCategories(Id collectionId, AppCategories externalCategories)
        {
            throw new NotImplementedException();
        }
    }
}
