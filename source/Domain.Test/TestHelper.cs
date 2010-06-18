using System;
using System.Collections.Generic;
using System.Xml.Linq;

using AtomSite.Domain;

namespace Domain.Test
{
    /// <summary>
    /// Hlper code used across tests
    /// </summary>
    internal static class TestHelper
    {
        /// <summary>
        /// Make some test AtomPersons
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static List<AtomPerson> MakePersonList(int count, XName name)
        {
            List<AtomPerson> persons = new List<AtomPerson>();

            for (int loopIndex = 0; loopIndex < count; loopIndex++)
            {
                AtomPerson item = new AtomPerson(name);
                item.Lang = "EN";
                item.Base = new Uri("http://base.com");
                item.Uri = new Uri("http://homepage" + loopIndex.ToString() + ".com");
                item.Email = "person" + loopIndex.ToString() + "@testdata.com";
                item.Name = "test person " + loopIndex.ToString();

                persons.Add(item);
            }

            return persons;
        }

        internal static List<AtomCategory> MakeAtomCategoryList(int count)
        {            
            List<AtomCategory> categories = new List<AtomCategory>();

            for (int loopIndex = 0; loopIndex < count; loopIndex++)
            {

                AtomCategory atomCategory = new AtomCategory()
                    {
                        Label = "aLabel" + loopIndex.ToString(),
                        Lang = "EN",
                        Scheme = new Uri("http://scheme.com"),
                        Term = "term" + loopIndex.ToString(),
                        Base = new Uri("http://base.com")
                    };

                categories.Add(atomCategory);
            }

            return categories;
        }

        internal static List<AppCategories> MakeAppCategoriesList(int count)
        {
            List<AppCategories> categories = new List<AppCategories>();

            for (int loopIndex = 0; loopIndex < count; loopIndex++)
            {

                AppCategories cat = new AppCategories()
                {
                    Scheme = new Uri("http://scheme" + loopIndex.ToString() + ".com"),
                    Fixed = true,
                    Href = new Uri("http://href" + loopIndex.ToString() + ".com"),
                };

                categories.Add(cat);
            }

            return categories;
        }


        internal static List<AppCollection> MakeAppCollections(int count)
        {
            List<AppCollection> result = new List<AppCollection>();

            for (int loopIndex = 0; loopIndex < count; loopIndex++)
            {

                AppCollection appCollection = new AppCollection()
                {
                    Default = true,
                    Dated = true,
                };

                result.Add(appCollection);
            }

            return result;
        }

        internal static List<AtomLink> MakeLinks(int count)
        {
            List<AtomLink> result = new List<AtomLink>();

            for (int loopIndex = 0; loopIndex < count; loopIndex++)
            {

                AtomLink atomLink = new AtomLink()
                    {
                        Base = new Uri("http://base.com"),
                        Count = 0,
                        Href = new Uri("http://link" + loopIndex.ToString() + ".com"),
                        Lang = "EN",
                        Title = "Test Link " + loopIndex.ToString()
                    };

                result.Add(atomLink);
            }

            return result;
        }
    }
}
