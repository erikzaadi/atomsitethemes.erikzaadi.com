using AtomSite.Domain;
using AtomSite.Repository.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Repository.Mock;

using AtomSite.Repository;

namespace Repository.Test
{
    /// <summary>
    /// Run tests against an IMediaRepository
    /// </summary>
    public abstract class BaseMediaRepositoryTest
    {
        #region proctected

        protected abstract IMediaRepository GetRepository();

        #endregion

        #region public

        public TestContext TestContext { get; set; }

        #endregion

        #region tests

        [TestMethod]
        public void CreateMediaRepositoryRepositoryNotNullTest()
        {
            IMediaRepository repository = GetRepository();

            Assert.IsNotNull(repository);
        }

        #endregion
    }

    ///// <summary>
    ///// run the tests against the sql repository
    ///// </summary>
    //[TestClass]
    //public class SqlMediaRepositoryTest : BaseMediaRepositoryTest
    //{
    //    protected override IMediaRepository GetRepository()
    //    {
    //        return new SqlMediaRepository(new AtomDataClassesDataContext());
    //    }
    //}

    /// <summary>
    /// run the tests against the mock repository
    /// </summary>
    [TestClass]
    public class MockMediaRepositoryTest : BaseMediaRepositoryTest
    {
        private readonly MockMediaRepository mockMediaRepository = new MockMediaRepository();

        protected override IMediaRepository GetRepository()
        {
            return mockMediaRepository;
        }
    }

    /// <summary>
    /// Run the tests against a file repository
    /// </summary>
    [TestClass]
    public class FileMediaRepositoryTest: BaseMediaRepositoryTest
    {
        protected override IMediaRepository GetRepository()
        {
            FileRepositoryHelper helper = new FileRepositoryHelper(TestContext);
            helper.CheckAppService();
            IAppServiceRepository appServiceRepository = new FileAppServiceRepository(helper.TestFilesPath());

            AppService appService = appServiceRepository.GetService();
            IAtomEntryRepository entryRepository = new FileAtomEntryRepository(helper.TestFilesPath(), appServiceRepository);


            return new FileMediaRepository(helper.MediaPath(), appServiceRepository, entryRepository);
        }
    }

}
