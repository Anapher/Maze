using System.Collections.Generic;
using Orcus.Server.Connection.Tasks.Audience;
using Orcus.Server.Connection.Tasks.Conditions;
using Orcus.Server.Connection.Tasks.Execution;
using Orcus.Server.Connection.Tasks.StopEvents;
using Orcus.Server.Connection.Tasks.Transmission;

namespace Orcus.Administration.Library.Tasks
{
    public class TaskContext
    {
        public AudienceCollection Audience { get; set; }
        public IList<ConditionInfo> Conditions { get; set; }
        public IList<TransmissionInfo> Transmission { get; set; }
        public IList<ExecutionInfo> Execution { get; set; }
        public IList<StopEventInfo> StopEvents { get; set; }
    }
}