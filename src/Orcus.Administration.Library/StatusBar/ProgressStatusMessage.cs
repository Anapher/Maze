namespace Orcus.Administration.Library.StatusBar
{
    public class ProgressStatusMessage : TextStatusMessage
    {
        private double? _progress;

        public ProgressStatusMessage(string message) : base(message)
        {
        }

        /// <summary>
        ///     The current progress from zero to one. If <code>null</code>, the progress will display a looping animation (or the
        ///     <see cref="StatusBarAnimation" /> if it isn't <see cref="StatusBarAnimation.None" />)
        /// </summary>
        public double? Progress
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}