using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Anapher.Wpf.Swan.Extensions;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.StatusBar;
using Maze.Administration.Library.ViewModels;
using Maze.Administration.Library.Views;
using Maze.Utilities;
using Prism.Commands;
using Prism.Regions;
using RegistryEditor.Administration.Model;
using RegistryEditor.Administration.Rest;
using RegistryEditor.Shared.Dtos;
using TreeViewEx.Extensions;
using Unclassified.TxLib;

namespace RegistryEditor.Administration.ViewModels
{
    public class RegistryEditorViewModel : ViewModelBase
    {
        private readonly ITargetedRestClient _restClient;
        private readonly IShellStatusBar _statusBar;
        private readonly IWindowService _windowService;
        private DelegateCommand<RegistryKeyViewModel> _createNewSubKeyCommand;
        private DelegateCommand<RegistryValueType?> _createRegistryValueCommand;
        private string _currentPath;
        private DelegateCommand<RegistryValueViewModel> _editRegistryValueCommand;
        private DelegateCommand<string> _navigateToPathCommand;
        private CancellationTokenSource _openCancellationTokenSource;
        private ObservableCollection<RegistryValueViewModel> _registryValues;
        private DelegateCommand<RegistryKeyViewModel> _removeRegistryKeyCommand;
        private DelegateCommand<RegistryValueViewModel> _removeRegistryValueCommand;
        private RegistryTreeViewModel _treeViewModel;

        public RegistryEditorViewModel(IWindowService windowService, IShellStatusBar statusBar, ITargetedRestClient restClient)
        {
            _windowService = windowService;
            _statusBar = statusBar;
            _restClient = restClient;
        }

        public RegistryTreeViewModel TreeViewModel
        {
            get => _treeViewModel;
            private set => SetProperty(ref _treeViewModel, value);
        }

        public string CurrentPath
        {
            get => _currentPath;
            set => SetProperty(ref _currentPath, value);
        }

        public ObservableCollection<RegistryValueViewModel> RegistryValues
        {
            get => _registryValues;
            set => SetProperty(ref _registryValues, value);
        }

        public DelegateCommand<RegistryKeyViewModel> CreateNewSubKeyCommand
        {
            get
            {
                return _createNewSubKeyCommand ?? (_createNewSubKeyCommand = new DelegateCommand<RegistryKeyViewModel>(async parameter =>
                {
                    if (_windowService.ShowDialog<CreateSubKeyViewModel>(vm => vm.Initialize(parameter.RegistryKey.Path), out var viewModel) == true)
                    {
                        var path = parameter.RegistryKey.Path + "\\" + viewModel.Name;

                        if (await RegistryEditorResource.CreateRegistryKey(path, _restClient)
                            .DisplayOnStatusBarCatchErrors(_statusBar, Tx.T("RegistryEditor:StatusBar.CreateSubKey")))
                        {
                            parameter.Entries.IsExpanded = true;

                            if (parameter.Entries.IsLoaded)
                                parameter.RefreshCommand.Execute();
                        }
                    }
                }));
            }
        }

        public DelegateCommand<RegistryKeyViewModel> RemoveRegistryKeyCommand
        {
            get
            {
                return _removeRegistryKeyCommand ?? (_removeRegistryKeyCommand = new DelegateCommand<RegistryKeyViewModel>(async parameter =>
                {
                    if (_windowService.ShowMessage(Tx.T("RegistryEditor:MessageBox.SureDeleteSubKey", "name", parameter.RegistryKey.Name), Tx.T("Warning"),
                            MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) != MessageBoxResult.OK)
                        return;

                    if (await RegistryEditorResource.DeleteRegistryKey(parameter.RegistryKey.Path, _restClient)
                        .DisplayOnStatusBarCatchErrors(_statusBar,
                            Tx.T("RegistryEditor:StatusBar.DeleteRegistryKey", "name", parameter.RegistryKey.Name)))
                        parameter.Parent?.Entries.All.Remove(parameter);
                }));
            }
        }

        public DelegateCommand<RegistryValueType?> CreateRegistryValueCommand
        {
            get
            {
                return _createRegistryValueCommand ?? (_createRegistryValueCommand = new DelegateCommand<RegistryValueType?>(async parameter =>
                {
                    if (_windowService.ShowDialog<CreateEditValueViewModel>(vm => vm.InitializeCreation(parameter.Value), out var viewModel) == true)
                    {
                        var root = TreeViewModel.Selection.AsRoot();

                        if (await RegistryEditorResource.SetRegistryValue(root.SelectedViewModel.RegistryKey.Path, viewModel.Value, _restClient)
                            .DisplayOnStatusBarCatchErrors(_statusBar, Tx.T("RegistryEditor:StatusBar.CreateValue")))
                            await OpenPath(_currentPath);
                    }
                }));
            }
        }

        public DelegateCommand<RegistryValueViewModel> EditRegistryValueCommand
        {
            get
            {
                return _editRegistryValueCommand ?? (_editRegistryValueCommand = new DelegateCommand<RegistryValueViewModel>(async parameter =>
                {
                    if (_windowService.ShowDialog<CreateEditValueViewModel>(vm => vm.InitializeEdit(parameter.Dto), out var viewModel) == true)
                    {
                        var root = TreeViewModel.Selection.AsRoot();

                        if (await RegistryEditorResource.SetRegistryValue(root.SelectedViewModel.RegistryKey.Path, viewModel.Value, _restClient)
                            .DisplayOnStatusBarCatchErrors(_statusBar, Tx.T("RegistryEditor:StatusBar.EditValue")))
                            await OpenPath(_currentPath);
                    }
                }));
            }
        }

        public DelegateCommand<RegistryValueViewModel> RemoveRegistryValueCommand
        {
            get
            {
                return _removeRegistryValueCommand ?? (_removeRegistryValueCommand = new DelegateCommand<RegistryValueViewModel>(async parameter =>
                {
                    if (_windowService.ShowMessage(Tx.T("RegistryEditor:MessageBox.SureRemoveRegistryValue", "name", parameter.Dto.Name), Tx.T("Warning"),
                            MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) != MessageBoxResult.OK)
                        return;

                    var root = TreeViewModel.Selection.AsRoot();

                    if (await RegistryEditorResource.DeleteRegistryValue(root.SelectedViewModel.RegistryKey.Path, parameter.Dto.Name, _restClient)
                        .DisplayOnStatusBarCatchErrors(_statusBar, Tx.T("RegistryEditor:StatusBar.DeleteRegistryValue", "name", parameter.Dto.Name)))
                    {
                        RegistryValues.Remove(parameter);
                        await OpenPath(_currentPath);
                    }
                }));
            }
        }

        public DelegateCommand<string> NavigateToPathCommand
        {
            get
            {
                return _navigateToPathCommand ?? (_navigateToPathCommand = new DelegateCommand<string>(parameter =>
                {
                    TreeViewModel.SelectAsync(new IntegratedRegistryKey {Path = parameter}).Forget();
                }));
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            TreeViewModel = new RegistryTreeViewModel(_restClient, _statusBar);
            TreeViewModel.Selection.AsRoot().SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            var root = TreeViewModel.Selection.AsRoot();
            var currentItem = root.SelectedViewModel;
            currentItem.BringIntoView();

            if (currentItem.Parent != null)
                currentItem.Parent.Entries.IsExpanded = true;

            OpenPath(currentItem.RegistryKey.Path).Forget();
        }

        private async Task OpenPath(string path)
        {
            _openCancellationTokenSource?.Cancel();
            _openCancellationTokenSource = new CancellationTokenSource();

            var token = _openCancellationTokenSource.Token;

            var successOrError = await RegistryEditorResource.GetValues(path, _restClient)
                .DisplayOnStatusBarCatchErrors(_statusBar, Tx.T("RegistryEditor:StatusBar.LoadValues"));
            if (successOrError.Failed)
                return;

            if (token.IsCancellationRequested)
                return;

            RegistryValues = new ObservableCollection<RegistryValueViewModel>(successOrError.Result.Select(x => new RegistryValueViewModel(x)));
            CurrentPath = path;
        }
    }
}