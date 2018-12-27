//Taken from https://github.com/kobake/AspNetCore.RouteAnalyzer (MIT)

using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Orcus.Server.Controllers
{
    [Route("routes")]
    public class RouteAnalyzerController : Controller
    {
        private readonly IRouteAnalyzer _routeAnalyzer;

        public RouteAnalyzerController(IActionDescriptorCollectionProvider provider)
        {
            _routeAnalyzer = new RouteAnalyzer(provider);
        }

        [HttpGet]
        public IActionResult ShowAllRoutes()
        {
            var infos = _routeAnalyzer.GetAllRouteInformations();
            var builder = new StringBuilder();
            foreach (var info in infos) builder.AppendLine(info.ToString());
            return Content(builder.ToString());
        }
    }

    public interface IRouteAnalyzer
    {
        IEnumerable<RouteInformation> GetAllRouteInformations();
    }

    public class RouteAnalyzer : IRouteAnalyzer
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public RouteAnalyzer(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public IEnumerable<RouteInformation> GetAllRouteInformations()
        {
            var ret = new List<RouteInformation>();

            var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items;
            foreach (var route in routes)
            {
                var info = new RouteInformation();

                // Area
                if (route.RouteValues.ContainsKey("area")) info.Area = route.RouteValues["area"];

                // Path and Invocation of Razor Pages
                if (route is PageActionDescriptor pageAction)
                {
                    info.Path = pageAction.ViewEnginePath;
                    info.Invocation = pageAction.RelativePath;
                }

                // Path of Route Attribute
                if (route.AttributeRouteInfo != null)
                {
                    var e = route;
                    info.Path = $"/{e.AttributeRouteInfo.Template}";
                }

                // Path and Invocation of Controller/Action
                if (route is ControllerActionDescriptor controllerAction)
                {
                    if (info.Path == "") info.Path = $"/{controllerAction.ControllerName}/{controllerAction.ActionName}";
                    info.Invocation = $"{controllerAction.ControllerName}Controller.{controllerAction.ActionName}";
                }

                // Additional information of invocation
                info.Invocation += $" ({route.DisplayName})";

                // Generating List
                ret.Add(info);
            }

            // Result
            return ret;
        }
    }

    public class RouteInformation
    {
        public string Area { get; set; }
        public string Path { get; set; }
        public string Invocation { get; set; }

        public override string ToString() => $"RouteInformation{{Area:\"{Area}\", Path:\"{Path}\", Invocation:\"{Invocation}\"}}";
    }
}