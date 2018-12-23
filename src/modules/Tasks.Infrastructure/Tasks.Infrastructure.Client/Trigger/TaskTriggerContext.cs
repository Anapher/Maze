using System;
using System.Threading.Tasks;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Client.Storage;

namespace Tasks.Infrastructure.Client.Trigger
{
    public class TaskTriggerContext : TriggerContext
    {
        private readonly string _sourceTrigger;
        private readonly ITaskStorage _taskStorage;
        private readonly TaskRunner _taskRunner;

        public TaskTriggerContext(TaskRunner taskRunner, string sourceTrigger, ITaskStorage taskStorage)
        {
            _taskRunner = taskRunner;
            _sourceTrigger = sourceTrigger;
            _taskStorage = taskStorage;
        }

        public override Task<TaskSessionTrigger> CreateSession(SessionKey sessionKey) => CreateSession(sessionKey, _sourceTrigger);

        public override async Task<TaskSessionTrigger> CreateSession(SessionKey sessionKey, string description)
        {
            var session = await _taskStorage.OpenSession(sessionKey, _taskRunner.OrcusTask, description);
            return new LocalTaskSessionTrigger(_taskRunner, session);
        }

        public override void ReportNextTrigger(DateTimeOffset dateTimeOffset)
        {
            _taskRunner.NextTrigger = dateTimeOffset;
        }
    }
}