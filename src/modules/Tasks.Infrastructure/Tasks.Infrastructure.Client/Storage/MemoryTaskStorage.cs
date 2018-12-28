using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Client.Storage
{
    public class MemoryTaskStorage : ITaskStorage
    {
        private readonly ConcurrentDictionary<Guid, object> _finishedTasks = new ConcurrentDictionary<Guid, object>();
        private readonly object _sessionsLock = new object();
        private readonly object _executionsLock = new object();
        private readonly object _commandResultsLock = new object();

        public List<TaskSession> Sessions { get; } = new List<TaskSession>();
        public List<TaskExecution> Executions { get; } = new List<TaskExecution>();
        public List<CommandResult> CommandResults { get; } = new List<CommandResult>();

        public Task<TaskSession> OpenSession(SessionKey sessionKey, MazeTask mazeTask, string description)
        {
            return Task.FromResult(new TaskSession
            {
                TaskSessionId = sessionKey.Hash,
                Description = description,
                CreatedOn = DateTimeOffset.UtcNow,
                TaskReference = new TaskReference {TaskId = mazeTask.Id},
                TaskReferenceId = mazeTask.Id,
                Executions = ImmutableList<TaskExecution>.Empty
            });
        }

        public Task StartExecution(MazeTask mazeTask, TaskSession taskSession, TaskExecution taskExecution)
        {
            taskExecution.TaskExecutionId = Guid.NewGuid();
            taskExecution.TaskSessionId = taskSession.TaskSessionId;

            lock (_executionsLock)
            lock (_sessionsLock)
            {
                if (!Sessions.Any(x => x.TaskSessionId == taskSession.TaskSessionId && x.TaskReferenceId == mazeTask.Id))
                    Sessions.Add(taskSession);

                Executions.Add(taskExecution);
            }

            return Task.CompletedTask;
        }

        public Task AppendCommandResult(MazeTask mazeTask, CommandResult commandResult)
        {
            lock (_commandResultsLock)
            {
                CommandResults.Add(commandResult);
            }

            return Task.CompletedTask;
        }

        public Task MarkTaskFinished(MazeTask mazeTask)
        {
            _finishedTasks.TryAdd(mazeTask.Id, null);
            return Task.CompletedTask;
        }

        public Task<bool> CheckTaskFinished(MazeTask mazeTask) => Task.FromResult(_finishedTasks.ContainsKey(mazeTask.Id));
    }
}
