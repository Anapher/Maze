using System;

namespace Maze.Administration.Library.Extensions
{
    /// <summary>
    ///     A helper class for <see cref="Uri"/>
    /// </summary>
    public static class UriHelper
    {
        /// <summary>
        ///     Combine two relative <see cref="Uri"/>s
        /// </summary>
        /// <param name="baseUri">The base uri</param>
        /// <param name="relativeUri">The relative uri that should be attached</param>
        /// <returns>Return the resulting, relative uri</returns>
        public static Uri CombineRelativeUris(Uri baseUri, Uri relativeUri) =>
            new Uri(baseUri.ToString().TrimEnd('/') + "/" + relativeUri.ToString().TrimStart('/'), UriKind.Relative);
    }
}