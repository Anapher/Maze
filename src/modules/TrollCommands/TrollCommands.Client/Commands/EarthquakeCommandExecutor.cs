using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Infrastructure.Client.Library;
using TrollCommands.Client.Utilities;
using TrollCommands.Shared.Commands;

namespace TrollCommands.Client.Commands
{
    public class EarthquakeCommandExecutor : ITaskExecutor<EarthquakeCommandInfo>
    {
        public async Task<HttpResponseMessage> InvokeAsync(EarthquakeCommandInfo commandInfo, TaskExecutionContext context,
            CancellationToken cancellationToken)
        {
            var random = new Random();
            ScreenUtils.Initialize(out var hwnd, out var hdc, out var rect);

            if (commandInfo.Power >= rect.Width)
                commandInfo.Power = rect.Width - 1;
            if (commandInfo.Power >= rect.Height)
                commandInfo.Power = rect.Height - 1;

            try
            {
                while (true)
                {
                    var screenshot = ScreenUtils.CreateCompatibleBitmap(hdc, rect.Width, rect.Height);
                    var hdc2 = ScreenUtils.CreateCompatibleDC(hdc);
                    ScreenUtils.SelectObject(hdc2, screenshot);

                    ScreenUtils.BitBlt(hdc2, 0, 0, rect.Width, rect.Height, hdc, 0, 0, ScreenUtils.TernaryRasterOperations.SRCCOPY);
                    ScreenUtils.BitBlt(hdc, 0, 0, rect.Width, rect.Height, hdc2, random.Next(-commandInfo.Power / 2, commandInfo.Power / 2),
                        random.Next(-commandInfo.Power / 2, commandInfo.Power / 2), ScreenUtils.TernaryRasterOperations.SRCCOPY);

                    ScreenUtils.BitBlt(hdc, 0, 0, rect.Width, rect.Height, hdc2, 0, 0, ScreenUtils.TernaryRasterOperations.SRCCOPY);

                    ScreenUtils.DeleteDC(hdc2);
                    ScreenUtils.DeleteObject(screenshot);

                    await Task.Delay(commandInfo.Interval, cancellationToken);
                }
            }
            finally
            {
                ScreenUtils.ReleaseDC(hwnd, hdc);
            }
        }
    }
}