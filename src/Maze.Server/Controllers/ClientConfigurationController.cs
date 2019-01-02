using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CodeElements.BizRunner;
using Maze.Server.BusinessLogic.ClientConfigurations;
using Maze.Server.Connection;
using Maze.Server.Connection.Clients;
using Maze.Server.Data.EfClasses;
using Maze.Server.Data.EfCode;
using Maze.Server.Library.Controllers;
using Maze.Server.Library.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maze.Server.Controllers
{
    [Route("v1/clients/configuration")]
    public class ClientConfigurationController : BusinessController
    {
        private readonly AppDbContext _context;

        public ClientConfigurationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentConfig()
        {
            if (User.IsAdministrator())
                return BadRequest("This route can only be accessed by clients");

            var clientId = User.GetClientId();
            var configurations = await _context.Set<ClientConfiguration>().AsNoTracking()
                .Where(x => x.ClientGroupId == null || x.ClientGroup.ClientGroupMemberships.Any(y => y.ClientId == clientId))
                .Select(x => new ClientConfigurationDataDto {ClientGroupId = x.ClientGroupId, Content = x.Content}).ToListAsync();

            return Ok(configurations);
        }

        [HttpGet("current/info")]
        public async Task<IActionResult> GetCurrentConfigInfo()
        {
            if (User.IsAdministrator())
                return BadRequest("This route can only be accessed by clients");

            var clientId = User.GetClientId();
            var configurations = await _context.Set<ClientConfiguration>().AsNoTracking()
                .Where(x => x.ClientGroupId == null || x.ClientGroup.ClientGroupMemberships.Any(y => y.ClientId == clientId))
                .Select(x => x.ContentHash).ToListAsync();

            return Ok(configurations);
        }

        [HttpGet("{id}")]
        [HttpGet]
        [Authorize("admin")]
        public async Task<IActionResult> GetConfig(int? id)
        {
            var config = await _context.Set<ClientConfiguration>().AsNoTracking().FirstOrDefaultAsync(x => x.ClientGroupId == id);
            if (config == null)
                return RestError(BusinessErrors.ClientConfigurations.NotFound);

            return Ok(Mapper.Map<ClientConfigurationDto>(config));
        }

        [HttpPut("{id}")]
        [HttpPut]
        [Authorize("admin")]
        public async Task<IActionResult> UpdateConfig(int? id, [FromBody] ClientConfigurationDto dto, [FromServices] IUpdateClientConfigAction action)
        {
            dto.ClientGroupId = id;
            await action.ToRunner(_context).ExecuteAsync(dto);
            return BizActionStatus(action);
        }

        [HttpPost]
        [Authorize("admin")]
        public async Task<IActionResult> CreateConfig([FromBody] ClientConfigurationDto dto, [FromServices] ICreateClientConfigAction action)
        {
            await action.ToRunner(_context).ExecuteAsync(dto);
            return BizActionStatus(action);
        }

        [HttpDelete("{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> DeleteConfig(int id, [FromServices] IDeleteClientConfigAction action)
        {
            await action.ToRunner(_context).ExecuteAsync(id);
            return BizActionStatus(action);
        }
    }
}