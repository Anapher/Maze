using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Orcus.Administration.Library.StatusBar
{
    /// <summary>
    ///     A simple status message that displays a text message
    /// </summary>
    public class TextStatusMessage : StatusMessage, INotifyPropertyChanged
    {
        private StatusBarAnimation _animation;
        private string _message;

        /// <summary>
        ///     Initialize a new instance of <see cref="TextStatusMessage"/>
        /// </summary>
        /// <param name="message">The message that should be displayed</param>
        public TextStatusMessage(string message)
        {
            _message = message;
        }

        /// <summary>
        ///     Gets or sets the current message that is displayed on this status
        /// </summary>
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

        /// <summary>
        ///     Gets or sets the current status bar animation
        /// </summary>
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