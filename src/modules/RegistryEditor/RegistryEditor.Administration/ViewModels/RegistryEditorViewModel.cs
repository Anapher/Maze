using System;
using System.Windows;
using Anapher.Wpf.Swan.Extensions;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.ViewModels;
using Orcus.Administration.Library.Views;
using Prism.Commands;
using Prism.Regions;
using RegistryEditor.Administration.Rest;
using TreeViewEx.Extensions;
using Unclassified.TxLib;

namespace RegistryEditor.Administration.ViewModels
{
    public class RegistryEditorViewModel : ViewModelBase
    {
        private readonly IPackageRestClient _restClient;
        private readonly IShellStatusBar _statusBar;
        private readonly IDialogWindow _window;
        private readonly IWindowService _windowService;
        private string _currentPath;

        private DelegateCommand<RegistryKeyViewModel> _createNewSubKeyCommand;
        private DelegateCommand<RegistryKeyViewModel> _removeRegistryKeyCommand;
        private RegistryTreeViewModel _treeViewModel;

        public RegistryEditorViewModel(IDialogWindow window, IWindowService windowService, IShellStatusBar statusBar, ITargetedRestClient restClient)
        {
            _window = window;
            _windowService = windowService;
            _statusBar = statusBar;
            _restClient = restClient.CreatePackageSpecific("RegistryEditor");
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

        public DelegateCommand<RegistryKeyViewModel> CreateNewSubKeyCommand
        {
            get
            {
                return _createNewSubKeyCommand ?? (_createNewSubKeyCommand = new DelegateCommand<RegistryKeyViewModel>(async parameter =>
                {
                    var viewModel = new CreateSubKeyViewModel(parameter.RegistryKey.Path);
                    if (_windowService.ShowDialog(viewModel, null, _window) == true)
                    {
                        var path = parameter.RegistryKey.Path + "\\" + viewModel.Name;

                        if (await RegistryEditorResource.CreateRegistryKey(path, _restClient)
                            .DisplayOnStatusBarCatchErrors(_statusBar, Tx.T("RegistryEditor:StatusBar.CreateSubKey")))
                            if (parameter.Entries.IsLoaded)
                                parameter.RefreshCommand.Execute();
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
                    if (_window.ShowMessage(Tx.T("RegistryEditor:MessageBox.SureDeleteSubKey", "name", parameter.RegistryKey.Name), Tx.T("Warning"),
                            MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) != MessageBoxResult.OK)
                        return;

                    if (await RegistryEditorResource.DeleteRegistryKey(parameter.RegistryKey.Path, _restClient)
                        .DisplayOnStatusBarCatchErrors(_statusBar,
                            Tx.T("RegistryEditor:StatusBar.DeleteRegistryKey", "name", parameter.RegistryKey.Name)))
                        parameter.Parent?.Entries.All.Remove(parameter);
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

            CurrentPath = root.SelectedValue.Path;
            OpenPath(CurrentPath);
        }

        private void OpenPath(string path)
        {

        }
    }
}