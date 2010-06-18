using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    [TestClass]
    public class AtomSourceTest
    {
        [TestMethod]
        public void SimpleCreateTest()
        {
            AtomSource atomSource = new AtomSource();

            Assert.IsNotNull(atomSource);
        }

        [TestMethod]
        public void FullCreateTest()
        {
            DateTime updatedDateTime = DateTime.Now;
            AtomSource atomSource = new AtomSource()
            {
                Authors = new List<AtomPerson>(),
                Base = new Uri("http://base.com"),
                Categories = new List<AtomCategory>(),
                Contributors = new List<AtomPerson>(),
                Extensions = new List<XElement>(),
                Generator = new AtomGenerator(),
                Icon = new Uri("http://icon.com"),
                Id = new Uri("tag:test.com:2008,test"),
                Lang = "EN",
                Links = new List<AtomLink>(),
                Logo = new Uri("http://logo.com"),
                Rights = new AtomRights()
                {
                    Text = "aRights"
                },
                Subtitle = new AtomText()
                {
                    Text = "aSubTitle"
                },
                Title = new AtomTitle()
                {
                    Text = "aTitle"
                },
                Updated = updatedDateTime
            };

            Assert.IsNotNull(atomSource);
            Assert.IsNotNull(atomSource.Authors);
            Assert.IsNotNull(atomSource.Contributors);
            Assert.IsNotNull(atomSource.Categories);
            Assert.IsNotNull(atomSource.Extensions);
            Assert.IsNotNull(atomSource.Generator);
            Assert.IsNotNull(atomSource.Icon);
            Assert.IsNotNull(atomSource.Id);
            Assert.IsNotNull(atomSource.Links);
            Assert.IsNotNull(atomSource.Logo);
            Assert.IsNotNull(atomSource.Rights);
            Assert.IsNotNull(atomSource.Subtitle);
            Assert.IsNotNull(atomSource.Title);

            Assert.AreEqual("EN", atomSource.Lang);
            Assert.AreEqual(updatedDateTime, atomSource.Updated);

            Assert.AreEqual("aRights", atomSource.Rights.Text);
            Assert.AreEqual("aSubTitle", atomSource.Subtitle.Text);
            Assert.AreEqual("aTitle", atomSource.Title.Text);

            Assert.AreEqual(new Uri("http://base.com"), atomSource.Base);
            Assert.AreEqual(new Uri("http://icon.com"), atomSource.Icon);
            Assert.AreEqual(new Uri("http://logo.com"), atomSource.Logo);

            Assert.AreEqual(0, atomSource.Authors.Count());
            Assert.AreEqual(0, atomSource.Contributors.Count());
            Assert.AreEqual(0, atomSource.Categories.Count());
            Assert.AreEqual(0, atomSource.Links.Count());
        }

        [TestMethod]
        public void CreateWithBaseTest()
        {
            DateTime updatedDateTime = DateTime.Now;
            AtomSource atomSource = new AtomSource()
                {
                    Base = new Uri("http://base.com"),
                };

            Assert.IsNotNull(atomSource);
            Assert.IsNotNull(atomSource.Base);
            Assert.AreEqual(new Uri("http://base.com"), atomSource.Base);
        }

        [TestMethod]
        public void CreateWithAuthorsTest()
        {
            AtomSource atomSource = new AtomSource()
                {
                    Authors = TestHelper.MakePersonList(5, Atom.AtomNs + "author")
                };

            Assert.IsNotNull(atomSource);
            Assert.IsNotNull(atomSource.Authors);
            Assert.AreEqual(5, atomSource.Authors.Count());
        }

        [TestMethod]
        public void CreateWithContributorsTest()
        {
            AtomSource atomSource = new AtomSource()
            {
                Contributors = TestHelper.MakePersonList(5, Atom.AtomNs + "contributor")
            };

            Assert.IsNotNull(atomSource);
            Assert.IsNotNull(atomSource.Contributors);
            Assert.AreEqual(5, atomSource.Contributors.Count());
        }

        [TestMethod]
        public void CreateWithCategoriesTest()
        {
            AtomSource atomSource = new AtomSource()
            {
                Categories = TestHelper.MakeAtomCategoryList(5)
            };

            Assert.IsNotNull(atomSource);
            Assert.IsNotNull(atomSource.Categories);
            Assert.AreEqual(5, atomSource.Categories.Count());
        }

        [TestMethod]
        public void CreateWithLinksTest()
        {
            AtomSource atomSource = new AtomSource()
            {
                Links = TestHelper.MakeLinks(5)
            };

            Assert.IsNotNull(atomSource);
            Assert.IsNotNull(atomSource.Links);
            Assert.AreEqual(5, atomSource.Links.Count());
        }
    }
}
