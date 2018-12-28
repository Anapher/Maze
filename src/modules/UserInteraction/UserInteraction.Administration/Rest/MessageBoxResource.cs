using System.Threading.Tasks;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Clients.Helpers;
using UserInteraction.Dtos.MessageBox;

namespace UserInteraction.Administration.Rest
{
    public class MessageBoxResource : ResourceBase<MessageBoxResource>
    {
        public MessageBoxResource() : base("UserInteraction/messageBox")
        {
        }

        public static Task<MsgBxResult> OpenAsync(OpenMessageBoxDto dto, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "open", dto).Execute(restClient).Return<MsgBxResult>();
    }
}