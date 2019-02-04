using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Clients.Helpers;
using Maze.Server.Connection.JsonConverters;
using Maze.Server.Connection.Modules;
using Maze.Server.Connection.Utilities;

namespace Maze.Administration.Library.Rest.Modules.V1
{
    public class ModulesResource : VersionedResource<ModulesResource>
    {
        public ModulesResource() : base("modules")
        {
        }

        public static Task<PackagesLock> FetchModules(NuGetFramework framework, IRestClient client)
        {
            return CreateRequest().AddQueryParam("framework", framework.ToString()).Execute(client).Return<PackagesLock>();
        }

        public static Task<InstalledModulesDto> GetInstalledModules(IMazeRestClient client)
        {
            return CreateRequest(HttpVerb.Get, "installed").Execute(client).Wrap<InstalledModulesDto>()
                .ConfigureJsonSettings(settings => settings.Converters.Add(new PackageIdentityConverter())).ToResult();
        }

        public static Task<List<PackageSearchMetadata>> FetchPackageInfo(List<PackageIdentity> packages, IMazeRestClient client)
        {
            return CreateRequest(HttpVerb.Post, "package")
                .WithBody(new JsonContent(packages, settings => settings.Converters.Add(new PackageIdentityConverter()))).Execute(client)
                .Wrap<List<PackageSearchMetadata>>().ConfigureJsonSettings(x => x.Converters.Add(new NuGetVersionConverter())).ToResult();
        }

        public static Task InstallModule(PackageIdentity package, IMazeRestClient client)
        {
            return CreateRequest(HttpVerb.Post).WithBody(new JsonContent(package, settings => settings.Converters.Add(new PackageIdentityConverter()))).Execute(client);
        }

        public static Task UninstallModule(PackageIdentity package, IMazeRestClient client)
        {
            return CreateRequest(HttpVerb.Delete).AddQueryParam("package", PackageIdentityConvert.ToString(package)).Execute(client);
        }

        public static Task<List<Uri>> FetchRepositorySources(IMazeRestClient client)
        {
            return CreateRequest(HttpVerb.Get, "sources").Execute(client).Return<List<Uri>>();
        }
    }
}