using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;
using Orcus.Server.Connection.JsonConverters;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Connection.Utilities;

namespace Orcus.Administration.Library.Rest.Modules.V1
{
    public class ModulesResource : VersionedResource<ModulesResource>
    {
        public ModulesResource() : base("modules")
        {
        }

        public static Task<PackagesLock> FetchModules(NuGetFramework framework, IOrcusRestClient client)
        {
            return CreateRequest().AddQueryParam("framework", framework.ToString()).Execute(client).Return<PackagesLock>();
        }

        public static Task<InstalledModulesDto> GetInstalledModules(IOrcusRestClient client)
        {
            return CreateRequest(HttpVerb.Get, "installed").Execute(client).Wrap<InstalledModulesDto>()
                .ConfigureJsonSettings(settings => settings.Converters.Add(new PackageIdentityConverter())).ToResult();
        }

        public static Task<List<PackageSearchMetadata>> FetchPackageInfo(List<PackageIdentity> packages, IOrcusRestClient client)
        {
            return CreateRequest(HttpVerb.Post, "package")
                .WithBody(new JsonContent(packages, settings => settings.Converters.Add(new PackageIdentityConverter()))).Execute(client)
                .Wrap<List<PackageSearchMetadata>>().ConfigureJsonSettings(x => x.Converters.Add(new NuGetVersionConverter())).ToResult();
        }

        public static Task InstallModule(PackageIdentity package, IOrcusRestClient client)
        {
            return CreateRequest(HttpVerb.Post).WithBody(package).Execute(client);
        }

        public static Task UninstallModule(PackageIdentity package, IOrcusRestClient client)
        {
            return CreateRequest(HttpVerb.Delete).AddQueryParam("package", PackageIdentityConvert.ToString(package)).Execute(client);
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