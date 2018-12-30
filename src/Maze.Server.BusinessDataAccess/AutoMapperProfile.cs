using System.Linq;
using AutoMapper;
using Maze.Server.Connection.Clients;
using Maze.Server.Data.EfClasses;

namespace Maze.Server.BusinessDataAccess
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Client, ClientDto>().ForMember(dto => dto.LatestSession,
                expression =>
                    expression.MapFrom(client => client.ClientSessions.OrderByDescending(x => x.CreatedOn).First()));

            CreateMap<ClientSession, ClientSessionDto>();
            CreateMap<ClientGroup, ClientGroupDto>().ForMember(x => x.Clients,
                expression => expression.MapFrom(g => g.ClientGroupMemberships.Select(x => x.ClientId).ToList()));
            CreateMap<ClientConfiguration, ClientConfigurationDto>();
        }
    }
}