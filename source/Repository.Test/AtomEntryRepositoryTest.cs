using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Repository;
using AtomSite.Repository.Mock;
using AtomSite.Repository.File;
using AtomSite.Repository.Dlinq;
using AtomSite.Domain;

namespace Repository.Test
{
    /// <summary>
    /// Run tests against an IAtomEntryRepository
    /// for all functions except getting sets of entries
    /// </summary>
    public abstract class BaseAtomEntryRepositoryTest: BaseAtomEntryTest
    {
        private const int PageSize = 1000;

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CreateAtomEntryRepositoryNotNullTest()
        {
            IAtomEntryRepository repository = GetEntryRepository();

            Assert.IsNotNull(repository);
        }

        [TestMethod]
        public void GetAtomEntriesNotNullTest()
        {
            MakeTestCollections(false);

            IAtomEntryRepository repository = GetEntryRepository();

            int totalEntries;
            EntryCriteria criteria = new EntryCriteria
            {
                WorkspaceName = TestWorkspaceName,
                CollectionName = TestCollection1Name,
                Authorized = true
            };

            IEnumerable<AtomSite.Domain.AtomEntry> results = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void AddEntryAndRetrieveTest()
        {
            IAtomEntryRepository repository = GetEntryRepository();

            // create an entry
            AtomSite.Domain.AtomEntry entry = new AtomSite.Domain.AtomEntry()
            {
              Id = new Id("test.com", "2008", Guid.NewGuid().ToString()),
              Title = new AtomText() { Text = "Test Title" },
              Updated = DateTimeOffset.Now,
              Published = DateTimeOffset.Now,
              Content = new AtomContent() { Text = "Test content" },
              Authors = new List<AtomSite.Domain.AtomPerson> { new AtomSite.Domain.AtomAuthor() { Name = "Author" } },
              Media = false,
              Control = new AppControl() { AllowAnnotate = false, Approved = false, Draft = true }
            };
            entry.SetProperty<string>("test", "test attribute");

            // save the entry
            entry = repository.CreateEntry(entry);

            AtomSite.Domain.AtomEntry dbEntry = repository.GetEntry(entry.Id);

            Assert.AreEqual(entry.Title, dbEntry.Title);
            Assert.IsTrue(entry.Updated.Subtract(dbEntry.Updated).Seconds < 1);
            if (entry.Published.HasValue)
            {
              Assert.IsTrue(entry.Published.Value.Subtract(dbEntry.Published.Value).Seconds < 1);
            }
            Assert.AreEqual(entry.Content, dbEntry.Content);
            Assert.AreEqual(entry.Authors.First(), dbEntry.Authors.First());
            Assert.AreEqual(entry.Media, dbEntry.Media);
            Assert.AreEqual(entry.Control, dbEntry.Control);
            Assert.AreEqual(dbEntry.GetProperty<string>("test"), "test attribute");
        }

        /// <summary>
        /// Try to retrieve an entry, but supply an incorrect workspace and collection
        /// Entry should not be found
        /// </summary>
        [TestMethod]
        public void AddEntryWorkspaceAndCollectionMismatchTest()
        {
            MakeTestCollections(false);

            IAtomEntryRepository repository = GetEntryRepository();

            // count the entries before adding one
            int totalEntries;

            // create an entry
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);

            // save the entry
            repository.CreateEntry(entry);

            // should be retrieved
            EntryCriteria criteria = new EntryCriteria
            {
                WorkspaceName = TestWorkspaceName,
                CollectionName = TestCollection1Name,
                Authorized = true
            };

            IEnumerable<AtomSite.Domain.AtomEntry> entriesFound = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsTrue(entriesFound.ContainsId(entry.Id), "entry not retrieved");

            // should not be retrieved
            criteria = new EntryCriteria
            {
                WorkspaceName = Guid.NewGuid().ToString(),
                CollectionName = TestCollection1Name,
                Authorized = true
            };

            // sql repository returns empty list, file provider throws an exception
            IEnumerable<AtomSite.Domain.AtomEntry> entriesNotFound;
            try
            {
                entriesNotFound = repository.GetEntries(criteria, 0, PageSize, out totalEntries);
            }
            catch (Exception)
            {
                entriesNotFound = new List<AtomSite.Domain.AtomEntry>();
            }
            
            Assert.IsFalse(entriesNotFound.ContainsId(entry.Id), "Entry retrieved with wrong workspace");

            // should not be retrieved
            criteria = new EntryCriteria
            {
                WorkspaceName = TestWorkspaceName,
                CollectionName = Guid.NewGuid().ToString(),
                Authorized = true
            };

            try
            {
                entriesNotFound = repository.GetEntries(criteria, 0, PageSize, out totalEntries);
            }
            catch (Exception)
            {
                entriesNotFound = new List<AtomSite.Domain.AtomEntry>();
            }

            Assert.IsFalse(entriesNotFound.ContainsId(entry.Id), "Entry retrieved with wrong collection");
        }

        [TestMethod]
        public void EntryDeleteTest()
        {
            MakeTestCollections(false);

            IAtomEntryRepository repository = GetEntryRepository();

            // create an entry
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);

            // save the entry
            repository.CreateEntry(entry);

            IAtomEntryRepository repository2 = GetEntryRepository();

            // can get the entry
            AtomSite.Domain.AtomEntry retrivedEntry = repository2.GetEntry(entry.Id);

            Assert.IsNotNull(retrivedEntry);
            Assert.AreEqual(retrivedEntry.Id.ToString(), entry.Id.ToString());

            // now delete it
            repository2.DeleteEntry(entry.Id);

            // check that you can't get the entry
            IAtomEntryRepository repository3 = GetEntryRepository();

            // can not get the entry - file provider throws an exception, others return null
            AtomSite.Domain.AtomEntry deletedEntry;
            try
            {
                deletedEntry = repository3.GetEntry(entry.Id);
            }
            catch (Exception)
            {

                deletedEntry = null;
            }

            Assert.IsNull(deletedEntry);
        }

        [TestMethod]
        public void GetEntryByIdTest()
        {
            MakeTestCollections(false);

            IAtomEntryRepository repository = GetEntryRepository();

            // create an entry
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);

            // save the entry
            repository.CreateEntry(entry);

            IAtomEntryRepository repository2 = GetEntryRepository();

            // try to get the entry by id on the new repository
            AtomSite.Domain.AtomEntry retrivedEntry = repository2.GetEntry(entry.Id);

            Assert.IsNotNull(retrivedEntry);
            DataTester.AssertEntriesEqual(entry, retrivedEntry);
        }

        [TestMethod]
        public void GetEntryDatesoffsetPlusTwoTest()
        {
            MakeTestCollections(false);

            // +2 hours off UTC
            TimeSpan offset = new TimeSpan(2, 0, 0);
            DateTime nowDate = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);
            DateTimeOffset nowDateOffset = new DateTimeOffset(nowDate, offset);

            IAtomEntryRepository repository = GetEntryRepository();

            // create an entry
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);
            entry.Edited = nowDateOffset;
            entry.Published = nowDateOffset;
            entry.Updated = nowDateOffset;

            // save the entry
            repository.CreateEntry(entry);

            IAtomEntryRepository repository2 = GetEntryRepository();

            // try to get the entry by id on the new repository
            AtomSite.Domain.AtomEntry retrivedEntry = repository2.GetEntry(entry.Id);

            Assert.IsNotNull(retrivedEntry);

            DataTester.AssertEntriesEqual(entry, retrivedEntry);
        }

        [TestMethod]
        public void GetEntryDatesoffsetMinusTwoTest()
        {
            MakeTestCollections(false);

            // +2 hours off UTC
            TimeSpan offset = new TimeSpan(-2, 0, 0);
            DateTime nowDate = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);
            DateTimeOffset nowDateOffset = new DateTimeOffset(nowDate, offset);

            IAtomEntryRepository repository = GetEntryRepository();

            // create an entry
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);
            entry.Edited = nowDateOffset;
            entry.Published = nowDateOffset;
            entry.Updated = nowDateOffset;

            // save the entry
            repository.CreateEntry(entry);

            IAtomEntryRepository repository2 = GetEntryRepository();

            // try to get the entry by id on the new repository
            AtomSite.Domain.AtomEntry retrivedEntry = repository2.GetEntry(entry.Id);

            Assert.IsNotNull(retrivedEntry);

            DataTester.AssertEntriesEqual(entry, retrivedEntry);
        }

        [TestMethod]
        public void EntryHasEtagTest()
        {
            MakeTestCollections(false);

            IAtomEntryRepository repository = GetEntryRepository();

            // create an entry
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);

            // save the entry
            repository.CreateEntry(entry);

            string eTag = repository.GetEntryEtag(entry.Id);

            Assert.IsFalse(string.IsNullOrEmpty(eTag));
        }

        [TestMethod]
        public void EtagsNotEqualOnDifferentEntriesTest()
        {
            MakeTestCollections(false);

            IAtomEntryRepository repository = GetEntryRepository();

            // create two entries
            AtomSite.Domain.AtomEntry entry1 = TestDataHelper.MakeTestEntry(TestCollection1Name);
            AtomSite.Domain.AtomEntry entry2 = TestDataHelper.MakeTestEntry(TestCollection1Name);

            // save the entries
            repository.CreateEntry(entry1);
            repository.CreateEntry(entry2);

            string eTag1 = repository.GetEntryEtag(entry1.Id);
            string eTag2 = repository.GetEntryEtag(entry2.Id);

            Assert.IsFalse(string.IsNullOrEmpty(eTag1));
            Assert.IsFalse(string.IsNullOrEmpty(eTag2));

            Assert.IsFalse(string.Equals(eTag1, eTag2, StringComparison.InvariantCultureIgnoreCase));
        }


        [TestMethod]
        public void EtagChangesWhenEntryChangesTest()
        {
            MakeTestCollections(false);

            IAtomEntryRepository repository = GetEntryRepository();

            // create an entry
            AtomSite.Domain.AtomEntry entry = TestDataHelper.MakeTestEntry(TestCollection1Name);
            entry.Control.Approved = false;

            // save it
            repository.CreateEntry(entry);
            string eTag1 = repository.GetEntryEtag(entry.Id);
            
            // update it
            Assert.IsFalse(string.IsNullOrEmpty(eTag1));

            entry.Control.Approved = true;
            repository.UpdateEntry(entry);

            string eTag2 = repository.GetEntryEtag(entry.Id);
            Assert.IsFalse(string.IsNullOrEmpty(eTag2));

            Assert.IsFalse(string.Equals(eTag1, eTag2, StringComparison.InvariantCultureIgnoreCase));
        }
    }
  
    /// <summary>
    /// run the tests against the sql repository
    /// </summary>
    [TestClass]
    public class DlinqAtomEntryRepositoryTest : BaseAtomEntryRepositoryTest
    {
        protected override IAtomEntryRepository GetEntryRepository()
        {
            return new DlinqAtomEntryRepository();
        }
        protected override IAppServiceRepository GetAppServiceRepository()
        {
          throw new NotImplementedException();
        }
    }

    ///// <summary>
    ///// run the tests against the sql repository
    ///// </summary>
    //[TestClass]
    //public class SqlAtomEntryRepositoryTest : BaseAtomEntryRepositoryTest
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
    public class MockAtomEntryRepositoryTest : BaseAtomEntryRepositoryTest
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

    /// <summary>
    /// Run the tests against a file repository
    /// </summary>
    [TestClass]
    public class FileAtomEntryRepositoryTest : BaseAtomEntryRepositoryTest
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
