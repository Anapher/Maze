using System;
using System.Collections.Generic;
using Orcus.Server.Connection.Tasks.Audience;
using Orcus.Server.Connection.Tasks.Commands;
using Orcus.Server.Connection.Tasks.Execution;
using Orcus.Server.Connection.Tasks.Filter;
using Orcus.Server.Connection.Tasks.StopEvents;
using Orcus.Server.Connection.Tasks.Transmission;

namespace Orcus.Server.Connection.Tasks
{
    public class OrcusTask
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public bool OneExecutionPerTarget { get; set; }
        public TimeSpan? RestartOnFailInterval { get; set; }
        public int? MaximumRestarts { get; set; }

        public AudienceCollection Audience { get; set; }
        public IList<FilterInfo> Filters { get; set; }
        public IList<TransmissionInfo> Transmission { get; set; }
        public IList<ExecutionInfo> Execution { get; set; }
        public IList<StopEventInfo> StopEvents { get; set; }
        public IList<CommandInfo> Commands { get; set; }
    }
}