using System;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Client.Options;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Data;
using FileMode = System.IO.FileMode;

namespace Tasks.Infrastructure.Client
{
    public interface ITaskSessionManager
    {
        Task<TaskSession> OpenSession(SessionKey sessionKey, OrcusTask orcusTask, string description);
        Task CreateExecution(OrcusTask orcusTask, TaskSession taskSession, TaskExecution taskExecution);
    }

    public class TaskSessionManager : ITaskSessionManager
    {
        private readonly IFileSystem _fileSystem;
        private readonly TasksOptions _options;
        private readonly AsyncReaderWriterLock _readerWriterLock;

        public TaskSessionManager(IFileSystem fileSystem, IOptions<TasksOptions> options)
        {
            _fileSystem = fileSystem;
            _options = options.Value;
            _readerWriterLock = new AsyncReaderWriterLock();

            var mapper = BsonMapper.Global;
            mapper.Entity<TaskSession>().Id(x => x.TaskSessionId).DbRef(x => x.Executions, nameof(TaskExecution));
            mapper.Entity<TaskExecution>().Id(x => x.TaskExecutionId);
        }

        public async Task<TaskSession> OpenSession(SessionKey sessionKey, OrcusTask orcusTask, string description)
        {
            using (await _readerWriterLock.ReaderLockAsync())
            {
                var file = _fileSystem.FileInfo.FromFileName(GetTaskDbFilename(orcusTask));
                if (file.Exists)
                {
                    using (var dbStream = file.OpenRead())
                    using (var db = new LiteDatabase(dbStream))
                    {
                        var collection = db.GetCollection<TaskSession>(nameof(TaskSession));
                        collection.EnsureIndex(x => x.TaskSessionHash, true);

                        var taskSession = collection.Include(x => x.Executions).Find(x => x.TaskSessionHash == sessionKey.Hash).SingleOrDefault();
                        if (taskSession != null)
                            return taskSession;
                    }
                }
            }

            return new TaskSession
            {
                Description = description,
                TaskSessionHash = sessionKey.Hash,
                CreatedOn = DateTimeOffset.UtcNow,
                TaskReference = new TaskReference {TaskId = orcusTask.Id},
                Transmissions = ImmutableList<TaskTransmission>.Empty,
                Executions = ImmutableList<TaskExecution>.Empty
            };
        }

        public async Task CreateExecution(OrcusTask orcusTask, TaskSession taskSession, TaskExecution taskExecution)
        {
            using (await _readerWriterLock.WriterLockAsync())
            {
                var file = _fileSystem.FileInfo.FromFileName(GetTaskDbFilename(orcusTask));
                file.Directory.Create();

                using (var dbStream = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite))
                using (var db = new LiteDatabase(dbStream))
                {
                    var sessions = db.GetCollection<TaskSession>(nameof(TaskSession));
                    sessions.EnsureIndex(x => x.TaskSessionHash, true);

                    var taskSessionEntity = sessions.Find(x => x.TaskSessionHash == taskSession.TaskSessionHash).SingleOrDefault();
                    if (taskSessionEntity == null)
                    {
                        taskSessionEntity = new TaskSession
                        {
                            Description = taskSession.Description,
                            TaskSessionHash = taskSession.TaskSessionHash,
                            CreatedOn = DateTimeOffset.UtcNow
                        };

                        var id = sessions.Insert(taskSessionEntity);
                        taskSessionEntity.TaskSessionId = id;
                    }

                    taskExecution.TaskSessionId = taskSessionEntity.TaskSessionId;

                    var executions = db.GetCollection<TaskExecution>(nameof(TaskSession));
                    executions.Insert(taskExecution);
                }
            }
        }

        private string GetTaskDbFilename(OrcusTask orcusTask) =>
            _fileSystem.Path.Combine(_options.SessionsDirectory, orcusTask.Id.ToString("N"));
    }
}