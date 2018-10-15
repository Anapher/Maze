using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Library.Tasks;
using Orcus.Utilities;

namespace Orcus.Server.Service.Tasks
{
    public class OrcusTaskManager
    {
        private readonly ITaskDirectory _taskDirectory;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _tasks;

        public OrcusTaskManager(ITaskDirectory taskDirectory, IServiceProvider serviceProvider)
        {
            _taskDirectory = taskDirectory;
            _serviceProvider = serviceProvider;
            _tasks = new ConcurrentDictionary<Guid, CancellationTokenSource>();
            ClientTasks = new ConcurrentBag<OrcusTask>();
        }

        public ConcurrentBag<OrcusTask> ClientTasks { get; }

        public async Task AddTask(OrcusTask orcusTask)
        {
            var taskService = new OrcusTaskService(orcusTask, _serviceProvider);
            var cancellationTokenSource = new CancellationTokenSource();

            if (!_tasks.TryAdd(orcusTask.Id, cancellationTokenSource))
            {
                cancellationTokenSource.Dispose();
                throw new InvalidOperationException($"Unable to add task with id {orcusTask.Id} because the task already exists.");
            }

            await _taskDirectory.WriteTask(orcusTask);

            var (executeOnServer, executeOnClient) = GetTaskExecutionMode(orcusTask);
            if (executeOnServer)
                RunTask(taskService, cancellationTokenSource.Token).ContinueWith(task => cancellationTokenSource.Dispose()).Forget();
            if (executeOnClient)
            {
                TransmitTask(orcusTask, cancellationTokenSource.Token).ContinueWith(task => cancellationTokenSource.Dispose()).Forget();
                ClientTasks.Add(orcusTask);
            }

            //add to database
        }

        private (bool server, bool client) GetTaskExecutionMode(OrcusTask orcusTask)
        {
            var executeOnServer = false;
            var executeOnClient = false;

            using (var scope = _serviceProvider.CreateScope())
            {
                foreach (var orcusTaskCommand in orcusTask.Commands)
                {
                    var serviceType = typeof(ITaskExecutor<>).MakeGenericType(orcusTaskCommand.GetType());
                    var service = scope.ServiceProvider.GetService(serviceType);
                    if (service == null)
                        executeOnClient = true;
                    else executeOnServer = true;
                }
            }

            return (executeOnServer, executeOnClient);
        }

        private async Task TransmitTask(OrcusTask orcusTask, CancellationToken cancellationToken)
        {

        }

        private async Task RunTask(OrcusTaskService taskService, CancellationToken cancellationToken)
        {
            try
            {
                await taskService.Run(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return; //just ignore
            }
            catch (Exception e)
            {
                taskService.Logger.LogCritical(e, "An error occurred when running task service for task {taskId}.", taskService.OrcusTask.Id);
                return;
            }

            //set finished
        }
    }
}
