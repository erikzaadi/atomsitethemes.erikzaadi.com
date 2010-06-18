using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    [TestClass]
    public class AtomCategoryTest
    {
        [TestMethod]
        public void SimpleCreateAtomCategoryTest()
        {
            AtomCategory atomCategory = new AtomCategory();

            Assert.IsNotNull(atomCategory);
        }

        [TestMethod]
        public void FullCreateAtomCategoryTest()
        {
            AtomCategory atomCategory = new AtomCategory()
                {
                    Label = "aLabel",
                    Lang = "EN",
                    Scheme = new Uri("http://scheme.com"),
                    Term = "term",
                    Base = new Uri("http://base.com")
                };

            Assert.IsNotNull(atomCategory);
            Assert.AreEqual("aLabel", atomCategory.Label);
            Assert.AreEqual("EN", atomCategory.Lang);

            Assert.AreEqual(new Uri("http://scheme.com"), atomCategory.Scheme);
            Assert.AreEqual("term", atomCategory.Term);
            Assert.AreEqual(new Uri("http://base.com"), atomCategory.Base);
        }
    }
}
