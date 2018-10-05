using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Orcus.Server.Connection.Commanding;
using Orcus.Server.Connection.Tasks.Audience;
using Orcus.Server.Connection.Tasks.Commands;
using Orcus.Server.Connection.Tasks.Conditions;
using Orcus.Server.Connection.Tasks.Execution;
using Orcus.Server.Connection.Tasks.StopEvents;
using Orcus.Server.Connection.Tasks.Transmission;

namespace Orcus.Server.Connection.Tasks
{
    public class OrcusTaskReader : OrcusTaskReaderBase
    {
        private const string Audience = "audience";
        private const string Conditions = "conditions";
        private const string Transmission = "transmission";
        private const string Execution = "execution";
        private const string Stop = "stop";
        private const string Commands = "commands";
        private readonly ITaskComponentResolver _componentResolver;

        /// <summary>
        ///     Orcus task file reader.
        /// </summary>
        public OrcusTaskReader(string path, ITaskComponentResolver componentResolver) : base(path)
        {
            _componentResolver = componentResolver;
        }

        /// <summary>
        ///     Orcus task file reader
        /// </summary>
        /// <param name="stream">Orcus task file stream.</param>
        /// <param name="componentResolver">The service resolver used to resolve the task services</param>
        public OrcusTaskReader(Stream stream, ITaskComponentResolver componentResolver) : this(stream, componentResolver, false)
        {
        }

        /// <summary>
        ///     Orcus task file reader
        /// </summary>
        /// <param name="xml">Orcus task file xml data.</param>
        /// <param name="componentResolver">The service resolver used to resolve the task services</param>
        public OrcusTaskReader(XDocument xml, ITaskComponentResolver componentResolver) : base(xml)
        {
            _componentResolver = componentResolver;
        }

        /// <summary>
        ///     Orcus task file reader
        /// </summary>
        /// <param name="stream">Orcus task file stream.</param>
        /// <param name="componentResolver">The service resolver used to resolve the task services</param>
        /// <param name="leaveStreamOpen">Leave the stream open</param>
        public OrcusTaskReader(Stream stream, ITaskComponentResolver componentResolver, bool leaveStreamOpen) : base(stream, leaveStreamOpen)
        {
            _componentResolver = componentResolver;
        }

        public AudienceCollection GetAudience()
        {
            var ns = Xml.Root.GetDefaultNamespace().NamespaceName;
            var audienceNode = Xml.Root.Elements(XName.Get(Audience, ns));

            var result = new AudienceCollection();
            foreach (var audienceElement in audienceNode.Elements())
                switch (audienceElement.Name.LocalName)
                {
                    case "All":
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

        public IEnumerable<ConditionInfo> GetConditions()
        {
            var ns = Xml.Root.GetDefaultNamespace().NamespaceName;
            var conditionsNode = Xml.Root.Elements(XName.Get(Conditions, ns));

            foreach (var conditionElement in conditionsNode.Elements())
            {
                var conditionType = _componentResolver.ResolveCondition(conditionElement.Name.LocalName);
                yield return InternalDeserialize<ConditionInfo>(conditionType, conditionElement);
            }
        }

        public IEnumerable<TransmissionInfo> GetTransmissionEvents()
        {
            var ns = Xml.Root.GetDefaultNamespace().NamespaceName;
            var transmissionNode = Xml.Root.Elements(XName.Get(Transmission, ns));

            foreach (var transmissionElement in transmissionNode.Elements())
            {
                var transmissionType = _componentResolver.ResolveTransmissionInfo(transmissionElement.Name.LocalName);
                yield return InternalDeserialize<TransmissionInfo>(transmissionType, transmissionElement);
            }
        }

        public IEnumerable<ExecutionInfo> GetExecutionEvents()
        {
            var ns = Xml.Root.GetDefaultNamespace().NamespaceName;
            var executionNode = Xml.Root.Elements(XName.Get(Execution, ns));

            foreach (var executionElement in executionNode.Elements())
            {
                var executionType = _componentResolver.ResolveExecutionInfo(executionElement.Name.LocalName);
                yield return InternalDeserialize<ExecutionInfo>(executionType, executionElement);
            }
        }

        public IEnumerable<StopEventInfo> GetStopEvents()
        {
            var ns = Xml.Root.GetDefaultNamespace().NamespaceName;
            var stopNode = Xml.Root.Elements(XName.Get(Stop, ns));

            foreach (var stopElement in stopNode.Elements())
            {
                var stopEventType = _componentResolver.ResolveStopEvent(stopElement.Name.LocalName);
                yield return InternalDeserialize<StopEventInfo>(stopEventType, stopElement);
            }
        }

        public IEnumerable<CommandInfo> GetCommands()
        {
            var ns = Xml.Root.GetDefaultNamespace().NamespaceName;
            var commands = Xml.Root.Elements(XName.Get(Commands, ns));

            foreach (var commandElement in commands.Elements())
            {
                var name = GetAttributeValue(commandElement, "name") ??
                           throw new TaskParsingException("The name of a command must not be empty.");
                
                var commandType = _componentResolver.ResolveCommand(name);
                var command = InternalDeserialize<CommandInfo>(commandType, commandElement);

                command.Name = name;

                var modules = GetAttributeValue(commandElement, "modules") ??
                              throw new TaskParsingException("The modules of a command may not be empty.");
                command.Modules = modules.Split(';');

                yield return command;
            }
        }

        public IEnumerable<CommandMetadata> GetCommandMetadata()
        {
            var ns = Xml.Root.GetDefaultNamespace().NamespaceName;
            var commands = Xml.Root.Elements(XName.Get(Commands, ns));

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
            var serializer = new XmlSerializer(type, new XmlRootAttribute(element.Name.LocalName));
            var result = serializer.Deserialize(element.CreateReader());
            return (T)result;
        }

        private static string GetAttributeValue(XElement element, string attributeName)
        {
            var attribute = element.Attribute(XName.Get(attributeName));
            return attribute?.Value;
        }
    }

    public interface ITaskComponentResolver
    {
        Type ResolveCondition(string name);
        Type ResolveTransmissionInfo(string name);
        Type ResolveExecutionInfo(string name);
        Type ResolveStopEvent(string name);
        Type ResolveCommand(string name);
    }
}