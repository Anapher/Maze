using System.Threading.Tasks;
using ClipboardManager.Shared.Channels;
using ClipboardManager.Shared.Dtos;
using Orcus.Administration.ControllerExtensions;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;

namespace ClipboardManager.Administration.Rest
{
    public class ClipboardManagerResource : ChannelResource<ClipboardManagerResource>
    {
        public ClipboardManagerResource() : base("ClipboardManager")
        {
        }

        public static Task<ClipboardData> GetClipboardData(ITargetedRestClient restClient) =>
            CreateRequest().Execute(restClient).Return<ClipboardData>();

        public static Task SetClipboardData(ClipboardData clipboardData, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Post).WithBody(clipboardData).Execute(restClient);

        public static Task<CallTransmissionChannel<IClipboardNotificationChannel>> GetClipboardNotificationChannel(ITargetedRestClient restClient) =>
            restClient.CreateChannel<ClipboardManagerResource, IClipboardNotificationChannel>("notificationChannel");
    }
}