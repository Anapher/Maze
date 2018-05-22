using System;
using System.Threading.Tasks;
using Anapher.Wpf.Swan;
using Orcus.Administration.Core.Clients;
using Orcus.Administration.Core.Rest.Modules.V1;

namespace Orcus.Administration.ViewModels.Main
{
    public interface IOrcusConnection
    {
        IOrcusRestClient Client { get; }
    }

    public class OverviewViewModel : PropertyChangedBase, IMainViewModel, IOrcusConnection
    {
        public OverviewViewModel(IOrcusRestClient client)
        {
            Client = client;
        }

        public event EventHandler<IMainViewModel> ShowView;

        public async Task LoadData(Action<string> updateStatus)
        {
            updateStatus("Fetch modules...");
            var modulesTask = ModulesResource.FetchModules(Client);

        }

        public void LoadViewModel()
        {
        }

        public void UnloadViewModel()
        {
        }

        public IOrcusRestClient Client { get; }
    }
}