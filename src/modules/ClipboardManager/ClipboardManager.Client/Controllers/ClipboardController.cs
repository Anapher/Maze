using System.Windows.Forms;
using ClipboardManager.Shared.Dtos;
using ClipboardManager.Shared.Extensions;
using Orcus.Client.Library.Services;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Parameters;
using Orcus.Modules.Api.Routing;

namespace ClipboardManager.Client.Controllers
{
    public class ClipboardController : OrcusController
    {
        private readonly IStaSynchronizationContext _synchronizationContext;

        public ClipboardController(IStaSynchronizationContext synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
        }

        [OrcusGet]
        public IActionResult GetClipboardData()
        {
            IDataObject dataObject = null;
            _synchronizationContext.Current.Send(state => dataObject = Clipboard.GetDataObject(), null);

            return Ok(ClipboardDataExtensions.FromDataObject(dataObject));
        }

        [OrcusPost]
        public IActionResult SetClipboardData([FromBody] ClipboardData clipboardData)
        {
            _synchronizationContext.Current.Send(state => ClipboardDataExtensions.SetClipboardData(clipboardData), null);
            return Ok();
        }
    }
}