using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json;
using NuGet.Packaging.Core;
using Orcus.Server.Connection.JsonConverters;

namespace Orcus.Server.Connection.Modules
{
    [JsonConverter(typeof(PackageLockConverter))]
    public class PackagesLock : IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>>
    {
        private readonly IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> _dictionary;

        public PackagesLock(IDictionary<PackageIdentity, IImmutableList<PackageIdentity>> source)
        {
            _dictionary = source.ToImmutableDictionary(x => x.Key, x => x.Value);
        }

        public IEnumerator<KeyValuePair<PackageIdentity, IImmutableList<PackageIdentity>>> GetEnumerator() =>
            _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _dictionary.Count;
        public bool ContainsKey(PackageIdentity key) => _dictionary.ContainsKey(key);

        public bool TryGetValue(PackageIdentity key, out IImmutableList<PackageIdentity> value) =>
            _dictionary.TryGetValue(key, out value);

        public IImmutableList<PackageIdentity> this[PackageIdentity key] => _dictionary[key];

        public IEnumerable<PackageIdentity> Keys => _dictionary.Keys;
        public IEnumerable<IImmutableList<PackageIdentity>> Values => _dictionary.Values;
        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> Clear() => _dictionary.Clear();

        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> Add(PackageIdentity key,
            IImmutableList<PackageIdentity> value) =>
            _dictionary.Add(key, value);

        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> AddRange(
            IEnumerable<KeyValuePair<PackageIdentity, IImmutableList<PackageIdentity>>> pairs) =>
            _dictionary.AddRange(pairs);

        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> SetItem(PackageIdentity key,
            IImmutableList<PackageIdentity> value) =>
            _dictionary.SetItem(key, value);

        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> SetItems(
            IEnumerable<KeyValuePair<PackageIdentity, IImmutableList<PackageIdentity>>> items) =>
            _dictionary.SetItems(items);

        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> RemoveRange(
            IEnumerable<PackageIdentity> keys) =>
            _dictionary.RemoveRange(keys);

        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> Remove(PackageIdentity key) =>
            _dictionary.Remove(key);

        public bool Contains(KeyValuePair<PackageIdentity, IImmutableList<PackageIdentity>> pair) =>
            _dictionary.Contains(pair);

        public bool TryGetKey(PackageIdentity equalKey, out PackageIdentity actualKey) =>
            _dictionary.TryGetKey(equalKey, out actualKey);
    }
}