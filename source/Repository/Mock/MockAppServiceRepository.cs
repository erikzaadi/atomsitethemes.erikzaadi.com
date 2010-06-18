using System;
using AtomSite.Domain;

namespace AtomSite.Repository.Mock
{
    /// <summary>
    /// Mock Implementation of IAppServiceRepository
    ///  - data isn't put anywhere, just stored in memory
    /// </summary>
    public class MockAppServiceRepository : IAppServiceRepository
    {
        #region data

        private AppService appService = new AppService();

        #endregion

        #region constructor

        public MockAppServiceRepository(AppService appSvc)
        {
          appService = appSvc;
        }
        public MockAppServiceRepository()
        {
            //appService.Domain = "blog";
            appService.DefaultSubdomain = "blog";
        }

        #endregion

        #region IAtomPubRepository Members

        public AppService GetService()
        {
            return this.appService;
        }

        public AppService UpdateService(AppService service)
        {

            if (service == null)
            {
                throw new ArgumentException("service");
            }

            this.appService = service;
            return this.appService;
        }

        #endregion
    }
}
