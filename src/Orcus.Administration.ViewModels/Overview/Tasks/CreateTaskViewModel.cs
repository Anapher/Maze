using System.Collections.Generic;
using Autofac;
using Orcus.Administration.Library.Tasks;
using Prism.Mvvm;

namespace Orcus.Administration.ViewModels.Overview.Tasks
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