using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows;
using Anapher.Wpf.Toolkit.Extensions;
using Anapher.Wpf.Toolkit.Windows;
using MahApps.Metro.IconPacks;
using Maze.Administration.Core.Extensions;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.Rest.Accounts.V1;
using Maze.Administration.Library.ViewModels;
using Maze.Administration.ViewModels.Overview.Administrators;
using Maze.Server.Connection.Accounts;
using Maze.Utilities;
using Prism.Commands;
using Unclassified.TxLib;

namespace Maze.Administration.ViewModels.Overview
{
    public class AdministratorsViewModel : OverviewTabBase
    {
        private readonly IRestClient _restClient;
        private readonly IWindowService _windowService;
        private ObservableCollection<AdministratorViewModel> _administrators;
        private DelegateCommand<AdministratorViewModel> _changePasswordCommand;
        private DelegateCommand<AdministratorViewModel> _removeCommand;
        private DelegateCommand _createAdministratorCommand;

        public AdministratorsViewModel(IRestClient restClient, IWindowService windowService) : base(Tx.T("Administrators"),
            PackIconFontAwesomeKind.GraduationCapSolid)
        {
            _restClient = restClient;
            _windowService = windowService;
        }

        public ObservableCollection<AdministratorViewModel> Administrators
        {
            get => _administrators;
            set => SetProperty(ref _administrators, value);
        }

        public DelegateCommand<AdministratorViewModel> RemoveCommand
        {
            get
            {
                return _removeCommand ?? (_removeCommand = new DelegateCommand<AdministratorViewModel>(async parameter =>
                {
                    if (await AccountsResource.DeleteAccount(parameter.AccountId, _restClient).OnErrorShowMessageBox(_windowService))
                    {
                        Administrators.Remove(parameter);
                    }
                }));
            }
        }

        public DelegateCommand<AdministratorViewModel> ChangePasswordCommand
        {
            get
            {
                return _changePasswordCommand ?? (_changePasswordCommand = new DelegateCommand<AdministratorViewModel>(async parameter =>
                {
                    if (_windowService.ShowDialog<ChangeAdministratorPasswordViewModel>(out var viewModel) == true)
                    {
                        using (var wrapper = new SecureStringWrapper(viewModel.NewPassword))
                        {
                            var account = new PasswordProvidingAccountDto {Password = Encoding.UTF8.GetString(wrapper.ToByteArray())};
                            parameter.CopyToEntity(account);

                            var errors = account.Validate(new ValidationContext(account)).ToList();
                            if (errors.Any())
                            {
                                _windowService.ShowErrorMessageBox(string.Join(Environment.NewLine, errors.Select(x => x.ErrorMessage)));
                                return;
                            }

                            if (await AccountsResource.PutAccount(account, _restClient).OnErrorShowMessageBox(_windowService))
                            {
                                _windowService.ShowMessage(Tx.T("AdministratorsView:PasswordChangedSuccessfully"),
                                    Tx.T("AdministratorsView:ChangePassword"), MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }));
            }
        }

        public DelegateCommand CreateAdministratorCommand
        {
            get
            {
                return _createAdministratorCommand ?? (_createAdministratorCommand = new DelegateCommand(async () =>
                {
                    if (_windowService.ShowDialog<CreateAdministratorViewModel>(out var viewModel) == true)
                    {
                        var result = await AccountsResource.PostAccount(viewModel.Entity, _restClient).OnErrorShowMessageBox(_windowService);
                        if (!result.Failed)
                        {
                            Administrators.Add(new AdministratorViewModel(result.Result, UpdateAccount));
                        }
                    }
                }));
            }
        }

        public override async void OnInitialize()
        {
            base.OnInitialize();

            var accounts = await AccountsResource.GetAccounts(_restClient);
            Administrators = new ObservableCollection<AdministratorViewModel>(accounts.Select(x => new AdministratorViewModel(x, UpdateAccount)));
        }

        private void UpdateAccount(AccountDto obj)
        {
            AccountsResource.PutAccount(obj, _restClient).OnErrorShowMessageBox(_windowService).Forget();
        }
    }
}