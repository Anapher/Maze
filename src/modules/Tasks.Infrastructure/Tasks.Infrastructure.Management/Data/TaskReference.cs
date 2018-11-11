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

#if DAPPER
        [Write(false)]
#endif
        public IList<TaskTransmission> Transmissions { get; set; }
#if DAPPER
        [Write(false)]
#endif
        public IList<TaskSession> Sessions { get; set; }
    }
}