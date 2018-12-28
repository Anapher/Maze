using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClipboardManager.Client.Utilities;
using ClipboardManager.Shared.Channels;
using ClipboardManager.Shared.Dtos;
using ClipboardManager.Shared.Extensions;
using ClipboardManager.Shared.Utilities;
using Maze.Client.Library.Services;
using Maze.ControllerExtensions;
using Maze.Modules.Api.Routing;

namespace ClipboardManager.Client.Channels
{
    [Route("notificationChannel")]
    public class ClipboardNotificationChannel : CallTransmissionChannel<IClipboardNotificationChannel>, IClipboardNotificationChannel
    {
        private readonly ClipboardWatcher _clipboardWatcher;
        private readonly IStaSynchronizationContext _synchronizationContext;
        private bool _isListening;
        private ClipboardData _currentClipboardData;

        public ClipboardNotificationChannel(ClipboardWatcher clipboardWatcher, IStaSynchronizationContext synchronizationContext)
        {
            _clipboardWatcher = clipboardWatcher;
            _synchronizationContext = synchronizationContext;
        }

        public event EventHandler<ClipboardData> ClipboardUpdated;

        public Task Listen()
        {
            if (_isListening)
                throw new InvalidOperationException("Already listening.");
            _isListening = true;

            _clipboardWatcher.ClipboardUpdated += ClipboardWatcherOnClipboardUpdated;
            return Task.CompletedTask;
        }

        public Task SetData(ClipboardData clipboardData)
        {
            _synchronizationContext.Current.Send(state =>
            {
                var oldValue = _currentClipboardData;
                try
                {
                    _currentClipboardData = clipboardData;
                    ClipboardManagerExtensions.SetClipboardData(clipboardData);
                }
                catch (Exception)
                {
                    _currentClipboardData = oldValue;
                }
            }, null);
            return Task.CompletedTask;
        }

        private void ClipboardWatcherOnClipboardUpdated(object sender, IDataObject e)
        {
            var dto = ClipboardDataExtensions.FromDataObject(e);
            if (dto.Equals(_currentClipboardData))
                return;

            _currentClipboardData = dto;
            var handler = ClipboardUpdated;
            handler?.Invoke(this, dto);
        }

        public override void Dispose()
        {
            base.Dispose();
            _clipboardWatcher.ClipboardUpdated -= ClipboardWatcherOnClipboardUpdated;
        }
    }
}