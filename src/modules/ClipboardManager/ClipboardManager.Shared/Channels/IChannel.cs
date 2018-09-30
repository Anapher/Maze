using System;
using System.Threading.Tasks;
using ClipboardManager.Shared.Dtos;

namespace ClipboardManager.Shared.Channels
{
    public interface IClipboardNotificationChannel
    {
        event EventHandler<ClipboardData> ClipboardUpdated;

        Task Listen();
        Task SetData(ClipboardData clipboardData);
    }
}