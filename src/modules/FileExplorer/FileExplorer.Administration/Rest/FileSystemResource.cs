using System.Collections.Generic;
using System.Threading.Tasks;
using FileExplorer.Shared.Dtos;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;

namespace FileExplorer.Administration.Rest
{
    public class FileSystemResource : ResourceBase<FileSystemResource>
    {
        public FileSystemResource() : base("fileSystem")
        {
        }

        public static Task<List<FileExplorerEntry>> QueryFiles(string path, IPackageRestClient restClient) =>
            CreateRequest().AddQueryParam("path", path).Execute(restClient).Return<List<FileExplorerEntry>>();

        public static Task<DirectoryEntry> GetDirectoryEntry(string path, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "directory").AddQueryParam("path", path).Execute(restClient).Return<List<DirectoryEntry>>();

        public static Task<string> ExpandEnvironmentVariables(string path, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "path").AddQueryParam("path", path).Execute(restClient).Return<string>();
    }
}