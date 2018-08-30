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

        public static Task<List<FileExplorerEntry>> QueryEntries(string path, IPackageRestClient restClient) =>
            CreateRequest().AddQueryParam("path", path).Execute(restClient).Return<List<FileExplorerEntry>>();

        public static Task<DirectoryEntry> GetDirectoryEntry(string path, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "directory").AddQueryParam("path", path).Execute(restClient).Return<DirectoryEntry>();

        public static Task<string> ExpandEnvironmentVariables(string path, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "path").AddQueryParam("path", path).Execute(restClient).Return<string>();

        public static Task<List<DirectoryEntry>> QueryDirectories(string path, IPackageRestClient restClient) =>
            CreateRequest().AddQueryParam("path", path).AddQueryParam("directoriesOnly", "true").Execute(restClient)
                .Return<List<DirectoryEntry>>();

        public static Task CreateDirectory(string path, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "directory").AddQueryParam("path", path).Execute(restClient);

        public static Task DeleteDirectory(string path, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Delete, "directory").AddQueryParam("path", path).Execute(restClient);

        public static Task DeleteFile(string path, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Delete, "file").AddQueryParam("path", path).Execute(restClient);

        public static Task MoveFile(string path, string newPath, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Patch, "file").AddQueryParam("path", path).AddQueryParam("newPath", newPath).Execute(restClient);

        public static Task MoveDirectory(string path, string newPath, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Patch, "directory").AddQueryParam("path", path).AddQueryParam("newPath", newPath).Execute(restClient);
    }
}