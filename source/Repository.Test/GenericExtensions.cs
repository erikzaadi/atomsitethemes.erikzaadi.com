using System.Collections.Generic;

namespace Repository.Test
{
    public static class GenericExtensions
    {
        #region generic extensions

        /// <summary>
        /// return a new enumerable containing the existing items and the new one
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="newItem"></param>
        /// <returns></returns>
        public static IEnumerable<T> Add<T>(this IEnumerable<T> enumerable, T newItem)
        {
            List<T> result = new List<T>(enumerable);
            result.Add(newItem);
            return result;
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> enumerable, params T[] newItems)
        {
            List<T> result = new List<T>(enumerable);
            result.AddRange(newItems);
            return result;
        }

        public static IEnumerable<T> RemoveAt<T>(this IEnumerable<T> enumerable, int index)
        {
            List<T> result = new List<T>(enumerable);
            result.RemoveAt(index);
            return result;
        }

        /// <summary>
        /// Wrap an object in a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IList<T> InList<T>(this T item)
        {
            List<T> result = new List<T>();
            result.Add(item);
            return result;
        }

        #endregion
    }
}
