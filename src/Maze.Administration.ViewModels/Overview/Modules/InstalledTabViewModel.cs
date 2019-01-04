using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace Maze.Administration.ViewModels.Overview.Modules
{
    public class InstalledTabViewModel : BindableBase, IModuleTabViewModel
    {
        private ICollectionView _modules;
        private string _searchText;

        public ICollectionView Modules
        {
            get => _modules;
            set => SetProperty(ref _modules, value);
        }

        public ICommand RefreshCommand { get; } = new DelegateCommand(() => { });

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    Modules.Refresh();
                }
            }
        }

        public bool IncludePrerelease { get; set; }

        public void Initialize(IModuleService service)
        {
            Modules = new ListCollectionView(service.InstalledModules) {Filter = Filter};
        }

        private bool Filter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            var moduleViewModel = (ModuleViewModel) obj;
            return moduleViewModel.Title?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) > -1;
        }
    }
}