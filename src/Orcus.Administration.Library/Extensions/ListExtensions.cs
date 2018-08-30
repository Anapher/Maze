using System;
using System.Collections.Generic;
using System.Linq;

namespace Orcus.Administration.Library.Extensions
{
    public static class ListExtensions
    {
        //https://stackoverflow.com/a/22801345/4166138
        public static void AddSorted<T>(this List<T> list, T item) where T : IComparable<T>
        {
            if (list.Count == 0)
            {
                list.Add(item);
                return;
            }

            if (list[list.Count - 1].CompareTo(item) <= 0)
            {
                list.Add(item);
                return;
            }

            if (list[0].CompareTo(item) >= 0)
            {
                list.Insert(0, item);
                return;
            }

            var index = list.BinarySearch(item);
            if (index < 0)
                index = ~index;
            list.Insert(index, item);
        }

        //https://codereview.stackexchange.com/a/37211
        public static void AddSorted<TSource, TKey>(this IList<TSource> list, TSource item, Func<TSource, TKey> selector)
        {
            var itemKey = selector(item);
            var comparer = Comparer<TKey>.Default;

            int i = 0;
            while (i < list.Count && comparer.Compare(selector(list[i]), itemKey) < 0)
                i++;

            list.Insert(i, item);
        }
    }
}