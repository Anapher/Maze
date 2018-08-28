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

        public static Task<PathTreeResponseDto> GetPathTree(PathTreeRequestDto requestDto, bool keepOrder,
            IPackageRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "pathTree", requestDto).AddQueryParam("keepOrder", keepOrder.ToString())
                .Execute(restClient).Return<PathTreeResponseDto>();

        //{
        //    //;
        //    var ads = await CreateRequest(HttpVerb.Post, "pathTree", requestDto).Execute(restClient);
        //    var data = await ads.Content.ReadAsStringAsync();
        //    return JsonConvert.DeserializeObject<PathTreeResponseDto>(data);
        //}
    }
}