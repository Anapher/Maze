using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.Shared.Dtos;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;

namespace FileExplorer.Administration.Rest
{
    public class FileExplorerResource : ResourceBase<FileExplorerResource>
    {
        public FileExplorerResource() : base(null)
        {
        }

        public static Task<RootElementsDto> GetRoot(IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "root").Execute(restClient).Return<RootElementsDto>();

        public static Task<PathTreeResponseDto> GetPathTree(PathTreeRequestDto requestDto, bool keepOrder, CancellationToken cancellationToken,
            IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "pathTree", requestDto).AddQueryParam("keepOrder", keepOrder.ToString())
                .Execute(restClient, cancellationToken).Return<PathTreeResponseDto>();

        public static Task Upload(HttpContent httpContent, string path, CancellationToken cancellationToken, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "upload", httpContent).AddQueryParam("path", path).Execute(restClient, cancellationToken);

        public static Task<HttpResponseMessage> DownloadFile(string path, CancellationToken cancellationToken, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "download").AddQueryParam("path", path)
                .ConfigureHeader(x => x.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip")))
                .Execute(restClient, cancellationToken);

        public static Task<HttpResponseMessage> DownloadDirectory(string path, CancellationToken cancellationToken, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "downloadDirectory").AddQueryParam("path", path)
                .ConfigureHeader(x => x.AcceptEncoding.Add(new StringWithQualityHeaderValue("zip")))
                .Execute(restClient, cancellationToken);

        public static Task CopyPathToClipboard(string path, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "clipboard").AddQueryParam("path", path)
                .Execute(restClient);
    }
}