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
using Orcus.Server.Connection;
using Orcus.Server.Connection.Utilities;
using Tasks.Infrastructure.Core;
#if NETCOREAPP2_1
using Tasks.Infrastructure.Server.Options;

#else
using Tasks.Infrastructure.Client.Options;

#endif

namespace Tasks.Infrastructure.Management
{
    public interface ITaskDirectory
    {
        Task<IEnumerable<OrcusTask>> LoadTasks();
        Task<string> WriteTask(OrcusTask orcusTask);
        Task RemoveTask(OrcusTask orcusTask);
        Hash ComputeTaskHash(OrcusTask orcusTask);
    }

    public class TaskDirectory : ITaskDirectory
    {
        private readonly ITaskComponentResolver _taskComponentResolver;
        private readonly IXmlSerializerCache _xmlSerializerCache;
        private readonly ILogger<TaskDirectory> _logger;
        private readonly TasksOptions _options;
        private readonly AsyncReaderWriterLock _tasksLock;
        private IImmutableDictionary<Guid, string> _taskFileNames;
        private readonly XmlWriterSettings _xmlWriterSettings;

        public TaskDirectory(IOptions<TasksOptions> options, ITaskComponentResolver taskComponentResolver, IXmlSerializerCache xmlSerializerCache,
            ILogger<TaskDirectory> logger)
        {
            _taskComponentResolver = taskComponentResolver;
            _xmlSerializerCache = xmlSerializerCache;
            _logger = logger;
            _options = options.Value;
            _tasksLock = new AsyncReaderWriterLock();
            _xmlWriterSettings = new XmlWriterSettings {OmitXmlDeclaration = false, Indent = true};
        }

        public async Task<IEnumerable<OrcusTask>> LoadTasks()
        {
            using (await _tasksLock.ReaderLockAsync())
            {
                return LoadTasksLockAquired();
            }
        }

        private IEnumerable<OrcusTask> LoadTasksLockAquired()
        {
            var directoryInfo = new DirectoryInfo(_options.Directory);
            if (!directoryInfo.Exists)
            {
                _taskFileNames = ImmutableDictionary<Guid, string>.Empty;
                return Enumerable.Empty<OrcusTask>();
            }

            var filenames = new Dictionary<Guid, string>();
            var list = new List<OrcusTask>();
            foreach (var fileInfo in directoryInfo.GetFiles($"*.{_options.FileExtension}", SearchOption.AllDirectories))
            {
                var reader = new OrcusTaskReader(fileInfo.FullName, _taskComponentResolver, _xmlSerializerCache);
                OrcusTask task;

                try
                {
                    task = reader.ReadTask();
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "The file {filename} could not be read.", fileInfo.FullName);
                    continue;
                }

                if (filenames.ContainsKey(task.Id))
                {
                    _logger.LogWarning("The task of the file {file1} has the same id like the task of {file2}. {file1} is skipped.",
                        fileInfo.FullName, filenames[task.Id]);
                    continue;
                }

                list.Add(task);
                filenames.Add(task.Id, fileInfo.FullName);
            }
            _taskFileNames = filenames.ToImmutableDictionary();

            return list;
        }

        public async Task<string> WriteTask(OrcusTask orcusTask)
        {
            using (await _tasksLock.WriterLockAsync())
            {
                if (_taskFileNames == null)
                    LoadTasksLockAquired();

                if (!_taskFileNames.TryGetValue(orcusTask.Id, out var filename))
                {
                    //new task
                    var name = NameGeneratorUtilities.ToFilename(orcusTask.Name, includeSpace: false) + "." + _options.FileExtension;
                    filename = NameGeneratorUtilities.MakeUnique(name, "_[N]", s => File.Exists(Path.Combine(_options.Directory, s)));

                    _taskFileNames = _taskFileNames.SetItem(orcusTask.Id, filename);
                }

                using (var fileStream = File.Create(filename))
                using (var xmlWriter = XmlWriter.Create(fileStream, _xmlWriterSettings))
                {
                    var writer = new OrcusTaskWriter(xmlWriter, _taskComponentResolver, _xmlSerializerCache);
                    writer.Write(orcusTask, TaskDetails.Server);
                }

                return filename;
            }
        }

        public async Task RemoveTask(OrcusTask orcusTask)
        {
            using (await _tasksLock.WriterLockAsync())
            {
                if (_taskFileNames == null)
                    LoadTasksLockAquired();

                if (!_taskFileNames.TryGetValue(orcusTask.Id, out var filename))
                    throw new ArgumentException("The task was not found");

                _taskFileNames = _taskFileNames.Remove(orcusTask.Id);
                File.Delete(filename);
            }
        }

        public Hash ComputeTaskHash(OrcusTask orcusTask)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(memoryStream, _xmlWriterSettings))
                {
                    var writer = new OrcusTaskWriter(xmlWriter, _taskComponentResolver, _xmlSerializerCache);
                    writer.Write(orcusTask, TaskDetails.Server);
                }

                using (var sha256 = SHA256.Create())
                    return new Hash(sha256.ComputeHash(memoryStream));
            }
        }
    }
}