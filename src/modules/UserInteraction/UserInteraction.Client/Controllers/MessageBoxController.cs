using System.Windows.Forms;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Parameters;
using Orcus.Modules.Api.Routing;
using UserInteraction.Dtos;

namespace UserInteraction.Client.Controllers
{
    [Route("messageBox")]
    public class MessageBoxController : OrcusController
    {
        //UserInteraction/messageBox/open
        [OrcusPost("open")]
        public IActionResult Open([FromBody] OpenMessageBoxDto dto)
        {
            MessageBox.Show(dto.Text, dto.Caption, (MessageBoxButtons) dto.Buttons,
                SystemIconToMessageBoxIcon(dto.Icon));

            return Ok();
        }

        private static MessageBoxIcon SystemIconToMessageBoxIcon(SystemIcon icon)
        {
            switch (icon)
            {
                case SystemIcon.Error:
                    return MessageBoxIcon.Error;
                case SystemIcon.Info:
                    return MessageBoxIcon.Information;
                case SystemIcon.Warning:
                    return MessageBoxIcon.Warning;
                case SystemIcon.Question:
                    return MessageBoxIcon.Question;
                default:
                    return MessageBoxIcon.None;
            }
        }
    }
}