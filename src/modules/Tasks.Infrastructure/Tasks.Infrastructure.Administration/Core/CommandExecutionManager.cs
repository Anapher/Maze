using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Autofac;
using Maze.Administration.Library.Clients;
using Maze.Server.Connection.Utilities;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.Library.Command;
using Tasks.Infrastructure.Administration.Rest.V1;
using Tasks.Infrastructure.Administration.ViewModels;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.Core
{
    public class CommandExecutionManager
    {
        private readonly IRestClient _restClient;
        private readonly ITaskComponentResolver _taskComponentResolver;
        private readonly IXmlSerializerCache _xmlSerializerCache;
        private readonly IComponentContext _componentContext;

        public CommandExecutionManager(IRestClient restClient, ITaskComponentResolver taskComponentResolver, IXmlSerializerCache xmlSerializerCache,
            IComponentContext componentContext)
        {
            _restClient = restClient;
            _taskComponentResolver = taskComponentResolver;
            _xmlSerializerCache = xmlSerializerCache;
            _componentContext = componentContext;
            PendingCommands = new ObservableCollection<PendingCommandViewModel>();
        }

        public ObservableCollection<PendingCommandViewModel> PendingCommands { get; }

        public TaskActivityWatcher Execute(MazeTask mazeTask, ICommandDescription commandDescription)
        {
            var task = TasksResource.Execute(mazeTask, _taskComponentResolver, _xmlSerializerCache, _restClient);

            var watcher = _componentContext.Resolve<TaskActivityWatcher>();
            watcher.InitializeWatch(mazeTask.Id);

            PendingCommands.Insert(0, new PendingCommandViewModel(commandDescription, task, mazeTask, watcher));
            return watcher;
        }
    }

    public class PendingCommandViewModel : BindableBase
    {
        public PendingCommandViewModel(ICommandDescription commandDescription, Task<TaskSessionsInfo> task, MazeTask mazeTask, TaskActivityWatcher taskActivityWatcher)
        {
            CommandDescription = commandDescription;
            Task = task;
            MazeTask = mazeTask;
            TaskActivityWatcher = taskActivityWatcher;

            Task.ContinueWith(x =>
            {
                RaisePropertyChanged(nameof(IsCompleted));
                TaskActivityWatcher.Dispose();
            });
        }

        public ICommandDescription CommandDescription { get; }
        public Task<TaskSessionsInfo> Task { get; }
        public MazeTask MazeTask { get; }

        public TaskActivityWatcher TaskActivityWatcher { get; }

        public bool IsCompleted => Task.IsCompleted;
    }
}
