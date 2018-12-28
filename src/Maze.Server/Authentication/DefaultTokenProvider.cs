using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Maze.Server.Data.EfClasses;
using Maze.Server.Options;

namespace Maze.Server.Authentication
{
    public class DefaultTokenProvider : ITokenProvider
    {
        private readonly string _audience;
        private readonly string _issuer;
        private readonly SymmetricSecurityKey _key;
        private readonly SigningCredentials _signingCredentials;
        private readonly TimeSpan _userTokenValidityPeriod;
        private readonly JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();

        public DefaultTokenProvider(IOptions<AuthenticationOptions> options)
        {
            _issuer = options.Value.Issuer;
            _audience = options.Value.Audience;
            _userTokenValidityPeriod = TimeSpan.FromHours(options.Value.AccountTokenValidityHours);
            _key = new SymmetricSecurityKey(Convert.FromBase64String(options.Value.Secret));
            _signingCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        }

        public JwtSecurityToken GetAccountToken(Account account)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.AccountId.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, account.Username),
                new Claim("role", "admin"),
                new Claim("role", "installingUser")
            };

            return new JwtSecurityToken(_issuer, _audience, claims, null, DateTime.UtcNow.Add(_userTokenValidityPeriod),
                _signingCredentials);
        }

        public JwtSecurityToken GetClientToken(Client client)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, client.ClientId.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim("role", "client")
            };

            return new JwtSecurityToken(_issuer, _audience, claims, null, DateTime.UtcNow.Add(_userTokenValidityPeriod), _signingCredentials);
        }

        public TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters
            {
                IssuerSigningKey = _key,
                ValidAudience = _audience,
                ValidIssuer = _issuer,
                ClockSkew = TimeSpan.Zero // Identity and resource servers are the same.
            };
        }

        public string TokenToString(JwtSecurityToken token)
        {
            return _handler.WriteToken(token);
        }
    }
}