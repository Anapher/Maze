using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Connection.Utilities;
using Orcus.Server.Service.Options;

namespace Orcus.Server.Service.Tasks
{
    public interface ITaskDirectory
    {
        Task<IEnumerable<OrcusTask>> GetTasks();
        Task WriteTask(OrcusTask orcusTask);
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

        public async Task<IEnumerable<OrcusTask>> GetTasks()
        {
            using (await _tasksLock.ReaderLockAsync())
            {
                return GetTasksLockAquired();
            }
        }

        private IEnumerable<OrcusTask> GetTasksLockAquired()
        {
            var directoryInfo = new DirectoryInfo(_options.Directory);
            if (!directoryInfo.Exists)
            {
                _taskFileNames = ImmutableDictionary<Guid, string>.Empty;
                return Enumerable.Empty<OrcusTask>();
            }

            var list = new List<OrcusTask>();
            foreach (var fileInfo in directoryInfo.GetFiles($"*.{_options.TaskFileExtension}", SearchOption.AllDirectories))
            {
                var reader = new OrcusTaskReader(fileInfo.FullName, _taskComponentResolver, _xmlSerializerCache);
                list.Add(reader.ReadTask());
            }

            return list;
        }

        public async Task WriteTask(OrcusTask orcusTask)
        {
            using (await _tasksLock.WriterLockAsync())
            {
                if (_taskFileNames == null)
                    GetTasksLockAquired();

                if (!_taskFileNames.TryGetValue(orcusTask.Id, out string filename))
                {
                    //new task
                    var name = NameGeneratorUtilities.ToFilename(orcusTask.Name, includeSpace: false) + "." + _options.TaskFileExtension;
                    filename = NameGeneratorUtilities.MakeUnique(name, "_[N]", s => File.Exists(Path.Combine(_options.Directory, s)));

                    _taskFileNames = _taskFileNames.SetItem(orcusTask.Id, filename);
                }

                using (var fileStream = File.Create(filename))
                using (var xmlWriter = XmlWriter.Create(fileStream, _xmlWriterSettings))
                {
                    var writer = new OrcusTaskWriter(xmlWriter, _taskComponentResolver, _xmlSerializerCache);
                    writer.Write(orcusTask, TaskDetails.Server);
                }
            }
        }
    }
}