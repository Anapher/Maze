using System.Windows.Forms;
using ClipboardManager.Client.Utilities;
using ClipboardManager.Shared.Dtos;
using ClipboardManager.Shared.Extensions;
using Maze.Client.Library.Services;
using Maze.Modules.Api;
using Maze.Modules.Api.Parameters;
using Maze.Modules.Api.Routing;

namespace ClipboardManager.Client.Controllers
{
    public class ClipboardController : MazeController
    {
        private readonly IStaSynchronizationContext _synchronizationContext;

        public ClipboardController(IStaSynchronizationContext synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
        }

        [MazeGet]
        public IActionResult GetClipboardData()
        {
            IDataObject dataObject = null;
            _synchronizationContext.Current.Send(state => dataObject = Clipboard.GetDataObject(), null);

            return Ok(ClipboardDataExtensions.FromDataObject(dataObject));
        }

        [MazePost]
        public IActionResult SetClipboardData([FromBody] ClipboardData clipboardData)
        {
            _synchronizationContext.Current.Send(state => ClipboardManagerExtensions.SetClipboardData(clipboardData), null);
            return Ok();
        }
    }
}