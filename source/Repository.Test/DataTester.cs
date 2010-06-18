using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Repository.Test
{
    public static class DataTester
    {
        public static void AssertEntriesEqual(AtomEntry entry, AtomEntry testEntry)
        {
            Assert.AreEqual(entry.Id.ToString(), testEntry.Id.ToString());
            Assert.AreEqual(entry.Title.Text, testEntry.Title.Text);
            Assert.AreEqual(entry.Title.Lang, testEntry.Title.Lang);

            Assert.AreEqual(entry.Text.Text, testEntry.Text.Text);
            Assert.AreEqual(entry.Text.Lang, testEntry.Text.Lang);

            Assert.AreEqual(entry.Published.HasValue, testEntry.Published.HasValue, "Published");

            if (entry.Published.HasValue)
            {
                AssertDatesApproximatelyEqual(entry.Published.Value, testEntry.Published.Value);
            }

            Assert.AreEqual(entry.Edited.HasValue, testEntry.Edited.HasValue, "Edited");

            if (entry.Edited.HasValue)
            {
                AssertDatesApproximatelyEqual(entry.Edited.Value, testEntry.Edited.Value);
            }

            AssertDatesApproximatelyEqual(entry.Updated, testEntry.Updated);
        }

        public static void AssertPersonsEqual(AtomPerson person, AtomPerson testPerson)
        {
            Assert.AreEqual(person.Name, testPerson.Name, "Person name");
            Assert.AreEqual(person.Email, testPerson.Email, "Person email");
            Assert.AreEqual(person.Type, testPerson.Type, "Person type");
            Assert.AreEqual(person.Uri, testPerson.Uri, "Person Uri");
        }

        private static void AssertDatesApproximatelyEqual(DateTimeOffset expected, DateTimeOffset actual)
        {
            Assert.AreEqual(expected.Offset, expected.Offset, "Offset not equal");
            AssertDatesApproximatelyEqual(expected.DateTime, actual.DateTime);
        }

        private static void AssertDatesApproximatelyEqual(DateTime expected, DateTime actual)
        {
            Assert.AreEqual(expected.Year, actual.Year, "Year not equal");
            Assert.AreEqual(expected.Month, actual.Month, "Month not equal");
            Assert.AreEqual(expected.Day, actual.Day, "Day not equal");
            Assert.AreEqual(expected.Hour, actual.Hour, "Hour not equal");
            Assert.AreEqual(expected.Minute, actual.Minute, "Minute not equal");
            Assert.AreEqual(expected.Second, actual.Second, "Second not equal");
        }

        public static void AssertAppCategoriesEqual(AppCategories expected, AppCategories actual)
        {
            Assert.AreEqual(expected.Href, actual.Href);
            Assert.AreEqual(expected.IsExternal, actual.IsExternal);
            Assert.AreEqual(expected.Fixed, actual.Fixed);
        }

        public static void AssertCategoriesEqual(AtomCategory category, AtomCategory testCategory)
        {
            Assert.AreEqual(category.Label, testCategory.Label);
            Assert.AreEqual(category.Term, testCategory.Term);
            //Assert.AreEqual(category.Lang, testCategory.Lang);
            Assert.AreEqual(category.Scheme, testCategory.Scheme);
            //Assert.AreEqual(category.Base, testCategory.Base);
        }

        public static void AssertLinksEqual(AtomLink link, AtomLink testLink)
        {
            Assert.AreEqual(link.Href, testLink.Href);
            Assert.AreEqual(link.Title, testLink.Title);
            Assert.AreEqual(link.Count, testLink.Count);
            Assert.AreEqual(link.HrefLang, testLink.HrefLang);
            Assert.AreEqual(link.Length, testLink.Length);
            Assert.AreEqual(link.Rel, testLink.Rel);
            Assert.AreEqual(link.Type, testLink.Type);

            if (link.Updated.HasValue)
            {
                Assert.IsTrue(testLink.Updated.HasValue);
                AssertDatesApproximatelyEqual(link.Updated.Value, testLink.Updated.Value);
            }
        }

        public static void AssertUsersEqual(User user, User testUser)
        {
            Assert.AreEqual(user.Name, testUser.Name);
            Assert.AreEqual(user.FullName, testUser.FullName);
            Assert.AreEqual(user.Email, testUser.Email);
            Assert.AreEqual(user.Password, testUser.Password);
            Assert.AreEqual(user.PasswordFormat, testUser.PasswordFormat);
        }
    }
}
