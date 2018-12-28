using System;
using System.Xml.Serialization;

namespace Maze.Server.Connection.Utilities
{
    public interface IXmlSerializerCache
    {
        XmlSerializer Resolve(Type type, string rootName);
    }
}