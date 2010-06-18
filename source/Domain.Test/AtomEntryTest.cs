using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    [TestClass]
    public class AtomEntryTest
    {
        [TestMethod]
        public void SimpleCreateAtomEntryTest()
        {
            AtomEntry atomEntry = new AtomEntry();

            Assert.IsNotNull(atomEntry);
        }

        [TestMethod]
        public void CreateAtomEntryWithIdTest()
        {
            AtomEntry atomEntry = new AtomEntry
                                      {
                                          Id = "tag:owner@domain.com,2008:collection,entry"
                                      };

            Assert.IsNotNull(atomEntry);
            Assert.IsNotNull(atomEntry.Id);
        }

        [TestMethod]
        public void CreateAtomEntryWithRightsTest()
        {
            AtomEntry atomEntry = new AtomEntry
                                      {
                                          Rights = new AtomRights
                                                       {
                                                           Text = "aRights"
                                                       },
                                      };

            Assert.IsNotNull(atomEntry);
            Assert.IsNotNull(atomEntry.Rights);
        }

        [TestMethod]
        public void CreateAtomEntryWithTitleTest()
        {
            AtomEntry atomEntry = new AtomEntry
                                      {
                                          Title = new AtomTitle
                                                      {
                                                          Text = "aTitle"
                                                      }
                                      };

            Assert.IsNotNull(atomEntry);
            Assert.IsNotNull(atomEntry.Title);
        }

        [TestMethod]
        public void CreateAtomEntryWithSummaryTest()
        {
            AtomEntry atomEntry = new AtomEntry
                                      {
                                          Summary = new AtomSummary
                                                        {
                                                            Text = "aSummary"
                                                        }
                                      };

            Assert.IsNotNull(atomEntry);
            Assert.IsNotNull(atomEntry.Summary);
        }

        [TestMethod]
        public void CreateFullAtomEntryTest()
        {
            DateTime editDate = DateTime.Now;
            DateTime publishedDate = DateTime.Now.AddHours(1);
            DateTime updatedDate = DateTime.Now.AddHours(2);

            AtomEntry atomEntry = new AtomEntry
                                      {
                                          Authors = new List<AtomPerson>(),
                                          Categories = new List<AtomCategory>(),
                                          Content = new AtomContent(),
                                          Contributors = new List<AtomPerson>(),
                                          Control = new AppControl(),
                                          Base = new Uri("http://base.com"),
                                          Edited = editDate,
                                          Lang = "EN",
                                          Links = new List<AtomLink>(),
                                          Published = publishedDate,
                                          Total = 3,
                                          Updated = updatedDate,
                                          InReplyTo = new ThreadInReplyTo(),
                                      };

            Assert.IsNotNull(atomEntry);
            Assert.IsNotNull(atomEntry.Authors);
            Assert.IsNotNull(atomEntry.Content);
            Assert.IsNotNull(atomEntry.Contributors);
            Assert.IsNotNull(atomEntry.Control);

            Assert.AreEqual(0, atomEntry.Authors.Count());
            Assert.AreEqual(0, atomEntry.Categories.Count());
            Assert.AreEqual(0, atomEntry.Contributors.Count());
            Assert.AreEqual(0, atomEntry.Links.Count());

            Assert.AreEqual(new Uri("http://base.com"), atomEntry.Base);
            Assert.AreEqual(editDate, atomEntry.Edited);
            Assert.AreEqual("EN", atomEntry.Lang);

            Assert.IsNotNull(atomEntry.Links);
            Assert.AreEqual(publishedDate, atomEntry.Published);
            Assert.AreEqual(3, atomEntry.Total);
            Assert.AreEqual(updatedDate, atomEntry.Updated);
            Assert.IsNotNull(atomEntry.InReplyTo);
        }

        [TestMethod]
        public void CreateAtomEntryWithAuthorsTest()
        {

            AtomEntry atomEntry = new AtomEntry
                                      {
                                          Authors = TestHelper.MakePersonList(5, Atom.AtomNs + "author")
                                      };

            Assert.IsNotNull(atomEntry);
            Assert.IsNotNull(atomEntry.Authors);
            Assert.AreEqual(5, atomEntry.Authors.Count());
        }

        [TestMethod]
        public void CreateAtomEntryWithContributorsTest()
        {

            AtomEntry atomEntry = new AtomEntry
                                      {
                                          Contributors = TestHelper.MakePersonList(5, Atom.AtomNs + "contributor")
                                      };

            Assert.IsNotNull(atomEntry);
            Assert.IsNotNull(atomEntry.Contributors);
            Assert.AreEqual(5, atomEntry.Contributors.Count());
        }

        [TestMethod]
        public void CreateAtomEntryWithCategoriesTest()
        {

            AtomEntry atomEntry = new AtomEntry
                                      {
                                          Categories = TestHelper.MakeAtomCategoryList(5)
                                      };

            Assert.IsNotNull(atomEntry);
            Assert.IsNotNull(atomEntry.Categories);
            Assert.AreEqual(5, atomEntry.Categories.Count());
        }

        [TestMethod]
        public void CreateAtomEntryWithLinksTest()
        {

            AtomEntry atomEntry = new AtomEntry
                                      {
                                          Links = TestHelper.MakeLinks(5)
                                      };

            Assert.IsNotNull(atomEntry);
            Assert.IsNotNull(atomEntry.Links);
            Assert.AreEqual(5, atomEntry.Links.Count());
        }

        [TestMethod]
        public void CreateAtomEntryWithDateOffsetsPlusTwo()
        {
            // +2 hours off UTC
            TimeSpan offset = new TimeSpan(2, 0, 0);
            DateTime nowDate = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);
            DateTimeOffset nowDateOffset = new DateTimeOffset(nowDate, offset);

            AtomEntry atomEntry = new AtomEntry
                                      {
                                          Edited = nowDateOffset,
                                          Published = nowDateOffset,
                                          Updated = nowDateOffset
                                      };

            Assert.IsTrue(atomEntry.Edited.HasValue);

            Assert.AreEqual(nowDateOffset, atomEntry.Edited.Value);
            Assert.AreEqual(nowDateOffset.DateTime, atomEntry.Edited.Value.DateTime);
            Assert.AreEqual(nowDateOffset.Offset, atomEntry.Edited.Value.Offset);

            Assert.IsTrue(atomEntry.Published.HasValue);
            Assert.AreEqual(nowDateOffset, atomEntry.Published.Value);
            Assert.AreEqual(nowDateOffset.DateTime, atomEntry.Published.Value.DateTime);
            Assert.AreEqual(nowDateOffset.Offset, atomEntry.Published.Value.Offset);

            Assert.AreEqual(nowDateOffset, atomEntry.Updated);
            Assert.AreEqual(nowDateOffset.DateTime, atomEntry.Updated.DateTime);
            Assert.AreEqual(nowDateOffset.Offset, atomEntry.Updated.Offset);
        }

        [TestMethod]
        public void CreateAtomEntryWithDateOffsetsMinusFive()
        {
            // -5 hours off UTC
            TimeSpan offset = new TimeSpan(-5, 0, 0);
            DateTime nowDate = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);
            DateTimeOffset nowDateOffset = new DateTimeOffset(nowDate, offset);

            AtomEntry atomEntry = new AtomEntry
                                      {
                                          Edited = nowDateOffset,
                                          Published = nowDateOffset,
                                          Updated = nowDateOffset
                                      };

            Assert.IsTrue(atomEntry.Edited.HasValue);

            Assert.AreEqual(nowDateOffset, atomEntry.Edited.Value);
            Assert.AreEqual(nowDateOffset.DateTime, atomEntry.Edited.Value.DateTime);
            Assert.AreEqual(nowDateOffset.Offset, atomEntry.Edited.Value.Offset);

            Assert.IsTrue(atomEntry.Published.HasValue);
            Assert.AreEqual(nowDateOffset, atomEntry.Published.Value);
            Assert.AreEqual(nowDateOffset.DateTime, atomEntry.Published.Value.DateTime);
            Assert.AreEqual(nowDateOffset.Offset, atomEntry.Published.Value.Offset);

            Assert.AreEqual(nowDateOffset, atomEntry.Updated);
            Assert.AreEqual(nowDateOffset.DateTime, atomEntry.Updated.DateTime);
            Assert.AreEqual(nowDateOffset.Offset, atomEntry.Updated.Offset);
        }

        [TestMethod]
        public void CreateAtomEntryWithDateOffsetsMixed()
        {
            // various offsets
            TimeSpan offset2 = new TimeSpan(2, 0, 0);
            TimeSpan offset5 = new TimeSpan(5, 0, 0);
            TimeSpan offset9 = new TimeSpan(9, 0, 0);

            DateTime nowDate = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);
            DateTimeOffset nowDateOffset2 = new DateTimeOffset(nowDate, offset2);
            DateTimeOffset nowDateOffset5 = new DateTimeOffset(nowDate, offset5);
            DateTimeOffset nowDateOffset9 = new DateTimeOffset(nowDate, offset9);

            AtomEntry atomEntry = new AtomEntry
                                      {
                                          Edited = nowDateOffset2,
                                          Published = nowDateOffset5,
                                          Updated = nowDateOffset9
                                      };

            Assert.IsTrue(atomEntry.Edited.HasValue);

            Assert.AreEqual(nowDateOffset2, atomEntry.Edited.Value);
            Assert.AreEqual(nowDateOffset2.DateTime, atomEntry.Edited.Value.DateTime);
            Assert.AreEqual(nowDateOffset2.Offset, atomEntry.Edited.Value.Offset);

            Assert.IsTrue(atomEntry.Published.HasValue);
            Assert.AreEqual(nowDateOffset5, atomEntry.Published.Value);
            Assert.AreEqual(nowDateOffset5.DateTime, atomEntry.Published.Value.DateTime);
            Assert.AreEqual(nowDateOffset5.Offset, atomEntry.Published.Value.Offset);

            Assert.AreEqual(nowDateOffset9, atomEntry.Updated);
            Assert.AreEqual(nowDateOffset9.DateTime, atomEntry.Updated.DateTime);
            Assert.AreEqual(nowDateOffset9.Offset, atomEntry.Updated.Offset);
        }

        [TestMethod]
        public void CreateAtomEntryWithDateOffsetsMixedNegative()
        {
            // various offsets
            TimeSpan offset2 = new TimeSpan(-2, 0, 0);
            TimeSpan offset5 = new TimeSpan(5, 30, 0);
            TimeSpan offset9 = new TimeSpan(-9, -15, 0);

            DateTime nowDate = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);
            DateTimeOffset nowDateOffset2 = new DateTimeOffset(nowDate, offset2);
            DateTimeOffset nowDateOffset5 = new DateTimeOffset(nowDate, offset5);
            DateTimeOffset nowDateOffset9 = new DateTimeOffset(nowDate, offset9);

            AtomEntry atomEntry = new AtomEntry
                                      {
                                          Edited = nowDateOffset2,
                                          Published = nowDateOffset5,
                                          Updated = nowDateOffset9
                                      };

            Assert.IsTrue(atomEntry.Edited.HasValue);

            Assert.AreEqual(nowDateOffset2, atomEntry.Edited.Value);
            Assert.AreEqual(nowDateOffset2.DateTime, atomEntry.Edited.Value.DateTime);
            Assert.AreEqual(nowDateOffset2.Offset, atomEntry.Edited.Value.Offset);

            Assert.IsTrue(atomEntry.Published.HasValue);
            Assert.AreEqual(nowDateOffset5, atomEntry.Published.Value);
            Assert.AreEqual(nowDateOffset5.DateTime, atomEntry.Published.Value.DateTime);
            Assert.AreEqual(nowDateOffset5.Offset, atomEntry.Published.Value.Offset);

            Assert.AreEqual(nowDateOffset9, atomEntry.Updated);
            Assert.AreEqual(nowDateOffset9.DateTime, atomEntry.Updated.DateTime);
            Assert.AreEqual(nowDateOffset9.Offset, atomEntry.Updated.Offset);
        }

        [ TestMethod ]
        public void CreateAtomEntryWithIdPath()
        {
            AtomEntry atomEntry = new AtomEntry
              {
                  Id = new Id("aOwner", "aDate", "aCollection", "aPath,bPath")
              };

            Assert.IsNotNull(atomEntry);
            Assert.IsNotNull(atomEntry.Id);
            Assert.IsNotNull(atomEntry.Id.GetParentId());
        }

    }
}
