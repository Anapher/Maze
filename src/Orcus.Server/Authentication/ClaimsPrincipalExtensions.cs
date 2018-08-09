using System.Linq;
using System.Security.Claims;

namespace Orcus.Server.Authentication
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsAdministrator(this ClaimsPrincipal principal) => principal.IsInRole("admin");

        public static int GetClientId(this ClaimsPrincipal principal) =>
            int.Parse(principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
    }
}