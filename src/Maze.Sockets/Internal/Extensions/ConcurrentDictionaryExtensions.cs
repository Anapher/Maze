using System;
using System.Collections.Concurrent;

namespace Maze.Sockets.Internal.Extensions
{
    public static class ConcurrentDictionaryExtensions
    {
        public static void Clear<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, Action<TValue> disposeObj)
        {
            foreach (var keyValue in dictionary)
            {
                dictionary.TryRemove(keyValue.Key, out _);
                disposeObj(keyValue.Value);
            }
        }
    }
}