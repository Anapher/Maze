using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
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
  public  class TaskComponentResolver: ITaskComponentResolver
    {
        private readonly IImmutableDictionary<string, Type> _filterTypes;
        private readonly IImmutableDictionary<string, Type> _transmissionTypes;
        private readonly IImmutableDictionary<string, Type> _executionTypes;
        private readonly IImmutableDictionary<string, Type> _stopEventTypes;
        private readonly IImmutableDictionary<string, Type> _commandTypes;

        public TaskComponentResolver(IComponentContext componentContext)
        {
            _filterTypes = ResolveTypes<FilterInfo>(componentContext);
            _transmissionTypes = ResolveTypes<TransmissionInfo>(componentContext);
            _executionTypes = ResolveTypes<ExecutionInfo>(componentContext);
            _stopEventTypes = ResolveTypes<StopEventInfo>(componentContext);
            _commandTypes = ResolveTypes<CommandInfo>(componentContext);
        }

        private IImmutableDictionary<string, Type> ResolveTypes<T>(IComponentContext context)
        {
            return context.Resolve<IEnumerable<T>>()
                .ToImmutableDictionary(x => x.GetType().Name.TrimEnd(nameof(T), StringComparison.Ordinal), x => x.GetType());
        }

        public Type ResolveFilter(string name)
        {
            return _filterTypes[name];
        }

        public Type ResolveTransmissionInfo(string name)
        {
            return _transmissionTypes[name];
        }

        public Type ResolveExecutionInfo(string name)
        {
            return _executionTypes[name];
        }

        public Type ResolveStopEvent(string name)
        {
            return _stopEventTypes[name];
        }

        public Type ResolveCommand(string name)
        {
            return _commandTypes[name];
        }

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
    }
}
