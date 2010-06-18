using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Utils;

namespace Repository.Test
{
    /// <summary>
    /// Helper for file paths and files
    /// Not static, since if it's instanced, it can serve different paths
    /// </summary>
    public class FileRepositoryHelper
    {
        #region private

        private const string usersFileName = "Users.config";
        private const string appServiceFileName = "Service.config";

        private TestContext testContext;

        #endregion

        #region constructor

        public FileRepositoryHelper(TestContext testContext)
        {
            this.testContext = testContext;
        }

        #endregion

        #region public

        public string TestFilesPath()
        {
            return FileHelper.CombineTrackbacksInPath(
                Path.Combine(testContext.TestDir, @"..\..\Web\"));
        }

        public string UsersFilePath()
        {
            return Path.Combine(TestFilesPath(), usersFileName);
        }

        public string AppServiceFilePath()
        {
            return Path.Combine(TestFilesPath(), appServiceFileName);
        }

        public string MediaPath()
        {
            return Path.Combine(TestFilesPath(), "media");
        }


        /// <summary>
        /// Make the app service if it doesn't exist
        /// </summary>
        /// <returns></returns>
        public void CheckAppService()
        {
            string filePath = AppServiceFilePath();

            if (! File.Exists(filePath))
            {
                MakeAppService(filePath);
            }
        }

        #endregion

        #region private

        private void MakeAppService(string filePath)
        {
            const string defaultAppServiceData = 
                "<?xml version=\"1.0\" encoding=\"utf-8\"?> " +
                "<service xmlns:atom=\"http://www.w3.org/2005/Atom\" xmlns:svc=\"http://atomsite.net/info/Schema.xhtml\" xmlns=\"http://www.w3.org/2007/app\" " +
                "svc:defaultSubdomain=\"blog\" />";

            File.WriteAllText(filePath, defaultAppServiceData);
        }

        #endregion

    }
}
