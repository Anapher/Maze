using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Orcus.Server.Connection.Commanding;
using Orcus.Server.Connection.Utilities;
using Tasks.Infrastructure.Core.Audience;
using Tasks.Infrastructure.Core.Commands;
using Tasks.Infrastructure.Core.Filter;
using Tasks.Infrastructure.Core.StopEvents;
using Tasks.Infrastructure.Core.Triggers;

namespace Tasks.Infrastructure.Core
{
    //https://github.com/NuGet/NuGet.Client/blob/dev/src/NuGet.Core/NuGet.Packaging/NuspecReader.cs
    public class OrcusTaskReader : OrcusTaskReaderBase
    {
        private readonly ITaskComponentResolver _componentResolver;
        private readonly IXmlSerializerCache _xmlSerializerCache;

        /// <summary>
        ///     Orcus task file reader.
        /// </summary>
        public OrcusTaskReader(string path, ITaskComponentResolver componentResolver, IXmlSerializerCache xmlSerializerCache) : base(path)
        {
            _componentResolver = componentResolver;
            _xmlSerializerCache = xmlSerializerCache;
        }

        /// <summary>
        ///     Orcus task file reader
        /// </summary>
        /// <param name="stream">Orcus task file stream.</param>
        /// <param name="componentResolver">The service resolver used to resolve the task services</param>
        /// <param name="xmlSerializerCache">The xml serializer cache for deserialization</param>
        public OrcusTaskReader(Stream stream, ITaskComponentResolver componentResolver, IXmlSerializerCache xmlSerializerCache) : this(stream,
            componentResolver, xmlSerializerCache, false)
        {
        }

        /// <summary>
        ///     Orcus task file reader
        /// </summary>
        /// <param name="xml">Orcus task file xml data.</param>
        /// <param name="componentResolver">The service resolver used to resolve the task services</param>
        /// <param name="xmlSerializerCache">The xml serializer cache for deserialization</param>
        public OrcusTaskReader(XDocument xml, ITaskComponentResolver componentResolver, IXmlSerializerCache xmlSerializerCache) : base(xml)
        {
            _componentResolver = componentResolver;
            _xmlSerializerCache = xmlSerializerCache;
        }

        /// <summary>
        ///     Orcus task file reader
        /// </summary>
        /// <param name="stream">Orcus task file stream.</param>
        /// <param name="componentResolver">The service resolver used to resolve the task services</param>
        /// <param name="leaveStreamOpen">Leave the stream open</param>
        /// <param name="xmlSerializerCache">The xml serializer cache for deserialization</param>
        public OrcusTaskReader(Stream stream, ITaskComponentResolver componentResolver, IXmlSerializerCache xmlSerializerCache, bool leaveStreamOpen)
            : base(stream, leaveStreamOpen)
        {
            _componentResolver = componentResolver;
            _xmlSerializerCache = xmlSerializerCache;
        }

        public OrcusTask ReadTask()
        {
            return new OrcusTask
            {
                Name = GetName(),
                Id = GetId(),
                Audience = GetAudience(),
                Filters = GetFilters().ToList(),
                Triggers = GetTriggers().ToList(),
                StopEvents = GetStopEvents().ToList(),
                Commands = GetCommands().ToList()
            };
        }

        public AudienceCollection GetAudience()
        {
            var ns = Xml.Root.GetDefaultNamespace().NamespaceName;
            var audienceNode = Xml.Root.Elements(XName.Get(XmlNames.Audience, ns));

            var result = new AudienceCollection();
            foreach (var audienceElement in audienceNode.Elements())
                switch (audienceElement.Name.LocalName)
                {
                    case "AllClients":
                        result.IsAll = true;
                        result.Clear();
                        break;
                    case "Server":
                        result.IncludesServer = true;
                        break;
                    case "Clients":
                        if (result.IsAll)
                            continue;

                        var ids = GetAttributeValue(audienceElement, "id");
                        if (ids != null)
                        {
                            var targets = CommandTargetCollection.Parse(ids);
                            result.AddRange(targets);
                        }

                        break;
                }

            return result;
        }

        public IEnumerable<FilterInfo> GetFilters()
        {
            var ns = Xml.Root.GetDefaultNamespace().NamespaceName;
            var conditionsNode = Xml.Root.Elements(XName.Get(XmlNames.Filters, ns));

            foreach (var conditionElement in conditionsNode.Elements())
            {
                var conditionType = _componentResolver.ResolveFilter(conditionElement.Name.LocalName);
                yield return InternalDeserialize<FilterInfo>(conditionType, conditionElement);
            }
        }

        public IEnumerable<TriggerInfo> GetTriggers()
        {
            var ns = Xml.Root.GetDefaultNamespace().NamespaceName;
            var transmissionNode = Xml.Root.Elements(XName.Get(XmlNames.Triggers, ns));

            foreach (var transmissionElement in transmissionNode.Elements())
            {
                var transmissionType = _componentResolver.ResolveTrigger(transmissionElement.Name.LocalName);
                yield return InternalDeserialize<TriggerInfo>(transmissionType, transmissionElement);
            }
        }
        
        public IEnumerable<StopEventInfo> GetStopEvents()
        {
            var ns = Xml.Root.GetDefaultNamespace().NamespaceName;
            var stopNode = Xml.Root.Elements(XName.Get(XmlNames.Stop, ns));

            foreach (var stopElement in stopNode.Elements())
            {
                var stopEventType = _componentResolver.ResolveStopEvent(stopElement.Name.LocalName);
                yield return InternalDeserialize<StopEventInfo>(stopEventType, stopElement);
            }
        }

        public IEnumerable<CommandInfo> GetCommands()
        {
            var ns = Xml.Root.GetDefaultNamespace().NamespaceName;
            var commands = Xml.Root.Elements(XName.Get(XmlNames.Commands, ns));

            foreach (var commandElement in commands.Elements())
            {
                var name = GetAttributeValue(commandElement, "name") ?? throw new TaskParsingException("The name of a command must not be empty.");

                var commandType = _componentResolver.ResolveCommand(name);
                var command = InternalDeserialize<CommandInfo>(commandType, commandElement);
                
                yield return command;
            }
        }

        public IEnumerable<CommandMetadata> GetCommandMetadata()
        {
            var ns = Xml.Root.GetDefaultNamespace().NamespaceName;
            var commands = Xml.Root.Elements(XName.Get(XmlNames.Commands, ns));

            foreach (var commandElement in commands.Elements())
            {
                if (commandElement.Name.LocalName != "Command")
                    throw new TaskParsingException("The Commands collection may only consist of commands.");

                var commandMetadata = new CommandMetadata();
                commandMetadata.Name = GetAttributeValue(commandElement, "name") ??
                                       throw new TaskParsingException("The name of a command must not be empty.");

                var modules = GetAttributeValue(commandElement, "modules") ??
                              throw new TaskParsingException("The modules of a command may not be empty.");
                commandMetadata.Modules = modules.Split(';');

                yield return commandMetadata;
            }
        }

        private T InternalDeserialize<T>(Type type, XElement element)
        {
            var serializer = _xmlSerializerCache.Resolve(type, element.Name.LocalName);
            var result = serializer.Deserialize(element.CreateReader());
            return (T) result;
        }

        private static string GetAttributeValue(XElement element, string attributeName)
        {
            var attribute = element.Attribute(XName.Get(attributeName));
            return attribute?.Value;
        }
    }
}