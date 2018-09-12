using System.Diagnostics;
using System.Threading.Tasks;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;

namespace TaskManager.Administration.Rest
{
    public class ProcessesResource : ResourceBase<ProcessesResource>
    {
        public ProcessesResource() : base("processes")
        {
        }

        public static Task Kill(int processId, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, processId + "/kill").Execute(restClient);

        public static Task KillTree(int rootProcessId, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, rootProcessId + "/killTree").Execute(restClient);

        public static Task Suspend(int processId, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, processId + "/suspend").Execute(restClient);

        public static Task Resume(int processId, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, processId + "/resume").Execute(restClient);

        public static Task SetPriority(int processId, ProcessPriorityClass priority, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, processId + "/setPriority").AddQueryParam("priority", ((int) priority).ToString()).Execute(restClient);
    }
}