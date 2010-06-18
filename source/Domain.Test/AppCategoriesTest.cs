using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    /// <summary>
    /// Test the AppCategories domain object
    /// </summary>
    [TestClass]
    public class AppCategoriesTest
    {
        [TestMethod]
        public void SimpleCreateAppCategoriesTest()
        {
            AppCategories categories = new AppCategories();

            Assert.IsNotNull(categories);
        }

        [TestMethod]
        public void CreateAppCategoriesWithEmptyCategoriesTest()
        {
            AppCategories categories = new AppCategories()
               {
                   Scheme = new Uri("http://foo.com"),
                   Fixed = true,
                   Href = null,
                   Categories = new List<AtomCategory>()
               };

            Assert.IsNotNull(categories);

            Assert.AreEqual(new Uri("http://foo.com"), categories.Scheme);
            Assert.IsTrue(categories.Fixed.HasValue);
            Assert.IsTrue(categories.Fixed.Value);
            Assert.IsNull(categories.Href);
            Assert.IsNotNull(categories.Categories);
            Assert.AreEqual(0, categories.Categories.Count());
        }

        [TestMethod]
        public void CreatAppCategorieseWithHrefTest()
        {
            AppCategories categories = new AppCategories()
                {
                    Scheme = new Uri("http://foo.com"),
                    Fixed = true,
                    Href = new Uri("http://bar.com"),
                };

            Assert.IsNotNull(categories);

            Assert.AreEqual(new Uri("http://foo.com"), categories.Scheme);
            Assert.IsTrue(categories.Fixed.HasValue);
            Assert.IsTrue(categories.Fixed.Value);
            Assert.AreEqual(new Uri("http://bar.com"), categories.Href);
            Assert.IsNull(categories.Categories);
        }

        [TestMethod]
        public void CreateAppCategoriesNotFixedTest()
        {
            AppCategories categories = new AppCategories()
            {
                Scheme = new Uri("http://foo.com"),
                Fixed = false,
                Href = null,
                Categories = new List<AtomCategory>()
            };

            Assert.IsNotNull(categories);

            Assert.AreEqual(new Uri("http://foo.com"), categories.Scheme);
            Assert.IsTrue(categories.Fixed.HasValue);
            Assert.IsFalse(categories.Fixed.Value);
            Assert.IsNull(categories.Href);
            Assert.IsNotNull(categories.Categories);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreatAppCategorieseWithBothTest()
        {
            AppCategories categories = new AppCategories()
               {
                   Scheme = new Uri("http://foo.com"),
                   Fixed = true,
                   Href = new Uri("http://bar.com"),
                   Categories = new List<AtomCategory>()
               };

        }

        [TestMethod]
        public void CreateAppCategoriesWithCategoryListTest()
        {
            AppCategories categories = new AppCategories()
            {
                Categories = TestHelper.MakeAtomCategoryList(5),
            };

            Assert.IsNotNull(categories);

            Assert.IsNotNull(categories.Categories);
            Assert.AreEqual(5, categories.Categories.Count());
        }
    }
}
