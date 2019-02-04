using Anapher.Wpf.Toolkit.Extensions;
using Anapher.Wpf.Toolkit.Windows;
using Prism.Commands;

namespace Maze.Administration.ViewModels.Overview
{
    public class OverviewViewModel
    {
        private readonly IWindowService _windowService;
        private DelegateCommand _deployCommand;

        public OverviewViewModel(IWindowService windowService)
        {
            _windowService = windowService;
        }

        public DelegateCommand DeployCommand
        {
            get { return _deployCommand ?? (_deployCommand = new DelegateCommand(() => { _windowService.ShowDialog<DeployClientViewModel>(); })); }
        }
    }
}