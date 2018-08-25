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

        public static async Task<RootElementsDto> GetRoot(IPackageRestClient restClient)
        {
            var response = await CreateRequest(HttpVerb.Get, "root").Execute(restClient); //.Return<RootElementsDto>();
            var st = await response.Content.ReadAsStringAsync();
            return null;
        }

        public static Task<PathTreeResponseDto> GetPathTree(PathTreeRequestDto requestDto, IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "pathTree").Execute(restClient).Return<PathTreeResponseDto>();
    }
}