using System.Collections.Generic;
using Autofac;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.Library;

namespace Tasks.Infrastructure.Administration.ViewModels
{
    public class CreateTaskViewModel : BindableBase
    {
        public CreateTaskViewModel(IComponentContext container)
        {
            var taskCreators = container.Resolve<IEnumerable<ITaskDescriber>>();
            var transmissionServices = container.Resolve<IEnumerable<ITransmissionServiceDescriber>>();

        }
    }
}