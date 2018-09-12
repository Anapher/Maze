using System.Threading.Tasks;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;

namespace TaskManager.Administration.Rest
{
    public class ProcessesWindowResource : ResourceBase<ProcessesWindowResource>
    {
        public ProcessesWindowResource() : base("processes")
        {
        }

        public static Task BringToFront(int processId, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, processId + "/window/bringToFront").Execute(restClient);

        public static Task Maximize(int processId, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, processId + "/window/maximize").Execute(restClient);

        public static Task Minimize(int processId, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, processId + "/window/minimize").Execute(restClient);

        public static Task Restore(int processId, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, processId + "/window/restore").Execute(restClient);

        public static Task Close(int processId, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, processId + "/window/close").Execute(restClient);
    }
}