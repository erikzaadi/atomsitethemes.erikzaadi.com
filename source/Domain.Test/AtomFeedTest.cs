using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    [TestClass]
    public class AtomFeedTest
    {
        [TestMethod]
        public void SimpleCreateTest()
        {
            AtomFeed atomFeed = new AtomFeed(); 

            Assert.IsNotNull(atomFeed);
        }

        [TestMethod]
        public void CreateWithRightsTest()
        {
            DateTime updatedDate = DateTime.Now;

            AtomFeed atomFeed = new AtomFeed()
            {
                Rights = new AtomRights()
                     {
                         Text = "aText"
                     }
            };

            Assert.IsNotNull(atomFeed);
            Assert.IsNotNull(atomFeed.Rights);
            Assert.AreEqual("aText", atomFeed.Rights.Text);
        }

        [TestMethod]
        public void CreateWithTitleTest()
        {
            DateTime updatedDate = DateTime.Now;

            AtomFeed atomFeed = new AtomFeed()
            {
                Title = new AtomTitle()
                {
                    Text = "aTitle"
                }
            };

            Assert.IsNotNull(atomFeed);
            Assert.IsNotNull(atomFeed.Title);
            Assert.AreEqual("aTitle", atomFeed.Title.Text);
        }

        [TestMethod]
        public void CreateWithSubtitleTest()
        {
            DateTime updatedDate = DateTime.Now;

            AtomFeed atomFeed = new AtomFeed()
            {
                Subtitle = new AtomSubtitle()
                {
                    Text = "aSubtitle"
                }
            };

            Assert.IsNotNull(atomFeed);
            Assert.IsNotNull(atomFeed.Subtitle);
            Assert.AreEqual("aSubtitle", atomFeed.Subtitle.Text);
        }

        [TestMethod]
        public void CreateWithIdTest()
        {
            DateTime updatedDate = DateTime.Now;

            AtomFeed atomFeed = new AtomFeed()
            {
                Id = new Uri("tag:owner@domain.com,2008:collection")
            };

            Assert.IsNotNull(atomFeed);
            Assert.IsNotNull(atomFeed.Id);
        }

        [TestMethod]
        public void FullCreateTest()
        {
            DateTime updatedDate = DateTime.Now;

            AtomFeed atomFeed = new AtomFeed()
                {
                    Authors = new List<AtomPerson>(),
                    Contributors = new List<AtomPerson>(),
                    Base = new Uri("http://base.com"),
                    Categories = new List<AtomCategory>(),
                    Entries = new List<AtomEntry>(),
                    Generator = new AtomGenerator(),
                    Updated = updatedDate,
                    Icon = new Uri("http://icon.com"),
                    Lang = "EN",
                    Links = new List<AtomLink>(),
                    Logo = new Uri("http://logo.com")
                };

            Assert.IsNotNull(atomFeed);
            Assert.IsNotNull(atomFeed.Authors);
            Assert.IsNotNull(atomFeed.Contributors);
            Assert.IsNotNull(atomFeed.Base);
            Assert.AreEqual(new Uri("http://base.com"), atomFeed.Base);
            Assert.IsNotNull(atomFeed.Categories);
            Assert.IsNotNull(atomFeed.Entries);
            Assert.IsNotNull(atomFeed.Generator);
            Assert.AreEqual(updatedDate, atomFeed.Updated);
            Assert.AreEqual(new Uri("http://icon.com"), atomFeed.Icon);
            Assert.AreEqual("EN", atomFeed.Lang);
            Assert.IsNotNull(atomFeed.Links);
            Assert.AreEqual(new Uri("http://logo.com"), atomFeed.Logo);
        }

        [TestMethod]
        public void CreateWithAuthorsTest()
        {
            AtomFeed atomFeed = new AtomFeed()
            {
                Authors = TestHelper.MakePersonList(5, Atom.AtomNs + "author")
            };

            Assert.IsNotNull(atomFeed);
            Assert.IsNotNull(atomFeed.Authors);
            Assert.AreEqual(5, atomFeed.Authors.Count());
        }

        [TestMethod]
        public void CreateWithContributorsTest()
        {
            AtomFeed atomFeed = new AtomFeed()
            {
                Contributors = TestHelper.MakePersonList(5, Atom.AtomNs + "contributor")
            };

            Assert.IsNotNull(atomFeed);
            Assert.IsNotNull(atomFeed.Contributors);
            Assert.AreEqual(5, atomFeed.Contributors.Count());
        }

        [TestMethod]
        public void CreateWithCategoriesTest()
        {
            AtomFeed atomFeed = new AtomFeed()
            {
                Categories = TestHelper.MakeAtomCategoryList(5)
            };

            Assert.IsNotNull(atomFeed);
            Assert.IsNotNull(atomFeed.Categories);
            Assert.AreEqual(5, atomFeed.Categories.Count());
        }

        [TestMethod]
        public void CreateWithLinksTest()
        {
            AtomFeed atomFeed = new AtomFeed()
            {
                Links = TestHelper.MakeLinks(5)
            };

            Assert.IsNotNull(atomFeed);
            Assert.IsNotNull(atomFeed.Links);
            Assert.AreEqual(5, atomFeed.Links.Count());
        }
    }
}
