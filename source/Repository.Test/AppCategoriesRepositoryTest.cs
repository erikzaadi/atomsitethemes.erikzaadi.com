using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;
using AtomSite.Utils;
using AtomSite.Repository;
using AtomSite.Repository.File;
using AtomSite.Repository.Mock;

namespace Repository.Test
{
    /// <summary>
    /// Run tests against an IAppServiceRepository
    /// </summary>
    public abstract class BaseAppCategoriesRepositoryTest
    {
        protected abstract IAppCategoriesRepository GetRepository();

        #region tests

        [TestMethod]
        public void AppCategoriesRepositoryCreateNotNullTest()
        {
            IAppCategoriesRepository repository  = GetRepository();

            Assert.IsNotNull(repository);
        }

        [TestMethod]
        public void AppCategoriesNotFoundTest()
        {
            IAppCategoriesRepository repository = GetRepository();

            // generate an id that will not be found
            Id appCollectionId = new Id("test", "test" + Guid.NewGuid());
            AppCategories externalCategories = new AppCategories();

            AppCategories categories;

            try
            {
                categories = repository.GetCategories(appCollectionId, externalCategories);
            }
            catch (Exception)
            {
                categories = null;
            }

            Assert.IsNull(categories);
        }

        [TestMethod]
        public void AppCategoriesLifeCycleTest()
        {
            IAppCategoriesRepository repository = GetRepository();

            Id appCollectionId = new Id("test", "test" + Guid.NewGuid());

            AppCategories testCategories = new AppCategories();
            testCategories.Fixed = true;

            // create it
            AppCategories returned = repository.CreateCategories(appCollectionId, testCategories);
            Assert.IsNotNull(returned);
            Assert.IsTrue(returned.Fixed.HasValue);
            Assert.IsTrue(returned.Fixed.Value);

            // try to load it again
            AppCategories externalCategories = new AppCategories();
            AppCategories loadedCategories = repository.GetCategories(appCollectionId, externalCategories);
            Assert.IsNotNull(loadedCategories);
        }

        #endregion
    }

    /// <summary>
    /// run the tests against the file repository
    /// </summary>
    [TestClass]
    public class FileAppCategoriesRepositoryTest : BaseAppCategoriesRepositoryTest
    {
        static string storePath;
        static IAppServiceRepository svcRepo;

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            storePath = FileHelper.CombineTrackbacksInPath(
                Path.Combine(testContext.TestDir, @"..\..\Web\"));
            svcRepo = new FileAppServiceRepository(storePath);
        }
        
        protected override IAppCategoriesRepository GetRepository()
        {

          return new FileAppCategoriesRepository(storePath, svcRepo);
        }
    }

    ///// <summary>
    ///// run the tests against the sql repository
    ///// </summary>
    //[TestClass]
    //public class SqlAppCategoriesRepositoryTest: BaseAppCategoriesRepositoryTest
    //{
    //    protected override IAppCategoriesRepository GetRepository()
    //    {
    //        return new SqlAppCategoriesRepository(new AtomDataClassesDataContext(""));
    //    }
    //}

    /// <summary>
    /// run the tests against the mock repository
    /// </summary>
    [TestClass]
    public class MockAppCategoriesRepositoryTest : BaseAppCategoriesRepositoryTest
    {
        // use the same mock repository instance throughout the test
        private readonly IAppCategoriesRepository appCategoriesRepository = new MockAppCategoriesRepository();

        protected override IAppCategoriesRepository GetRepository()
        {
            return appCategoriesRepository;
        }
    }
}