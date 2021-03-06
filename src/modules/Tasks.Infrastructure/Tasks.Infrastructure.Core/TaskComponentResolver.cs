using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Maze.Server.Connection.Extensions;
using Tasks.Infrastructure.Core.Commands;
using Tasks.Infrastructure.Core.Filter;
using Tasks.Infrastructure.Core.StopEvents;
using Tasks.Infrastructure.Core.Triggers;

namespace Tasks.Infrastructure.Core
{
    /// <summary>
    ///     Default implementation for <see cref="ITaskComponentResolver"/>
    /// </summary>
    public class TaskComponentResolver : ITaskComponentResolver
    {
        private readonly IImmutableDictionary<string, Type> _commandTypes;
        private readonly IImmutableDictionary<string, Type> _filterTypes;
        private readonly IImmutableDictionary<string, Type> _stopEventTypes;
        private readonly IImmutableDictionary<string, Type> _triggerTypes;

        public TaskComponentResolver(IServiceProvider componentContext)
        {
            _filterTypes = ResolveTypes<FilterInfo>(componentContext);
            _triggerTypes = ResolveTypes<TriggerInfo>(componentContext);
            _stopEventTypes = ResolveTypes<StopEventInfo>(componentContext);
            _commandTypes = ResolveTypes<CommandInfo>(componentContext);
        }

        public Type ResolveFilter(string name) => _filterTypes[name];
        public Type ResolveTrigger(string name) => _triggerTypes[name];
        public Type ResolveStopEvent(string name) => _stopEventTypes[name];
        public Type ResolveCommand(string name) => _commandTypes[name];

        public string ResolveName(Type type)
        {
            if (typeof(FilterInfo).IsAssignableFrom(type))
                return type.Name.TrimEnd(nameof(FilterInfo), StringComparison.Ordinal);
            if (typeof(TriggerInfo).IsAssignableFrom(type))
                return type.Name.TrimEnd(nameof(TriggerInfo), StringComparison.Ordinal);
            if (typeof(StopEventInfo).IsAssignableFrom(type))
                return type.Name.TrimEnd(nameof(StopEventInfo), StringComparison.Ordinal);
            if (typeof(CommandInfo).IsAssignableFrom(type))
                return type.Name.TrimEnd(nameof(CommandInfo), StringComparison.Ordinal);

            throw new ArgumentException($"The type {type.FullName} is not supported.");
        }

        private IImmutableDictionary<string, Type> ResolveTypes<T>(IServiceProvider context)
        {
            var name = typeof(T).Name;
            var services = (IEnumerable<T>) context.GetService(typeof(IEnumerable<T>));
            return services
                .ToImmutableDictionary(x => x.GetType().Name.TrimEnd(name, StringComparison.Ordinal), x => x.GetType());
        }
    }
}