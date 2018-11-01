using System.Collections.Generic;
using Autofac;
using Orcus.Administration.Library.Services;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.ViewModels.CreateTask;

namespace Tasks.Infrastructure.Administration.ViewModels
{
    public class CreateTaskViewModel : BindableBase
    {
        public CreateTaskViewModel(IWindowService windowService, IComponentContext container)
        {
            TreeViewModels = new List<ITreeViewItem>
            {
                new CommandsViewModel(windowService, container)
            };

        }

        public List<ITreeViewItem> TreeViewModels { get; }
    }
}