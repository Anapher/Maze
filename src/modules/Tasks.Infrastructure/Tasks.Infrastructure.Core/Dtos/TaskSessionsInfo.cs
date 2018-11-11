using System.Collections.Generic;

namespace Tasks.Infrastructure.Core.Dtos
{
    public class TaskSessionsInfo
    {
        public IList<TaskSessionDto> Sessions { get; set; }
        public IList<TaskExecutionDto> Executions { get; set; }
        public IList<CommandResultDto> Results { get; set; }
    }
}