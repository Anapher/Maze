using System;
using ClipboardManager.Shared.Dtos;

namespace ClipboardManager.Shared.Channels
{
    public interface IClipboardNotificationChannel
    {
        event EventHandler<ClipboardData> ClipboardUpdated;

        void Listen();
    }
}