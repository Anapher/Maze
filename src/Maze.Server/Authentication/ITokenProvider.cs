using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Maze.Server.Data.EfClasses;

namespace Maze.Server.Authentication
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