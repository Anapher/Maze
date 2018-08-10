using System;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Routing;

namespace TestModule.Client
{
    public class TestController : OrcusController
    {
        [OrcusGet]
        public IActionResult Test() => Ok("Processor Count on this PC: " + Environment.ProcessorCount);
    }
}