using System;
using System.Collections.Generic;
using System.Linq;

namespace Maze.Administration.Library.Extensions
{
    public static class ListExtensions
    {
        //https://stackoverflow.com/a/22801345/4166138
        /// <summary>
        ///     Add an item to an already sorted <see cref="List{T}"/> at the correct position
        /// </summary>
        /// <typeparam name="T">The type of the item</typeparam>
        /// <param name="list">The list the item should be added to.</param>
        /// <param name="item">The item that should be added</param>
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
        /// <summary>
        /// Add an item to an already sorted <see cref="IList{T}"/> at the correct position
        /// </summary>
        /// <typeparam name="TSource">The item type</typeparam>
        /// <typeparam name="TKey">The key type that should be sorted</typeparam>
        /// <param name="list">The list the item should be added to.</param>
        /// <param name="item">The item that should be added</param>
        /// <param name="selector">The selector that selects the value to sort with.</param>
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