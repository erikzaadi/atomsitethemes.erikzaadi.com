using System;
using System.Collections.Generic;
using System.Linq;
using AtomSite.Repository.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;
using AtomSite.Repository;
using AtomSite.Repository.Mock;

namespace Repository.Test
{
    /// <summary>
    /// Run tests against an IAtomEntryRepository
    /// for retrieving sets of entries
    /// </summary>
    public abstract class BaseAtomEntryRetrievalTest : BaseAtomEntryTest
    {
        #region private

        private const int PageSize = 1000;

        #endregion

        #region properties

        public TestContext TestContext { get; set; }

        #endregion

        #region tests

        [TestMethod]
        public void GetEntriesByDateTest()
        {
            MakeTestCollections(true);

            DateTime now = DateTime.Now;

            DateTime oneMonthAgo = now.AddMonths(-1);
            DateTime twoMonthAgo = oneMonthAgo.AddMonths(-1);

            AtomEntry entry1 = TestDataHelper.MakeTestEntry(now, TestCollection1Name);
            AtomEntry entry2 = TestDataHelper.MakeTestEntry(oneMonthAgo, TestCollection1Name);
            AtomEntry entry3 = TestDataHelper.MakeTestEntry(twoMonthAgo, TestCollection1Name);

            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry1);
            repository.CreateEntry(entry2);
            repository.CreateEntry(entry3);

            //now retrieve recent entries
            EntryCriteria criteria = new EntryCriteria
            {
                WorkspaceName = TestWorkspaceName,
                CollectionName = TestCollection1Name,
                StartDate = now.AddDays(-1),
                EndDate = now.AddDays(1),
                Authorized = true
            };

            int totalEntries;
            IEnumerable<AtomEntry> entriesAfter = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.AreEqual(1, entriesAfter.Count(), "recent entries");
            DataTester.AssertEntriesEqual(entry1, entriesAfter.First());

            // a month back - two entries
            criteria = new EntryCriteria
            {
                WorkspaceName = TestWorkspaceName,
                CollectionName = TestCollection1Name,
                StartDate = oneMonthAgo.AddDays(-1),
                EndDate = now.AddDays(1),
                Authorized = true
            };

            IEnumerable<AtomEntry> oneMonthEntries = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.AreEqual(2, oneMonthEntries.Count(), "one month of entries");
            DataTester.AssertEntriesEqual(entry1, oneMonthEntries.First());
            DataTester.AssertEntriesEqual(entry2, oneMonthEntries.Skip(1).First());


            criteria = new EntryCriteria
            {
                WorkspaceName = TestWorkspaceName,
                CollectionName = TestCollection1Name,
                StartDate = twoMonthAgo.AddDays(-1),
                EndDate = now.AddDays(1),
                Authorized = true
            };

            // two months back - three entries
            IEnumerable<AtomEntry> twoMonthEntries = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.AreEqual(3, twoMonthEntries.Count(), "two month of entries");
            DataTester.AssertEntriesEqual(entry1, twoMonthEntries.First());
            DataTester.AssertEntriesEqual(entry2, twoMonthEntries.Skip(1).First());
            DataTester.AssertEntriesEqual(entry3, twoMonthEntries.Skip(2).First());

        }

        [TestMethod]
        public void GetEntriesSortOrderTwoEntriesTest()
        {
            MakeTestCollections(true);

            DateTime now = DateTime.Now;

            DateTime oneDayAgo = now.AddDays(-1);

            // make an entry for today 
            AtomEntry entry1 = TestDataHelper.MakeTestEntry(now, TestCollection1Name);
            entry1.Edited = now.AddHours(1);

            // and an entry for yesterday
            AtomEntry entry2 = TestDataHelper.MakeTestEntry(oneDayAgo, TestCollection1Name);
            // created yesterday, edited after the other
            entry2.Edited = now.AddHours(2);

            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry1);
            repository.CreateEntry(entry2);

            //now retrieve recent entries
            EntryCriteria criteria = new EntryCriteria
            {
                WorkspaceName = TestWorkspaceName,
                CollectionName = TestCollection1Name,
                Authorized = true
            };

            int totalEntries;
            IEnumerable<AtomEntry> entries = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entries);
            Assert.IsTrue(entries.Count() > 0);
            DataTester.AssertEntriesEqual(entry1, entries.First());
            DataTester.AssertEntriesEqual(entry2, entries.Skip(1).First());

            // now get them in edit order - the second entry should now be first
            criteria.SortMethod = SortMethod.EditDesc;

            IEnumerable<AtomEntry> entriesEditSort = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entriesEditSort);
            Assert.IsTrue(entriesEditSort.Count() > 0);

            DataTester.AssertEntriesEqual(entry2, entriesEditSort.First());
            DataTester.AssertEntriesEqual(entry1, entriesEditSort.Skip(1).First());

            // date ascending order
            criteria.SortMethod = SortMethod.DateAsc;

            IEnumerable<AtomEntry> entriesSortDateAsc = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entriesSortDateAsc);
            Assert.IsTrue(entriesSortDateAsc.Count() > 0);

            DataTester.AssertEntriesEqual(entry2, entriesSortDateAsc.First());
            DataTester.AssertEntriesEqual(entry1, entriesSortDateAsc.Skip(1).First());

            // date descending order
            criteria.SortMethod = SortMethod.DateDesc;

            IEnumerable<AtomEntry> entriesSortDateDesc = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entriesSortDateDesc);
            Assert.IsTrue(entriesSortDateDesc.Count() > 0);

            DataTester.AssertEntriesEqual(entry1, entriesSortDateDesc.First());
            DataTester.AssertEntriesEqual(entry2, entriesSortDateDesc.Skip(1).First());
        }

        [TestMethod]
        public void GetEntriesSortOrderThreeEntriesTest()
        {
            MakeTestCollections(true);

            DateTime now = DateTime.Now;

            DateTime oneDayAgo = now.AddDays(-1);
            DateTime twoDaysAgo = now.AddDays(-2);

            // make an entry for today 
            AtomEntry entry1 = TestDataHelper.MakeTestEntry(now, TestCollection1Name);
            entry1.Edited = now.AddHours(1);

            // and an entry for yesterday
            AtomEntry entry2 = TestDataHelper.MakeTestEntry(oneDayAgo, TestCollection1Name);
            // created yesterday, edited after the others
            entry2.Edited = now.AddHours(3);

            // third entry, day before yesterday
            AtomEntry entry3 = TestDataHelper.MakeTestEntry(twoDaysAgo, TestCollection1Name);
            // created yesterday, edited in the middle
            entry2.Edited = now.AddHours(2);


            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry1);
            repository.CreateEntry(entry2);
            repository.CreateEntry(entry3);

            //now retrieve recent entries with no sort order
            EntryCriteria criteria = new EntryCriteria
            {
                WorkspaceName = TestWorkspaceName,
                CollectionName = TestCollection1Name,
                Authorized = true
            };

            int totalEntries;
            IEnumerable<AtomEntry> entries = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entries);
            Assert.IsTrue(entries.Count() > 0);
            DataTester.AssertEntriesEqual(entry1, entries.First());
            DataTester.AssertEntriesEqual(entry2, entries.Skip(1).First());
            DataTester.AssertEntriesEqual(entry3, entries.Skip(2).First());

            // now get them in edit order - the second entry should now be first
            criteria.SortMethod = SortMethod.EditDesc;

            IEnumerable<AtomEntry> entriesEditSort = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entriesEditSort);
            Assert.IsTrue(entriesEditSort.Count() > 0);

            DataTester.AssertEntriesEqual(entry2, entriesEditSort.First());
            DataTester.AssertEntriesEqual(entry3, entriesEditSort.Skip(2).First());
            DataTester.AssertEntriesEqual(entry1, entriesEditSort.Skip(1).First());

            // date ascending order
            criteria.SortMethod = SortMethod.DateAsc;

            IEnumerable<AtomEntry> entriesSortDateAsc = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entriesSortDateAsc);
            Assert.IsTrue(entriesSortDateAsc.Count() > 0);

            DataTester.AssertEntriesEqual(entry3, entriesSortDateAsc.First());
            DataTester.AssertEntriesEqual(entry2, entriesSortDateAsc.Skip(1).First());
            DataTester.AssertEntriesEqual(entry1, entriesSortDateAsc.Skip(2).First());

            // date descending order
            criteria.SortMethod = SortMethod.DateDesc;
            IEnumerable<AtomEntry> entriesSortDateDesc = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entriesSortDateDesc);
            Assert.IsTrue(entriesSortDateDesc.Count() > 0);

            DataTester.AssertEntriesEqual(entry1, entriesSortDateDesc.First());
            DataTester.AssertEntriesEqual(entry2, entriesSortDateDesc.Skip(1).First());
            DataTester.AssertEntriesEqual(entry3, entriesSortDateDesc.Skip(2).First());
        }

        [TestMethod]
        public void GetEntriesDraftAuthorizedTest()
        {
            MakeTestCollections(true);

            DateTime now = DateTime.Now;

            DateTime oneDayAgo = now.AddDays(-1);

            // make an entry for today 
            AtomEntry entry1 = TestDataHelper.MakeTestEntry(now, TestCollection1Name);

            // and an entry for yesterday
            AtomEntry entry2 = TestDataHelper.MakeTestEntry(oneDayAgo, TestCollection1Name);
            entry2.Control.Draft = true;

            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry1);
            repository.CreateEntry(entry2);

            //now retrieve entries with authorized
            EntryCriteria criteria = new EntryCriteria
            {
                WorkspaceName = TestWorkspaceName,
                CollectionName = TestCollection1Name,
                Authorized = true
            };

            int totalEntries;
            IEnumerable<AtomEntry> entries = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entries);
            Assert.AreEqual(2, entries.Count());

            // now get them without auth
            criteria.Authorized = false;
            IEnumerable<AtomEntry> entriesNotAuth = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entriesNotAuth);
            Assert.AreEqual(1, entriesNotAuth.Count());
        }

        [TestMethod]
        public void GetEntriesAprovedAuthorizedTest()
        {
            MakeTestCollections(true);

            DateTime now = DateTime.Now;

            DateTime oneDayAgo = now.AddDays(-1);

            // make an entry for today 
            AtomEntry entry1 = TestDataHelper.MakeTestEntry(now, TestCollection1Name);

            // and an entry for yesterday
            AtomEntry entry2 = TestDataHelper.MakeTestEntry(oneDayAgo, TestCollection1Name);
            entry2.Control.Approved = false;

            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry1);
            repository.CreateEntry(entry2);

            //now retrieve entries with authorized
            EntryCriteria criteria = new EntryCriteria
            {
                WorkspaceName = TestWorkspaceName,
                CollectionName = TestCollection1Name,
                Authorized = true
            };

            int totalEntries;
            IEnumerable<AtomEntry> entries = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entries);
            Assert.AreEqual(2, entries.Count());

            // now get them without auth
            criteria.Authorized = false;
            IEnumerable<AtomEntry> entriesNotAuth = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entriesNotAuth);
            Assert.AreEqual(1, entriesNotAuth.Count());

            DataTester.AssertEntriesEqual(entry1, entriesNotAuth.First());
        }

        [TestMethod]
        public void GetEntriesByPersonTest()
        {
            MakeTestCollections(false);

            DateTime now = DateTime.Now;

            DateTime oneDayAgo = now.AddDays(-1);
            DateTime twoDaysAgo = now.AddDays(-2);

            // make two people
            AtomPerson person1 = TestDataHelper.MakeTestPerson();

            AtomPerson person2 = TestDataHelper.MakeTestPerson();

            // make an entry for today 
            AtomEntry entry1 = TestDataHelper.MakeTestEntry(now, TestCollection1Name);
            // person 1 is the author
            entry1.Authors = entry1.Authors.Add(person1);

            // and an entry for yesterday
            AtomEntry entry2 = TestDataHelper.MakeTestEntry(oneDayAgo, TestCollection1Name);
            // person 2 is the author
            entry2.Authors = entry2.Authors.Add(person2);

            // and the day before yesteday
            AtomEntry entry3 = TestDataHelper.MakeTestEntry(twoDaysAgo, TestCollection1Name);

            // join authors - both people
            entry3.Authors = entry1.Authors.Add(person1, person2);


            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry1);
            repository.CreateEntry(entry2);
            repository.CreateEntry(entry3);

            //now retrieve entries by person 1

            EntryCriteria criteria = new EntryCriteria
            {
                WorkspaceName = TestWorkspaceName,
                CollectionName = TestCollection1Name,
                PersonName = person1.Name,
                PersonType = person1.Type,
                Authorized = true,
            };

            int totalEntries;
            IEnumerable<AtomEntry> entriesPerson1 = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entriesPerson1);
            Assert.AreEqual(2, entriesPerson1.Count());

            // should contain entries 1 and 3
            Assert.IsTrue(entriesPerson1.ContainsId(entry1.Id), "Entry 1 not found");
            Assert.IsTrue(entriesPerson1.ContainsId(entry3.Id), "Entry 3 not found");


            // now get them with person 2
            criteria = new EntryCriteria
            {
                WorkspaceName = TestWorkspaceName,
                CollectionName = TestCollection1Name,
                PersonName = person2.Name,
                PersonType = person2.Type,
                Authorized = true
            };

            IEnumerable<AtomEntry> entriesPerson2 = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entriesPerson2);
            Assert.AreEqual(2, entriesPerson2.Count());

            // should contain entries 2 and 3
            Assert.IsTrue(entriesPerson2.ContainsId(entry2.Id), "Entry 2 not found");
            Assert.IsTrue(entriesPerson2.ContainsId(entry3.Id), "Entry 3 not found");
        }

        [TestMethod]
        public void GetEntriesByCategoryTest()
        {
            MakeTestCollections(true);

            DateTime now = DateTime.Now;

            DateTime oneDayAgo = now.AddDays(-1);
            DateTime twoDaysAgo = now.AddDays(-2);

            // make two categories
            AtomCategory category1 = GetCategory(true);
            AtomCategory category2 = GetCategory(false);


            // make an entry for today 
            AtomEntry entry1 = TestDataHelper.MakeTestEntry(now, TestCollection1Name);
            // in category 1
            entry1.Categories = entry1.Categories.Add(category1);

            // and an entry for yesterday
            AtomEntry entry2 = TestDataHelper.MakeTestEntry(oneDayAgo, TestCollection1Name);
            // in category 2
            entry2.Categories = entry2.Categories.Add(category2);

            // and the day before yesteday
            AtomEntry entry3 = TestDataHelper.MakeTestEntry(twoDaysAgo, TestCollection1Name);

            // in both categories
            entry3.Categories = entry3.Categories.Add(category1, category2);

            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry1);
            repository.CreateEntry(entry2);
            repository.CreateEntry(entry3);

            //now retrieve entries by category 1

            EntryCriteria criteria = new EntryCriteria
            {
                WorkspaceName = TestWorkspaceName,
                CollectionName = TestCollection1Name,
                CategoryScheme = category1.Scheme,
                CategoryTerm = category1.Term,
                Authorized = true,                
            };

            int totalEntries;
            IEnumerable<AtomEntry> entriesPerson1 = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entriesPerson1);
            Assert.AreEqual(2, entriesPerson1.Count());

            // should contain entries 1 and 3
            Assert.IsTrue(entriesPerson1.ContainsId(entry1.Id), "Entry 1 not found");
            Assert.IsTrue(entriesPerson1.ContainsId(entry3.Id), "Entry 3 not found");


            // now get them with category 2
            criteria = new EntryCriteria
            {
                WorkspaceName = TestWorkspaceName,
                CollectionName = TestCollection1Name,
                CategoryScheme = category2.Scheme,
                CategoryTerm = category2.Term,
                Authorized = true,
            };

            IEnumerable<AtomEntry> entriesPerson2 = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.IsNotNull(entriesPerson2);
            Assert.AreEqual(2, entriesPerson2.Count());

            // should contain entries 2 and 3
            Assert.IsTrue(entriesPerson2.ContainsId(entry2.Id), "Entry 2 not found");
            Assert.IsTrue(entriesPerson2.ContainsId(entry3.Id), "Entry 3 not found");
        }

        [TestMethod]
        public void GetEntryAnnotationsNoAnnotationsTest()
        {
            MakeTestCollections(false);

            // make an entry 
            AtomEntry entry = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);

            // save the entry
            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            int totalEntries;
            IEnumerable<AtomEntry> annotations = repository.GetEntries(
				new EntryCriteria
                {
                    Annotations = true,
                    EntryId = entry.Id,
					Authorized = true,
					Deep = true
                },  0, PageSize, out totalEntries);

            // should be no annotations
            Assert.IsNotNull(annotations);
            Assert.AreEqual(0, totalEntries);
            Assert.AreEqual(0, annotations.Count());
        }

        [TestMethod]
        public void GetEntryAnnotationsOneAnnotationsTest()
        {
            MakeTestCollections(false);

            // make an entry 
            AtomEntry entry = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);

            // save the entry
            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // attach an annotation
            AtomEntry annotation = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            annotation.Id = TestDataHelper.MakeReplyId(entry.Id, annotation.Published.Value.DateTime);

			repository.CreateEntry(annotation);

            // get annotations
            int totalEntries;
			IEnumerable<AtomEntry> annotations = repository.GetEntries(
				new EntryCriteria
				{
                    Annotations = true,
                    EntryId = entry.Id,
					Authorized = true,
					Deep = true
				}, 0, PageSize, out totalEntries);

            // should be 1 annotation
            Assert.IsNotNull(annotations);
            Assert.AreEqual(1, totalEntries);
            Assert.AreEqual(1, annotations.Count());

            AtomEntry entry1 = annotations.First();

            DataTester.AssertEntriesEqual(annotation, entry1);
        }

        [TestMethod]
        public void GetEntryAnnotationsTwoAnnotationsNotDeepTest()
        {
            MakeTestCollections(false);

            // make an entry 
            AtomEntry entry = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);

            // save the entry
            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // attach an annotation
            AtomEntry annotation = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            annotation.Id = TestDataHelper.MakeReplyId(entry.Id, annotation.Published.Value.DateTime);

            repository.CreateEntry(annotation);

            // attach an annotation to that
            AtomEntry annotation2 = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            annotation2.Id = TestDataHelper.MakeReplyId(annotation.Id, annotation2.Published.Value.DateTime);

            repository.CreateEntry(annotation2);

            // get annotations, not deep
            int totalEntries;
            IEnumerable<AtomEntry> annotations = repository.GetEntries(
                new EntryCriteria
                {
                    Annotations = true,
                    EntryId = entry.Id,
                    Authorized = true,
                    Deep = false
                }, 0, PageSize, out totalEntries);

            // should be 1 annotation
            Assert.IsNotNull(annotations);
            Assert.AreEqual(1, totalEntries);
            Assert.AreEqual(1, annotations.Count());

            AtomEntry entry1 = annotations.First();

            DataTester.AssertEntriesEqual(annotation, entry1);
        }

        [TestMethod]
        public void GetEntryAnnotationsTwoAnnotationsDeepTest()
        {
            MakeTestCollections(false);

            // make an entry 
            AtomEntry entry = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);

            // save the entry
            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // attach an annotation
            AtomEntry annotation = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            annotation.Id = TestDataHelper.MakeReplyId(entry.Id, annotation.Published.Value.DateTime);

            repository.CreateEntry(annotation);

            // attach an annotation to that
            AtomEntry annotation2 = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            annotation2.Id = TestDataHelper.MakeReplyId(annotation.Id, annotation2.Published.Value.DateTime);

            repository.CreateEntry(annotation2);

            // get annotations, deep
            int totalEntries;
            IEnumerable<AtomEntry> annotations = repository.GetEntries(
                new EntryCriteria
                {
                    Annotations = true,
                    EntryId = entry.Id,
                    Authorized = true,
                    Deep = true
                }, 0, PageSize, out totalEntries);

            // should be 2 annotation
            Assert.IsNotNull(annotations);
            Assert.AreEqual(2, totalEntries);
            Assert.AreEqual(2, annotations.Count());

            Assert.IsTrue(annotations.ContainsId(annotation.Id));
            Assert.IsTrue(annotations.ContainsId(annotation2.Id));
            Assert.IsFalse(annotations.ContainsId(entry.Id));
        }

        [TestMethod]
        public void GetEntryAnnotationsSixAnnotationsDeepTest()
        {
            MakeTestCollections(false);

            // make an entry 
            AtomEntry entry = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);

            // save the entry
            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // attach two annotations
            AtomEntry annotation1 = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            annotation1.Id = TestDataHelper.MakeReplyId(entry.Id, annotation1.Published.Value.DateTime);

            repository.CreateEntry(annotation1);

            AtomEntry annotation2 = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            annotation2.Id = TestDataHelper.MakeReplyId(entry.Id, annotation2.Published.Value.DateTime);

            repository.CreateEntry(annotation2);


            // attach two annotation to each of these
            AtomEntry annotation11 = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            annotation11.Id = TestDataHelper.MakeReplyId(annotation1.Id, annotation11.Published.Value.DateTime);

            repository.CreateEntry(annotation11);

            AtomEntry annotation12 = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            annotation12.Id = TestDataHelper.MakeReplyId(annotation1.Id, annotation12.Published.Value.DateTime);

            repository.CreateEntry(annotation12);

            AtomEntry annotation21 = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            annotation21.Id = TestDataHelper.MakeReplyId(annotation2.Id, annotation21.Published.Value.DateTime);

            repository.CreateEntry(annotation21);

            AtomEntry annotation22 = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            annotation22.Id = TestDataHelper.MakeReplyId(annotation2.Id, annotation22.Published.Value.DateTime);

            repository.CreateEntry(annotation22);

            // shallow annnotations
            int totalShallowEntries;
            IEnumerable<AtomEntry> shallowAnnotations = repository.GetEntries(
                new EntryCriteria
                {
                    Annotations = true,
                    EntryId = entry.Id,
                    Authorized = true,
                    Deep = false
                }, 0, PageSize, out totalShallowEntries);

            // should be 2 shallow annotation
            Assert.IsNotNull(shallowAnnotations);
            Assert.AreEqual(2, totalShallowEntries);
            Assert.AreEqual(2, shallowAnnotations.Count());

            // get annotations, deep
            int totalDeepEntries;
            IEnumerable<AtomEntry> deepAnnotations = repository.GetEntries(
                new EntryCriteria
                {
                    Annotations = true,
                    EntryId = entry.Id,
                    Authorized = true,
                    Deep = true
                }, 0, PageSize, out totalDeepEntries);

            // should be 6 deep annotations
            Assert.IsNotNull(deepAnnotations);
            Assert.AreEqual(6, totalDeepEntries);
            Assert.AreEqual(6, deepAnnotations.Count());
        }


        [TestMethod]
        public void ApproveAllApprovedAlreadyNoAnnotationsTest()
        {
            MakeTestCollections(false);

            // make an entry 
            AtomEntry entry = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            entry.Control.Approved = true;

            // save the entry
            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // get the entry - not approved
            AtomEntry retrievedEntry = repository.GetEntry(entry.Id);
            Assert.IsNotNull(retrievedEntry);
            Assert.IsNotNull(retrievedEntry.Control);
            Assert.IsTrue(retrievedEntry.Control.Approved.HasValue);
            Assert.IsTrue(retrievedEntry.Control.Approved.Value);

            int count = repository.ApproveAll(entry.Id);

            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void ApproveAllNoAnnotationsTest()
        {
            MakeTestCollections(false);

            // make an entry 
            AtomEntry entry = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            entry.Control.Approved = false;

            // save the entry
            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // get the entry - not approved
            AtomEntry retrievedEntry = repository.GetEntry(entry.Id);
            Assert.IsNotNull(retrievedEntry);
            Assert.IsNotNull(retrievedEntry.Control);
            Assert.IsTrue(retrievedEntry.Control.Approved.HasValue);
            Assert.IsFalse(retrievedEntry.Control.Approved.Value);

            int count = repository.ApproveAll(entry.Id);

            Assert.AreEqual(1, count);

            // get the entry - approved now
            retrievedEntry = repository.GetEntry(entry.Id);
            Assert.IsNotNull(retrievedEntry);
            Assert.IsNotNull(retrievedEntry.Control);
            Assert.IsTrue(retrievedEntry.Control.Approved.HasValue);
            Assert.IsTrue(retrievedEntry.Control.Approved.Value);
        }

        [TestMethod]
        public void ApproveAllEntryWithTwoAnnotationsTest()
        {
            MakeTestCollections(false);

            // make an entry 
            AtomEntry entry = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            entry.Control.Approved = false;

            // save the entry
            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // attach two annotations, one approved
            AtomEntry annotation1 = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            annotation1.Id = TestDataHelper.MakeReplyId(entry.Id, annotation1.Published.Value.DateTime);
            annotation1.Control.Approved = true;

            repository.CreateEntry(annotation1);

            AtomEntry annotation2 = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);
            annotation2.Id = TestDataHelper.MakeReplyId(entry.Id, annotation2.Published.Value.DateTime);
            annotation2.Control.Approved = false;

            repository.CreateEntry(annotation2);

            int count = repository.ApproveAll(entry.Id);

            Assert.AreEqual(2, count);

            // get the entry - approved now
            AtomEntry retrievedEntry = repository.GetEntry(entry.Id);
            Assert.IsNotNull(retrievedEntry);
            Assert.IsNotNull(retrievedEntry.Control);
            Assert.IsTrue(retrievedEntry.Control.Approved.HasValue);
            Assert.IsTrue(retrievedEntry.Control.Approved.Value);

            retrievedEntry = repository.GetEntry(annotation2.Id);
            Assert.IsNotNull(retrievedEntry);
            Assert.IsNotNull(retrievedEntry.Control);
            Assert.IsTrue(retrievedEntry.Control.Approved.HasValue);
            Assert.IsTrue(retrievedEntry.Control.Approved.Value);
        }

        [TestMethod]
        public void EntrySearchCriteriaTest()
        {
            MakeTestCollections(false);

            // make an entry 
            AtomEntry entry = TestDataHelper.MakeTestEntry(DateTime.Now, TestCollection1Name);

            string correctGuid = Guid.NewGuid().ToString();
            string otherGuid = Guid.NewGuid().ToString();

            // save the entry
            entry.Content = new AtomContent
                 {
                     Type = "text",
                     Text = "some entry text " + correctGuid + ""
                 };

            IAtomEntryRepository repository = GetEntryRepository();
            repository.CreateEntry(entry);

            // get entries
            EntryCriteria criteria = new EntryCriteria
             {
                 WorkspaceName = TestWorkspaceName,
                 CollectionName = TestCollection1Name,
                 Authorized = true,
                 SearchTerm = correctGuid
             };

            int totalEntries;
            IEnumerable<AtomEntry> searchEnties = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.AreEqual(1, searchEnties.Count());
            Assert.IsTrue(searchEnties.ContainsId(entry.Id));

            // try the wrong guid
            criteria.SearchTerm = otherGuid;
            searchEnties = repository.GetEntries(criteria, 0, PageSize, out totalEntries);

            Assert.AreEqual(0, searchEnties.Count());
        }

        #endregion
    }

    ///// <summary>
    ///// run the tests against the sql repository
    ///// </summary>
    //[TestClass]
    //public class SqlAtomEntryRetrievalTest : BaseAtomEntryRetrievalTest
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
    public class MockAtomEntryRetrievalTest : BaseAtomEntryRetrievalTest
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
    public class FileAtomEntryRetrievalTest : BaseAtomEntryRetrievalTest
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
