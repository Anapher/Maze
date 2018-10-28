using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autofac;
using Tasks.Infrastructure.Administration.Library.Command;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public class CreateCommandViewModel
    {
        public CreateCommandViewModel(IComponentContext container)
        {
            Children = new ObservableCollection<CommandViewModel>();

            Commands = container.Resolve<IEnumerable<ICommandDescription>>().ToList();

        }

        public ObservableCollection<CommandViewModel> Children { get; }
        public List<ICommandDescription> Commands { get; }
    }

    public class CommandViewModel
    {
        public object View { get; set; }
        public ICommandDescription Description { get; set; }
        public object ViewModel { get; set; }
    }
}