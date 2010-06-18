using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    [TestClass]
    public class AppWorkspaceTest
    {
        [TestMethod]
        public void CreateAppWorkspaceWithTitleTest()
        {
            AppWorkspace appWorkspace = new AppWorkspace()
            {
                Title = new AtomTitle()
                {
                    Text = "aTitle"
                }
            };

            Assert.IsNotNull(appWorkspace);
            Assert.IsNotNull(appWorkspace.Title, "No title");
            Assert.AreEqual("aTitle", appWorkspace.Title.Text);
        }

        [TestMethod]
        public void CreateAppWorkspaceWithAllPropertiesTest()
        {
            AppWorkspace appWorkspace = new AppWorkspace()
                {
                    Title = new AtomTitle()
                        {
                            Text = "aTitle"
                        },
                    Name = "aName",
                    Theme = "aTheme",
                    Default = false,
                    Collections = new List<AppCollection>(),
                  //TODO: Authors = new List<AtomPerson>(),
                  //TODO: Contributors = new List<AtomPerson>()
                };

            Assert.IsNotNull(appWorkspace);
            Assert.IsNotNull(appWorkspace.Title, "No title");
            Assert.IsNotNull(appWorkspace.Collections);
            Assert.AreEqual(0, appWorkspace.Collections.Count());

            Assert.IsNotNull(appWorkspace.Authors);
            Assert.AreEqual(0, appWorkspace.Authors.Count());

            Assert.IsNotNull(appWorkspace.Contributors);
            Assert.AreEqual(0, appWorkspace.Contributors.Count());

            Assert.AreEqual("aTitle", appWorkspace.Title.Text);
            Assert.AreEqual("aName", appWorkspace.Name);
            Assert.AreEqual("aTheme", appWorkspace.Theme);
            Assert.AreEqual(false, appWorkspace.Default);
        }

        [TestMethod]
        public void CreateAppWorkspaceWithCollectionsTest()
        {
            AppWorkspace appWorkspace = new AppWorkspace()
            {
                Title = new AtomTitle()
                {
                    Text = "aTitle"
                },
                Collections = TestHelper.MakeAppCollections(4),
            };

            Assert.IsNotNull(appWorkspace);
            Assert.IsNotNull(appWorkspace.Collections);
            Assert.AreEqual(4, appWorkspace.Collections.Count());
        }

        [TestMethod]
        public void CreateAppWorkspaceWithAuthorsTest()
        {
            AppWorkspace appWorkspace = new AppWorkspace()
            {
                Title = new AtomTitle()
                {
                    Text = "aTitle"
                },
                //TODO: Authors = TestHelper.MakePersonList(4, Atom.AtomNs + "author"),
                Authors = Enumerable.Repeat<string>("author",4)
            };
          
            Assert.IsNotNull(appWorkspace);
            Assert.IsNotNull(appWorkspace.Authors);
            Assert.AreEqual(4, appWorkspace.Authors.Count());
        }

        [TestMethod]
        public void CreateAppWorkspaceWithContributorsTest()
        {
            AppWorkspace appWorkspace = new AppWorkspace()
            {
                Title = new AtomTitle()
                {
                    Text = "aTitle"
                },
              //TODO: Contributors = TestHelper.MakePersonList(4, Atom.AtomNs + "contributor"),
                Contributors = Enumerable.Repeat<string>("contributor",4)
            };

            Assert.IsNotNull(appWorkspace);
            Assert.IsNotNull(appWorkspace.Contributors);
            Assert.AreEqual(4, appWorkspace.Contributors.Count());
        }

    }
}
