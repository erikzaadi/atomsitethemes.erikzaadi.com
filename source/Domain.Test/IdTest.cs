using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    [TestClass]
    public class IdTest
    {
        public const string ValidTagForTest = "tag:owner,2008:collection,entryPath";
        public const string InvalidTagForTest = "http://example.com";

        [TestMethod]
        public void CastFromValidUriTest()
        {
            Id id = new Uri(ValidTagForTest);

            Assert.IsNotNull(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CastFromInvalidUriTest()
        {
            Id id = new Uri(InvalidTagForTest);
        }

        [TestMethod]
        public void CastFromValidStringTest()
        {
            Id id = ValidTagForTest;

            Assert.IsNotNull(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CastFromInvalidStringTest()
        {
            Id id = InvalidTagForTest;
        }

        [TestMethod]
        public void CreateWithValidTagTest()
        {
            Id id = new Uri(ValidTagForTest);

            Assert.IsNotNull(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateWithInvalidTagTest()
        {
            Id id = new Uri(InvalidTagForTest);

            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void CreateWithOwnerCollectionTest()
        {
            Id id = new Id("aOwner", "aCollection");

            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void CreateWithOwnerDateCollectionTest()
        {
            Id id = new Id("aOwner", "aDate", "aCollection");

            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void CreateWithOwnerDateCollectionPathTest()
        {
            Id id = new Id("aOwner", DateTime.Now, "aCollection", "aPath");

            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void CreateWithOwnerDateStringCollectionPathTest()
        {
            Id id = new Id("aOwner", "aDate", "aCollection", "aPath");

            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void GetParentIdNullTest()
        {
            Id baseId = new Id("aOwner", "aDate", "aCollection", "aPath");

            Id parent = baseId.GetParentId();

            Assert.IsNull(parent);
        }

        [TestMethod] public void GetParentIdValidTest()
        {
            Id baseId = new Id("aOwner", "aDate", "aCollection", "aPath");

            Id ChildId = new Id(baseId.Owner, baseId.Date, baseId.Collection, baseId.EntryPath + ", bPath");

            Id parent = ChildId.GetParentId();

            Assert.IsNotNull(parent);

            Assert.AreEqual(baseId.Owner, parent.Owner);
            Assert.AreEqual(baseId.Date, parent.Date);
            Assert.AreEqual(baseId.Collection, parent.Collection);
            Assert.AreEqual(baseId.EntryPath, parent.EntryPath);
        }
    }
}
