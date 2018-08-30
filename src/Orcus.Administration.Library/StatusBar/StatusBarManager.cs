using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mvvm;

namespace Orcus.Administration.Library.StatusBar
{
    public class StatusBarManager : BindableBase, IShellStatusBar
    {
        private readonly List<StatusMessage> _messages;
        private readonly object _messagesLock = new object();
        private StatusMessage _currentStatusMessage;
        private object _rightContent;

        public StatusBarManager()
        {
            _messages = new List<StatusMessage>();
        }

        public StatusMessage CurrentStatusMessage
        {
            get => _currentStatusMessage;
            private set => SetProperty(ref _currentStatusMessage, value);
        }

        public object RightContent
        {
            get => _rightContent;
            set => SetProperty(ref _rightContent, value);
        }

        public TMessage PushStatus<TMessage>(TMessage message) where TMessage : StatusMessage
        {
            lock (_messagesLock)
            {
                _messages.Add(message);
            }

            message.Disposed += MessageOnDisposed;
            OnMessagesChanged();

            return message;
        }

        private void MessageOnDisposed(object sender, EventArgs e)
        {
            var message = (StatusMessage) sender;
            lock (_messagesLock)
            {
                _messages.Remove(message);
            }

            OnMessagesChanged();
        }

        private void OnMessagesChanged()
        {
            StatusMessage statusMessage;
            lock (_messagesLock)
            {
                statusMessage = _messages.LastOrDefault();
            }

            CurrentStatusMessage = statusMessage;
        }
    }
}