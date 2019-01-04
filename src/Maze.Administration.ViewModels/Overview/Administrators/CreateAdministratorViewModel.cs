using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text;
using Maze.Administration.Core.Extensions;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.Views;
using Maze.Server.Connection.Accounts;
using Prism.Commands;
using Prism.Mvvm;

namespace Maze.Administration.ViewModels.Overview.Administrators
{
    public class CreateAdministratorViewModel : BindableBase
    {
        private readonly IWindowService _windowService;
        private DelegateCommand _cancelCommand;
        private DelegateCommand<SecureString> _createCommand;
        private bool? _dialogResult;
        private bool _isEnabled = true;
        private string _username;

        public CreateAdministratorViewModel(IWindowService windowService)
        {
            _windowService = windowService;
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public PasswordProvidingAccountDto Entity { get; private set; }

        public DelegateCommand<SecureString> CreateCommand
        {
            get
            {
                return _createCommand ?? (_createCommand = new DelegateCommand<SecureString>(parameter =>
                {
                    if (parameter == null)
                        return;

                    using (var secureStringWrapper = new SecureStringWrapper(parameter))
                    {
                        var account = new PasswordProvidingAccountDto
                        {
                            Username = Username, Password = Encoding.UTF8.GetString(secureStringWrapper.ToByteArray()), IsEnabled = IsEnabled
                        };

                        var errors = account.Validate(new ValidationContext(account)).ToList();
                        if (errors.Any())
                        {
                            _windowService.ShowErrorMessageBox(string.Join(Environment.NewLine, errors.Select(x => x.ErrorMessage)));
                            return;
                        }

                        Entity = account;
                    }


                    DialogResult = true;
                }));
            }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new DelegateCommand(() => { DialogResult = false; })); }
        }
    }
}