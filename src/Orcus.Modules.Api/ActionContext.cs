using System.Collections.Generic;

namespace Orcus.Modules.Api
{
    /// <summary>
    ///     Context object for execution of action which has been selected as part of a request.
    /// </summary>
    public abstract class ActionContext
    {
        /// <summary>
        ///     Gets or sets the route data for the current request.
        /// </summary>
        public abstract IReadOnlyDictionary<string, object> RouteData { get; }

        /// <summary>
        ///     Gets or sets the <see cref="OrcusContext" /> for the current request.
        /// </summary>
        public abstract OrcusContext Context { get; }
    }
}