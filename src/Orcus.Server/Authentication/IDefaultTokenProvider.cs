using System.IdentityModel.Tokens.Jwt;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.Authentication
{
    public interface IDefaultTokenProvider : ITokenGenerator
    {
        JwtSecurityToken GetAccountToken(Account account);
        JwtSecurityToken GetClientToken(Client client);
    }

    public interface ITokenGenerator
    {
        string TokenToString(JwtSecurityToken token);
    }
}