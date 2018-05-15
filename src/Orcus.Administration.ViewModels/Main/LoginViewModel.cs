using System;
using Anapher.Wpf.Swan;

namespace Orcus.Administration.ViewModels.Main
{
    public class LoginViewModel : PropertyChangedBase, IMainViewModel
    {
        private bool _isLoggedIn;
        private bool _isLoggingIn;

        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            set => SetProperty(value, ref _isLoggingIn);
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetProperty(value, ref _isLoggedIn);
        }

        public event EventHandler<IMainViewModel> ShowView;

        public void LoadViewModel()
        {
        }

        public void UnloadViewModel()
        {
        }
    }
}