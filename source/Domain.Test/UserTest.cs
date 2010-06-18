using System;
using System.Collections.Generic;
using System.Linq;
using AtomSite.Domain;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Domain.Test
{
    [TestClass]
    public class UserTest
    {
        [TestMethod]
        public void CreateTest()
        {
            User user = new User();

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void CreateWithDataTest()
        {
            User user = new User()
                {
                    Base = new Uri("http://www.base.com"),
                    Email = "testemail@test.com",
                    Name = "fred",
                    FullName = "Fred Q Bloggs",
                    Lang = "EN",
                    Password = "Secret",
                    PasswordFormat = "plaintext"
                };

            Assert.IsNotNull(user);

            Assert.AreEqual(new Uri("http://www.base.com"), user.Base);
            Assert.AreEqual("testemail@test.com", user.Email);
            Assert.AreEqual("fred", user.Name);
            Assert.AreEqual("Fred Q Bloggs", user.FullName);
            Assert.AreEqual("EN", user.Lang);
            Assert.AreEqual("Secret", user.Password);
            Assert.AreEqual("plaintext", user.PasswordFormat);
        }

        [TestMethod]
        public void CreateWithIdTest()
        {
            User user = new User();

            Assert.IsNotNull(user);

            string idUri = "http://www.test.com";

            user.Ids = new List<string> {idUri};

            Assert.AreEqual(1, user.Ids.Count());
            Assert.AreEqual(idUri, user.Ids.First());
        }

    }
}
