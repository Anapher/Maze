using System.Collections.Generic;
using Tasks.Infrastructure.Core.Audience;
using Tasks.Infrastructure.Core.Commands;
using Tasks.Infrastructure.Core.Filter;
using Tasks.Infrastructure.Core.StopEvents;
using Tasks.Infrastructure.Core.Triggers;

namespace Tasks.Infrastructure.Administration.Library
{
    public class TaskContext
    {
        public AudienceCollection Audience { get; set; }
        public IList<FilterInfo> Filters { get; set; }
        public IList<TriggerInfo> Triggers { get; set; }
        public IList<CommandInfo> Commands { get; set; }
        public IList<StopEventInfo> StopEvents { get; set; }
    }
}