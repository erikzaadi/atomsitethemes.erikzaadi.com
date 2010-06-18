using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Repository;
using AtomSite.Repository.Mock;
using AtomSite.Repository.File;

namespace Repository.Test
{
    /// <summary>
    /// Test saving and loading entries with data attached to them
    /// </summary>
    public abstract class BaseAtomEntryLinkedDataTest : BaseAtomEntryTest
    {
        #region private

        #endregion

        #region properties

        public TestContext TestContext { get; set; }

        #endregion

        #region tests

        [TestMethod]
        public void EntryWithPersonSaveAndLoadTest()
        {
            MakeTestCollections(false);

            // make an entry with an attached person
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);
            AtomSite.Domain.AtomPerson newPerson = new AtomSite.Domain.AtomPerson
            {
                Email = "fred@fred" + Guid.NewGuid() + ".com",
                Name = "TestAPerson" + Guid.NewGuid()
            };

            entry.Authors = new List<AtomSite.Domain.AtomPerson> { newPerson };

            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // load it again and check that the person is still there
            IAtomEntryRepository repository2 = GetEntryRepository();
            AtomSite.Domain.AtomEntry loadedEntry = repository2.GetEntry(entry.Id);

            Assert.AreEqual(1, loadedEntry.Authors.Count());

            AtomSite.Domain.AtomPerson loadedPerson = loadedEntry.Authors.First();

            DataTester.AssertPersonsEqual(newPerson, loadedPerson);
        }

        [TestMethod]
        public void EntryUpdateWithPersonSaveAndLoadTest()
        {
            MakeTestCollections(false);

            // make an entry with an attached person
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);

            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            IAtomEntryRepository repository2 = GetEntryRepository();
            AtomSite.Domain.AtomEntry updatedEntry = repository2.GetEntry(entry.Id);

            AtomSite.Domain.AtomPerson newPerson = new AtomSite.Domain.AtomPerson
            {
                Email = "fred@fred" + Guid.NewGuid() + ".com",
                Name = "TestPerson" + Guid.NewGuid()
            };

            updatedEntry.Authors = new List<AtomSite.Domain.AtomPerson> { newPerson };

            // update the entry
            repository2.UpdateEntry(updatedEntry);

            // load it again and check that the person is still there
            IAtomEntryRepository repository3 = GetEntryRepository();
            AtomSite.Domain.AtomEntry loadedEntry = repository3.GetEntry(entry.Id);

            Assert.AreEqual(1, loadedEntry.Authors.Count());

            AtomSite.Domain.AtomPerson loadedPerson = loadedEntry.Authors.First();

            DataTester.AssertPersonsEqual(newPerson, loadedPerson);        
        }

        [TestMethod]
        public void EntryUpdateRemovePersonTest()
        {
            MakeTestCollections(false);

            // make an entry with two attached persons
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);

            AtomSite.Domain.AtomPerson newPerson1 = new AtomSite.Domain.AtomPerson
            {
                Email = "fred@fred" + Guid.NewGuid() + ".com",
                Name = "TestPerson" + Guid.NewGuid()
            };

            AtomSite.Domain.AtomPerson newPerson2 = new AtomSite.Domain.AtomPerson
            {
                Email = "bob@bob" + Guid.NewGuid() + ".com",
                Name = "TestPerson" + Guid.NewGuid()
            };

            entry.Authors = new List<AtomSite.Domain.AtomPerson> { newPerson1, newPerson2 };

            // save the entry
            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // load it again
            IAtomEntryRepository repository2 = GetEntryRepository();
            AtomSite.Domain.AtomEntry loadedEntry = repository2.GetEntry(entry.Id);

            Assert.AreEqual(2, loadedEntry.Authors.Count());

            // remove a person
            List<AtomSite.Domain.AtomPerson> persons = new List<AtomSite.Domain.AtomPerson>(loadedEntry.Authors);
            persons.RemoveAt(persons.Count - 1);

            loadedEntry.Authors = persons;

            repository2.UpdateEntry(loadedEntry);

            // load it again and check that a person has been removed
            IAtomEntryRepository repository3 = GetEntryRepository();
            AtomSite.Domain.AtomEntry reloadedEntry = repository3.GetEntry(entry.Id);

            Assert.AreEqual(1, reloadedEntry.Authors.Count());
        }

        [TestMethod]
        public void EntryWithCategorySaveAndLoadTest()
        {
            MakeTestCollections(false);

            // make an entry with an attached person
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);
            AtomSite.Domain.AtomCategory entryCategory = GetCategory(true);

            entry.Categories = entryCategory.InList();

            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // load it again and check that the category is still there
            IAtomEntryRepository repository2 = GetEntryRepository();
            AtomSite.Domain.AtomEntry loadedEntry = repository2.GetEntry(entry.Id);

            Assert.AreEqual(1, loadedEntry.Categories.Count());

            AtomSite.Domain.AtomCategory loadedCategory = loadedEntry.Categories.First();

            DataTester.AssertCategoriesEqual(entryCategory, loadedCategory);
        }

        [TestMethod]
        public void EntryUpdateRemoveCategoryTest()
        {
            MakeTestCollections(false);

            // make an entry with two attached categories
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);

            AtomSite.Domain.AtomCategory newCategory1 = GetCategory(true);
            AtomSite.Domain.AtomCategory newCategory2 = GetCategory(false);

            entry.Categories = new List<AtomSite.Domain.AtomCategory> { newCategory1, newCategory2 };

            // save the entry
            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // load it again
            IAtomEntryRepository repository2 = GetEntryRepository();
            AtomSite.Domain.AtomEntry loadedEntry = repository2.GetEntry(entry.Id);

            Assert.AreEqual(2, loadedEntry.Categories.Count());

            // remove a category
            loadedEntry.Categories = loadedEntry.Categories.RemoveAt(1);

            repository2.UpdateEntry(loadedEntry);

            // load it again and check that a category has been removed
            IAtomEntryRepository repository3 = GetEntryRepository();
            AtomSite.Domain.AtomEntry reloadedEntry = repository3.GetEntry(entry.Id);

            Assert.AreEqual(1, reloadedEntry.Categories.Count());
        }

        [TestMethod]
        public void EntryWithLinksSaveAndLoadTest()
        {
            MakeTestCollections(false);

            // make an entry with an attached link
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);
            AtomSite.Domain.AtomLink newLink = new AtomSite.Domain.AtomLink
            {
                Href = new Uri("http://www.foo.com"),
                Lang = "EN",
                HrefLang = "EN",
                Title = "A foo website",
                Type = "link",
                Updated = DateTimeOffset.Now
            };

            entry.Links = newLink.InList();

            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // load it again and check that the link is still there
            IAtomEntryRepository repository2 = GetEntryRepository();
            AtomSite.Domain.AtomEntry loadedEntry = repository2.GetEntry(entry.Id);

            Assert.AreEqual(1, loadedEntry.Links.Count());

            AtomSite.Domain.AtomLink loadedLink = loadedEntry.Links.First();

            DataTester.AssertLinksEqual(newLink, loadedLink);
        }

        [TestMethod]
        public void EntryWithRatersSaveAndLoadTest()
        {
            MakeTestCollections(false);

            // make an entry with an attached person
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);
            const string rater = "127.0.0.1";

            entry.Raters = rater.InList();

            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // load it again and check that the category is still there
            IAtomEntryRepository repository2 = GetEntryRepository();
            AtomSite.Domain.AtomEntry loadedEntry = repository2.GetEntry(entry.Id);

            Assert.AreEqual(1, loadedEntry.Raters.Count());
            Assert.AreEqual(rater, loadedEntry.Raters.First());
        }

        [TestMethod]
        public void EntryWithRatersDeleteTest()
        {
            MakeTestCollections(false);

            // make an entry with an attached raters
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);
            const string rater1 = "127.0.0.1";
            const string rater2 = "127.0.0.2";

            entry.Raters = new List<string>  {rater1, rater2};

            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // load it again - should have 2 raters
            IAtomEntryRepository repository2 = GetEntryRepository();
            AtomSite.Domain.AtomEntry loadedEntry = repository2.GetEntry(entry.Id);

            Assert.AreEqual(2, loadedEntry.Raters.Count());

            // remove a rater
            loadedEntry.Raters = rater1.InList();
            repository2.UpdateEntry(loadedEntry);

            // load it again
            loadedEntry = repository2.GetEntry(entry.Id);

            Assert.AreEqual(1, loadedEntry.Raters.Count());
            Assert.AreEqual(rater1, loadedEntry.Raters.First());
        }

        #endregion
    }

    ///// <summary>
    ///// run the tests against the sql repository
    ///// </summary>
    //[TestClass]
    //public class SqlAtomEntryLinkedDataTest : BaseAtomEntryLinkedDataTest
    //{
    //    protected override IAtomEntryRepository GetEntryRepository()
    //    {
    //        return new SqlAtomEntryRepository(new AtomDataClassesDataContext(""));
    //    }

    //    protected override IAppServiceRepository GetAppServiceRepository()
    //    {
    //        return new SqlAppServiceRepository(new AtomDataClassesDataContext(""));
    //    }
    //}

    /// <summary>
    /// run the tests against the mock repository
    /// </summary>
    [TestClass]
    public class MockAtomEntryLinkedDataTest : BaseAtomEntryLinkedDataTest
    {
        private readonly MockAtomEntryRepository mockAtomEntryRepository = new MockAtomEntryRepository();
        private readonly IAppServiceRepository mockAppServiceRepository = new MockAppServiceRepository();

        protected override IAtomEntryRepository GetEntryRepository()
        {
            return mockAtomEntryRepository;
        }

        protected override IAppServiceRepository GetAppServiceRepository()
        {
            return mockAppServiceRepository;
        }
    }

    [TestClass]
    public class FileAtomEntryLinkedDataTest : BaseAtomEntryLinkedDataTest
    {

        protected override IAppServiceRepository GetAppServiceRepository()
        {
            FileRepositoryHelper helper = new FileRepositoryHelper(TestContext);
            helper.CheckAppService();
            return new FileAppServiceRepository(helper.TestFilesPath());
        }

        protected override IAtomEntryRepository GetEntryRepository()
        {
            IAppServiceRepository appServiceRepository = GetAppServiceRepository();

            FileRepositoryHelper helper = new FileRepositoryHelper(TestContext);
            return new FileAtomEntryRepository(helper.TestFilesPath(), appServiceRepository);
        }
    }
}
