using Anapher.Wpf.Toolkit.Extensions;
using Anapher.Wpf.Toolkit.Windows;
using Prism.Commands;
using Prism.Mvvm;

namespace Maze.Administration.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IWindowService _windowService;
        private DelegateCommand _deployCommand;

        public MainViewModel(IWindowService windowService)
        {
            _windowService = windowService;
        }

        public DelegateCommand DeployCommand
        {
            get { return _deployCommand ?? (_deployCommand = new DelegateCommand(() => { _windowService.ShowDialog<DeployClientViewModel>(); })); }
        }
    }
}