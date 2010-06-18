using System;
using System.Linq;

using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    [TestClass]
    public class AppServiceTest
    {

        [TestMethod]
        public void SimpleCreateAppServiceTest()
        {
            AppService appService = new AppService();
            Assert.IsNotNull(appService);
        }


        [TestMethod]
        public void FullCreateAppServiceTest()
        {
            AppService appService = new AppService()
                {
                  //TODO: Admins = new List<AtomPerson>(),
                    DefaultSubdomain = "foo",
                    //Domain = "aDomain",
                    Lang = "EN",
                    Secure = false,
                    ServiceType = ServiceType.Single,
                    Workspaces = new List<AppWorkspace>()
                };


            Assert.IsNotNull(appService);
            Assert.IsNotNull(appService.Admins);
            Assert.AreEqual(0, appService.Admins.Count());

            Assert.IsNotNull(appService.Workspaces);
            Assert.AreEqual("foo", appService.DefaultSubdomain);
            Assert.AreEqual("EN", appService.Lang);
            Assert.AreEqual(false, appService.Secure);
            Assert.AreEqual(ServiceType.Single, appService.ServiceType);
        }

        [TestMethod]
        public void CreateWithAdminsAppServiceTest()
        {
            AppService appService = new AppService()
                {
                  Admins = Enumerable.Repeat<string>("admin", 4)
                };

            Assert.IsNotNull(appService);
            Assert.IsNotNull(appService.Admins);
            Assert.AreEqual(4, appService.Admins.Count());

        }
    }
}
