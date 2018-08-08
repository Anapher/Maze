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

        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> Add(PackageIdentity key, IImmutableList<PackageIdentity> value) => throw new System.NotImplementedException();

        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> AddRange(IEnumerable<KeyValuePair<PackageIdentity, IImmutableList<PackageIdentity>>> pairs) => throw new System.NotImplementedException();

        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> SetItem(PackageIdentity key, IImmutableList<PackageIdentity> value) => throw new System.NotImplementedException();

        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> SetItems(IEnumerable<KeyValuePair<PackageIdentity, IImmutableList<PackageIdentity>>> items) => throw new System.NotImplementedException();

        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> RemoveRange(IEnumerable<PackageIdentity> keys) => throw new System.NotImplementedException();

        public IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> Remove(PackageIdentity key) => throw new System.NotImplementedException();

        public bool Contains(KeyValuePair<PackageIdentity, IImmutableList<PackageIdentity>> pair) => throw new System.NotImplementedException();

        public bool TryGetKey(PackageIdentity equalKey, out PackageIdentity actualKey) => throw new System.NotImplementedException();
    }
}