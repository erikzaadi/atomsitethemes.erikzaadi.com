using System;
using System.Text;
using AtomSite.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Repository.Test
{
    public static class TestDataHelper
    {
        #region constants

        private const string TestWorkspaceName = "TestWorkspace";

        #endregion

        #region public methods to create data

        public static AtomEntry MakeTestEntry(string collectionName)
        {
            return TestDataHelper.MakeTestEntry(DateTime.Now, collectionName);
        }

        public static AtomEntry MakeTestEntry(DateTime published, string collectionName)
        {
            string owner = string.Format("{0}.someblog.com", TestWorkspaceName);
            string path = "path" + RandomChars(12);
            Id entryId = new Id(owner, published, collectionName, path);

            return new AtomEntry
            {
                Title = new AtomText
                {
                    Text = "Test entry " + Guid.NewGuid(),
                    Type = "text"
                },
                Content = new AtomContent
                {
                    Text = "Test entry " + Guid.NewGuid(),
                    Type = "text"
                },
                Summary = new AtomText
                {
                    Text = "This entry is generated test data",
                    Type = "text"
                },
                Control = new AppControl
                {
                    AllowAnnotate = true,
                    Approved = true,
                    Draft = false
                },
                Id = entryId,
                Published = published
            };
        }

        public static Id MakeReplyId(Id parent, DateTime published)
        {
            if (!Uri.IsWellFormedUriString(parent.ToString(), UriKind.RelativeOrAbsolute))
            {
                throw new Exception("id is not a valid uri: " + parent);
            }

            string childPath = RandomChars(12);
            Id resultId = new Id(parent.Owner, published, parent.Collection, parent.EntryPath + "," + childPath);

            string uriString = resultId.ToString();
            if (!Uri.IsWellFormedUriString(resultId.ToString(), UriKind.RelativeOrAbsolute))
            {
                throw new Exception("id is not a valid uri: " + uriString);
            }

            return resultId;
        }

        public static AtomPerson MakeTestPerson()
        {
            return new AtomPerson
            {
                Name = "TestPerson " + Guid.NewGuid(),
                Email = "Somemail@" + Guid.NewGuid() + ".com",
                Uri = new Uri("http://www.testperson.com")
            };
        }

        public static AtomCategory MakeTestCategory()
        {
            return new AtomCategory
            {
                Scheme = new Uri("http://www.TestCategories.com"),
                Term = Guid.NewGuid().ToString(),
                Label = Guid.NewGuid().ToString(),
                Lang = "EN"
            };
        }

        public static AppWorkspace MakeTestWorkspace()
        {
            AppWorkspace result = new AppWorkspace
            {
                Title = new AtomText
                {
                    Text = "Test workspace title " + Guid.NewGuid(),
                    Type = "text"
                },
                Name = "Test workspace name" + Guid.NewGuid(),
                Default = false
            };

            return result;
        }

        public static AppCollection MakeTestAppCollection()
        {
            // containing a collection
            string collectionTitle = "Test collection title " + Guid.NewGuid();
            AppCollection result = new AppCollection
            {
                Title = new AtomText
                {
                    Text = collectionTitle,
                    Type = "text"
                },
                Icon = new Uri("http://www.icon.com/" + Guid.NewGuid().ToString()),
                Href = new Uri("http://www.href.com/" + Guid.NewGuid().ToString()),
                Theme = "aTheme",
                Dated = true,
                Default = false,
                Id = new Id("test", DateTime.Now, RandomChars(12), "")
            };

            return result;
        }

        public static User MakeTestUser()
        {
            string userGuid = Guid.NewGuid().ToString();

            User newUser = new User
            {
                Email = userGuid + "@test.com",
                Name = userGuid,
                FullName = "Mr " + userGuid + " Bloggs",
                Lang = "EN",
                Password = "secret",
                PasswordFormat = "plaintext",
            };

            return newUser;
        }

        #endregion

        #region helpers

        private static Random random = new Random();


        /// <summary>
        /// A short random string, for unique names when guids are too long
        /// </summary>
        /// <returns></returns>
        public static string RandomChars(int length)
        {
            const int OrdA = 97; // ascii index for lower case a

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                char nextChar = Convert.ToChar(OrdA + random.Next(25));
                result.Append(nextChar);             
            }
            return result.ToString();
        }

        #endregion
    }
}
