using System;
using System.Collections.Generic;
using System.Linq;

namespace Orcus.Administration.Extensions
{
    public static class ListExtensions
    {
        public static void PartialSort<T, TKey>(this IList<T> list, int start, int length, Func<T, TKey> keySelector)
        {
            foreach (var item in list.Skip(start).Take(length).OrderBy(keySelector)) list[start++] = item;
        }
    }
}