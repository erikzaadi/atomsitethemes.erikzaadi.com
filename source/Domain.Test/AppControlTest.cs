using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    /// <summary>
    /// Test the AppControl domain object
    /// </summary>
    [TestClass]
    public class AppControlTest
    {
        [TestMethod]
        public void CreateAppControlTrueTest()
        {
            AppControl appControl = new AppControl()
                {
                    Draft = true,
                    Approved = true,
                    AllowAnnotate = true
                };

            Assert.IsNotNull(appControl);

            Assert.IsTrue(appControl.Draft.HasValue);
            Assert.IsTrue(appControl.Draft.Value);

            Assert.IsTrue(appControl.Approved.HasValue);
            Assert.IsTrue(appControl.Approved.Value);

            Assert.IsTrue(appControl.AllowAnnotate.HasValue);
            Assert.IsTrue(appControl.AllowAnnotate.Value);
        }

        [TestMethod]
        public void CreateAppControlFalseTest()
        {
            AppControl appControl = new AppControl()
            {
                Draft = false,
                Approved = false,
                AllowAnnotate = false
            };

            Assert.IsNotNull(appControl);

            Assert.IsTrue(appControl.Draft.HasValue);
            Assert.IsFalse(appControl.Draft.Value);

            Assert.IsTrue(appControl.Approved.HasValue);
            Assert.IsFalse(appControl.Approved.Value);

            Assert.IsTrue(appControl.AllowAnnotate.HasValue);
            Assert.IsFalse(appControl.AllowAnnotate.Value);
        }
    }
}
