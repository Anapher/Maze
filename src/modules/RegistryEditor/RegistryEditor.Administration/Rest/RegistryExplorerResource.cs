using System.Collections.Generic;
using System.Threading.Tasks;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;
using RegistryEditor.Shared.Dtos;

namespace RegistryEditor.Administration.Rest
{
    public class RegistryEditorResource : ResourceBase<RegistryEditorResource>
    {
        public RegistryEditorResource() : base(null)
        {
        }

        public static Task<List<RegistryKeyDto>> QuerySubKeys(string path, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "subKeys").AddQueryParam("path", path).Execute(restClient).Return<List<RegistryKeyDto>>();

        public static Task DeleteRegistryKey(string path, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Delete, "subKeys").AddQueryParam("path", path).Execute(restClient);

        public static Task CreateRegistryKey(string path, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "subKeys", path).Execute(restClient);

        public static Task<List<RegistryValueDto>> GetValues(string path, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "subKeys/values").AddQueryParam("path", path).Execute(restClient).Return<List<RegistryValueDto>>();

        public static Task DeleteRegistryValue(string path, string valueName, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Delete, "subKeys/values").AddQueryParam("path", path).AddQueryParam("name", valueName).Execute(restClient);

        public static Task SetRegistryValue(string path, RegistryValueDto registryValue, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "subKeys/values", registryValue).AddQueryParam("path", path).Execute(restClient);
    }
}