using NuGet.Packaging.Core;

namespace Maze.Service.Commander.Routing
{
    public class RouteDescription
    {
        public RouteDescription(PackageIdentity packageIdentity, string method, string[] segments)
        {
            PackageIdentity = packageIdentity;
            Method = method;
            Segments = segments;
        }

        /// <summary>
        ///     Gets the origin package the route was created from
        /// </summary>
        public PackageIdentity PackageIdentity { get; }

        /// <summary>
        ///     Gets the method of the route.
        /// </summary>
        /// <value>A <see cref="string" /> containing the method of the route.</value>
        public string Method { get; }

        /// <summary>
        ///     Gets or set the segments for the route
        /// </summary>
        /// <value>An <see cref="System.Collections.Generic.IEnumerable{T}" />, containing the segments for the route.</value>
        public string[] Segments { get; }
    }
}