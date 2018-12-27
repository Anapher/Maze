using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.Authentication
{
    public interface ITokenProvider : ITokenGenerator
    {
        JwtSecurityToken GetAccountToken(Account account);
        JwtSecurityToken GetClientToken(Client client);
        TokenValidationParameters GetValidationParameters();
    }

    public interface ITokenGenerator
    {
        string TokenToString(JwtSecurityToken token);
    }
}