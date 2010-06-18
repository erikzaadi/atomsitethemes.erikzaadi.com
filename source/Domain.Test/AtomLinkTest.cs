using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    [TestClass]
    public class AtomLinkTest
    {
        [TestMethod]
        public void SimpleCreateAtomLinkTest()
        {
            AtomLink atomLink = new AtomLink();

            Assert.IsNotNull(atomLink);
        }

        [TestMethod]
        public void FullCreateAtomLinkTest()
        {
            DateTime updatedDate = DateTime.Now;

            AtomLink atomLink = new AtomLink()
            {
                Base = new Uri("http://base.com"),
                Count = 0,
                Href = new Uri("http://href.com"),
                Lang = "EN",
                Title = "Test Link",
                HrefLang = "EN",
                Length = "10",
                Rel = "foo",
                Type = "http",
                Updated = updatedDate
            };

            Assert.IsNotNull(atomLink);
            Assert.IsNotNull(atomLink.Base);
            Assert.IsNotNull(atomLink.Href);

            Assert.AreEqual(new Uri("http://base.com"), atomLink.Base);
            Assert.AreEqual(0, atomLink.Count);
            Assert.AreEqual(new Uri("http://href.com"), atomLink.Href);
            Assert.AreEqual("EN", atomLink.Lang);
            Assert.AreEqual("Test Link", atomLink.Title);
            Assert.AreEqual("EN", atomLink.HrefLang);
            Assert.AreEqual("10", atomLink.Length);
            Assert.AreEqual("foo", atomLink.Rel);
            Assert.AreEqual("http", atomLink.Type);
            Assert.AreEqual(updatedDate, atomLink.Updated);
        }
    }
}
