using System;
using System.Collections.Generic;
#if DAPPER
using Dapper.Contrib.Extensions;

#endif

namespace Tasks.Infrastructure.Management.Data
{
    public class TaskReference
    {
#if DAPPER
        [ExplicitKey]
#endif
        public Guid TaskId { get; set; }

        public bool IsCompleted { get; set; }

        public IList<TaskSession> Sessions { get; set; }
    }
}