using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Maze.Server.Connection.Commanding;
using Maze.Server.Connection.Utilities;
using Tasks.Infrastructure.Core.Audience;
using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Infrastructure.Core
{
    /// <summary>
    ///     Writer for an <see cref="MazeTask"/>
    /// </summary>
    public class MazeTaskWriter
    {
        private readonly XmlWriter _xmlWriter;
        private readonly ITaskComponentResolver _componentResolver;
        private readonly IXmlSerializerCache _serializerCache;
        private readonly XmlSerializerNamespaces _namespaces;

        public MazeTaskWriter(XmlWriter xmlWriter, ITaskComponentResolver componentResolver, IXmlSerializerCache serializerCache)
        {
            _xmlWriter = xmlWriter;
            _componentResolver = componentResolver;
            _serializerCache = serializerCache;

            _namespaces = new XmlSerializerNamespaces();
            _namespaces.Add(string.Empty, string.Empty);
        }

        public MazeTaskWriter(Stream stream, ITaskComponentResolver componentResolver, IXmlSerializerCache serializerCache) : this(
            XmlWriter.Create(stream, new XmlWriterSettings {OmitXmlDeclaration = false, Indent = false}), componentResolver, serializerCache)
        {
        }

        public void Write(MazeTask mazeTask, TaskDetails details)
        {
            _xmlWriter.WriteStartElement(XmlNames.Root);

            WriteMetadata(mazeTask);

            if (details == TaskDetails.Server)
            {
                WriteAudience(mazeTask.Audience);
                WriteElements(mazeTask.Filters, XmlNames.Filters);
            }

            if (details >= TaskDetails.Client)
            {
                WriteElements(mazeTask.Triggers, XmlNames.Triggers);
            }

            WriteElements(mazeTask.StopEvents, XmlNames.Stop);
            WriteCommands(mazeTask.Commands);

            _xmlWriter.WriteEndElement();
            _xmlWriter.Dispose();
        }

        private void WriteMetadata(MazeTask mazeTask)
        {
            _xmlWriter.WriteStartElement(XmlNames.Metadata);
            _xmlWriter.WriteElementString(XmlNames.Name, mazeTask.Name);
            _xmlWriter.WriteElementString(XmlNames.Id, mazeTask.Id.ToString("D"));
            _xmlWriter.WriteEndElement();
        }

        private void WriteAudience(AudienceCollection audienceCollection)
        {
            _xmlWriter.WriteStartElement(XmlNames.Audience);

            if (audienceCollection.IsAll)
            {
                _xmlWriter.WriteStartElement("AllClients");
                _xmlWriter.WriteEndElement();
            }

            if (audienceCollection.IncludesServer)
            {
                _xmlWriter.WriteStartElement("Server");
                _xmlWriter.WriteEndElement();
            }

            if (!audienceCollection.IsAll)
            {
                _xmlWriter.WriteStartElement("Clients");

                var targets = new CommandTargetCollection(audienceCollection);
                _xmlWriter.WriteAttributeString("id", targets.ToString());
                _xmlWriter.WriteEndElement();
            }

            _xmlWriter.WriteEndElement();
        }

        private void WriteElements<T>(IList<T> elements, string section)
        {
            if (elements?.Any() != true)
                return;

            _xmlWriter.WriteStartElement(section);

            foreach (var element in elements)
            {
                var name = _componentResolver.ResolveName(element.GetType());
                var serializer = _serializerCache.Resolve(element.GetType(), name);

                serializer.Serialize(_xmlWriter, element, _namespaces);
            }

            _xmlWriter.WriteEndElement();
        }

        private void WriteCommands(IList<CommandInfo> commands)
        {
            _xmlWriter.WriteStartElement(XmlNames.Commands);

            foreach (var commandInfo in commands)
            {
                var commandAttribute = commandInfo.GetType().GetCustomAttribute<MazeCommandAttribute>();
                var serializer = _serializerCache.Resolve(commandInfo.GetType(), "Command");

                using (var stream = new MemoryStream())
                {
                    using (var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings {OmitXmlDeclaration = true}))
                    {
                        serializer.Serialize(xmlWriter, commandInfo, _namespaces);
                    }

                    stream.Position = 0;

                    var xmlReader = XmlReader.Create(stream);
                    var isFirstElement = true;
                    while (xmlReader.Read())
                    {
                        if (isFirstElement)
                        {
                            _xmlWriter.WriteStartElement(XmlNames.Command);

                            if (commandAttribute != null)
                            {
                                _xmlWriter.WriteAttributeString(XmlNames.CommandName, commandAttribute.Name);
                                _xmlWriter.WriteAttributeString(XmlNames.CommandModules, commandAttribute.Modules);
                            }
                            else
                            {
                                var name = _componentResolver.ResolveName(commandInfo.GetType());
                                _xmlWriter.WriteAttributeString(XmlNames.CommandName, name);
                            }

                            _xmlWriter.WriteAttributes(xmlReader, false);

                            if (xmlReader.IsEmptyElement)
                                _xmlWriter.WriteEndElement();
                            isFirstElement = false;
                        }
                        else
                        {
                            _xmlWriter.WriteShallowNode(xmlReader);
                        }
                    }
                }
            }

            _xmlWriter.WriteEndElement();
        }
    }
}