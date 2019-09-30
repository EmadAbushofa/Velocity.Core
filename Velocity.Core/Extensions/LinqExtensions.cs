using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Velocity.Core.Extensions
{
    public static class LinqExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
                collection.Add(item);
        }

        public static void AddRange<T, TSource>(this ICollection<T> collection, IEnumerable<TSource> enumerable, Func<TSource, T> selector)
            => AddRange(collection, enumerable.Select(selector));

        public static void Replace<T>(this ICollection<T> currentCollection, IEnumerable<T> newCollection)
        {
            currentCollection.Clear();
            foreach (var item in newCollection)
                currentCollection.Add(item);
        }

        public static void Replace<T, TSource>(this ICollection<T> currentCollection, IEnumerable<TSource> newCollection, Func<TSource, T> selector)
            => Replace(currentCollection, newCollection.Select(selector));


        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
                action(item);
        }

        public static List<T> AsList<T>(this IEnumerable<T> enumerable) => enumerable as List<T> ??
                                                                           enumerable.ToList();
        public static IList<T> AsIList<T>(this IEnumerable<T> enumerable) => enumerable as IList<T> ??
                                                                             enumerable.ToList();
        public static async Task<IList<T>> AsIListAsync<T>(this Task<IEnumerable<T>> enumerable)
        {
            var list = await enumerable;
            return list as IList<T> ?? list.ToList();
        }

        public static void AddIfNotExisted<T>(this ICollection<T> collection, T element)
        {
            if (!collection.Contains(element))
                collection.Add(element);
        }

        public static void AddIfNotExisted<T>(this ICollection<T> collection, T element, Func<T, bool> predicate)
        {
            if (!collection.All(predicate))
                collection.Add(element);
        }
    }
}