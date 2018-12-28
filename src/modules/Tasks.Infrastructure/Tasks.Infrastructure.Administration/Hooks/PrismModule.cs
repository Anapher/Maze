using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Maze.Administration.Library;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.Menu.MenuBase;
using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Views;
using Maze.Server.Connection.Commanding;
using Prism.Commands;
using Prism.Modularity;
using Prism.Regions;
using Tasks.Infrastructure.Administration.Core;
using Tasks.Infrastructure.Administration.Library.Command;
using Tasks.Infrastructure.Administration.Resources;
using Tasks.Infrastructure.Administration.ViewModels;
using Tasks.Infrastructure.Administration.Views;
using Tasks.Infrastructure.Core.Audience;
using Tasks.Infrastructure.Core.Dtos;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.Hooks
{
    public class PrismModule : IModule
    {
        public const string ModuleName = "Tasks.Infrastructure";

        private readonly IRegionManager _regionManager;
        private readonly ClientsContextMenu _clientsContextMenu;
        private readonly IEnumerable<ICommandDescription> _commandDescriptions;
        private readonly VisualStudioIcons _icons;
        private readonly IWindowService _windowService;
        private readonly CommandExecutionManager _commandExecutionManager;

        public PrismModule(IRegionManager regionManager, ClientsContextMenu clientsContextMenu, IEnumerable<ICommandDescription> commandDescriptions,
            VisualStudioIcons icons, IWindowService windowService, CommandExecutionManager commandExecutionManager)
        {
            _regionManager = regionManager;
            _clientsContextMenu = clientsContextMenu;
            _commandDescriptions = commandDescriptions;
            _icons = icons;
            _windowService = windowService;
            _commandExecutionManager = commandExecutionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(TasksView));
            Tx.LoadFromEmbeddedResource("Tasks.Infrastructure.Administration.Resources.Tasks.Infrastructure.Translation.txd");

            var tasksSection = new MenuSection<ItemCommand<ClientViewModel>>();

            var tasksSubMenu = new NavigationalEntry<ItemCommand<ClientViewModel>>
            {
                Header = Tx.T("TasksInfrastructure:CreateTask.Commands", 2),
                IsOrdered = true,
                Icon = _icons.SelectCommandColumn
            };
            InitializeCommands(tasksSubMenu);
            tasksSection.Add(tasksSubMenu);

            var createTaskCommand = new ItemCommand<ClientViewModel>
            {
                Header = Tx.T("TasksInfrastructure:CreateTask"),
                Icon = _icons.NewCatalog,
                Command = new DelegateCommand<ClientViewModel>(client => CreateTaskCommand(new List<ClientViewModel> {client})),
                MultipleCommand = new DelegateCommand<IList<ClientViewModel>>(CreateTaskCommand)
            };
            tasksSection.Add(createTaskCommand);

            _clientsContextMenu.Add(tasksSection);
        }

        private void InitializeCommands(NavigationalEntry<ItemCommand<ClientViewModel>> rootEntry)
        {
            var navigationalEntries = new Dictionary<string, NavigationalEntry<ItemCommand<ClientViewModel>>>();

            foreach (var commandDescription in _commandDescriptions)
            {
                var serviceNamespace = ServiceNamespace.Parse(commandDescription.Namespace);
                var menuEntry = rootEntry;

                foreach (var relativeSegment in serviceNamespace.GetRelativeSegments())
                {
                    var lastEntry = menuEntry;

                    if (navigationalEntries.TryGetValue(relativeSegment, out menuEntry))
                        continue;

                    menuEntry = new NavigationalEntry<ItemCommand<ClientViewModel>>(true)
                    {
                        Header = ServiceNamespace.TranslateSegment(relativeSegment)
                    };

                    navigationalEntries.Add(relativeSegment, menuEntry);
                    lastEntry.Add(menuEntry);
                }

                menuEntry.Add(new ItemCommand<ClientViewModel>
                {
                    Header = commandDescription.Name,
                    Icon = commandDescription.Icon,
                    Command = new DelegateCommand<ClientViewModel>(x => ExecuteCommand(new List<ClientViewModel> {x}, commandDescription)),
                    MultipleCommand = new DelegateCommand<IList<ClientViewModel>>(x => ExecuteCommand(x, commandDescription))
                });
            }
        }

        private void ExecuteCommand(IEnumerable<ClientViewModel> obj, ICommandDescription commandDescription)
        {
            var audienceCollection = new AudienceCollection();
            audienceCollection.AddRange(obj.Select(x => new CommandTarget(CommandTargetType.Client, x.ClientId)));

            if (_windowService.ShowDialog<ExecuteCommandViewModel>(vm => vm.Initialize(commandDescription, audienceCollection), out var viewModel) == true)
            {
                var watcher = _commandExecutionManager.Execute(viewModel.MazeTask, commandDescription);

                _windowService.ShowDialog<TaskOverviewViewModel>(vm =>
                    vm.Initialize(watcher, commandDescription.Name));
            }
        }

        private void CreateTaskCommand(IList<ClientViewModel> obj)
        {
            _windowService.ShowDialog<CreateTaskViewModel>();
        }
    }
}