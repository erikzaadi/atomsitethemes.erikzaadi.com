using System;
using System.Collections.Generic;
using System.Linq;

using AtomSite.Domain;
using AtomSite.Repository;

namespace Repository.Test
{
    public abstract class BaseAtomEntryTest
    {
        protected const string TestWorkspaceName = "TestWorkspace";
        protected string TestCollection1Name;
        protected string TestCollection2Name;

        protected abstract IAtomEntryRepository GetEntryRepository();
        protected abstract IAppServiceRepository GetAppServiceRepository();

        protected void MakeTestCollections(bool uniqueNames)
        {
            if (uniqueNames)
            {
                TestCollection1Name = "TestCollection1_" + Guid.NewGuid();
                TestCollection2Name = "TestCollection2_" + Guid.NewGuid();
            }
            else
            {
                TestCollection1Name = "TestCollection1";
                TestCollection2Name = "TestCollection2";                
            }

            IAppServiceRepository repository = GetAppServiceRepository();

            AppService service = repository.GetService();

            // if the workspace does not exist, make it
            AppWorkspace testWorkspace = service.Workspaces.FindByName(TestWorkspaceName);
            if (testWorkspace == null)
            {
                testWorkspace = new AppWorkspace
                {
                    Title = new AtomText
                    {
                        Text = "Test workspace",
                        Type = "text"
                    },
                    Name = TestWorkspaceName,
                    Default = false
                };

                service.Workspaces = service.Workspaces.Add(testWorkspace);

                repository.UpdateService(service);
            }

            // now check the collection
            service = repository.GetService();
            testWorkspace = service.Workspaces.FindByName(TestWorkspaceName);

            AppCollection testCollection = testWorkspace.Collections.FindByTitle(TestCollection1Name);
            if (testCollection == null)
            {
                testCollection = MakeTestCollection(TestCollection1Name);
                testWorkspace.Collections = testWorkspace.Collections.Add(testCollection);
                service.Workspaces = service.Workspaces.ReplaceByName(testWorkspace);

                repository.UpdateService(service);
            }

            service = repository.GetService();
            testWorkspace = service.Workspaces.FindByName(TestWorkspaceName);

            AppCollection testCollection2 = testWorkspace.Collections.FindByTitle(TestCollection2Name);
            if (testCollection2 == null)
            {
                testCollection2 = MakeTestCollection(TestCollection2Name);
                testWorkspace.Collections = testWorkspace.Collections.Add(testCollection2);
                service.Workspaces = service.Workspaces.ReplaceByName(testWorkspace);

                repository.UpdateService(service);
            }
        }

        /// <summary>
        /// remove test data that can clutter up the database
        /// </summary>
        protected void DeleteTestCollections()
        {
            IAppServiceRepository repository = GetAppServiceRepository();

            AppService service = repository.GetService();

            AppWorkspace workspace = service.Workspaces.FindByName(TestWorkspaceName);

            IEnumerable<AppCollection> collections = 
                workspace.Collections.RemoveTitles(TestCollection1Name, TestCollection2Name);

            workspace.Collections = collections;
            service.Workspaces = service.Workspaces.ReplaceByName(workspace);

            repository.UpdateService(service);
        }

        private AppCollection MakeTestCollection(string collectionTitle)
        {
            string owner = MakeOwnerFromWorkspaceName(TestWorkspaceName);

            var result = new AppCollection
            {
                Title = new AtomText
                {
                    Text = collectionTitle,
                    Type = "text"
                },
                Id = new Id(owner, DateTime.Now, collectionTitle, ""),
                Theme = "aTheme"
            };

            result.Categories = MakeCollectionCategories();
            return result;
        }

        private static string MakeOwnerFromWorkspaceName(string testWorkspaceName)
        {
            return string.Format("{0}.someblog.com", testWorkspaceName);
        }

        private IEnumerable<AppCategories> MakeCollectionCategories()
        {
            AppCategories cats = new AppCategories
            {
                Scheme = new Uri("http://TestScheme.com"),
                Fixed = false,
            };

            // add two categories
            cats.Categories = new List<AtomSite.Domain.AtomCategory>
                {
                    TestDataHelper.MakeTestCategory(), TestDataHelper.MakeTestCategory()
                };

            return cats.InList();
        }

        protected AtomCategory GetCategory(bool first)
        {
            IAppServiceRepository repository = GetAppServiceRepository();

            AppService service = repository.GetService();

            AppWorkspace testWorkspace = service.Workspaces.FindByName(TestWorkspaceName);
            string collectionTitle = first ? TestCollection1Name : TestCollection2Name;
            AppCollection testCollection = testWorkspace.Collections.FindByTitle(collectionTitle);

            AppCategories cats = testCollection.Categories.First();
            return cats.Categories.First();
        }

    }
}
