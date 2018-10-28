using System.Collections.Generic;
using Autofac;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.ViewModels.CreateTask;

namespace Tasks.Infrastructure.Administration.ViewModels
{
    public class CreateTaskViewModel : BindableBase
    {
        public CreateTaskViewModel(IComponentContext container)
        {
            TreeViewModels = new List<object>
            {
                new CreateCommandViewModel(container)
            };

        }

        public List<object> TreeViewModels { get; }
    }
}