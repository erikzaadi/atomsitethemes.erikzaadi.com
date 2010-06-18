using System;
using System.Collections.Generic;
using System.Linq;

using AtomSite.Domain;

namespace Repository.Test
{
    /// <summary>
    /// Extension methods on domain objects for filtering and finding data
    /// TODO: put this in the domain project?
    /// </summary>
    public static class DomainExtensions
    {
        #region App workspace

        public static AppWorkspace FindByName(this IEnumerable<AppWorkspace> workspaces, string name)
        {
            if (workspaces == null)
            {
                return null;
            }

            return (from ws in workspaces
                    where ws.Name == name
                    select ws).SingleOrDefault();
        }

        public static IEnumerable<AppWorkspace> ReplaceByName(this IEnumerable<AppWorkspace> workspaces, AppWorkspace replaceItem)
        {
            if (workspaces == null)
            {
                return null;
            }

            List<AppWorkspace> result = new List<AppWorkspace>(workspaces);
            int index = result.IndexOfName(replaceItem.Name);

            if (index >= 0)
            {
                result[index] = replaceItem;
            }

            return result;
        }

        private static int IndexOfName(this IList<AppWorkspace> workspaces, string name)
        {
            for (int index = 0; index < workspaces.Count; index++)
            {
                if (String.Equals(name, workspaces[index].Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return index;
                }
            }

            return -1;
        }

        #endregion

        #region AppCollection

        public static IEnumerable<AppCollection> ReplaceById(this IEnumerable<AppCollection> collections, AppCollection replaceItem)
        {
            if (collections == null)
            {
                return null;
            }

            List<AppCollection> result = new List<AppCollection>(collections);
            int index = result.IndexOfId(replaceItem.Id);

            if (index >= 0)
            {
                result[index] = replaceItem;
            }

            return result;
        }

        private static int IndexOfId(this IList<AppCollection> list, Id id) 
        {
            string idString = id.ToString();
            for (int index = 0; index < list.Count; index++)
            {
                if (String.Equals(idString, list[index].Id.ToString())) 
                {
                    return index;
                }
            }

            return -1;
        }

        public static AppCollection FindByTitle(this IEnumerable<AppCollection> collections, string title)
        {
            if (collections == null)
            {
                return null;
            }

            return (from coll in collections
                    where coll.Title.Text == title
                    select coll).SingleOrDefault();
        }

        /// <summary>
        /// find the default collection
        /// </summary>
        /// <param name="collections"></param>
        /// <returns></returns>
        public static AppCollection Default(this IEnumerable<AppCollection> collections)
        {
            if (collections == null)
            {
                return null;
            }

            return (from coll in collections
                    where coll.Default
                    select coll).SingleOrDefault();
        }

        #endregion

        #region atom entry

        public static bool ContainsId(this IEnumerable<AtomEntry> entries, Id id)
        {
            return entries.Any(entry => entry.Id.ToString() == id.ToString());
        }

        #endregion
    }
}
