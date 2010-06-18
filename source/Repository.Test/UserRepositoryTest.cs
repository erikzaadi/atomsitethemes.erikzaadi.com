using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtomSite.Domain;
using AtomSite.Repository;
using AtomSite.Repository.File;
using AtomSite.Repository.Mock;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Repository.Test
{
    public abstract class BaseUserRepositoryTest
    {
        private const int pageSize = 100;
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


        protected abstract IUserRepository GetRepository();

        [TestMethod]
        public void GetUserRepositoryTest()
        {
            IUserRepository repository = GetRepository();

            Assert.IsNotNull(repository);
        }

        [TestMethod]
        public void GetUsersTest()
        {
            IUserRepository repository = GetRepository();

            int total;
            IEnumerable<User> users = repository.GetUsers(0, pageSize, out total);

            Assert.IsNotNull(users);
        }

        [TestMethod]
        public void AddUserTest()
        {
            User newUser = TestDataHelper.MakeTestUser();

            IUserRepository repository = GetRepository();
            repository.CreateUser(newUser);

            // now try to load it
            repository = GetRepository();
            User loadedUser = repository.GetUser(newUser.Name);

            Assert.IsNotNull(loadedUser);

            DataTester.AssertUsersEqual(newUser, loadedUser);
        }

        [TestMethod]
        public void MissingUserTest()
        {
            IUserRepository repository = GetRepository();
            User missing = repository.GetUser(Guid.NewGuid().ToString());
            Assert.IsNull(missing);
        }

        [TestMethod]
        public void UpdateUserTest()
        {
            User newUser = TestDataHelper.MakeTestUser();

            IUserRepository repository = GetRepository();
            repository.CreateUser(newUser);

            // change the email
            newUser.Email = Guid.NewGuid().ToString() + "@test2.com";

            repository = GetRepository();
            repository.UpdateUser(newUser);

            // load it
            repository = GetRepository();
            User loadedUser = repository.GetUser(newUser.Name);

            DataTester.AssertUsersEqual(newUser, loadedUser);
        }

        [TestMethod]
        public void AddUserWithIdTest()
        {
            User newUser = TestDataHelper.MakeTestUser();
            string userId = new Uri("http://id.com/" + newUser.Name).ToString();

            newUser.Ids = userId.InList();

            IUserRepository repository = GetRepository();
            repository.CreateUser(newUser);

            // now try to load it
            repository = GetRepository();
            User loadedUser = repository.GetUser(newUser.Name);

            Assert.IsNotNull(loadedUser);

            DataTester.AssertUsersEqual(newUser, loadedUser);

            Assert.AreEqual(1, loadedUser.Ids.Count());
            Assert.AreEqual(userId, loadedUser.Ids.First());
        }
    }


    ///// <summary>
    ///// run the tests against the sql repository
    ///// </summary>
    //[TestClass]
    //public class SqlUserRepositoryTest : BaseUserRepositoryTest
    //{
    //    protected override IUserRepository GetRepository()
    //    {
    //        return new SqlUserRepository(new AtomDataClassesDataContext(""));
    //    }
    //}

    /// <summary>
    /// run the tests against the mock repository
    /// </summary>
    [TestClass]
    public class MockUserRepositoryTest : BaseUserRepositoryTest
    {
        private readonly IUserRepository mockUserRepsitory = new MockUserRepository();

        protected override IUserRepository GetRepository()
        {
            return mockUserRepsitory;
        }
    }

    [TestClass]
    public class FileUserRepositoryTest : BaseUserRepositoryTest
    {
        protected override IUserRepository GetRepository()
        {
            FileRepositoryHelper helper = new FileRepositoryHelper(TestContext);
            return new FileUserRepository(helper.UsersFilePath());
        }
    }

}
