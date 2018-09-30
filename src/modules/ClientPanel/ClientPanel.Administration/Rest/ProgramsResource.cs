using System.Threading.Tasks;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;

namespace ClientPanel.Administration.Rest
{
    public class ProgramsResource : ResourceBase<ProgramsResource>
    {
        public ProgramsResource() : base("ClientPanel/programs")
        {
        }

        public static Task StartTaskManager(ITargetedRestClient restClient) => CreateRequest(HttpVerb.Get, "taskManager").Execute(restClient);
        public static Task StartRegEdit(ITargetedRestClient restClient) => CreateRequest(HttpVerb.Get, "regEdit").Execute(restClient);
        public static Task StartDeviceManager(ITargetedRestClient restClient) => CreateRequest(HttpVerb.Get, "deviceManager").Execute(restClient);
        public static Task StartEventLog(ITargetedRestClient restClient) => CreateRequest(HttpVerb.Get, "eventLog").Execute(restClient);
        public static Task StartControlPanel(ITargetedRestClient restClient) => CreateRequest(HttpVerb.Get, "controlPanel").Execute(restClient);
        public static Task StartServices(ITargetedRestClient restClient) => CreateRequest(HttpVerb.Get, "services").Execute(restClient);
        public static Task StartComputerManagement(ITargetedRestClient restClient) => CreateRequest(HttpVerb.Get, "computerManagement").Execute(restClient);
    }
}