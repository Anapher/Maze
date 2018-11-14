using System.Collections.Generic;
using System.Threading.Tasks;
using FileExplorer.Shared.Dtos;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;

namespace FileExplorer.Administration.Rest
{
    public class FileSystemResource : ResourceBase<FileSystemResource>
    {
        public FileSystemResource() : base("FileExplorer/fileSystem")
        {
        }

        public static Task<List<FileExplorerEntry>> QueryEntries(string path, ITargetedRestClient restClient) =>
            CreateRequest().AddQueryParam("path", path).Execute(restClient).Return<List<FileExplorerEntry>>();

        public static Task<DirectoryEntry> GetDirectoryEntry(string path, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "directory").AddQueryParam("path", path).Execute(restClient).Return<DirectoryEntry>();

        public static Task<string> ExpandEnvironmentVariables(string path, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "path").AddQueryParam("path", path).Execute(restClient).Return<string>();

        public static Task<List<DirectoryEntry>> QueryDirectories(string path, ITargetedRestClient restClient) =>
            CreateRequest().AddQueryParam("path", path).AddQueryParam("directoriesOnly", "true").Execute(restClient)
                .Return<List<DirectoryEntry>>();

        public static Task CreateDirectory(string path, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "directory").AddQueryParam("path", path).Execute(restClient);

        public static Task DeleteDirectory(string path, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Delete, "directory").AddQueryParam("path", path).Execute(restClient);

        public static Task DeleteFile(string path, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Delete, "file").AddQueryParam("path", path).Execute(restClient);

        public static Task MoveFile(string path, string newPath, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Patch, "file").AddQueryParam("path", path).AddQueryParam("newPath", newPath).Execute(restClient);

        public static Task MoveDirectory(string path, string newPath, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Patch, "directory").AddQueryParam("path", path).AddQueryParam("newPath", newPath).Execute(restClient);

        public static Task<FilePropertiesDto> GetFileProperties(string path, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "file/properties").AddQueryParam("path", path).Execute(restClient).Return<FilePropertiesDto>();

        public static Task ExecuteFile(ExecuteFileDto dto, bool waitForExit, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "file/execute", dto).AddQueryParam("waitForExit", waitForExit.ToString()).Execute(restClient);

        public static Task<string[]> GetFileVerbs(string path, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "file/verbs").AddQueryParam("path", path).Execute(restClient).Return<string[]>();
    }
}