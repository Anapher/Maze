using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Services;
using Maze.Administration.Library.Utilities;
using Maze.Administration.Library.ViewModels;
using Maze.Server.Connection.Commanding;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.ViewModels.CreateTask.Base;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Audience;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public class AudienceViewModel : BindableBase, ITaskConfiguringViewModel
    {
        private readonly AudienceStatusChildViewModel _audienceStatus;
        private readonly IClientManager _clientManager;
        private bool _allClients;
        private bool _includeServer;

        public AudienceViewModel(IClientManager clientManager)
        {
            _clientManager = clientManager;
            _audienceStatus = new AudienceStatusChildViewModel {NodeViewModel = this};
            Childs = new List<AudienceStatusChildViewModel> {_audienceStatus};

            Clients = new ViewModelObservableCollection<CheckableModel<ClientViewModel>, ClientViewModel>(clientManager.ClientViewModels,
                CreateCheckableClient);
            Clients.CollectionChanged += TargetsOnCollectionChanged;

            Groups = new ViewModelObservableCollection<CheckableModel<ClientGroupViewModel>, ClientGroupViewModel>(clientManager.GroupViewModels, CreateGroupViewModel);
            Groups.CollectionChanged += TargetsOnCollectionChanged;
        }

        public List<AudienceStatusChildViewModel> Childs { get; }
        public ViewModelObservableCollection<CheckableModel<ClientViewModel>, ClientViewModel> Clients { get; }
        public ViewModelObservableCollection<CheckableModel<ClientGroupViewModel>, ClientGroupViewModel> Groups { get; }

        public bool IncludeServer
        {
            get => _includeServer;
            set => SetProperty(ref _includeServer, value);
        }

        public bool AllClients
        {
            get => _allClients;
            set => SetProperty(ref _allClients, value);
        }

        public bool IsSelected { get; set; }
        public object NodeViewModel => this;

        public void Initialize(MazeTask mazeTask)
        {
            AllClients = mazeTask.Audience.IsAll;
            IncludeServer = mazeTask.Audience.IncludesServer;

            if (!mazeTask.Audience.IsAll)
            {
                foreach (var commandTarget in mazeTask.Audience)
                {
                    for (int i = commandTarget.From; i < commandTarget.To + 1; i++)
                    {
                        if (commandTarget.Type == CommandTargetType.Client)
                        {
                            var client = Clients.FirstOrDefault(x => x.Model.ClientId == i);
                            if (client != null)
                                client.IsChecked = true;
                        }
                        else
                        {
                            var group = Groups.FirstOrDefault(x => x.Model.ClientGroupId == i);
                            if (group != null)
                                group.IsChecked = true;
                        }
                    }
                }
            }
        }

        public void InitializeClients(IEnumerable<int> clientIds)
        {
            foreach (var clientId in clientIds)
            {
                var client = Clients.FirstOrDefault(x => x.Model.ClientId == clientId);
                if (client != null)
                    client.IsChecked = true;
            }
        }

        public IEnumerable<ValidationResult> ValidateInput() => Enumerable.Empty<ValidationResult>();

        public IEnumerable<ValidationResult> ValidateContext(MazeTask mazeTask) => Enumerable.Empty<ValidationResult>();

        public void Apply(MazeTask mazeTask)
        {
            mazeTask.Audience = new AudienceCollection {IsAll = AllClients, IncludesServer = IncludeServer};
            if (!AllClients)
            {
                mazeTask.Audience.AddRange(Clients.Where(x => x.IsChecked)
                    .Select(x => new CommandTarget(CommandTargetType.Client, x.Model.ClientId)));
                mazeTask.Audience.AddRange(Groups.Where(x => x.IsChecked)
                    .Select(x => new CommandTarget(CommandTargetType.Group, x.Model.ClientGroupId)));
            }
        }

        public void UpdateEstimatedClients()
        {
            if (AllClients)
                _audienceStatus.EstimatedClients = Clients.Count;
            else
            {
                var clients = new HashSet<int>();
                clients.UnionWith(Clients.Where(x => x.IsChecked).Select(x => x.Model.ClientId));
                clients.UnionWith(Groups.Where(x => x.IsChecked).SelectMany(x => x.Model.Clients.Select(y => y.ClientId)));

                _audienceStatus.EstimatedClients = clients.Count;
            }
        }

        private void TargetsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateEstimatedClients();
        }

        private CheckableModel<ClientViewModel> CreateCheckableClient(ClientViewModel arg)
        {
            var viewModel = new CheckableModel<ClientViewModel>(arg);
            //we don't care about unsubscribing
            viewModel.PropertyChanged += CheckableTargetOnPropertyChanged;
            return viewModel;
        }

        private CheckableModel<ClientGroupViewModel> CreateGroupViewModel(ClientGroupViewModel arg)
        {
            var viewModel = new CheckableModel<ClientGroupViewModel>(arg);
            //we don't care about unsubscribing
            viewModel.PropertyChanged += CheckableTargetOnPropertyChanged;
            return viewModel;
        }

        private void CheckableTargetOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CheckableModel<ClientViewModel>.IsChecked))
            {
                UpdateEstimatedClients();
            }
        }
    }
}