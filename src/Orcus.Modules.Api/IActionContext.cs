using System;
using Orcus.Modules.Api.Response;

namespace Orcus.Modules.Api
{
    public interface IActionContext
    {
        IServiceProvider ServiceProvider { get; }
        OrcusResponse Response { get; }
    }
}