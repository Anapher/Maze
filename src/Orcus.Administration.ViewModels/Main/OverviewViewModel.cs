using System;
using System.Threading.Tasks;
using Anapher.Wpf.Swan;
using Orcus.Administration.Core.Clients;

namespace Orcus.Administration.ViewModels.Main
{
    public class OverviewViewModel : PropertyChangedBase, IMainViewModel
    {
        public OverviewViewModel(IOrcusRestClient client)
        {

        }

        public event EventHandler<IMainViewModel> ShowView;

        public async Task LoadData()
        {

        }

        public void LoadViewModel()
        {
        }

        public void UnloadViewModel()
        {
        }
    }
}