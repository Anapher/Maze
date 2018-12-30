using System.Threading.Tasks;
using AutoMapper;
using CodeElements.BizRunner;
using Maze.Server.BusinessLogic.ClientConfigurations;
using Maze.Server.Connection;
using Maze.Server.Connection.Clients;
using Maze.Server.Data.EfClasses;
using Maze.Server.Data.EfCode;
using Maze.Server.Library.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maze.Server.Controllers
{
    [Route("v1/clients/configuration")]
    [Authorize("admin")]
    public class ClientConfigurationController : BusinessController
    {
        private readonly AppDbContext _context;

        public ClientConfigurationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentConfig() => NotFound();

        [HttpGet("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetConfig(int? id)
        {
            var config = await _context.Set<ClientConfiguration>().AsNoTracking().FirstOrDefaultAsync(x => x.ClientGroupId == id);
            if (config == null)
                return RestError(BusinessErrors.ClientConfigurations.NotFound);

            return Ok(Mapper.Map<ClientConfigurationDto>(config));
        }

        [HttpPut("{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateConfig(int? id, [FromBody] ClientConfigurationDto dto, [FromServices] IUpdateClientConfigAction action)
        {
            dto.ClientGroupId = id;
            await action.ToRunner(_context).ExecuteAsync(dto);
            return BizActionStatus(action);
        }

        [HttpPost]
        public async Task<IActionResult> CreateConfig([FromBody] ClientConfigurationDto dto, [FromServices] ICreateClientConfigAction action)
        {
            await action.ToRunner(_context).ExecuteAsync(dto);
            return BizActionStatus(action);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfig(int id, [FromServices] IDeleteClientConfigAction action)
        {
            await action.ToRunner(_context).ExecuteAsync(id);
            return BizActionStatus(action);
        }
    }
}