using System;
#if DAPPER
using Dapper.Contrib.Extensions;
#endif

namespace Tasks.Infrastructure.Management.Data
{
    public class CommandResult
    {
#if DAPPER
        [ExplicitKey]
#endif
        public Guid CommandResultId { get; set; }
        public Guid TaskExecutionId { get; set; }

        public string CommandName { get; set; }
        public string Result { get; set; }
        public int? Status { get; set; }
        public DateTimeOffset FinishedAt { get; set; }
    }
}
