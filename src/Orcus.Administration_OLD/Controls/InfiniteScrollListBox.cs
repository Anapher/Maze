using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using Orcus.Administration.Modules.Models;

namespace Orcus.Administration.Controls
{
    internal class InfiniteScrollListBox : ListBox
    {
        public readonly SemaphoreSlim ItemsLock = new SemaphoreSlim(1, 1);

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new InfiniteScrollListBoxAutomationPeer(this);
        }
    }

    internal class InfiniteScrollListBoxAutomationPeer : ListBoxAutomationPeer
    {
        public InfiniteScrollListBoxAutomationPeer(ListBox owner) : base(owner) { }

        protected override List<AutomationPeer> GetChildrenCore()
        {
            var infiniteScrollListBox = Owner as InfiniteScrollListBox;

            try
            {
                infiniteScrollListBox?.ItemsLock.Wait();

                // Don't return the LoadingStatusIndicator as an AutomationPeer, otherwise narrator will report it as an item in the list of packages, even when not visible
                return base.GetChildrenCore()?.Where(lbiap => !(((ListBoxItemAutomationPeer)lbiap).Item is LoadingStatusIndicator)).ToList() ?? new List<AutomationPeer>();
            }
            finally
            {
                infiniteScrollListBox?.ItemsLock.Release();
            }
        }
    }

    internal class LoadingStatusIndicator : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private LoadingStatus _status = LoadingStatus.Unknown;
        private string _errorMessage;
        private string _loadingMessage;

        public LoadingStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public string LoadingMessage
        {
            get
            {
                return _loadingMessage;
            }
            set
            {
                if (_loadingMessage != value)
                {
                    _loadingMessage = value;
                    OnPropertyChanged(nameof(LoadingMessage));
                }
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, e);
            }
        }

        public void UpdateLoadingState(IItemLoaderState loaderState)
        {
            Status = loaderState.LoadingStatus;
        }

        public void Reset(string loadingMessage)
        {
            Status = LoadingStatus.Unknown;
            LoadingMessage = loadingMessage;
        }

        public void SetError(string message)
        {
            Status = LoadingStatus.ErrorOccurred;
            ErrorMessage = message;
        }
    }
}