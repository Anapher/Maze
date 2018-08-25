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

        public static Task<PathTreeResponseDto> GetPathTree(PathTreeRequestDto requestDto, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "pathTree").Execute(restClient).Return<PathTreeResponseDto>();
    }
}