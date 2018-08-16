using System.Threading.Tasks;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;
using UserInteraction.Dtos;

namespace UserInteraction.Administration.Rest
{
    public class MessageBoxResource : ResourceBase<MessageBoxResource>
    {
        public MessageBoxResource() : base("messageBox")
        {
        }

        public static Task OpenAsync(OpenMessageBoxDto dto, IPackageRestClient restClient)
        {
            return CreateRequest(HttpVerb.Post, "open", dto).Execute(restClient);
        }
    }
}