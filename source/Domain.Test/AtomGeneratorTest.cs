using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    [TestClass]
    public class AtomGeneratorTest
    {
        [TestMethod]
        public void CreateTest()
        {
            AtomGenerator atomGenerator = new AtomGenerator()
                  {
                      Base = new Uri("http://base.com"),
                      Lang = "EN",
                      Text = "aText",
                      Uri = new Uri("http://uri.com"),
                      Version = "aVersion"
                  };

            Assert.IsNotNull(atomGenerator);
            Assert.IsNotNull(atomGenerator.Base);
            Assert.IsNotNull(atomGenerator.Uri);

            Assert.AreEqual("EN", atomGenerator.Lang);
            Assert.AreEqual("aText", atomGenerator.Text);
            Assert.AreEqual("aVersion", atomGenerator.Version);
        }
    }
}
