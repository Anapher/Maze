using System;
using System.Collections.Generic;
#if DAPPER
using Dapper.Contrib.Extensions;

#endif

namespace Tasks.Infrastructure.Management.Data
{
    public class TaskSession
    {
        public string TaskSessionId { get; set; }
        public Guid TaskReferenceId { get; set; }

        public string Description { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

#if DAPPER
        [Write(false)]
#endif
        public TaskReference TaskReference { get; set; }

#if DAPPER
        [Write(false)]
#endif
        public IList<TaskExecution> Executions { get; set; }
    }
}