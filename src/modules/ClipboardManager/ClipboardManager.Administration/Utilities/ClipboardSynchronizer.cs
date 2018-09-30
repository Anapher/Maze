using System;
using System.Threading.Tasks;
using System.Windows;
using ClipboardManager.Administration.Rest;
using ClipboardManager.Shared.Channels;
using ClipboardManager.Shared.Dtos;
using ClipboardManager.Shared.Extensions;
using ClipboardManager.Shared.Utilities;
using Orcus.Administration.ControllerExtensions;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Services;

namespace ClipboardManager.Administration.Utilities
{
    public class ClipboardSynchronizer : IDisposable
    {
        private readonly ITargetedRestClient _restClient;
        private readonly IAppDispatcher _dispatcher;
        private readonly ClipboardWatcher _clipboardWatcher;
        private CallTransmissionChannel<IClipboardNotificationChannel> _channel;
        private ClipboardData _currentClipboardData;

        public ClipboardSynchronizer(ITargetedRestClient restClient, IAppDispatcher dispatcher, ClipboardWatcher clipboardWatcher)
        {
            _restClient = restClient;
            _dispatcher = dispatcher;
            _clipboardWatcher = clipboardWatcher;
        }

        public void Dispose()
        {
            Disable();
        }

        public async Task Enable()
        {
            var currentClipboardData = Clipboard.GetDataObject();

            _channel = await ClipboardManagerResource.GetClipboardNotificationChannel(_restClient);
            _channel.Interface.ClipboardUpdated += InterfaceOnClipboardUpdated;
            await _channel.Interface.SetData(ClipboardDataExtensions.FromDataObject(currentClipboardData));
            await _channel.Interface.Listen();

            _clipboardWatcher.ClipboardUpdated += ClipboardWatcherOnClipboardUpdated;
        }

        public void Disable()
        {
            _channel?.Dispose();
            _clipboardWatcher.ClipboardUpdated -= ClipboardWatcherOnClipboardUpdated;
        }

        private void ClipboardWatcherOnClipboardUpdated(object sender, IDataObject e)
        {
            var dto = ClipboardDataExtensions.FromDataObject(e);
            if (dto.Equals(_currentClipboardData))
                return;

            _currentClipboardData = dto;
            _channel.Interface.SetData(dto);
        }

        private void InterfaceOnClipboardUpdated(object sender, ClipboardData e)
        {
            if (e.Equals(_currentClipboardData))
                return;
            _currentClipboardData = e;

            _dispatcher.Current.BeginInvoke(new Action(() =>
            {
                var oldValue = _currentClipboardData;
                try
                {
                    _currentClipboardData = e;
                    ClipboardExtensions.SetClipboardData(e);
                }
                catch (Exception)
                {
                    _currentClipboardData = oldValue;
                }
            }));
        }
    }
}