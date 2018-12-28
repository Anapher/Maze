using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.Shared.Channels;
using FileExplorer.Shared.Dtos;
using Maze.Administration.ControllerExtensions;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Clients.Helpers;

namespace FileExplorer.Administration.Rest
{
    public class FileExplorerResource : ChannelResource<FileExplorerResource>
    {
        public FileExplorerResource() : base("FileExplorer")
        {
        }

        public static Task<RootElementsDto> GetRoot(ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "root").Execute(restClient).Return<RootElementsDto>();

        public static Task<PathTreeResponseDto> GetPathTree(PathTreeRequestDto requestDto, bool keepOrder, CancellationToken cancellationToken,
            ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "pathTree", requestDto).AddQueryParam("keepOrder", keepOrder.ToString())
                .Execute(restClient, cancellationToken).Return<PathTreeResponseDto>();

        public static Task Upload(HttpContent httpContent, string path, CancellationToken cancellationToken, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "upload", httpContent).AddQueryParam("path", path).Execute(restClient, cancellationToken);

        public static Task<HttpResponseMessage> DownloadFile(string path, CancellationToken cancellationToken, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "download").AddQueryParam("path", path)
                .ConfigureHeader(x => x.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip")))
                .Execute(restClient, cancellationToken);

        public static Task<HttpResponseMessage> DownloadDirectory(string path, CancellationToken cancellationToken, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "downloadDirectory").AddQueryParam("path", path)
                .ConfigureHeader(x => x.AcceptEncoding.Add(new StringWithQualityHeaderValue("zip")))
                .Execute(restClient, cancellationToken);

        public static Task CopyPathToClipboard(string path, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "clipboard").AddQueryParam("path", path)
                .Execute(restClient);

        public static Task<CallTransmissionChannel<IHashFileAction>> ComputeHash(ITargetedRestClient restClient) =>
            restClient.CreateChannel<FileExplorerResource, IHashFileAction>("computeHash");
    }
}