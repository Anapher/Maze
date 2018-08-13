using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using Anapher.Wpf.Swan;
using Orcus.Administration.Core;
using Orcus.Administration.Core.Clients;
using Orcus.Administration.Core.Exceptions;
using Orcus.Administration.Core.Rest.Modules.V1;
using Orcus.Administration.ViewModels.Utilities;
using Orcus.Server.Connection.Modules;
using Unclassified.TxLib;

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
            private set => SetProperty(value, ref _errorMessage);
        }

        private string _statusMessage;

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(value, ref _statusMessage);
        }

        public AsyncRelayCommand<SecureString> LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new AsyncRelayCommand<SecureString>(async parameter =>
                {
                    IsLoggingIn = true;

                    IOrcusRestClient client;
                    IReadOnlyList<SourcedPackageIdentity> modules;

                    try
                    {
                        StatusMessage = Tx.T("LoginView:Status.Authenticating");

                        client = await OrcusRestConnector.TryConnect(Username, parameter,
                            new ServerInfo {ServerUri = new Uri("http://localhost:50485/")});

                        StatusMessage = Tx.T("LoginView:Status.RetrieveModules");

                        modules = await ModulesResource.FetchModules(client);

                        if (modules.Any())
                        {
                            StatusMessage = Tx.T("LoginView:Status.LoadModules");

                        }
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
                    await viewModel.LoadData(s => Debug.Print(s));

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