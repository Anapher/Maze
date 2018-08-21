using System.Windows.Forms;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Parameters;
using Orcus.Modules.Api.Routing;
using UserInteraction.Dtos.MessageBox;

namespace UserInteraction.Client.Controllers
{
    [Route("messageBox")]
    public class MessageBoxController : OrcusController
    {
        //UserInteraction/messageBox/open
        [OrcusPost("open")]
        public IActionResult Open([FromBody] OpenMessageBoxDto dto)
        {
            var result = MessageBox.Show(dto.Text, dto.Caption, (MessageBoxButtons) dto.Buttons,
                SystemIconToMessageBoxIcon(dto.Icon));

            return Ok((MsgBxIcon) result);
        }

        private static MessageBoxIcon SystemIconToMessageBoxIcon(MsgBxIcon icon)
        {
            switch (icon)
            {
                case MsgBxIcon.Error:
                    return MessageBoxIcon.Error;
                case MsgBxIcon.Info:
                    return MessageBoxIcon.Information;
                case MsgBxIcon.Warning:
                    return MessageBoxIcon.Warning;
                case MsgBxIcon.Question:
                    return MessageBoxIcon.Question;
                default:
                    return MessageBoxIcon.None;
            }
        }
    }
}