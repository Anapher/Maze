using System.Security;
using Prism.Commands;
using Prism.Mvvm;

namespace Maze.Administration.ViewModels.Overview.Administrators
{
    public class ChangeAdministratorPasswordViewModel : BindableBase
    {
        private DelegateCommand _cancelCommand;
        private bool? _dialogResult;
        private DelegateCommand<SecureString> _okCommand;

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public SecureString NewPassword { get; private set; }

        public DelegateCommand<SecureString> OkCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new DelegateCommand<SecureString>(parameter =>
                {
                    if (!(parameter?.Length > 0))
                        return;

                    NewPassword = parameter;
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