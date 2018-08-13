using Anapher.Wpf.Swan;
using Orcus.Administration.ViewModels.Main;
using Unclassified.TxLib;

namespace Orcus.Administration.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        private IMainViewModel _viewModel;

        public MainViewModel()
        {
            Tx.LoadFromEmbeddedResource("Orcus.Administration.ViewModels.Resources.translation.txd");
            ViewModel = new LoginViewModel();
        }

        public IMainViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                if (value != _viewModel)
                {
                    if (_viewModel != null)
                    {
                        _viewModel.ShowView -= ViewModelOnShowView;
                        _viewModel.UnloadViewModel();
                    }

                    if (value != null)
                    {
                        value.ShowView += ViewModelOnShowView;
                        value.LoadViewModel();
                    }

                    _viewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        private void ViewModelOnShowView(object sender, IMainViewModel mainViewModel)
        {
            ViewModel = mainViewModel;
        }
    }
}