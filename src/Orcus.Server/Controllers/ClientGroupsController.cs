using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CodeElements.BizRunner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Orcus.Server.BusinessLogic.ClientGroups;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Clients;
using Orcus.Server.Data.EfCode;
using Orcus.Server.Library.Controllers;
using Orcus.Server.Library.Hubs;

namespace Orcus.Server.Controllers
{
    [Route("v1/clients/groups"), Authorize("admin")]
    public class ClientGroupsController : BusinessController
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<AdministrationHub> _hubContext;

        public ClientGroupsController(AppDbContext context, IHubContext<AdministrationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var groups = await _context.Groups.ProjectTo<ClientGroupDto>().ToListAsync();
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroup(int id)
        {
            var group = await _context.Groups.Where(x => x.ClientGroupId == id).ProjectTo<ClientGroupDto>().FirstOrDefaultAsync();
            if (group == null)
                return RestError(BusinessErrors.ClientGroups.GroupNotFound);

            return Ok(group);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClientGroupDto clientGroupDto, [FromServices] ICreateClientGroupAction action)
        {
            var result = await action.ToRunner(_context).ExecuteAsync(clientGroupDto);
            return await BizActionStatus(action, async () =>
            {
                clientGroupDto.ClientGroupId = result.ClientGroupId;
                await _hubContext.Clients.All.SendAsync(HubEventNames.ClientGroupCreated, clientGroupDto);

                return CreatedAtAction(nameof(GetGroup), new {id = result.ClientGroupId}, clientGroupDto);
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClientGroupDto clientGroupDto, [FromServices] IUpdateClientGroupAction action)
        {
            clientGroupDto.ClientGroupId = id;

            var result = await action.ToRunner(_context).ExecuteAsync(clientGroupDto);
            return await BizActionStatus(action, async () =>
            {
                await _hubContext.Clients.All.SendAsync(HubEventNames.ClientGroupUpdated, Mapper.Map<ClientGroupDto>(result));
                return Ok();
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromServices] IDeleteClientGroupAction action)
        {
            await action.ToRunner(_context).ExecuteAsync(id);
            return await BizActionStatus(action, async () =>
            {
                await _hubContext.Clients.All.SendAsync(HubEventNames.ClientGroupRemoved, id);
                return Ok();
            });
        }

        [HttpPost("{id}/add")]
        public async Task<IActionResult> AddClients(int id, [FromBody] int[] clientIds, [FromServices] IAddClientsToClientGroupAction action)
        {
            var clientGroup = await action.ToRunner(_context).ExecuteAsync((id, clientIds));
            return await BizActionStatus(action, async () =>
            {
                await _hubContext.Clients.All.SendAsync(HubEventNames.ClientGroupUpdated, Mapper.Map<ClientGroupDto>(clientGroup));
                return Ok();
            });
        }

        [HttpPost("{id}/remove")]
        public async Task<IActionResult> RemoveClients(int id, [FromBody] int[] clientIds, [FromServices] IRemoveClientsFromClientGroupAction action)
        {
            var clientGroup = await action.ToRunner(_context).ExecuteAsync((id, clientIds));
            return await BizActionStatus(action, async () =>
            {
                await _hubContext.Clients.All.SendAsync(HubEventNames.ClientGroupUpdated, Mapper.Map<ClientGroupDto>(clientGroup));
                return Ok();
            });
        }
    }
}