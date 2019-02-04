using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anapher.Wpf.Toolkit.Extensions;
using Anapher.Wpf.Toolkit.Windows;
using Maze.Administration.Library.Deployment;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Services;
using Maze.Administration.Library.ViewModels;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;

namespace Maze.Administration.ViewModels
{
    public class ClientDeployerViewExtensions
    {
        public static void Initialize(object view, IClientDeployer model)
        {
            var type = view.GetType();
            var method = type.GetMethod(nameof(IClientDeployerView<FooDeployer>.Initialize));
            method.Invoke(view, new object[] {model});
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class FooDeployer : IClientDeployer
        {
            public string Name { get; }
            public string Description { get; }

            public Task Deploy(IEnumerable<ClientGroupViewModel> groups, ILogger logger, CancellationToken cancellationToken) =>
                throw new NotImplementedException();
        }
    }

    public class DeployClientViewModel : BindableBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWindowService _windowService;
        private DelegateCommand _buildCommand;
        private IClientDeployer _selectedDeployer;
        private object _view;

        public DeployClientViewModel(IEnumerable<IClientDeployer> deployers, IClientManager clientManager, IWindowService windowService,
            IServiceProvider serviceProvider)
        {
            _windowService = windowService;
            _serviceProvider = serviceProvider;
            Deployers = deployers.ToList();
            SelectedDeployer = Deployers.FirstOrDefault();
            Groups = new CheckableMappedObservableCollection<ClientGroupViewModel>(clientManager.GroupViewModels);
        }

        public List<IClientDeployer> Deployers { get; }
        public CheckableMappedObservableCollection<ClientGroupViewModel> Groups { get; }

        public IClientDeployer SelectedDeployer
        {
            get => _selectedDeployer;
            set
            {
                if (SetProperty(ref _selectedDeployer, value))
                {
                    var viewType = typeof(IClientDeployerView<>).MakeGenericType(value.GetType());
                    var view = _serviceProvider.GetService(viewType);
                    if (view != null) ClientDeployerViewExtensions.Initialize(view, value);

                    View = view;
                }
            }
        }

        public object View
        {
            get => _view;
            set => SetProperty(ref _view, value);
        }

        public DelegateCommand BuildCommand
        {
            get
            {
                return _buildCommand ?? (_buildCommand = new DelegateCommand(() =>
                {
                    var groups = Groups.Where(x => x.IsChecked).Select(x => x.Model);
                    _windowService.ShowDialog<DeployClientBuildViewModel>(vm => vm.Build(SelectedDeployer, groups));
                }, () => SelectedDeployer != null)).ObservesProperty(() => SelectedDeployer);
            }
        }
    }
}