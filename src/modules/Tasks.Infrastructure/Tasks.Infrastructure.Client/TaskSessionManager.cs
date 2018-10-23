using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using Orcus.Client.Library.Services;
using Tasks.Infrastructure.Client.Data;
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
        Task CreateExecution(OrcusTask orcusTask, TaskSession taskSession, TaskExecution taskExecution, Stream resultStream);
        Task Synchronize(IOrcusRestClient orcusRestClient);
    }

    public class TaskSessionManager : ITaskSessionManager
    {
        private readonly IFileSystem _fileSystem;
        private readonly ITaskExecutionTransmitter _taskExecutionTransmitter;
        private readonly TasksOptions _options;
        private readonly AsyncReaderWriterLock _readerWriterLock;
        private readonly ConcurrentStack<TaskExecutionReference> _outstandingExecutions;
        private bool _isOutstandingExecutionsInitialized;

        public TaskSessionManager(IFileSystem fileSystem, ITaskExecutionTransmitter taskExecutionTransmitter, IOptions<TasksOptions> options)
        {
            _fileSystem = fileSystem;
            _taskExecutionTransmitter = taskExecutionTransmitter;
            _options = options.Value;
            _readerWriterLock = new AsyncReaderWriterLock();

            var mapper = BsonMapper.Global;
            mapper.Entity<TaskSession>().Id(x => x.TaskSessionId).DbRef(x => x.Executions, nameof(TaskExecution));
            mapper.Entity<TaskExecution>().Id(x => x.TaskExecutionId);
            mapper.Entity<TaskExecutionTransmission>().Id(x => x.TaskExecutionId, false);
            
            _outstandingExecutions = new ConcurrentStack<TaskExecutionReference>();
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

        public async Task CreateExecution(OrcusTask orcusTask, TaskSession taskSession, TaskExecution taskExecution, Stream resultStream)
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
                    var executionId = executions.Insert(taskExecution);

                    if (resultStream != null)
                    {
                        db.FileStorage.Upload(executionId.AsString, null, resultStream);
                    }

                    var transmissions = db.GetCollection<TaskExecutionTransmission>(nameof(TaskExecutionTransmission));
                    transmissions.Insert(new TaskExecutionTransmission {TaskExecutionId = executionId});

                    _taskExecutionTransmitter.EnqueueExecution(new TaskExecutionReference(file.FullName, executionId, taskExecution));
                }
            }
        }

        private async Task InitializeOutstandingExecutions()
        {
            if (!_isOutstandingExecutionsInitialized)
            {
                _isOutstandingExecutionsInitialized = true;

                using (await _readerWriterLock.ReaderLockAsync())
                {
                    var result = new List<TaskExecutionReference>();

                    foreach (var filename in _fileSystem.Directory.EnumerateFiles(_options.SessionsDirectory))
                    {
                        var name = _fileSystem.Path.GetFileName(filename);
                        if (!Guid.TryParseExact(name, "N", out _))
                            continue;

                        using (var dbStream = _fileSystem.File.OpenRead(filename))
                        using (var db = new LiteDatabase(dbStream))
                        {
                            var transmissions = db.GetCollection<TaskExecutionTransmission>(nameof(TaskExecutionTransmission));
                            result.AddRange(transmissions.FindAll().Select(x => new TaskExecutionReference(filename, x.TaskExecutionId, null)));
                        }
                    }

                    _outstandingExecutions.PushRange(result.ToArray(), 0, result.Count);
                }
            }
        }

        public async Task Synchronize(IOrcusRestClient orcusRestClient)
        {
            await InitializeOutstandingExecutions();

            while (_outstandingExecutions.TryPeek(out var taskExecutionReference))
            {
                using (await _readerWriterLock.ReaderLockAsync())
                {

                }
            }
        }

        private string GetTaskDbFilename(OrcusTask orcusTask) =>
            _fileSystem.Path.Combine(_options.SessionsDirectory, orcusTask.Id.ToString("N"));
    }
}