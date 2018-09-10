using AutoMapper;
using TaskManager.Administration.ViewModel;
using TaskManager.Shared.Dtos;

namespace TaskManager.Administration
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProcessDto, ProcessViewModel>();
        }
    }
}