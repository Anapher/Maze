using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;

namespace Maze.Administration.Library.Extensions
{
    public static class MemoryCacheExtensions
    {
        public static T GetOrSetValueSafe<T>(this IMemoryCache cache, object key, TimeSpan expiration,
            Func<T> valueFactory)
        {
            if (cache.TryGetValue(key, out Lazy<T> cachedValue))
                return cachedValue.Value;

            cache.GetOrCreate(key, entry =>
            {
                entry.SetSlidingExpiration(expiration);
                return new Lazy<T>(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            });

            return cache.Get<Lazy<T>>(key).Value;
        }

        public static T GetOrSetValueSafe<T>(this IMemoryCache cache, object key, Action<ICacheEntry> configureEntry,
            Func<T> valueFactory)
        {
            if (cache.TryGetValue(key, out Lazy<T> cachedValue))
                return cachedValue.Value;

            cache.GetOrCreate(key, entry =>
            {
                var lazy = new Lazy<T>(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication);
                configureEntry(entry);

                return lazy;
            });

            return cache.Get<Lazy<T>>(key).Value;
        }
    }
}