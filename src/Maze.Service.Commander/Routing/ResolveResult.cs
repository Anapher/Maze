using System.Collections.Generic;

namespace Orcus.Service.Commander.Routing
{
    public class ResolveResult
    {
        public ResolveResult(bool success = true)
        {
            Success = success;
        }

        /// <summary>
        /// The resolved route description
        /// </summary>
        public RouteDescription RouteDescription { get; set; }

        /// <summary>
        ///     Gets or sets the captured parameters
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// True if the resolve was successful
        /// </summary>
        public bool Success { get; }
    }
}