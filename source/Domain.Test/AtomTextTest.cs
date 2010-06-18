using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    /// <summary>
    /// Test the AppControl domain object
    /// </summary>
    [TestClass]
    public class AtomTextTest
    {
        [TestMethod]
        public void SimpleCreateAtomTextTest()
        {
            AtomText atomText = new AtomText();
            Assert.IsNotNull(atomText);
        }

        [TestMethod]
        public void CreateFullAtomTextTest()
        {
            AtomText atomText = new AtomText()
                {
                    Base = new Uri("http://base.com"),
                    Lang = "EN",
                    Text = "Hello",
                    Type = "atype"
                };

            Assert.IsNotNull(atomText);
            Assert.AreEqual(new Uri("http://base.com"), atomText.Base);
            Assert.AreEqual("EN", atomText.Lang);
            Assert.AreEqual("Hello", atomText.Text);
            Assert.AreEqual("atype", atomText.Type);
        }

        [TestMethod]
        public void SimpleCreateAtomTitleTest()
        {
            AtomTitle atomTitle = new AtomTitle();
            Assert.IsNotNull(atomTitle);
        }

        [TestMethod]
        public void SimpleCreateAtomSubtitleTest()
        {
            AtomSubtitle atomSubTitle = new AtomSubtitle();
            Assert.IsNotNull(atomSubTitle);
        }

        [TestMethod]
        public void SimpleCreateAtomSummaryTest()
        {
            AtomSummary atomSummary = new AtomSummary();
            Assert.IsNotNull(atomSummary);
        }

        [TestMethod]
        public void SimpleCreateAtomRightsTest()
        {
            AtomRights atomRights = new AtomRights();
            Assert.IsNotNull(atomRights);
        }

    }
}
