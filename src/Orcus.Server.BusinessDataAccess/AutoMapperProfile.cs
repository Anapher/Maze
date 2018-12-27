using System.Linq;
using AutoMapper;
using Orcus.Server.Connection.Clients;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.BusinessDataAccess
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
        }
    }
}