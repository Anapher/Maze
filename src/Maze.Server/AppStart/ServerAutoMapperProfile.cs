using AutoMapper;
using NuGet.Protocol;

namespace Orcus.Server.AppStart
{
    public class ServerAutoMapperProfile : Profile
    {
        public ServerAutoMapperProfile()
        {
            CreateMap<LocalPackageSearchMetadata, PackageSearchMetadata>()
                .ForMember(x => x.DependencySetsInternal, opts => opts.MapFrom(x => x.DependencySets))
                .ForMember(x => x.PackageId, opt => opt.MapFrom(x => x.Identity.Id))
                .ForMember(x => x.Version, opt => opt.MapFrom(x => x.Identity.Version));
        }
    }
}