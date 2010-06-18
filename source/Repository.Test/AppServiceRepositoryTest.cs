using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;
using AtomSite.Repository;
using AtomSite.Repository.Mock;
using AtomSite.Repository.File;

namespace Repository.Test
{
    /// <summary>
    /// Base class to run tests against an IAppServiceRepository
    /// </summary>
    public abstract class BaseAppServiceRepositoryTest
    {
        #region private

        private const int PageSize = 1000;

        protected abstract IAppServiceRepository GetRepository();

        #endregion

        #region properties

        public TestContext TestContext { get; set; }

        #endregion


        #region tests

        /// <summary>
        /// The simplest test - can the object be created
        /// </summary>
        [TestMethod]
        public void CreateAppServiceRepositoryNotNullTest()
        {
            IAppServiceRepository repository = GetRepository();

            Assert.IsNotNull(repository);
        }

        [TestMethod]
        public void GetAppServiceTest()
        {
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Admins);
            Assert.IsNotNull(appService.Workspaces);

            Assert.IsTrue(
                (appService.ServiceType == ServiceType.Single) ||
                (appService.ServiceType == ServiceType.MultiFolder) ||
                (appService.ServiceType == ServiceType.MultiSubdomain) );

            Assert.IsFalse(String.IsNullOrEmpty(appService.DefaultSubdomain));
        }

        [TestMethod]
        public void UpdateAppServiceTypeTest()
        {
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();
            ServiceType originalServiceType = appService.ServiceType;

            Assert.IsNotNull(appService, "no app service");

            appService.ServiceType = ServiceType.MultiFolder;

            AppService updatedAppService = repository.UpdateService(appService);

            Assert.AreEqual(ServiceType.MultiFolder, updatedAppService.ServiceType);

            updatedAppService.ServiceType = originalServiceType;
            repository.UpdateService(updatedAppService);
        }

        [TestMethod]
        public void UpdateAppServiceAddAdminTest()
        {
            IAppServiceRepository repository = GetRepository();

            // get the app service admin list
            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Admins);

            int initialCount = appService.Admins.Count();

            // put a new person in it, should increase the count
            AtomSite.Domain.AtomPerson newAdmin = TestDataHelper.MakeTestPerson();

            //TODO: appService.Admins = appService.Admins.Add(newAdmin);

            Assert.AreEqual(initialCount + 1, appService.Admins.Count());

            // persist it
            repository.UpdateService(appService);
            
            // and reload, check the count
            // to verify that the data was saved and loaded

            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();

            int countWithAddedItem = loadedAppService.Admins.Count();

            Assert.AreEqual(initialCount + 1, countWithAddedItem);

            // remove a person, check that the count goes down
            loadedAppService.Admins = loadedAppService.Admins.RemoveAt(initialCount);
            secondRepository.UpdateService(loadedAppService);

            IAppServiceRepository thirdRepository = GetRepository();
            loadedAppService = thirdRepository.GetService();
            int countAfterRemove = loadedAppService.Admins.Count();

            Assert.AreEqual(initialCount, countAfterRemove);
        }

        [TestMethod]
        public void UpdateAppServiceAddWorkspaceTest()
        {
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Workspaces);

            int initialCount = appService.Workspaces.Count();

            // put a new workspace in it
            AtomSite.Domain.AppWorkspace newWorkspace = TestDataHelper.MakeTestWorkspace();
            appService.Workspaces = appService.Workspaces.Add(newWorkspace);

            Assert.AreEqual(initialCount + 1, appService.Workspaces.Count());

            // persist it
            repository.UpdateService(appService);

            // and reload, check the count
            // to verify that the data was saved and loaded

            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();

            int countWithAddedItem = loadedAppService.Workspaces.Count();

            Assert.AreEqual(initialCount + 1, countWithAddedItem);

            // remove the last workspace, check that the count goes down
            loadedAppService.Workspaces = loadedAppService.Workspaces.RemoveAt(initialCount);
            secondRepository.UpdateService(loadedAppService);

            IAppServiceRepository thirdRepository = GetRepository();
            loadedAppService = thirdRepository.GetService();
            int countAfterRemove = loadedAppService.Workspaces.Count();

            Assert.AreEqual(initialCount, countAfterRemove);
        }

        [TestMethod]
        public void UpdateAppServiceAddWorkspaceCollectionTest()
        {
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Workspaces);

            // put a new workspace in it
            AtomSite.Domain.AppWorkspace newWorkspace = TestDataHelper.MakeTestWorkspace();
            string workspaceName = newWorkspace.Name;

            // containing a collection
            AtomSite.Domain.AppCollection newCollection = TestDataHelper.MakeTestAppCollection();
            string collectionTitle = newCollection.Title.Text;

            newWorkspace.Collections = newCollection.InList();
            appService.Workspaces = appService.Workspaces.Add(newWorkspace);

            // persist it
            repository.UpdateService(appService);

            // and reload, check the collection
            // to verify that the data was saved and loaded

            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();

            AppCollection loadedCollection = loadedAppService.Workspaces.FindByName(workspaceName).Collections.FindByTitle(collectionTitle);

            Assert.IsNotNull(loadedCollection);
            Assert.AreEqual(collectionTitle, loadedCollection.Title.Text);
            Assert.AreEqual("aTheme", loadedCollection.Theme);

            // delete the workspaces
            DeleteWorkspace(secondRepository, workspaceName);
        }

        [TestMethod]
        public void UpdateAppServiceRemoveWorkspaceCollectionTest()
        {
            // step one - create a workspace containing two collections
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Workspaces);

            // put a new workspaces in it
            AtomSite.Domain.AppWorkspace newWorkspace = TestDataHelper.MakeTestWorkspace();
            string workspaceName = newWorkspace.Name;

            // containing 2 collections
            AtomSite.Domain.AppCollection newCollection1 = TestDataHelper.MakeTestAppCollection();
            string collectionTitle1 = newCollection1.Title.Text;

            AtomSite.Domain.AppCollection newCollection2 = TestDataHelper.MakeTestAppCollection();
            string collectionTitle2 = newCollection2.Title.Text;

            newWorkspace.Collections = newWorkspace.Collections.Add(newCollection1, newCollection2);
            appService.Workspaces = appService.Workspaces.Add(newWorkspace);

            // persist it
            repository.UpdateService(appService);

            // step 2: reload, delete a collection
            // to verify that the data was saved and loaded
            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();

            // have to move workspaces out into a list to alter them
            List<AppWorkspace> workspaces = new List<AppWorkspace>(loadedAppService.Workspaces);
            AtomSite.Domain.AppWorkspace keptWorkspace = loadedAppService.Workspaces.FindByName(workspaceName);

            Assert.IsNotNull(keptWorkspace);

            AppCollection keptCollection = keptWorkspace.Collections.FindByTitle(collectionTitle2);
            Assert.IsNotNull(keptCollection);

            int countBefore = keptWorkspace.Collections.Count();

            // keep this collection, remove the other one
            keptWorkspace.Collections = keptCollection.InList();

            // persist it
            appService.Workspaces = workspaces;
            secondRepository.UpdateService(appService);

            // Step 3: load it again, one should be left
            IAppServiceRepository thirdRepository = GetRepository();

            AppService modifiedAppService = thirdRepository.GetService();

            keptWorkspace = modifiedAppService.Workspaces.FindByName(workspaceName);
            Assert.IsNotNull(keptWorkspace);

            keptCollection = keptWorkspace.Collections.FindByTitle(collectionTitle2);
            Assert.IsNotNull(keptCollection);

            AppCollection removedCollection = keptWorkspace.Collections.FindByTitle(collectionTitle1);
            Assert.IsNull(removedCollection, "Collection was not removed");

            int countAfter = keptWorkspace.Collections.Count();
            Assert.AreEqual(countBefore - 1, countAfter);
        }

        [TestMethod]
        public void UpdateAppServiceUpdateWorkspaceCollectionTest()
        {
            // step one - create a workspace containing a collection
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Workspaces);

            // put a new workspaces in it
            AtomSite.Domain.AppWorkspace newWorkspace = TestDataHelper.MakeTestWorkspace();
            string workspaceName = newWorkspace.Name;

            // containing a collection
            AtomSite.Domain.AppCollection newCollection = TestDataHelper.MakeTestAppCollection();
            string collectionTitle = newCollection.Title.Text;

            newWorkspace.Collections = newCollection.InList();

            appService.Workspaces = appService.Workspaces.Add(newWorkspace);

            // persist it
            repository.UpdateService(appService);

            // step 2: reload, alter a collection
            // to verify that the data was saved and loaded
            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();

            // have to move workspaces out into a list to alter them
            List<AppWorkspace> workspaces = new List<AppWorkspace>(loadedAppService.Workspaces);
            newWorkspace = workspaces.FindByName(workspaceName);

            Assert.IsNotNull(newWorkspace);

            newCollection = newWorkspace.Collections.FindByTitle(collectionTitle);
            Assert.IsNotNull(newCollection);
            newCollection.Theme = "ThemeB";            

            // persist it
            newWorkspace.Collections = newCollection.InList();
            appService.Workspaces = workspaces;

            secondRepository.UpdateService(appService);

            // Step 3: load it again, check it
            IAppServiceRepository thirdRepository = GetRepository();

            AppService modifiedAppService = thirdRepository.GetService();

            newWorkspace = modifiedAppService.Workspaces.FindByName(workspaceName);
            Assert.IsNotNull(newWorkspace);

            newCollection = newWorkspace.Collections.FindByTitle(collectionTitle);
            Assert.IsNotNull(newCollection);
            Assert.AreEqual("ThemeB", newCollection.Theme);

            // delete the workspaces
            DeleteWorkspace(secondRepository, workspaceName);
        }

        [TestMethod]
        public void UpdateAppServiceAddWorkspaceAuthorTest()
        {
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Workspaces);

            // put a new workspace in it
            AtomSite.Domain.AppWorkspace newWorkspace = TestDataHelper.MakeTestWorkspace();
            string workspaceName = newWorkspace.Name;

            // containing an author
            AtomSite.Domain.AtomPerson newAuthor = TestDataHelper.MakeTestPerson();

            //TODO: newWorkspace.Authors = newAuthor.InList();
            appService.Workspaces = appService.Workspaces.Add(newWorkspace);

            // persist it
            repository.UpdateService(appService);

            // and reload, check the collection
            // to verify that the data was saved and loaded

            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();
            Assert.IsNotNull(loadedAppService);

            AppWorkspace loadedWorkspace = loadedAppService.Workspaces.FindByName(workspaceName);

            Assert.IsNotNull(loadedWorkspace);
            Assert.AreEqual(1, loadedWorkspace.Authors.Count());

            //TODO: AtomSite.Domain.AtomPerson loadedPerson = loadedWorkspace.Authors.First();

            DataTester.AssertPersonsEqual(newAuthor, null);

            // delete the workspaces
            DeleteWorkspace(secondRepository, workspaceName);
        }

        [TestMethod]
        public void UpdateAppServiceAddWorkspaceContributorTest()
        {
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Workspaces);

            // put a new workspace in it
            AtomSite.Domain.AppWorkspace newWorkspace = TestDataHelper.MakeTestWorkspace();
            string workspaceName = newWorkspace.Name;

            // containing an author
            AtomSite.Domain.AtomPerson newContributor = TestDataHelper.MakeTestPerson();

            newWorkspace.Contributors = null;//TODO: newContributor.InList();
            appService.Workspaces = appService.Workspaces.Add(newWorkspace);

            // persist it
            repository.UpdateService(appService);

            // and reload, check the collection
            // to verify that the data was saved and loaded

            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();
            Assert.IsNotNull(loadedAppService);

            AppWorkspace loadedWorkspace = loadedAppService.Workspaces.FindByName(workspaceName);

            Assert.IsNotNull(loadedWorkspace);
            Assert.AreEqual(1, loadedWorkspace.Contributors.Count());

            AtomSite.Domain.AtomPerson loadedPerson = null;//TODO: loadedWorkspace.Contributors.First();

            DataTester.AssertPersonsEqual(newContributor, loadedPerson);

            // delete the workspaces
            DeleteWorkspace(secondRepository, workspaceName);
        }

        [TestMethod]
        public void UpdateAppServiceDeleteWorkspaceAuthorTest()
        {
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Workspaces);

            // put a new workspace in it
            AtomSite.Domain.AppWorkspace newWorkspace = TestDataHelper.MakeTestWorkspace();
            string workspaceName = newWorkspace.Name;

            // containing two authors
            AtomSite.Domain.AtomPerson newAuthor1 = TestDataHelper.MakeTestPerson();
            AtomSite.Domain.AtomPerson newAuthor2 = TestDataHelper.MakeTestPerson();

            newWorkspace.Authors = null; //TODO: new List<AtomSite.Domain.AtomPerson> { newAuthor1, newAuthor2 };
            appService.Workspaces = appService.Workspaces.Add(newWorkspace);

            // persist it
            repository.UpdateService(appService);

            // and reload, check the collection
            // to verify that the data was saved and loaded

            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();
            Assert.IsNotNull(loadedAppService);

            AppWorkspace loadedWorkspace = loadedAppService.Workspaces.FindByName(workspaceName);

            Assert.IsNotNull(loadedWorkspace);
            Assert.AreEqual(2, loadedWorkspace.Authors.Count());

            // now remove one
            loadedWorkspace.Authors = loadedWorkspace.Authors.RemoveAt(0);
            appService.Workspaces = appService.Workspaces.ReplaceByName(loadedWorkspace);

            // persist it
            secondRepository.UpdateService(appService);

            loadedAppService = secondRepository.GetService();
            Assert.IsNotNull(loadedAppService);

            loadedWorkspace = loadedAppService.Workspaces.FindByName(workspaceName);

            Assert.IsNotNull(loadedWorkspace);
            Assert.AreEqual(1, loadedWorkspace.Authors.Count());

            // delete the workspaces
            DeleteWorkspace(secondRepository, workspaceName);
        }

        [TestMethod]
        public void UpdateAppServiceAddWorkspaceCollectionAuthorTest()
        {
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Workspaces);

            // put a new workspace in it
            AtomSite.Domain.AppWorkspace newWorkspace = TestDataHelper.MakeTestWorkspace();
            string workspaceName = newWorkspace.Name;

            // containing a collection
            AtomSite.Domain.AppCollection newCollection = TestDataHelper.MakeTestAppCollection();
            string collectionTitle = newCollection.Title.Text;

            // containing an author
            AtomSite.Domain.AtomPerson newAuthor = TestDataHelper.MakeTestPerson();

            newCollection.Authors = null;//TODO: newAuthor.InList();
            newWorkspace.Collections = newCollection.InList();
            appService.Workspaces = appService.Workspaces.Add(newWorkspace);

            // persist it
            repository.UpdateService(appService);

            // and reload, check the collection
            // to verify that the data was saved and loaded

            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();
            Assert.IsNotNull(loadedAppService);

            AppCollection loadedCollection = loadedAppService.Workspaces.FindByName(workspaceName).Collections.FindByTitle(collectionTitle);

            Assert.IsNotNull(loadedCollection);
            Assert.AreEqual(1, loadedCollection.Authors.Count());

            AtomSite.Domain.AtomPerson loadedPerson = null;//TODO: loadedCollection.Authors.First();

            DataTester.AssertPersonsEqual(newAuthor, loadedPerson);

            // delete the workspaces
            DeleteWorkspace(secondRepository, workspaceName);
        }

        [TestMethod]
        public void UpdateAppServiceAddWorkspaceCollectionContributorTest()
        {
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Workspaces);

            // put a new workspace in it
            AtomSite.Domain.AppWorkspace newWorkspace = TestDataHelper.MakeTestWorkspace();
            string workspaceName = newWorkspace.Name;

            // containing a collection
            AtomSite.Domain.AppCollection newCollection = TestDataHelper.MakeTestAppCollection();
            string collectionTitle = newCollection.Title.Text;

            // containing a contributor
            AtomSite.Domain.AtomPerson newContributor = TestDataHelper.MakeTestPerson();

            newCollection.Contributors = null;// newContributor.InList();
            newWorkspace.Collections = newCollection.InList();
            appService.Workspaces = appService.Workspaces.Add(newWorkspace);

            // persist it
            repository.UpdateService(appService);

            // and reload, check the collection
            // to verify that the data was saved and loaded

            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();
            Assert.IsNotNull(loadedAppService);

            AppCollection loadedCollection = loadedAppService.Workspaces.FindByName(workspaceName).Collections.FindByTitle(collectionTitle);

            Assert.IsNotNull(loadedCollection);
            Assert.AreEqual(1, loadedCollection.Contributors.Count());

            AtomSite.Domain.AtomPerson loadedPerson = null;// loadedCollection.Contributors.First();

            DataTester.AssertPersonsEqual(newContributor, loadedPerson);

            // delete the workspaces
            DeleteWorkspace(secondRepository, workspaceName);
        }

        [TestMethod]
        public void UpdateAppServiceAddWorkspaceCollectionAcceptsTest()
        {
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Workspaces);

            // put a new workspace in it
            AtomSite.Domain.AppWorkspace newWorkspace = TestDataHelper.MakeTestWorkspace();
            string workspaceName = newWorkspace.Name;

            // containing a collection
            AtomSite.Domain.AppCollection newCollection = TestDataHelper.MakeTestAppCollection();
            string collectionTitle = newCollection.Title.Text;

            // containing an accepts
            newCollection.Accepts = new AppAccept() { Value = "fish" }.InList();
            newWorkspace.Collections = newCollection.InList();
            appService.Workspaces = appService.Workspaces.Add(newWorkspace);

            // persist it
            repository.UpdateService(appService);

            // and reload, check the collection
            // to verify that the data was saved and loaded

            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();
            Assert.IsNotNull(loadedAppService);

            AppCollection loadedCollection = loadedAppService.Workspaces.FindByName(workspaceName).Collections.FindByTitle(collectionTitle);

            Assert.IsNotNull(loadedCollection);
            Assert.AreEqual(1, loadedCollection.Accepts.Count());

            AppAccept loadedAccepts = loadedCollection.Accepts.First();

            Assert.AreEqual(new AppAccept() { Value = "fish" }, loadedAccepts);

            // delete the workspaces
            DeleteWorkspace(secondRepository, workspaceName);
        }

        [TestMethod]
        public void UpdateAppServiceAddWorkspaceCollectionTwoAcceptsTest()
        {
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Workspaces);

            // put a new workspace in it
            AtomSite.Domain.AppWorkspace newWorkspace = TestDataHelper.MakeTestWorkspace();
            string workspaceName = newWorkspace.Name;

            // containing a collection
            AtomSite.Domain.AppCollection newCollection = TestDataHelper.MakeTestAppCollection();
            string collectionTitle = newCollection.Title.Text;

            // containing an accepts
            newCollection.Accepts = new List<AppAccept> {new AppAccept("foo"), new AppAccept("bar")};
            newWorkspace.Collections = newCollection.InList();
            appService.Workspaces = appService.Workspaces.Add(newWorkspace);

            // persist it
            repository.UpdateService(appService);

            // and reload, check the collection
            // to verify that the data was saved and loaded

            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();
            Assert.IsNotNull(loadedAppService);

            AppCollection loadedCollection = loadedAppService.Workspaces.FindByName(workspaceName).Collections.FindByTitle(collectionTitle);

            Assert.IsNotNull(loadedCollection);
            Assert.AreEqual(2, loadedCollection.Accepts.Count());

            Assert.IsTrue(loadedCollection.Accepts.Contains(new AppAccept("foo")));
            Assert.IsTrue(loadedCollection.Accepts.Contains(new AppAccept("bar")));

            // delete the workspaces
            DeleteWorkspace(secondRepository, workspaceName);
        }

        [TestMethod]
        public void UpdateAppServiceAddWorkspaceCollectionAcceptsDeleteTest()
        {
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Workspaces);

            // put a new workspace in it
            AtomSite.Domain.AppWorkspace newWorkspace = TestDataHelper.MakeTestWorkspace();
            string workspaceName = newWorkspace.Name;

            // containing a collection
            AtomSite.Domain.AppCollection newCollection = TestDataHelper.MakeTestAppCollection();
            string collectionTitle = newCollection.Title.Text;

            // containing an accepts
            newCollection.Accepts = new List<AppAccept> { new AppAccept("foo"), new AppAccept("bar") };
            newWorkspace.Collections = newCollection.InList();
            appService.Workspaces = appService.Workspaces.Add(newWorkspace);

            // persist it
            repository.UpdateService(appService);

            // and reload, check the collection
            // to verify that the data was saved and loaded

            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();
            Assert.IsNotNull(loadedAppService);

            AppWorkspace loadedWorkspace = loadedAppService.Workspaces.FindByName(workspaceName);
            AppCollection loadedCollection = loadedWorkspace.Collections.FindByTitle(collectionTitle);

            Assert.IsNotNull(loadedCollection);
            Assert.AreEqual(2, loadedCollection.Accepts.Count());

            // now remove one
            loadedCollection.Accepts = new List<AppAccept> { new AppAccept( "foo") };
            loadedWorkspace.Collections = loadedWorkspace.Collections.ReplaceById(loadedCollection);

            appService.Workspaces = appService.Workspaces.ReplaceByName(loadedWorkspace);

            // persist it
            secondRepository.UpdateService(appService);

            // reload it

            loadedAppService = secondRepository.GetService();
            Assert.IsNotNull(loadedAppService);

            loadedWorkspace = loadedAppService.Workspaces.FindByName(workspaceName);
            loadedCollection = loadedWorkspace.Collections.FindByTitle(collectionTitle);

            Assert.IsNotNull(loadedCollection);
            Assert.AreEqual(1, loadedCollection.Accepts.Count());

            Assert.IsTrue(loadedCollection.Accepts.Contains(new AppAccept("foo")));
            Assert.IsFalse(loadedCollection.Accepts.Contains(new AppAccept("bar")));

            // delete the workspaces
            DeleteWorkspace(secondRepository, workspaceName);
        }

        [TestMethod]
        public void UpdateAppServiceAddWorkspaceCollectionAppCategoriesTest()
        {
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Workspaces);

            // put a new workspace in it
            AtomSite.Domain.AppWorkspace newWorkspace = TestDataHelper.MakeTestWorkspace();
            string workspaceName = newWorkspace.Name;

            // containing a collection
            AtomSite.Domain.AppCollection newCollection = TestDataHelper.MakeTestAppCollection();
            string collectionTitle = newCollection.Title.Text;

            // containing an app category
            AtomSite.Domain.AppCategories newCats = new AppCategories
                {
                    Scheme = new Uri("http://www.foo.com"),
                    Href = new Uri("http://www.foo.com"),
                    Base = new Uri("http://www.base.com"),
                    Fixed = true
                };

            newCollection.Categories = newCats.InList();
            newWorkspace.Collections = newCollection.InList();
            appService.Workspaces = appService.Workspaces.Add(newWorkspace);

            // persist it
            repository.UpdateService(appService);

            // and reload, check the collection
            // to verify that the data was saved and loaded

            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();
            Assert.IsNotNull(loadedAppService);

            AppCollection loadedCollection = loadedAppService.Workspaces.FindByName(workspaceName).Collections.FindByTitle(collectionTitle);

            Assert.IsNotNull(loadedCollection);
            Assert.AreEqual(1, loadedCollection.Categories.Count());

            AppCategories loadedCats = loadedCollection.Categories.First();

            DataTester.AssertAppCategoriesEqual(newCats, loadedCats);

            // delete the workspaces
            DeleteWorkspace(secondRepository, workspaceName);
        }

        [TestMethod]
        public void UpdateAppServiceAddCategoryTest()
        {
            IAppServiceRepository repository = GetRepository();

            AppService appService = repository.GetService();

            Assert.IsNotNull(appService, "no app service");
            Assert.IsNotNull(appService.Workspaces);

            // put a new workspace in it
            AtomSite.Domain.AppWorkspace newWorkspace = TestDataHelper.MakeTestWorkspace();
            string workspaceName = newWorkspace.Name;

            // containing a collection
            AtomSite.Domain.AppCollection newCollection = TestDataHelper.MakeTestAppCollection();
            string collectionTitle = newCollection.Title.Text;

            // containing an app category
            AtomSite.Domain.AppCategories newCats = new AppCategories
            {
                Scheme = new Uri("http://www.foo.com"),
                Base = new Uri("http://www.base.com"),
                Fixed = true
            };

            // containing a category
            AtomSite.Domain.AtomCategory newAtomCat = new AtomSite.Domain.AtomCategory
                {
                    Base = new Uri("http://www.base.com"),
                    Label = Guid.NewGuid().ToString(),
                    Term = Guid.NewGuid().ToString(),
                    Lang = "EN",
                    Scheme = new Uri("http://www.foo.com")
                };

            newCats.Categories = newAtomCat.InList();

            newCollection.Categories = newCats.InList();
            newWorkspace.Collections = newCollection.InList();
            appService.Workspaces = appService.Workspaces.Add(newWorkspace);

            // persist it
            repository.UpdateService(appService);

            // and reload, check the collection
            // to verify that the data was saved and loaded

            IAppServiceRepository secondRepository = GetRepository();

            AppService loadedAppService = secondRepository.GetService();
            Assert.IsNotNull(loadedAppService);

            AppCollection loadedCollection = loadedAppService.Workspaces.FindByName(workspaceName).Collections.FindByTitle(collectionTitle);

            Assert.IsNotNull(loadedCollection);
            Assert.AreEqual(1, loadedCollection.Categories.Count());

            AppCategories loadedCats = loadedCollection.Categories.First();

            Assert.AreEqual(1, loadedCats.Categories.Count(), "Atom category not found");
            AtomSite.Domain.AtomCategory loadedAtomCat = loadedCats.Categories.First();

            DataTester.AssertCategoriesEqual(newAtomCat, loadedAtomCat);

            // delete the workspaces
            DeleteWorkspace(secondRepository, workspaceName);
        }

        #endregion

        #region private

        private void DeleteWorkspace(IAppServiceRepository repository, string workspaceName)
        {
            AppService appService = repository.GetService();

            int initialCount = appService.Workspaces.Count();

            appService.Workspaces = appService.Workspaces.RemoveByName(workspaceName);

            Assert.AreEqual(initialCount - 1, appService.Workspaces.Count(), "Workspace was not removed");

            repository.UpdateService(appService);

            appService = repository.GetService();

            // check that it's gone
            AppWorkspace workspace = appService.Workspaces.FindByName(workspaceName);

            Assert.IsNull(workspace, "Workspace was not deleted " + workspaceName);
        }

        #endregion
    }

    ///// <summary>
    ///// run the tests against the sql repository
    ///// </summary>
    //[TestClass]
    //public class SqlAppServiceRepositoryTest: BaseAppServiceRepositoryTest
    //{
    //    protected override IAppServiceRepository GetRepository()
    //    {
    //        return new SqlAppServiceRepository(new AtomDataClassesDataContext(""));
    //    }
    //}

    /// <summary>
    /// run the tests against the mock repository
    /// </summary>
    [TestClass]
    public class MockAppServiceRepositoryTest : BaseAppServiceRepositoryTest
    {
        private readonly IAppServiceRepository mockAppServiceRepository = new MockAppServiceRepository();

        protected override IAppServiceRepository GetRepository()
        {
            return mockAppServiceRepository;
        }
    }

    /// <summary>
    /// Run the tests against a file repository
    /// </summary>
    [TestClass]
    public class FileAppServiceRepositoryTest : BaseAppServiceRepositoryTest
    {

        protected override IAppServiceRepository GetRepository()
        {
            FileRepositoryHelper helper = new FileRepositoryHelper(TestContext);
            helper.CheckAppService();
            return new FileAppServiceRepository(helper.TestFilesPath());
        }
    }
}