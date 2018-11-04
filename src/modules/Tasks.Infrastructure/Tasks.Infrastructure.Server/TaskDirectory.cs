using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Utilities;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Server.Options;

namespace Tasks.Infrastructure.Server
{
    public interface ITaskDirectory
    {
        Task<IEnumerable<OrcusTask>> GetTasks();
        Task<string> WriteTask(OrcusTask orcusTask);
        Hash ComputeTaskHash(OrcusTask orcusTask);
    }

    public class TaskDirectory : ITaskDirectory
    {
        private readonly ITaskComponentResolver _taskComponentResolver;
        private readonly IXmlSerializerCache _xmlSerializerCache;
        private readonly TasksOptions _options;
        private readonly AsyncReaderWriterLock _tasksLock;
        private IImmutableDictionary<Guid, string> _taskFileNames;
        private readonly XmlWriterSettings _xmlWriterSettings;

        public TaskDirectory(IOptions<TasksOptions> options, ITaskComponentResolver taskComponentResolver, IXmlSerializerCache xmlSerializerCache)
        {
            _taskComponentResolver = taskComponentResolver;
            _xmlSerializerCache = xmlSerializerCache;
            _options = options.Value;
            _tasksLock = new AsyncReaderWriterLock();
            _xmlWriterSettings = new XmlWriterSettings {OmitXmlDeclaration = false, Indent = true};
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

        public async Task<IEnumerable<OrcusTask>> GetTasks()
        {
            using (await _tasksLock.ReaderLockAsync())
            {
                return GetTasksLockAcquired();
            }
        }

        private IEnumerable<OrcusTask> GetTasksLockAcquired()
        {
            var directoryInfo = new DirectoryInfo(_options.Directory);
            if (!directoryInfo.Exists)
            {
                _taskFileNames = ImmutableDictionary<Guid, string>.Empty;
                return Enumerable.Empty<OrcusTask>();
            }

            var list = new List<OrcusTask>();
            var filenames = new Dictionary<Guid, string>();
            foreach (var fileInfo in directoryInfo.GetFiles($"*.{_options.FileExtension}", SearchOption.AllDirectories))
            {
                var reader = new OrcusTaskReader(fileInfo.FullName, _taskComponentResolver, _xmlSerializerCache);
                var task = reader.ReadTask();

                list.Add(task);
                filenames.Add(task.Id, fileInfo.Name);
            }

            _taskFileNames = filenames.ToImmutableDictionary();

            return list;
        }

        public async Task<string> WriteTask(OrcusTask orcusTask)
        {
            using (await _tasksLock.WriterLockAsync())
            {
                if (_taskFileNames == null)
                    GetTasksLockAcquired();

                if (!_taskFileNames.TryGetValue(orcusTask.Id, out string filename))
                {
                    //new task
                    var name = NameGeneratorUtilities.ToFilename(orcusTask.Name, includeSpace: false) + "." + _options.FileExtension;
                    filename = NameGeneratorUtilities.MakeUnique(name, "_[N]", s => File.Exists(Path.Combine(_options.Directory, s)));

                    _taskFileNames = _taskFileNames.SetItem(orcusTask.Id, filename);
                }
                
                Directory.CreateDirectory(_options.Directory);
                using (var fileStream = File.Create(Path.Combine(_options.Directory, filename)))
                using (var xmlWriter = XmlWriter.Create(fileStream, _xmlWriterSettings))
                {
                    var writer = new OrcusTaskWriter(xmlWriter, _taskComponentResolver, _xmlSerializerCache);
                    writer.Write(orcusTask, TaskDetails.Server);
                }

                return filename;
            }
        }
    }
}