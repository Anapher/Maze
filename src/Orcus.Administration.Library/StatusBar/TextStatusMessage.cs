using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Orcus.Administration.Library.StatusBar
{
    public class TextStatusMessage : StatusMessage, INotifyPropertyChanged
    {
        private StatusBarAnimation _animation;
        private string _message;

        public TextStatusMessage(string message)
        {
            _message = message;
        }

        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }

        public StatusBarAnimation Animation
        {
            get => _animation;
            set
            {
                if (_animation != value)
                {
                    _animation = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}