using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    [TestClass]
    public class AtomContentTest
    {
        [TestMethod]
        public void SimpleCreateAtomContentTest()
        {
            AtomContent atomContent = new AtomContent();

            Assert.IsNotNull(atomContent);
        }

        [TestMethod]
        public void FullCreateAtomContentTest()
        {
            AtomContent atomContent = new AtomContent()
              {
                  Base = new Uri("http://base.com"),
                  Lang = "EN",
                  Src = new Uri("http://src.com"),
                  Text = "This is the text",
                  Type = "aType"
              };

            Assert.IsNotNull(atomContent);
            Assert.AreEqual(new Uri("http://base.com"), atomContent.Base);
            Assert.AreEqual("EN", atomContent.Lang);
            Assert.AreEqual(new Uri("http://src.com"), atomContent.Src);
            Assert.AreEqual("This is the text", atomContent.Text);
            Assert.AreEqual("aType", atomContent.Type);
        }
    }
}
