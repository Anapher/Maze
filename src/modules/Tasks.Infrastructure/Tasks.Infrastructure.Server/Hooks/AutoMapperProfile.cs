using AutoMapper;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Server.Hooks
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<TaskSessionDto, TaskSession>();
            CreateMap<TaskExecutionDto, TaskExecution>();
            CreateMap<CommandResultDto, CommandResult>();

            CreateMap<TaskSession, TaskSessionDto>();
            CreateMap<TaskExecution, TaskExecutionDto>();
            CreateMap<CommandResult, CommandResultDto>();
        }
    }
}