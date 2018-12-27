using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;

namespace Orcus.Server.Service.Extensions
{
    public static class MemoryCacheExtensions
    {
        public static T GetOrSetValueSafe<T>(this IMemoryCache cache, string key, TimeSpan expiration,
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
    }
}