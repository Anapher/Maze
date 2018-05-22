using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orcus.Administration.Core.Clients;
using Orcus.Administration.Core.Clients.Helpers;
using Orcus.Server.Connection.Modules;

namespace Orcus.Administration.Core.Rest.Modules.V1
{
    public class ModulesResource : VersionedResource<ModulesResource>
    {
        public ModulesResource() : base("modules")
        {
        }

        public static Task InstallModule(SourcedPackageIdentity package, IOrcusRestClient client)
        {
            return CreateRequest(HttpVerb.Post).WithBody(package).Execute(client);
        }

        public static Task<List<InstalledModule>> FetchModules(IOrcusRestClient client)
        {
            return CreateRequest().Execute(client).Return<List<InstalledModule>>();
        }

        public static Task<List<Uri>> FetchRepositorySources(IOrcusRestClient client)
        {
            return CreateRequest(HttpVerb.Get, "sources").Execute(client).Return<List<Uri>>();
        }

        public static Task AddRepositorySource(Uri uri, IOrcusRestClient client)
        {
            return CreateRequest(HttpVerb.Post, "sources", uri).Execute(client);
        }

        public static Task DeleteRepositorySource(Uri uri, IOrcusRestClient client)
        {
            return CreateRequest(HttpVerb.Delete, "sources", uri).Execute(client);
        }
    }
}