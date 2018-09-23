using Prism.Commands;
using Prism.Mvvm;

namespace RegistryEditor.Administration.ViewModels
{
    public class CreateSubKeyViewModel : BindableBase
    {
        private DelegateCommand _cancelCommand;
        private DelegateCommand _createCommand;
        private bool? _dialogResult;
        private string _name;

        public CreateSubKeyViewModel(string subKeyLocation)
        {
            SubKeyLocation = subKeyLocation;
        }

        public string SubKeyLocation { get; }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public DelegateCommand CreateCommand
        {
            get
            {
                return _createCommand ?? (_createCommand =
                           new DelegateCommand(() => DialogResult = true, () => !string.IsNullOrWhiteSpace(Name)).ObservesProperty(() => Name));
            }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new DelegateCommand(() => DialogResult = false)); }
        }
    }
}