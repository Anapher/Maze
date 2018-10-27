using System;
using System.Threading.Tasks;
using Orcus.Modules.Api.Extensions;
using Tasks.Infrastructure.Client.Library;

namespace Tasks.Infrastructure.Client.Trigger
{
    public class TaskTriggerContext : TriggerContext
    {
        private readonly string _sourceTrigger;
        private readonly TaskRunner _taskRunner;

        public TaskTriggerContext(TaskRunner taskRunner, string sourceTrigger)
        {
            _taskRunner = taskRunner;
            _sourceTrigger = sourceTrigger;
        }

        public override Task<TaskSessionTrigger> CreateSession(SessionKey sessionKey) => CreateSession(sessionKey, _sourceTrigger);

        public override async Task<TaskSessionTrigger> CreateSession(SessionKey sessionKey, string description)
        {
            var sessionManager = _taskRunner.Services.GetRequiredService<ITaskSessionManager>();
            var session = await sessionManager.OpenSession(sessionKey, _taskRunner.OrcusTask, description);

            return new LocalTaskSessionTrigger(_taskRunner, session);
        }

        public override void ReportNextTrigger(DateTimeOffset dateTimeOffset)
        {
        }
    }
}