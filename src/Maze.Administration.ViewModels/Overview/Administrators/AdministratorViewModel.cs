using System;
using Maze.Server.Connection.Accounts;
using Prism.Mvvm;

namespace Maze.Administration.ViewModels.Overview.Administrators
{
    public class AdministratorViewModel : BindableBase
    {
        private readonly Action<AccountDto> _updateAccount;
        private bool _isEnabled;
        private string _username;

        public AdministratorViewModel(AccountDto account, Action<AccountDto> updateAccount)
        {
            _updateAccount = updateAccount;

            AccountId = account.AccountId;
            CreatedOn = account.CreatedOn;

            SetProperty(ref _username, account.Username, nameof(Username));
            SetProperty(ref _isEnabled, account.IsEnabled, nameof(IsEnabled));
        }

        public int AccountId { get; }
        public DateTime CreatedOn { get; }

        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value)) UpdateEntity();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (SetProperty(ref _isEnabled, value)) UpdateEntity();
            }
        }

        public void CopyToEntity(AccountDto account)
        {
            account.AccountId = AccountId;
            account.Username = Username;
            account.IsEnabled = IsEnabled;
        }

        private void UpdateEntity()
        {
            var dto = new AccountDto();
            CopyToEntity(dto);

            _updateAccount(dto);
        }
    }
}