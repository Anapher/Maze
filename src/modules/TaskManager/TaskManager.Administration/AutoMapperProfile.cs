using AutoMapper;
using TaskManager.Administration.ViewModels;
using TaskManager.Shared.Dtos;

namespace TaskManager.Administration
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProcessDto, ProcessViewModel>().ForAllMembers(opt => opt.Condition((dto, model, source, destination) => source != null));
        }
    }
}