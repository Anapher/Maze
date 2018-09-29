using System;
using System.Windows.Forms;
using ClipboardManager.Client.Utilities;
using ClipboardManager.Shared.Channels;
using ClipboardManager.Shared.Dtos;
using ClipboardManager.Shared.Extensions;
using Orcus.ControllerExtensions;
using Orcus.Modules.Api.Routing;

namespace ClipboardManager.Client.Channels
{
    [Route("notificationChannel")]
    public class ClipboardNotificationChannel : CallTransmissionChannel<IClipboardNotificationChannel>, IClipboardNotificationChannel
    {
        private readonly ClipboardWatcher _clipboardWatcher;
        private bool _isListening;

        public ClipboardNotificationChannel(ClipboardWatcher clipboardWatcher)
        {
            _clipboardWatcher = clipboardWatcher;
        }

        public event EventHandler<ClipboardData> ClipboardUpdated;

        public void Listen()
        {
            if (_isListening)
                throw new InvalidOperationException("Already listening.");
            _isListening = true;

            _clipboardWatcher.ClipboardUpdated += ClipboardWatcherOnClipboardUpdated;
        }

        private void ClipboardWatcherOnClipboardUpdated(object sender, IDataObject e)
        {
            var dto = ClipboardDataExtensions.FromDataObject(e);

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