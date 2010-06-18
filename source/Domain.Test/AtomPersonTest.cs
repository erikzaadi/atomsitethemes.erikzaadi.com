using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    [TestClass]
    public class AtomPersonTest
    {
        [TestMethod]
        public void SimpleCreateTest()
        {
            AtomPerson atomPerson = new AtomPerson();

            Assert.IsNotNull(atomPerson);
        }

        [TestMethod]
        public void FullCreateTest()
        {
            AtomPerson atomPerson = new AtomPerson()
                {
                    Base = new Uri("http://www.base.com"),
                    Email = "fred@bloggs.com",
                    Lang = "EN",
                    Name = "Fred Bloggs",
                    Uri = new Uri("http://www.fred.com")
                };

            Assert.IsNotNull(atomPerson);
            Assert.IsNotNull(atomPerson.Base);
            Assert.IsNotNull(atomPerson.Uri);

            Assert.AreEqual("fred@bloggs.com", atomPerson.Email);
            Assert.AreEqual("EN", atomPerson.Lang);
            Assert.AreEqual("Fred Bloggs", atomPerson.Name);

            Assert.AreEqual(new Uri("http://www.base.com"), atomPerson.Base);
            Assert.AreEqual(new Uri("http://www.fred.com"), atomPerson.Uri);
        }
    }
}
