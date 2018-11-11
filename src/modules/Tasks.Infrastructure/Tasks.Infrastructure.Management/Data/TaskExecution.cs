using System;
using System.Collections.Generic;
#if DAPPER
using Dapper.Contrib.Extensions;

#endif

namespace Tasks.Infrastructure.Management.Data
{
    public class TaskExecution
    {
#if DAPPER
        [ExplicitKey]
#endif
        public Guid TaskExecutionId { get; set; }

        public Guid TaskReferenceId { get; set; }
        public string TaskSessionId { get; set; }

        public int? TargetId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

#if DAPPER
        [Write(false)]
#endif
        public IList<CommandResult> CommandResults { get; set; }
    }
}