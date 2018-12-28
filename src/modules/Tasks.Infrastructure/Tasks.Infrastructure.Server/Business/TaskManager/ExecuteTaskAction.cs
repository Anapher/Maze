using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Maze.Server.Connection.Utilities;
using Maze.Server.Data.EfCode;
using Maze.Server.Library.Hubs;
using Maze.Server.Library.Services;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Server.Core;
using Tasks.Infrastructure.Server.Core.Storage;
using Tasks.Infrastructure.Server.Filter;
using Tasks.Infrastructure.Server.Library;
using Tasks.Infrastructure.Server.Rest.V1;

namespace Tasks.Infrastructure.Server.Business.TaskManager
{
    public interface IExecuteTaskAction : IGenericActionAsync<MazeTask, TaskSessionsInfo> { }

    public class ExecuteTaskAction : BusinessActionErrors, IExecuteTaskAction
    {
        private readonly IMazeTaskManagerManagement _management;
        private readonly IConnectionManager _connectionManager;
        private readonly AppDbContext _dbContext;
        private readonly ITaskComponentResolver _taskComponentResolver;
        private readonly IXmlSerializerCache _xmlSerializerCache;
        private readonly IHubContext<AdministrationHub> _hubContext;

        public ExecuteTaskAction(IMazeTaskManagerManagement management, IConnectionManager connectionManager, AppDbContext dbContext,
            ITaskComponentResolver taskComponentResolver, IXmlSerializerCache xmlSerializerCache, IHubContext<AdministrationHub> hubContext)
        {
            _management = management;
            _connectionManager = connectionManager;
            _dbContext = dbContext;
            _taskComponentResolver = taskComponentResolver;
            _xmlSerializerCache = xmlSerializerCache;
            _hubContext = hubContext;
        }

        public async Task<TaskSessionsInfo> BizActionAsync(MazeTask inputData)
        {
            var builder = new TaskSessionInfoBuilder(inputData.Id);
            builder.TaskSessionAdded += BuilderOnTaskSessionAdded;
            builder.TaskExecutionAdded += BuilderOnTaskExecutionAdded;
            builder.TaskResultAdded += BuilderOnTaskResultAdded;

            var storage = new MemoryTaskResultStorage();

            //trigger locally
            await _management.TriggerNow(inputData, SessionKey.Create("Execute"), storage);

            builder.Add(new TaskSessionsInfo
            {
                Sessions = storage.Sessions,
                Executions = storage.Executions,
                Results = storage.CommandResults
            }, TargetId.ServerId);

            //trigger on clients
            var audienceFilter = new AudienceFilter(inputData.Audience);
            var clients = (await _dbContext.Clients.Select(x => x.ClientId).ToListAsync()).ToHashSet();

            var onlineClients = _connectionManager.ClientConnections.Where(x => clients.Contains(x.Key));

            var tasks = new Dictionary<Task<TaskSessionsInfo>, int>();

            foreach (var onlineClient in onlineClients)
            {
                if (audienceFilter.Invoke(onlineClient.Key))
                {
                    //add all at once because the tasks don't do anything except waiting for the completion source anyways
                    var task = TasksResource.ExecuteTask(inputData, _taskComponentResolver, _xmlSerializerCache, onlineClient.Value);
                    tasks.Add(task, onlineClient.Key);
                }
            }

            while (tasks.Any())
            {
                var task = await Task.WhenAny(tasks.Keys);
                builder.Add(task.Result, new TargetId(tasks[task]));

                tasks.Remove(task);
            }

            return builder.Build();
        }

        private void BuilderOnTaskResultAdded(object sender, CommandResultDto e)
        {
            _hubContext.Clients.All.SendAsync(HubEventNames.TaskCommandResultCreated, e);
        }

        private void BuilderOnTaskExecutionAdded(object sender, TaskExecutionDto e)
        {
            _hubContext.Clients.All.SendAsync(HubEventNames.TaskExecutionCreated, e);
        }

        private void BuilderOnTaskSessionAdded(object sender, TaskSessionDto e)
        {
            _hubContext.Clients.All.SendAsync(HubEventNames.TaskSessionCreated, e);
        }
    }

    public class TaskSessionInfoBuilder
    {
        private readonly Guid _taskId;
        private readonly Dictionary<string, TaskSessionDto> _sessions;
        private readonly Dictionary<Guid, TaskExecutionDto> _executions;
        private readonly Dictionary<Guid, CommandResultDto> _results;

        public TaskSessionInfoBuilder(Guid taskId)
        {
            _taskId = taskId;

            _sessions = new Dictionary<string, TaskSessionDto>();
            _executions = new Dictionary<Guid, TaskExecutionDto>();
            _results = new Dictionary<Guid, CommandResultDto>();
        }

        public event EventHandler<TaskSessionDto> TaskSessionAdded;
        public event EventHandler<TaskExecutionDto> TaskExecutionAdded;
        public event EventHandler<CommandResultDto> TaskResultAdded;

        public void Add(TaskSessionsInfo taskSessions, TargetId targetId)
        {
            if (taskSessions.Sessions?.Count > 0)
                foreach (var session in taskSessions.Sessions)
                {
                    AddSession(session);
                }

            if (taskSessions.Executions?.Count > 0)
                foreach (var execution in taskSessions.Executions)
                    AddExecution(execution, targetId);

            if (taskSessions.Results?.Count > 0)
                foreach (var commandResult in taskSessions.Results)
                    AddCommandResult(commandResult);
        }

        public TaskSessionsInfo Build()
        {
            //filter all executions that have a sessions
            var executionsDictionary = _executions.Where(x => _sessions.ContainsKey(x.Value.TaskSessionId)).ToDictionary(x => x.Key, x => x.Value);

            //filter all results that have an execution
            var results = _results.Where(x => executionsDictionary.ContainsKey(x.Value.TaskExecutionId)).Select(x => x.Value).ToList();

            //filter all sessions that have an execution
            var sessions = _sessions.Values.Where(x => executionsDictionary.Any(y => y.Value.TaskSessionId == x.TaskSessionId)).ToList();

            //filter all executions that have a result
            var executions = executionsDictionary.Values.Where(x => results.Any(y => y.TaskExecutionId == x.TaskExecutionId)).ToList();

            return new TaskSessionsInfo
            {
                Sessions = sessions,
                Executions = executions,
                Results = results
            };
        }

        private void AddSession(TaskSessionDto taskSessionDto)
        {
            if (taskSessionDto.Validate(new ValidationContext(taskSessionDto)).Any())
                return;

            if (taskSessionDto.TaskReferenceId != _taskId)
                return;

            if (_sessions.TryAdd(taskSessionDto.TaskSessionId, taskSessionDto))
            {
                TaskSessionAdded?.Invoke(this, taskSessionDto);
            }
        }

        private void AddExecution(TaskExecutionDto taskExecutionDto, TargetId targetId)
        {
            if (taskExecutionDto.Validate(new ValidationContext(taskExecutionDto)).Any())
                return;

            if (taskExecutionDto.TaskReferenceId != _taskId)
                return;

            taskExecutionDto.TargetId = targetId.IsServer ? (int?) null : targetId.ClientId;

            if (_executions.TryAdd(taskExecutionDto.TaskExecutionId, taskExecutionDto))
            {
                TaskExecutionAdded?.Invoke(this, taskExecutionDto);
            }
        }

        private void AddCommandResult(CommandResultDto commandResultDto)
        {
            if (commandResultDto.Validate(new ValidationContext(commandResultDto)).Any())
                return;

            if (_results.TryAdd(commandResultDto.CommandResultId, commandResultDto))
            {
                TaskResultAdded?.Invoke(this, commandResultDto);
            }
        }
    }
}
