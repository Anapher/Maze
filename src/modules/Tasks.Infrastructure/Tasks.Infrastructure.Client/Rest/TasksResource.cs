using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orcus.Client.Library.Clients;
using Orcus.Client.Library.Clients.Helpers;

namespace Tasks.Infrastructure.Client.Rest
{
    public class TasksResource : VersionedResource<TasksResource>
    {
        public TasksResource() : base("tasks")
        {
        }

        public static Task<List<string>> GetTasks(IRestClient restClient) => CreateRequest().Execute(restClient).Return<List<string>>();
    }
}
