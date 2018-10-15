using System.Collections.Generic;
using Orcus.Server.Connection.Tasks.Audience;
using Orcus.Server.Connection.Tasks.Execution;
using Orcus.Server.Connection.Tasks.Filter;
using Orcus.Server.Connection.Tasks.StopEvents;
using Orcus.Server.Connection.Tasks.Triggers;

namespace Orcus.Administration.Library.Tasks
{
    public class TaskContext
    {
        public AudienceCollection Audience { get; set; }
        public IList<FilterInfo> Conditions { get; set; }
        public IList<TriggerInfo> Transmission { get; set; }
        public IList<ExecutionInfo> Execution { get; set; }
        public IList<StopEventInfo> StopEvents { get; set; }
    }
}