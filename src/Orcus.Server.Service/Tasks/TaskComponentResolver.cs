using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Autofac;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Connection.Tasks.Commands;
using Orcus.Server.Connection.Tasks.Execution;
using Orcus.Server.Connection.Tasks.Filter;
using Orcus.Server.Connection.Tasks.StopEvents;
using Orcus.Server.Connection.Tasks.Transmission;
using Orcus.Server.Service.Extensions;

namespace Orcus.Server.Service.Tasks
{
    public class TaskComponentResolver : ITaskComponentResolver
    {
        private readonly IImmutableDictionary<string, Type> _commandTypes;
        private readonly IImmutableDictionary<string, Type> _executionTypes;
        private readonly IImmutableDictionary<string, Type> _filterTypes;
        private readonly IImmutableDictionary<string, Type> _stopEventTypes;
        private readonly IImmutableDictionary<string, Type> _transmissionTypes;

        public TaskComponentResolver(IComponentContext componentContext)
        {
            _filterTypes = ResolveTypes<FilterInfo>(componentContext);
            _transmissionTypes = ResolveTypes<TransmissionInfo>(componentContext);
            _executionTypes = ResolveTypes<ExecutionInfo>(componentContext);
            _stopEventTypes = ResolveTypes<StopEventInfo>(componentContext);
            _commandTypes = ResolveTypes<CommandInfo>(componentContext);
        }

        public Type ResolveFilter(string name) => _filterTypes[name];
        public Type ResolveTransmissionInfo(string name) => _transmissionTypes[name];
        public Type ResolveExecutionInfo(string name) => _executionTypes[name];
        public Type ResolveStopEvent(string name) => _stopEventTypes[name];
        public Type ResolveCommand(string name) => _commandTypes[name];

        public string ResolveName(Type type)
        {
            if (typeof(FilterInfo).IsAssignableFrom(type))
                return type.Name + nameof(FilterInfo);
            if (typeof(TransmissionInfo).IsAssignableFrom(type))
                return type.Name + nameof(TransmissionInfo);
            if (typeof(ExecutionInfo).IsAssignableFrom(type))
                return type.Name + nameof(ExecutionInfo);
            if (typeof(StopEventInfo).IsAssignableFrom(type))
                return type.Name + nameof(StopEventInfo);
            if (typeof(CommandInfo).IsAssignableFrom(type))
                return type.Name + nameof(CommandInfo);

            throw new ArgumentException($"The type {type.FullName} is not supported.");
        }

        private IImmutableDictionary<string, Type> ResolveTypes<T>(IComponentContext context)
        {
            return context.Resolve<IEnumerable<T>>()
                .ToImmutableDictionary(x => x.GetType().Name.TrimEnd(nameof(T), StringComparison.Ordinal), x => x.GetType());
        }
    }
}