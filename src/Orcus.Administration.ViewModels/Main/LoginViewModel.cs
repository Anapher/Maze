using System;
using System.Security;
using Anapher.Wpf.Swan;
using Orcus.Administration.Core;
using Orcus.Administration.Core.Clients;
using Orcus.Administration.Core.Exceptions;
using Orcus.Administration.ViewModels.Utilities;

namespace Orcus.Administration.ViewModels.Main
{
    public class LoginViewModel : PropertyChangedBase, IMainViewModel
    {
        private string _errorMessage;
        private bool _isLoggingIn;
        private AsyncRelayCommand<SecureString> _loginCommand;
        private string _username = "vince";

        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            set => SetProperty(value, ref _isLoggingIn);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(value, ref _username);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(value, ref _errorMessage);
        }

        public AsyncRelayCommand<SecureString> LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new AsyncRelayCommand<SecureString>(async parameter =>
                {
                    IsLoggingIn = true;

                    IOrcusRestClient client;
                    try
                    {
                        client = await OrcusRestConnector.TryConnect(Username, parameter,
                            new ServerInfo {ServerUri = new Uri("http://localhost:50485/")});
                    }
                    catch (RestAuthenticationException e)
                    {
                        ErrorMessage = e.GetRestExceptionMessage();
                        IsLoggingIn = false;
                        return;
                    }
                    catch (Exception e)
                    {
                        ErrorMessage = e.Message;
                        IsLoggingIn = false;
                        e.ShowMessageBox();
                        return;
                    }

                    var viewModel = new OverviewViewModel(client);
                    await viewModel.LoadData();

                    ShowView.Invoke(this, viewModel);
                }));
            }
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