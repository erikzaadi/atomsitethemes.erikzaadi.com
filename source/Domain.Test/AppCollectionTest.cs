using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    /// <summary>
    /// Test the AppCategories domain object
    /// </summary>
    [TestClass]
    public class AppCollectionTest
    {
        [TestMethod]
        public void SimpleCreateAppCollectionTest()
        {
             AppCollection appCollection = new AppCollection();

             Assert.IsNotNull(appCollection);
         }

        [TestMethod]
        public void CreateWithFlagsFalseAppCollectionTest()
        {
            AppCollection appCollection = new AppCollection
            {
                Default = false,
                Dated = false,
            };

            Assert.IsNotNull(appCollection);
            Assert.AreEqual(false, appCollection.Default);
            Assert.AreEqual(false, appCollection.Dated);
        }

        [TestMethod]
        public void CreateWithFlagsTrueAppCollectionTest()
        {
            AppCollection appCollection = new AppCollection
            {
                Default = true,
                Dated = true,
            };

            Assert.IsNotNull(appCollection);
            Assert.AreEqual(true, appCollection.Default);
            Assert.AreEqual(true, appCollection.Dated);
        }


        [TestMethod]
        public void FullCreateAppCollectionTest()
        {
            AppCollection appCollection = new AppCollection
                  {
                      Title = new AtomTitle
                          {
                              Text = "A title"
                          },
                      Href = new Uri("http://www.foo.com"),
                      Default = false,
                      Dated = true,
                      Accepts = new List<AppAccept> { new AppAccept("text"), new AppAccept("html")},
                      Categories = new List<AppCategories>(),
                      Id = new Id("test", DateTime.Now, "test", "test"),
                      Subtitle = new AtomSubtitle
                           {
                               Text = "A subtitle"
                           },
                      Icon = new Uri("http://www.icon.com"),
                      Logo = new Uri("http://www.logo.com"),
                      Rights = new AtomRights
                           {
                               Text = "All rights reversed"
                           },
                      Theme = "bTheme",
                    //TODO: Authors = new List<AtomPerson>(),
                    //TODO: Contributors = new List<AtomPerson>()
                  };

            Assert.IsNotNull(appCollection);
            Assert.IsNotNull(appCollection.Title, "No title found");
            Assert.AreEqual("A title", appCollection.Title.Text);

            Assert.IsNotNull(appCollection.Href);
            Assert.AreEqual(new Uri("http://www.foo.com"), appCollection.Href);

            // default set false but is now true - is this expected ?
            Assert.AreEqual(false, appCollection.Default);
            Assert.AreEqual(true, appCollection.Dated);

            Assert.IsNotNull(appCollection.Accepts);
            Assert.AreEqual(2, appCollection.Accepts.Count());

            Assert.IsNotNull(appCollection.Categories);
            Assert.IsNotNull(appCollection.Id);
            Assert.IsNotNull(appCollection.Subtitle);
            Assert.AreEqual("A subtitle", appCollection.Subtitle.Text);


            Assert.IsNotNull(appCollection.Icon);
            Assert.AreEqual(new Uri("http://www.icon.com"), appCollection.Icon);

            Assert.IsNotNull(appCollection.Logo);
            Assert.AreEqual(new Uri("http://www.logo.com"), appCollection.Logo);

            Assert.IsNotNull(appCollection.Rights);
            Assert.AreEqual("All rights reversed", appCollection.Rights.Text);
            Assert.AreEqual("bTheme", appCollection.Theme);

            Assert.IsNotNull(appCollection.Authors);
            Assert.AreEqual(0, appCollection.Authors.Count());

            Assert.IsNotNull(appCollection.Contributors);
            Assert.AreEqual(0, appCollection.Contributors.Count());
        }

        [TestMethod]
        public void CreateWithAuthorsTest()
        {
            AppCollection appCollection = new AppCollection
            {
                //TODO: Authors = null;//TestHelper.MakePersonList(5, Atom.AtomNs + "author")
                Authors = Enumerable.Repeat<string>("author", 5)
            };

            Assert.IsNotNull(appCollection);
            Assert.IsNotNull(appCollection.Authors);
            // authors are not present - is this expected?
            Assert.AreEqual(5, appCollection.Authors.Count());
        }

        [TestMethod]
        public void CreateWithContributorsTest()
        {
            AppCollection appCollection = new AppCollection
            {
              //TODO: Contributors = TestHelper.MakePersonList(5, Atom.AtomNs+"contributor")
              Contributors = Enumerable.Repeat<string>("contributor", 5)
            };

            Assert.IsNotNull(appCollection);
            Assert.IsNotNull(appCollection.Contributors);
            // contribs are not present - is this expected?
            Assert.AreEqual(5, appCollection.Contributors.Count());
        }


        [TestMethod]
        public void CreateWithCategoriesTest()
        {
            AppCollection appCollection = new AppCollection
            {
                Categories = TestHelper.MakeAppCategoriesList(5)
            };

            Assert.IsNotNull(appCollection);
            Assert.IsNotNull(appCollection.Categories);
            Assert.AreEqual(5, appCollection.Categories.Count());
        }
    }
}
