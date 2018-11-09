using System;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using RequestTransmitter.Client;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Client.Options;
using Tasks.Infrastructure.Client.Rest;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Data;
using FileMode = System.IO.FileMode;

namespace Tasks.Infrastructure.Client
{
    public interface ITaskSessionManager
    {
        Task<TaskSession> OpenSession(SessionKey sessionKey, OrcusTask orcusTask, string description);
        Task<Guid> StartExecution(OrcusTask orcusTask, TaskSession taskSession, TaskExecution taskExecution);
        Task AppendCommandResult(OrcusTask orcusTask, CommandResult commandResult);
    }

    public class TaskSessionManager : ITaskSessionManager
    {
        private readonly IFileSystem _fileSystem;
        private readonly TasksOptions _options;
        private readonly AsyncReaderWriterLock _readerWriterLock;
        private readonly IRequestTransmitter _requestTransmitter;

        public TaskSessionManager(IFileSystem fileSystem, IRequestTransmitter requestTransmitter, IOptions<TasksOptions> options)
        {
            _fileSystem = fileSystem;
            _requestTransmitter = requestTransmitter;
            _options = options.Value;
            _readerWriterLock = new AsyncReaderWriterLock();

            var mapper = BsonMapper.Global;
            mapper.Entity<TaskSession>().Id(x => x.TaskSessionId, autoId: false).DbRef(x => x.Executions, nameof(TaskExecution));
            mapper.Entity<TaskExecution>().Id(x => x.TaskExecutionId);
        }

        public async Task<TaskSession> OpenSession(SessionKey sessionKey, OrcusTask orcusTask, string description)
        {
            using (await _readerWriterLock.ReaderLockAsync())
            {
                var file = _fileSystem.FileInfo.FromFileName(GetTaskDbFilename(orcusTask));
                if (file.Exists)
                    using (var dbStream = file.OpenRead())
                    using (var db = new LiteDatabase(dbStream))
                    {
                        var collection = db.GetCollection<TaskSession>(nameof(TaskSession));
                        collection.EnsureIndex(x => x.TaskSessionId, true);

                        var taskSession = collection.IncludeAll().FindById(sessionKey.Hash);
                        if (taskSession != null)
                            return taskSession;
                    }
            }

            return new TaskSession
            {
                TaskSessionId = sessionKey.Hash,
                Description = description,
                CreatedOn = DateTimeOffset.UtcNow,
                TaskReference = new TaskReference {TaskId = orcusTask.Id},
                Transmissions = ImmutableList<TaskTransmission>.Empty,
                Executions = ImmutableList<TaskExecution>.Empty
            };
        }

        public async Task<Guid> StartExecution(OrcusTask orcusTask, TaskSession taskSession, TaskExecution taskExecution)
        {
            using (await _readerWriterLock.WriterLockAsync())
            {
                var file = _fileSystem.FileInfo.FromFileName(GetTaskDbFilename(orcusTask));
                file.Directory.Create();

                using (var dbStream = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite))
                using (var db = new LiteDatabase(dbStream))
                {
                    var sessions = db.GetCollection<TaskSession>(nameof(TaskSession));
                    sessions.EnsureIndex(x => x.TaskSessionId, unique: true);

                    var taskSessionEntity = sessions.FindById(taskSession.TaskSessionId);
                    if (taskSessionEntity == null)
                    {
                        taskSessionEntity = new TaskSession
                        {
                            TaskSessionId = taskSession.TaskSessionId,
                            Description = taskSession.Description,
                            CreatedOn = DateTimeOffset.UtcNow
                        };

                        _requestTransmitter.Transmit(TasksResource.CreateSessionRequest(taskSessionEntity));
                    }

                    taskExecution.TaskSessionId = taskSessionEntity.TaskSessionId;

                    var executions = db.GetCollection<TaskExecution>(nameof(TaskExecution));
                    Guid executionId = executions.Insert(taskExecution);

                    _requestTransmitter.Transmit(TasksResource.CreateExecutionRequest(taskExecution));

                    return executionId;
                }
            }
        }

        public async Task AppendCommandResult(OrcusTask orcusTask, CommandResult commandResult)
        {
            using (await _readerWriterLock.WriterLockAsync())
            {
                var file = _fileSystem.FileInfo.FromFileName(GetTaskDbFilename(orcusTask));
                file.Directory.Create();

                using (var dbStream = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite))
                using (var db = new LiteDatabase(dbStream))
                {
                    var results = db.GetCollection<CommandResult>(nameof(CommandResult));
                    results.Insert(commandResult);

                    await _requestTransmitter.Transmit(TasksResource.CreateCommandResultRequest(commandResult));
                }
            }
        }

        private string GetTaskDbFilename(OrcusTask orcusTask) => _fileSystem.Path.Combine(_options.SessionsDirectory, orcusTask.Id.ToString("N"));
    }
}