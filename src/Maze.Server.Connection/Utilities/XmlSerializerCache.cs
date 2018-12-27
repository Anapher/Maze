using System;
using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace Orcus.Server.Connection.Utilities
{
    public class XmlSerializerCache : IXmlSerializerCache
    {
        private readonly ConcurrentDictionary<CompositeXmlKey, XmlSerializer> _serializers;

        public XmlSerializerCache()
        {
            _serializers = new ConcurrentDictionary<CompositeXmlKey, XmlSerializer>();
        }

        public XmlSerializer Resolve(Type type, string rootName)
        {
            return _serializers.GetOrAdd(new CompositeXmlKey(type, rootName), key => new XmlSerializer(key.Type, new XmlRootAttribute(key.Root)));
        }

        private struct CompositeXmlKey
        {
            public Type Type { get; }
            public string Root { get; }

            public CompositeXmlKey(Type type, string root)
            {
                Type = type;
                Root = root;
            }

            private bool Equals(CompositeXmlKey other) => Type == other.Type && string.Equals(Root, other.Root);

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is CompositeXmlKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Type.GetHashCode() * 397) ^ Root.GetHashCode();
                }
            }
        }
    }
}