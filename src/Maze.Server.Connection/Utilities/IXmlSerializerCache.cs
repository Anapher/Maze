using System;
using System.Xml.Serialization;

namespace Orcus.Server.Connection.Utilities
{
    public interface IXmlSerializerCache
    {
        XmlSerializer Resolve(Type type, string rootName);
    }
}