using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using Maze.Server.Connection;
using Maze.Server.Connection.Utilities;
using Tasks.Infrastructure.Core;
using TasksCacheDictionary = System.Collections.Immutable.ImmutableDictionary<System.Guid, (string, Tasks.Infrastructure.Core.MazeTask)>;

#if NETCOREAPP2_1
using Tasks.Infrastructure.Server.Options;

#else
using Tasks.Infrastructure.Client.Options;

#endif

namespace Tasks.Infrastructure.Management
{
    /// <summary>
    ///     Management for the directory that contains the tasks
    /// </summary>
    public interface ITaskDirectory
    {
        /// <summary>
        ///     Read all tasks from the directory. When the tasks were already loaded before, the cached task list is returned.
        /// </summary>
        /// <returns>Return all tasks from the directory.</returns>
        Task<IEnumerable<MazeTask>> LoadTasks();

        /// <summary>
        ///     Read all tasks from the directory.
        /// </summary>
        /// <returns>Return all tasks from the directory.</returns>
        Task<IEnumerable<MazeTask>> LoadTasksRefresh();

        /// <summary>
        ///     Write a task to the directory and replace an existing one if necessary.
        /// </summary>
        /// <param name="mazeTask">The task that should be saved.</param>
        /// <returns>Return the file name of the task</returns>
        Task<string> WriteTask(MazeTask mazeTask);

        /// <summary>
        ///     Get an task encoded (xml) by the task id
        /// </summary>
        /// <param name="taskId">The task id of the task.</param>
        /// <returns>Return the encoded task (utf8/xml).</returns>
        Task<byte[]> GetEncodedTask(Guid taskId);

        /// <summary>
        ///     Remove a task from the directory
        /// </summary>
        /// <param name="taskId">The id of the task that should be removed.</param>
        /// <returns>Return true if the task was successfully removed, false if the task was not found.</returns>
        Task<bool> RemoveTask(Guid taskId);

        /// <summary>
        ///     Compute the hash value of a task.
        /// </summary>
        /// <param name="mazeTask">The task which should be hashed.</param>
        /// <returns>Return the hash value of the task as SHA256 hash.</returns>
        Hash ComputeTaskHash(MazeTask mazeTask);
    }

    public class TaskDirectory : ITaskDirectory
    {
        private readonly ITaskComponentResolver _taskComponentResolver;
        private readonly IXmlSerializerCache _xmlSerializerCache;
        private readonly ILogger<TaskDirectory> _logger;
        private readonly TasksOptions _options;
        private readonly AsyncReaderWriterLock _tasksLock;
        private readonly XmlWriterSettings _xmlWriterSettings;
        private IImmutableDictionary<Guid, (string, MazeTask)> _cachedTasks;
        private readonly string _tasksDirectory;

        public TaskDirectory(IOptions<TasksOptions> options, ITaskComponentResolver taskComponentResolver, IXmlSerializerCache xmlSerializerCache,
            ILogger<TaskDirectory> logger)
        {
            _taskComponentResolver = taskComponentResolver;
            _xmlSerializerCache = xmlSerializerCache;
            _logger = logger;
            _options = options.Value;
            _tasksDirectory = Environment.ExpandEnvironmentVariables(_options.Directory);

            _tasksLock = new AsyncReaderWriterLock();
            _xmlWriterSettings = new XmlWriterSettings {OmitXmlDeclaration = false, Indent = true};
        }

        public async Task<IEnumerable<MazeTask>> LoadTasks()
        {
            if (_cachedTasks != null)
                return _cachedTasks.Values.Select(x => x.Item2);
            
            using (await _tasksLock.ReaderLockAsync())
            {
                if (_cachedTasks != null)
                    return _cachedTasks.Values.Select(x => x.Item2);

                return LoadTasksLockAquired();
            }
        }

        public async Task<IEnumerable<MazeTask>> LoadTasksRefresh()
        {
            using (await _tasksLock.ReaderLockAsync())
            {
                return LoadTasksLockAquired();
            }
        }

        private IEnumerable<MazeTask> LoadTasksLockAquired()
        {
            var directoryInfo = new DirectoryInfo(_tasksDirectory);
            if (!directoryInfo.Exists)
            {
                _cachedTasks = TasksCacheDictionary.Empty;
                return _cachedTasks.Values.Select(x => x.Item2);
            }

            var tasks = new Dictionary<Guid, (string, MazeTask)>();
            foreach (var fileInfo in directoryInfo.GetFiles($"*.{_options.FileExtension}", SearchOption.AllDirectories))
            {
                var reader = new MazeTaskReader(fileInfo.FullName, _taskComponentResolver, _xmlSerializerCache);
                MazeTask task;

                try
                {
                    task = reader.ReadTask();
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "The file {filename} could not be read.", fileInfo.FullName);
                    continue;
                }

                if (_cachedTasks?.ContainsKey(task.Id) == true)
                {
                    _logger.LogWarning("The task of the file {file1} has the same id like the task of {file2}. {file1} is skipped.",
                        fileInfo.FullName, _cachedTasks[task.Id].Item1);
                    continue;
                }

                tasks.Add(task.Id, (fileInfo.FullName, task));
            }
            
            _cachedTasks = tasks.ToImmutableDictionary();

            return tasks.Values.Select(x => x.Item2);
        }

        public async Task<string> WriteTask(MazeTask mazeTask)
        {
            using (await _tasksLock.WriterLockAsync())
            {
                if (_cachedTasks == null)
                    LoadTasksLockAquired();

                string filename;
                if (_cachedTasks.TryGetValue(mazeTask.Id, out var taskLink))
                    filename = taskLink.Item1;
                else
                {
                    //new task
                    var name = NameGeneratorUtilities.ToFilename(mazeTask.Name, includeSpace: false) + "." + _options.FileExtension;
                    filename = NameGeneratorUtilities.MakeUnique(name, "_[N]", s => File.Exists(Path.Combine(_tasksDirectory, s)));
                    filename = Path.Combine(_tasksDirectory, filename);
                }

                Directory.CreateDirectory(_tasksDirectory);

                using (var fileStream = File.Create(filename))
                using (var xmlWriter = XmlWriter.Create(fileStream, _xmlWriterSettings))
                {
                    var writer = new MazeTaskWriter(xmlWriter, _taskComponentResolver, _xmlSerializerCache);
                    writer.Write(mazeTask, TaskDetails.Server);
                }

                _cachedTasks = _cachedTasks.SetItem(mazeTask.Id, (filename, mazeTask));
                return filename;
            }
        }

        public async Task<byte[]> GetEncodedTask(Guid taskId)
        {
            using (await _tasksLock.ReaderLockAsync())
            {
                if (_cachedTasks == null)
                    LoadTasksLockAquired();

                if (!_cachedTasks.TryGetValue(taskId, out var taskLink))
                    throw new ArgumentException("The task was not found");

                return File.ReadAllBytes(taskLink.Item1);
            }
        }

        public async Task<bool> RemoveTask(Guid taskId)
        {
            using (await _tasksLock.WriterLockAsync())
            {
                if (_cachedTasks == null)
                    LoadTasksLockAquired();

                if (!_cachedTasks.TryGetValue(taskId, out var taskLink))
                    return false;

                File.Delete(taskLink.Item1);
                _cachedTasks = _cachedTasks.Remove(taskId);
                return true;
            }
        }

        public Hash ComputeTaskHash(MazeTask mazeTask)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(memoryStream, _xmlWriterSettings))
                {
                    var writer = new MazeTaskWriter(xmlWriter, _taskComponentResolver, _xmlSerializerCache);
                    writer.Write(mazeTask, TaskDetails.Server);
                }

                using (var sha256 = SHA256.Create())
                    return new Hash(sha256.ComputeHash(memoryStream));
            }
        }
    }
}