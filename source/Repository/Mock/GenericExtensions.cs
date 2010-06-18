using System;
using System.Collections.Generic;
using System.Linq;

namespace AtomSite.Repository
{
    public static class GenericExtensions
    {
        /// <summary>
        /// True if any item in the IEnumerable satifiies the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Exists<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            return enumerable.Where(predicate).Count() > 0;
        }

        /// <summary>
        /// Page the result set given the page size and page index
        /// </summary>
        /// <param name="items"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public static IQueryable<T> Page<T>(this IQueryable<T> items, int pageSize, int pageIndex)
        {
            if (pageSize == 0)
            {
                return items;
            }

            int skipCount = pageSize * pageIndex;
            return items.Skip(skipCount).Take(pageSize);
        }

        public static IEnumerable<T> Page<T>(this IEnumerable<T> items, int pageSize, int pageIndex)
        {
            if (pageSize == 0)
            {
                return items;
            }

            int skipCount = pageSize * pageIndex;
            return items.Skip(skipCount).Take(pageSize);
        }


        /// <summary>
        /// Return all items from the source list that were not found in the test list
        /// </summary>
        /// <param name="source"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        public static IList<T> Except<T>(this IList<T> source, IList<T> test)
        {
            // this is set difference, so use the library function (Linq Extension method in IEnumerable<T>) for that
            IEnumerable<T> enumSource = source;
            return new List<T>(enumSource.Except(test));
        }
    }
}
