using System.Linq;
using System.Security.Claims;

namespace Maze.Server.Library.Utilities
{
    /// <summary>
    ///     Extensions to a <see cref="ClaimsPrincipal"/>
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        ///     Check if the <see cref="ClaimsPrincipal"/> is an administrator
        /// </summary>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> that should be checked.</param>
        /// <returns>Return true if it is an administrator.</returns>
        public static bool IsAdministrator(this ClaimsPrincipal principal) => principal.IsInRole("admin");

        /// <summary>
        ///     Get the client id of a client-<see cref="ClaimsPrincipal"/>
        /// </summary>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> that is a client whos id should be retrived.</param>
        /// <returns>Return the client id</returns>
        public static int GetClientId(this ClaimsPrincipal principal) =>
            int.Parse(principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

        /// <summary>
        ///     Get the account id of an administrator-<see cref="ClaimsPrincipal"/>
        /// </summary>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> that is a administrator whos id should be retrived.</param>
        /// <returns>Return the account id</returns>
        public static int GetAccountId(this ClaimsPrincipal principal) =>
            int.Parse(principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
    }
}