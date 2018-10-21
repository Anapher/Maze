using System.Threading;
using System.Threading.Tasks;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Core.Data;

namespace Tasks.Infrastructure.Client
{
    public class LocalTaskSessionTrigger : TaskSessionTrigger
    {
        private readonly TaskRunner _taskRunner;

        public LocalTaskSessionTrigger(TaskRunner taskRunner, TaskSession taskSession)
        {
            _taskRunner = taskRunner;
            Info = taskSession;
        }

        public override Task Invoke()
        {
            return _taskRunner.Execute(Info, CancellationToken.None);
        }

        public override TaskSession Info { get; }
    }
}